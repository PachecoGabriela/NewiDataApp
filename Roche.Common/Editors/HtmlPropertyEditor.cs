using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Components;

namespace Roche.Common.Editors
{
    [PropertyEditor(typeof(string), false)]
    [EditorAlias("HtmlPropertyEditor")]
    public class HtmlPropertyEditor : BlazorPropertyEditorBase
    {
        public HtmlPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        public override DxHtmlEditorModel ComponentModel => (DxHtmlEditorModel)base.ComponentModel;

        protected override IComponentModel CreateComponentModel()
        {
            var model = new DxHtmlEditorModel()
            {
                Height = "230px"
            };

            model.MarkupChanged = EventCallback.Factory.Create<string>(this, value => {
                model.Markup = value;
                OnControlValueChanged();
                WriteValue();
            });

            return model;
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();

            ComponentModel.Markup = (string)PropertyValue ?? String.Empty;
        }

        protected override object GetControlValueCore() => ComponentModel.Markup;
    }
}
