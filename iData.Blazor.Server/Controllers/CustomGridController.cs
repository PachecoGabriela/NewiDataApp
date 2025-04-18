using DevExpress.Blazor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.SystemModule;

namespace iData.Blazor.Server.Controllers
{
    public class CustomGridController : ViewController<ListView>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View.Editor is DxGridListEditor grid)
            {
                grid.GridModel.FilterMenuButtonDisplayMode = GridFilterMenuButtonDisplayMode.Always;
                grid.GridModel.ColumnResizeMode = GridColumnResizeMode.NextColumn;

                var dataGridAdapter = grid.GetGridAdapter();
                if (dataGridAdapter != null)
                {
                    dataGridAdapter.GridModel.SelectAllCheckboxMode = GridSelectAllCheckboxMode.AllPages;
                    //var numVisible = dataGridAdapter.GridDataColumnModels.Count(m => m.VisibleIndex > -1);

                    //var size = 1600 / numVisible;
                    //foreach (var col in dataGridAdapter.GridDataColumnModels)
                    //{
                    //    col.Width ??= size.ToString("F0"); 
                    //}
                    //dataGridAdapter.ComponentCaptured += (o, e) =>
                    //{
                    //    e.Grid.AutoFitColumnWidths();
                    //};

                    //grid.GridModel.CustomizeElement = e =>
                    //{
                    //    if (e.ElementType == GridElementType.DataRow && e.VisibleIndex % 2 == 0)
                    //    {
             
                    //    }
                    //};
                }
            }
        }
    }
}
