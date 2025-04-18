using Amazon;
using Amazon.Redshift;
using Amazon.RedshiftDataAPIService;
using Amazon.RedshiftDataAPIService.Model;
using Amazon.RedshiftServerless;
using Amazon.RedshiftServerless.Model;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using BasicSQLFormatter;
using DevExpress.ExpressApp.Blazor.Services;
using iData.Dp.CompGov.Entities;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Text.Json;

namespace iData.Dp.CompGov.Service
{
    public class RedshiftService
    {
        private IConfiguration _Configuration;
        private IXafApplicationProvider _appProvider;
        private RedshiftConfig _Redshiftconfig;
        public RedshiftConfig RedshiftConfig
        {
            get
            {
                if (_Redshiftconfig == null)
                {
                    _Redshiftconfig = _appProvider.GetApplication()
                                          .CreateObjectSpace(typeof(RedshiftConfig))
                                          .GetObjects<RedshiftConfig>()
                                          .Single();

                    _Redshiftconfig.LogProperties();
                }
                return _Redshiftconfig;
            }
        }

        public RedshiftService(IXafApplicationProvider appProvider, IConfiguration configuration)
        {
            _appProvider = appProvider;
            _Configuration = configuration;
        }

        private AmazonRedshiftDataAPIServiceClient CreateClient()
        {
            Console.WriteLine("Creating AmazonRedshiftDataAPIServiceClient using the default AWS credentials provider chain.");

            var regionEndpoint = RegionEndpoint.GetBySystemName(RedshiftConfig.Region);

            Console.WriteLine($"RegionEndpoint: {regionEndpoint.SystemName} - {regionEndpoint.DisplayName}");

            var client = new AmazonRedshiftDataAPIServiceClient(regionEndpoint);
#if DEBUG
            client = new AmazonRedshiftDataAPIServiceClient(_Configuration["AWS_ACCESS_KEY_ID"], _Configuration["AWS_SECRET_ACCESS_KEY"], regionEndpoint);
#endif

            Console.WriteLine("AmazonRedshiftDataAPIServiceClient created successfully.");

            return client;
        }

