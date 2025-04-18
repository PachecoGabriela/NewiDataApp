using DevExpress.Xpo;
using DevExpress.Persistent.Validation;

namespace iData.Dp.CompGov.Entities
{
    public class RedshiftConfig : XPCustomObject
    {
        private string region;

        private string workGroupName;
        private string schema;
        private string databaseName;
        private string secretArn;
        private string externalSchema;

        public RedshiftConfig(Session session) : base(session) { }

        [Key(AutoGenerate = true), System.ComponentModel.Browsable(false)]
        public int Id { get; set; }

        private string title;
        public string Title
        {
            get => title;
            set => SetPropertyValue(nameof(Title), ref title, value);
        }

        [RuleRequiredField("RedshiftConfig_Region_Required", DefaultContexts.Save)]
        public string Region
        {
            get => region;
            set => SetPropertyValue(nameof(Region), ref region, value);
        }


        [RuleRequiredField("RedshiftConfig_SecretArn_Required", DefaultContexts.Save)]
        [DisplayName("Secret ARN")]
        public string SecretArn
        {
            get => secretArn;
            set => SetPropertyValue(nameof(SecretArn), ref secretArn, value);
        }

        [RuleRequiredField("RedshiftConfig_WorkGroupName_Required", DefaultContexts.Save)]
        public string WorkGroupName
        {
            get => workGroupName;
            set => SetPropertyValue(nameof(WorkGroupName), ref workGroupName, value);
        }

        [RuleRequiredField("RedshiftConfig_DatabaseName_Required", DefaultContexts.Save)]
        public string DatabaseName
        {
            get => databaseName;
            set => SetPropertyValue(nameof(DatabaseName), ref databaseName, value);
        }

        [RuleRequiredField("RedshiftConfig_Schema_Required", DefaultContexts.Save)]
        public string Schema
        {
            get => schema;
            set => SetPropertyValue(nameof(Schema), ref schema, value);
        }

        [RuleRequiredField("RedshiftConfig_ExternalSchema_Required", DefaultContexts.Save)]
        public string ExternalSchema
        {
            get => externalSchema;
            set => SetPropertyValue(nameof(ExternalSchema), ref externalSchema, value);
        }

        public void LogProperties()
        {
            Console.WriteLine($"RedshiftConfig");
            Console.WriteLine($"Id: {Id}");
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Region: {Region}");
            Console.WriteLine($"WorkGroupName: {WorkGroupName}");
            Console.WriteLine($"Schema: {Schema}");
            Console.WriteLine($"DatabaseName: {DatabaseName}");
            Console.WriteLine($"SecretArn: {SecretArn}");
            Console.WriteLine($"ExternalSchema: {ExternalSchema}");
        }
    }
}
