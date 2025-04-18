using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Blazor.Components.Models;

namespace Roche.Common.Editors
{
    [PropertyEditor(typeof(string), false)]
    public class BlazorPasswordPropertyEditor : StringPropertyEditor
    {
        public BlazorPasswordPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override void OnControlCreated()
        {
            base.OnControlCreated();

            ((DxTextBoxModel)ComponentModel).SetAttribute("type", "password");
        }
    }
}
