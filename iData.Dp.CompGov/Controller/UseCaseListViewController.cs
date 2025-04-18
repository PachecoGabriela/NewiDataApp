using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using iData.Dp.CompGov.Entities;
using iData.Dp.CompGov.Service;
using Newtonsoft.Json;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.ExpressApp.Blazor.Editors;
using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;
using Newtonsoft.Json.Linq;
using Amazon.Runtime;

namespace iData.Dp.CompGov.Controller;

public class UseCaseListViewController : ObjectViewController<ListView, UseCase>, IProgressReporter
{
    private const string bucketName = "s3-idata-dev-computationalgovernance";
    private const string keyName = "put.json";

    private readonly RedshiftService _redshiftService;

    public UseCaseListViewController()
    {
        SimpleAction updateFromDataProductAction = new SimpleAction(this, "LoadUseCases", "Edit")
        {
            Caption = "Update from Data Product"
        };
        updateFromDataProductAction.Execute += UpdateFromDataProductAction_Execute;


        SimpleAction awsSync = new SimpleAction(this, "SyncToAWS", "Edit")
        {
            Caption = "Sync to Redshift"
        };
        awsSync.Execute += AwsSync_Execute;
    }

    private async void AwsSync_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        var tableName = "DataEntitlements_DP_UseCasesWithDevices";

        bool success = await _redshiftService.CreateTable(this, tableName);

        if (success)
            success = await _redshiftService.DeleteFromTable(this, tableName);

        if (success)
            success = await _redshiftService.InsertDataIntoTable(this, tableName, ObjectSpace.GetObjects<UseCase>());

        if (success)
            ReportProgress("Successfully synchronized.");
        else
            ReportProgress("There was an error during synchronization.");

