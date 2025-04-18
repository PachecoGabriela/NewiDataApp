using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using iData.Dp.AsapRecords.BusinessObjects; // Cambia a tu namespace real
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Blazor.Components.Models;
using iData.Blazor.Server.Components;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;

namespace iData.Blazor.Server.Editors
{
    [PropertyEditor(typeof(AsapAssay), "DropDownEditor", false)]
    public class CustomDropdownPropertyEditor : BlazorPropertyEditorBase
    {
        private readonly IObjectSpace objectSpace;
        public CustomDropdownPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) { }

        public override InputSelectModel<AsapAssay> ComponentModel => (InputSelectModel<AsapAssay>)base.ComponentModel;

        protected override IComponentModel CreateComponentModel()
        {

            var model = new InputSelectModel<AsapAssay>();

            model.ValueExpression = () => model.Value;
            model.ValueChanged = EventCallback.Factory.Create<AsapAssay>(this, value => {
                model.Value = value;
                OnControlValueChanged();
                WriteValue();
            });

            if (objectSpace != null)
            {
                model.Options = objectSpace.GetObjects<AsapAssay>().ToList();
            }


            return model;
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            ComponentModel.Value = (AsapAssay)PropertyValue;
        }

        protected override object GetControlValueCore()
        {
            return ComponentModel.Value;
        }

        protected override void ApplyReadOnly()
        {
            base.ApplyReadOnly();
            ComponentModel?.SetAttribute("disabled", !AllowEdit);
        }
    }

    public class InputSelectModel<T> : ComponentModelBase
    {
        public T Value
        {
            get => GetPropertyValue<T>();
            set => SetPropertyValue(value);
        }

        public EventCallback<T> ValueChanged
        {
            get => GetPropertyValue<EventCallback<T>>();
            set => SetPropertyValue(value);
        }

        public Expression<Func<T>> ValueExpression
        {
            get => GetPropertyValue<Expression<Func<T>>>();
            set => SetPropertyValue(value);
        }

        public List<T> Options
        {
            get => GetPropertyValue<List<T>>();
            set => SetPropertyValue(value);
        }

        public override Type ComponentType => typeof(InputSelect<T>);
    }
}
