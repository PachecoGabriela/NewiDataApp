using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace iData.Dp.CompGov.Editors;

[PropertyEditor(typeof(string), false)]
public class RedshiftViewer(Type objectType, IModelMemberViewItem model) : BlazorPropertyEditorBase(objectType, model)
{
    public override ValueModel ComponentModel => (ValueModel)base.ComponentModel;
    protected override IComponentModel CreateComponentModel()
    {
        var model = new ValueModel();
        model.ValueExpression = () => model.Value;
        model.ValueChanged = EventCallback.Factory.Create<string>(this, value =>
        {
            model.Value = value;
            OnControlValueChanged();
            WriteValue();
        });
        return model;
    }
    protected override void ReadValueCore()
    {
        base.ReadValueCore();
        ComponentModel.Value = (string)PropertyValue;
    }
    protected override object GetControlValueCore() => ComponentModel.Value;
    protected override void ApplyReadOnly()
    {
        base.ApplyReadOnly();
        ComponentModel?.SetAttribute("readonly", !AllowEdit);
    }

    public class ValueModel : ComponentModelBase
    {
        public string Value
        {
            get => GetPropertyValue<string>();
            set => SetPropertyValue(value);
        }
        public EventCallback<string> ValueChanged
        {
            get => GetPropertyValue<EventCallback<string>>();
            set => SetPropertyValue(value);
        }
        public Expression<Func<string>> ValueExpression
        {
            get => GetPropertyValue<Expression<Func<string>>>();
            set => SetPropertyValue(value);
        }
        public override Type ComponentType => typeof(RedshiftComponent);
    }
}
