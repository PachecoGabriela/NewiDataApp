using DevExpress.Blazor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using iData.Dp.CompGov.Entities;
using iData.Dp.CompGov.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iData.Dp.CompGov.Controller
{
    public class IntakeRequestUsecaseCandidateListViewController : ObjectViewController<ListView, IntakeRequestUsecaseCandidate>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View.Editor is DxGridListEditor grid)
            {
                grid.GridModel.ShowAllRows = true;       
                var dataGridAdapter = grid.GetGridAdapter();
                if (dataGridAdapter != null)
                {
                    foreach (var col in dataGridAdapter.GridDataColumnModels)
                    {
                        if (new[] { "Description", "Summary", "BusinessValue", "AdditionalMaterialLinks", "Notes" }.Contains(col.FieldName))
                        {
                            col.CellDisplayTemplate = context =>
                            {
                                var uc = (IntakeRequestUsecaseCandidate)context.DataItem;
                                var propertyValue = uc.GetType().GetProperty(col.FieldName)?.GetValue(uc, null)?.ToString();

                                return (RenderFragment)((builder) =>
                                {
                                    builder.OpenElement(0, "text");
                                    builder.AddMarkupContent(1, propertyValue ?? "");
                                    builder.CloseElement();
                                });
                            };
                        }
                    }
                }
            }
        }
    }
}