        private AmazonRedshiftServerlessClient CreateServerlessClient()
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(RedshiftConfig.Region);
            var client = new AmazonRedshiftServerlessClient(regionEndpoint);

#if DEBUG
            client = new AmazonRedshiftServerlessClient(_Configuration["AWS_ACCESS_KEY_ID"], _Configuration["AWS_SECRET_ACCESS_KEY"], regionEndpoint);
#endif
            return client;
        }
        private AmazonSecretsManagerClient CreateSecretsManagerClient()
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(RedshiftConfig.Region);
            var client = new AmazonSecretsManagerClient(regionEndpoint);

#if DEBUG
            client = new AmazonSecretsManagerClient(_Configuration["AWS_ACCESS_KEY_ID"], _Configuration["AWS_SECRET_ACCESS_KEY"], regionEndpoint);
#endif
            return client;
        }

        async Task<string> GetSecret()
        {
            using var client = CreateSecretsManagerClient();

            GetSecretValueRequest request = new() { SecretId = RedshiftConfig.SecretArn };
            GetSecretValueResponse response = await client.GetSecretValueAsync(request);

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);

            return $"Username={data["username"]};Password='{data["password"]}'";
        }

        public async Task<string> GetConnectionString()
        {
            using var client = CreateServerlessClient();

            GetWorkgroupRequest lwr = new() { WorkgroupName = RedshiftConfig.WorkGroupName };

            var workgroupTask = client.GetWorkgroupAsync(lwr);
            var passwordTask = GetSecret();
            await Task.WhenAll(workgroupTask, passwordTask);
            var endpoint = (await workgroupTask).Workgroup.Endpoint;
            var password = await passwordTask;

            var host = $"Host={endpoint.Address}:{endpoint.Port};";
#if DEBUG
            host = "Host=localhost:5439;";
#endif
            return host + $"Database={RedshiftConfig.DatabaseName};{password}";
        }

        public async Task<bool> ValidateConnection(IProgressReporter progressReporter)
        {
            using var client = CreateClient();
            try
            {
                progressReporter.ReportProgress("Establishing connection to Redshift...");

                var validateRequest = new ExecuteStatementRequest
                {
                    Database = RedshiftConfig.DatabaseName,
                    SecretArn = RedshiftConfig.SecretArn,
                    WorkgroupName = RedshiftConfig.WorkGroupName,
                    Sql = "SELECT 1"
                };

                Console.WriteLine($"ExecuteStatementRequest:");
                Console.WriteLine($"Database: {validateRequest.Database}");
                Console.WriteLine($"SecretArn: {validateRequest.SecretArn}");
                Console.WriteLine($"WorkGroupName: {validateRequest.WorkgroupName}");
                Console.WriteLine($"Sql: {validateRequest.Sql}");

                var validateResponse = await client.ExecuteStatementAsync(validateRequest);

                if (validateResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    progressReporter.ReportProgress("Connection to Redshift successful.");
                    return true;
                }
                else
                {
                    progressReporter.ReportProgress("Connection to Redshift failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error connecting to Redshift: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DropMaterializedView(IProgressReporter progressReporter, string viewName)
        {
            using var client = CreateClient();
            try
            {
                string dropSql = $"DROP MATERIALIZED VIEW IF EXISTS {RedshiftConfig.Schema}.{viewName}";

                progressReporter.ReportProgress($"Executing Statement: {dropSql}");

                var dropRequest = new ExecuteStatementRequest
                {
                    Database = RedshiftConfig.DatabaseName,
                    SecretArn = RedshiftConfig.SecretArn,
                    WorkgroupName = RedshiftConfig.WorkGroupName,
                    Sql = dropSql
                };

                var dropResponse = await client.ExecuteStatementAsync(dropRequest);
                var dropStatus = await WaitForStatementToComplete(client, dropResponse.Id, progressReporter);

                if (dropStatus == "FINISHED")
                {
                    progressReporter.ReportProgress($"Drop {RedshiftConfig.Schema}.{viewName} successful.");
                    return true;
                }
                else
                {
                    progressReporter.ReportProgress($"Drop {RedshiftConfig.Schema}.{viewName} failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error dropping view: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateMaterializedView(IProgressReporter progressReporter, UseCaseSpecificDataProductEndpoint endpoint)
        {
            using var client = CreateClient();
            try
            {
                var config = RedshiftConfig;
                var exsc = config.ExternalSchema;
                var sc = config.Schema;
                var v = endpoint.EndpointName;
                var dp = endpoint.DataProduct.RedshiftTableName;
                var ucs = "dataentitlements_dp_usecaseswithdevices";


                string sql = $@"
            CREATE MATERIALIZED VIEW {sc}.{v} AS 
            SELECT dp.* 
            FROM {exsc}.{dp} dp
            INNER JOIN {sc}.{ucs} ucs
            ON dp.deviceserialno = ucs.deviceserial
            AND dp.devicesystemtype = ucs.systemtype
            WHERE ucs.usecaseid = '{endpoint.UseCase.ID}'
            AND dp.hastwelvetests = TRUE
            AND dp.date >= ucs.validitystart 
            AND (dp.date <= ucs.validityend OR ucs.validityend IS NULL)";

                string formattedSql = new SQLFormatter(sql).Format().Trim();

                progressReporter.ReportProgress($"Executing Statement:\n{formattedSql}\n");

                var createRequest = new ExecuteStatementRequest
                {
                    Database = config.DatabaseName,
                    SecretArn = config.SecretArn,
                    WorkgroupName = config.WorkGroupName,
                    Sql = sql
                };

                var createResponse = await client.ExecuteStatementAsync(createRequest);
                var createStatus = await WaitForStatementToComplete(client, createResponse.Id, progressReporter);

                if (createStatus == "FINISHED")
                {
                    progressReporter.ReportProgress("Materialized view creation successful.");
                    return true;
                }
                else
                {
                    progressReporter.ReportProgress("Materialized view creation failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error creating materialized view: {ex.Message}");
                return false;
            }
        }


        private async Task<string> WaitForStatementToComplete(AmazonRedshiftDataAPIServiceClient client, string statementId, IProgressReporter progressReporter)
        {
            string status;
            Console.WriteLine($"Starting to wait for statement to complete. Statement ID: {statementId}");

            do
            {
                var describeRequest = new DescribeStatementRequest
                {
                    Id = statementId
                };

                var describeResponse = await client.DescribeStatementAsync(describeRequest);
                status = describeResponse.Status;

                Console.WriteLine($"Statement ID: {statementId}, Status: {status}");

                if (status == "FAILED" || status == "FINISHED")
                {
                    if (status == "FAILED")
                    {
                        progressReporter.ReportProgress($"Statement failed: {describeResponse.Error}");
                        Console.WriteLine($"Statement ID: {statementId} failed with error: {describeResponse.Error}");
                    }
                    break;
                }

                await Task.Delay(1000); // Wait for a second before checking the status again
            } while (true);

            Console.WriteLine($"Statement ID: {statementId} has completed with status: {status}");
            return status;
        }

        public async Task<bool> DeleteFromTable(IProgressReporter progressReporter, string tableName)
        {
            using var client = CreateClient();
            try
            {
                string dropSql = $"DELETE FROM {RedshiftConfig.Schema}.{tableName}";
                progressReporter.ReportProgress($"Executing: {dropSql}");

                var dropRequest = new ExecuteStatementRequest
                {
                    Database = RedshiftConfig.DatabaseName,
                    SecretArn = RedshiftConfig.SecretArn,
                    WorkgroupName = RedshiftConfig.WorkGroupName,
                    Sql = dropSql
                };

                var dropResponse = await client.ExecuteStatementAsync(dropRequest);
                var dropStatus = await WaitForStatementToComplete(client, dropResponse.Id, progressReporter);

                return dropStatus == "FINISHED";
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error deleting from table: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateTable(IProgressReporter progressReporter, string tableName)
        {
            using var client = CreateClient();
            try
            {
                string sql = $@"
                CREATE TABLE IF NOT EXISTS {RedshiftConfig.Schema}.{tableName} (
                    usecaseid VARCHAR(50),
                    deviceserial VARCHAR(20),
                    systemtype VARCHAR(50),
                    validitystart DATE,
                    validityend DATE
                )";

                string formattedSql = new SQLFormatter(sql).Format().Trim();

                progressReporter.ReportProgress("Executing: " + formattedSql);

                var createRequest = new ExecuteStatementRequest
                {
                    Database = RedshiftConfig.DatabaseName,
                    SecretArn = RedshiftConfig.SecretArn,
                    WorkgroupName = RedshiftConfig.WorkGroupName,
                    Sql = formattedSql
                };

                var createResponse = await client.ExecuteStatementAsync(createRequest);
                var createStatus = await WaitForStatementToComplete(client, createResponse.Id, progressReporter);

                return createStatus == "FINISHED";
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error creating table: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> InsertDataIntoTable(IProgressReporter progressReporter, string tableName, IList<UseCase> useCases)
        {
            using var client = CreateClient();
            try
            {
                var valueTuples = useCases
                    .SelectMany(useCase => useCase.DeviceDataAccessGrants, (useCase, device) => new { useCase, device })
                    .SelectMany(ud => ud.device.ValidityPeriods, (ud, period) => new
                    {
                        ud.useCase.ID,
                        ud.device.Serial,
                        ud.device.SystemType,
                        ValidityStart = period.Start.ToString("yyyy-MM-dd"),
                        ValidityEnd = period.End.HasValue ? "'" + period.End.Value.ToString("yyyy-MM-dd") + "'" : "NULL"
                    })
                    .Select(vp => $"('{vp.ID}', '{vp.Serial}', '{vp.SystemType}', '{vp.ValidityStart}', {vp.ValidityEnd})")
                    .ToList();

                for (int i = 0; i < valueTuples.Count; i += 100)
                {
                    var batch = valueTuples.Skip(i).Take(100).ToList();
                    if (!await ExecuteBatchInsert(progressReporter, client, tableName, batch))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error inserting data: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ExecuteBatchInsert(IProgressReporter progressReporter, AmazonRedshiftDataAPIServiceClient client, string tableName, List<string> valueTuples)
        {
            string sql = new SQLFormatter($@"
            INSERT INTO {RedshiftConfig.Schema}.{tableName} (usecaseid, deviceserial, systemtype, validitystart, validityend) 
            VALUES {string.Join(",", valueTuples)}").Format();

            string formattedSql = new SQLFormatter(sql).Format().Trim();
            progressReporter.ReportProgress("Executing: " + formattedSql);

            var insertRequest = new ExecuteStatementRequest
            {
                Database = RedshiftConfig.DatabaseName,
                SecretArn = RedshiftConfig.SecretArn,
                WorkgroupName = RedshiftConfig.WorkGroupName,
                Sql = sql
            };

            var insertResponse = await client.ExecuteStatementAsync(insertRequest);
            return await WaitForStatementToComplete(client, insertResponse.Id, progressReporter) == "FINISHED";
        }

        public async Task<bool> CreateReadonlyRole(IProgressReporter progressReporter, string username, string table)
        {
            using var client = CreateClient();
            try
            {
                string sql = $@"
                                BEGIN;
                                CALL sp_create_dp_readonly_user('{username}_readonly_role', '{table}');
                                END;";

                progressReporter.ReportProgress($"Executing Statement: {sql}");

                var createRequest = new ExecuteStatementRequest
                {
                    Database = RedshiftConfig.DatabaseName,
                    SecretArn = RedshiftConfig.SecretArn,
                    WorkgroupName = RedshiftConfig.WorkGroupName,
                    Sql = sql
                };

                var createResponse = await client.ExecuteStatementAsync(createRequest);
                var createStatus = await WaitForStatementToComplete(client, createResponse.Id, progressReporter);

                if (createStatus == "FINISHED")
                {
                    progressReporter.ReportProgress("Readonly role creation successful.");
                    return true;
                }
                else
                {
                    progressReporter.ReportProgress("Readonly role creation failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                progressReporter.ReportProgress($"Error creating readonly role: {ex.Message}");
                return false;
            }
        }
    }

}