        View.RefreshDataSource();
        View.Refresh();
    }

    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
        if (View.Editor is DxGridListEditor grid)
        {
            grid.GridModel.ShowAllRows = true;
            grid.GridModel.CustomizeElement = e =>
            {
                if (e.ElementType == GridElementType.DataRow && e.VisibleIndex % 2 == 0)
                {
                    e.CssClass = "alt-item";
                }
            };
            var dataGridAdapter = grid.GetGridAdapter();
            if (dataGridAdapter != null)
            {
                foreach (var col in dataGridAdapter.GridDataColumnModels)
                {
                    if (col.FieldName == "Description")
                    {
                        col.CellDisplayTemplate = context =>
                        {
                            var uc = (UseCase)context.DataItem;
                            return (RenderFragment)((builder) =>
                            {
                                builder.OpenElement(0, "text");
                                builder.AddMarkupContent(1, uc.Description);
                                builder.CloseElement();
                            });
                        };
                    }
                }
            }
        }
    }

    [ActivatorUtilitiesConstructor]
    public UseCaseListViewController(RedshiftService redshiftService) : this()
    {
        _redshiftService = redshiftService;
    }

    private async void UpdateFromDataProductAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        try
        {
            string json = await ReadJsonFromS3();

            if (!string.IsNullOrEmpty(json))
            {
                JObject root = JObject.Parse(json);
                var useCasesArray = root.SelectToken("dataUseCasesWithDevices.data") as JArray;

                if (useCasesArray != null)
                {
                    foreach (var useCaseToken in useCasesArray)
                    {
                        var useCaseId = useCaseToken["useCaseId"]?.ToString();
                        var name = useCaseToken["name"]?.ToString();
                        var description = useCaseToken["description"]?.ToString();

                        if (string.IsNullOrEmpty(useCaseId) || string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        var useCase = CreateOrLoad<UseCase>(CriteriaOperator.Parse(nameof(UseCase.ID) + " == ?", useCaseId));
                        useCase.IsActive = true;
                        useCase.Title = name;
                        useCase.Description = description ?? string.Empty;
                        useCase.ID = useCaseId;

                        var applicableDevices = useCaseToken["applicableDevices"] as JArray;
                        if (applicableDevices != null)
                        {
                            var existingDevices = useCase.DeviceDataAccessGrants.ToList();

                            foreach (var deviceToken in applicableDevices)
                            {
                                var serialNumber = deviceToken["serialNumber"]?.ToString();
                                var referenceSystemType = deviceToken["referenceSystemType"]?.ToString();

                                var device = existingDevices.FirstOrDefault(d => d.Serial == serialNumber && d.SystemType == referenceSystemType);
                                if (device == null)
                                {
                                    device = ObjectSpace.CreateObject<DeviceDataAccessGrant>();
                                    device.Serial = serialNumber;
                                    device.SystemType = deviceToken["systemType"]?.ToString(); 
                                    device.SystemReferenceType = referenceSystemType;
                                    useCase.DeviceDataAccessGrants.Add(device);
                                }
                                else
                                {
                                    existingDevices.Remove(device);
                                }

                                var applicablePeriods = deviceToken["applicablePeriods"] as JArray;
                                if (applicablePeriods != null)
                                {
                                    var existingPeriods = device.ValidityPeriods.ToList();

                                    foreach (var periodToken in applicablePeriods)
                                    {
                                        var startStr = periodToken["start"]?.ToString();
                                        var endStr = periodToken["end"]?.ToString();

                                        if (DateTime.TryParse(startStr, out DateTime start))
                                        {
                                            DateTime? end = null;
                                            if (!string.IsNullOrEmpty(endStr) && DateTime.TryParse(endStr, out DateTime endValue))
                                            {
                                                end = endValue;
                                            }

                                            var period = existingPeriods.FirstOrDefault(p => p.Start == start && p.End == end);
                                            if (period == null)
                                            {
                                                period = ObjectSpace.CreateObject<DeviceDataAccessGrantValidityPeriod>();
                                                period.Start = start;
                                                period.End = end;
                                                device.ValidityPeriods.Add(period);
                                            }
                                            else
                                            {
                                                existingPeriods.Remove(period);
                                            }
                                        }
                                    }

                                    foreach (var period in existingPeriods)
                                    {
                                        ObjectSpace.Delete(period);
                                    }
                                }
                                else
                                {
                                    foreach (var period in device.ValidityPeriods.ToList())
                                    {
                                        ObjectSpace.Delete(period);
                                    }
                                }
                            }

                            foreach (var device in existingDevices)
                            {
                                ObjectSpace.Delete(device);
                            }
                        }
                        else
                        {
                            foreach (var device in useCase.DeviceDataAccessGrants.ToList())
                            {
                                ObjectSpace.Delete(device);
                            }
                        }
                    }

                    FlagObsoleteUseCases(ObjectSpace, useCasesArray);
                    ObjectSpace.CommitChanges();
                }

                View.RefreshDataSource();
                View.Refresh();
            }
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("An error occurred while updating use cases.", ex);
        }
    }

    private void FlagObsoleteUseCases(IObjectSpace objectSpace, JArray useCaseTokens)
    {
        var existingUseCases = objectSpace.GetObjects<UseCase>();
        var useCaseIdsFromJson = useCaseTokens.Select(uc => uc["useCaseId"]?.ToString()).ToList();

        foreach (var existingUseCase in existingUseCases)
        {
            if (!useCaseIdsFromJson.Contains(existingUseCase.ID))
            {
                existingUseCase.IsActive = false;
            }
        }
    }

    private async Task<string> ReadJsonFromS3()
    {
        try
        {
            var config = ObjectSpace.GetObjects<RedshiftConfig>().Single();

            var credentials = FallbackCredentialsFactory.GetCredentials();

    
            Console.WriteLine("Using AWS Access Key ID: " + credentials.GetCredentials().AccessKey);


            var regionEndpoint = RegionEndpoint.GetBySystemName(config.Region);
            var s3Client = new AmazonS3Client(regionEndpoint);

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };
            
            using GetObjectResponse response = await s3Client.GetObjectAsync(request);
            using StreamReader reader = new StreamReader(response.ResponseStream);

            return reader.ReadToEnd();
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            return null;
        }
    }

    private T CreateOrLoad<T>(CriteriaOperator criteria) where T : class
    {
        var obj = ObjectSpace.FindObject<T>(criteria);
        return obj ?? ObjectSpace.CreateObject<T>();
    }


    public void ReportProgress(string message)
    {
        Application.ShowViewStrategy.ShowMessage(message, InformationType.Info);
    }
}