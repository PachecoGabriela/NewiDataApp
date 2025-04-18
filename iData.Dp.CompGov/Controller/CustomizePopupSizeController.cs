using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Templates;

namespace iData.Dp.CompGov.Controller
{
    public class CustomizePopupSizeController : WindowController
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            Window.TemplateChanged += Window_TemplateChanged;
        }
        private void Window_TemplateChanged(object sender, EventArgs e)
        {
            
            if (Window.Template is IPopupWindowTemplateSize size
                && Window.View.Id == "UseCaseSpecificDataProductEndpoint_DetailView")
            {
                size.MaxWidth = "100vw";
                size.Width = "1500px";
                size.MaxHeight = "80vh";
                size.Height = "800px";
            }
        }
        protected override void OnDeactivated()
        {
            Window.TemplateChanged -= Window_TemplateChanged;
            base.OnDeactivated();
        }
    }
}