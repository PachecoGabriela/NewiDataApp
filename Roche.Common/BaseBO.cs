using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Roche.Common.AuditTrail;
using System.Runtime.CompilerServices;

namespace Roche.Common
{
    public interface IReadOnlyEntity
    {
        
    }

    [NonPersistent]
    [XafDefaultProperty(nameof(DisplayName))]
    public abstract class BaseBO : BaseObject
    {
        protected BaseBO(Session session) : base(session) { }

        string recordIdentifier;
        string updateUser;
        string insertUser;
        private DateTime insertTimestamp;
        private DateTime updateTimestamp;


        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false), AuditTrailDisable]
        [DisplayName("Inserted on")]
        [ModelDefault("DisplayFormat", "{0:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'}")]
        [NonCloneable]
        public DateTime InsertTimestamp
        {
            get => insertTimestamp;
            set => SetPropertyValue(ref insertTimestamp, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false), AuditTrailDisable]
        [DevExpress.Xpo.DisplayName("Inserted by")]
        [NonCloneable]
        public string InsertUser
        {
            get => insertUser;
            set => SetPropertyValue(ref insertUser, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false), AuditTrailDisable]
        [DevExpress.Xpo.DisplayName("Updated on")]
        [ModelDefault("DisplayFormat", "{0:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'}")]
        [NonCloneable]
        public DateTime UpdateTimestamp
        {
            get => updateTimestamp;
            set => SetPropertyValue(ref updateTimestamp, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false), AuditTrailDisable]
        [DevExpress.Xpo.DisplayName("Updated by")]
        [NonCloneable]
        public string UpdateUser
        {
            get => updateUser;
            set => SetPropertyValue(ref updateUser, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false), VisibleInListView(false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [NonCloneable]
        public string RecordIdentifier
        {
            get => recordIdentifier;
            set => SetPropertyValue(ref recordIdentifier, value);
        }

        protected override void OnSaving()
        {
            if (Session.IsNewObject(this))
            {
                insertTimestamp = DateTime.Now.ToUniversalTime();
                insertUser = SecuritySystem.CurrentUserName;
                if (string.IsNullOrEmpty(RecordIdentifier))
                {
                    int nextSequence = DistributedIdGeneratorHelper.Generate(Session.DataLayer, this.GetType().Name);
                    RecordIdentifier = string.Format("iData_{0}_{1:D6}", GetCodeElementName(), nextSequence);
                }
            }
            updateTimestamp = DateTime.Now.ToUniversalTime();
            updateUser = SecuritySystem.CurrentUserName;

            base.OnSaving();
        }

        protected virtual string GetCodeElementName() => GetType().Name;

        [DisplayName("Change log")]
        [ModelDefault("AllowLink", "False")]
        public XPCollection<AuditDataItemPersistent> AuditTrail => AuditedObjectWeakReference.GetAuditTrail(Session, this);

        protected bool HasWritePermission(string property)
        {
            return SecuritySystem.IsGranted(new PermissionRequest(XPObjectSpace.FindObjectSpaceByObject(this), GetType(), SecurityOperations.Write, this, property));
        }


        [NonPersistent]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public abstract string DisplayName { get; }

        public override string ToString() => DisplayName;

        protected XPCollection<T> GetCollection<T>(SortProperty[] sortProperties, [CallerMemberName] string propertyName = null) where T : class
        {
            var coll = base.GetCollection<T>(propertyName);
            coll.Sorting.AddRange(sortProperties);
            return coll;
        }

        protected bool SetPropertyValue<T>(ref T propertyValueHolder, T newValue, [CallerMemberName] string propertyName = null)
            => base.SetPropertyValue<T>(propertyName, ref propertyValueHolder, newValue);

        protected bool SetDelayedPropertyValue<T>(T value, [CallerMemberName] string propertyName = null)
            => base.SetDelayedPropertyValue(propertyName, value);
    }
}
