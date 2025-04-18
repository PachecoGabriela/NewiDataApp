using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Microsoft.Extensions.DependencyInjection;
using iData.Dp.AsapRecords.BusinessObjects;
using System.Globalization;
using System.Reflection;
using System.Text;
using CsvHelper;
using OfficeOpenXml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;

namespace iData.Dp.AsapRecords.DatabaseUpdate;

[ImageName("AsapEntry")]
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();

        if (!ObjectSpace.GetObjectsQuery<AsapEntryRecord>().Any())
        {
            ImportAsapEntriesFromCsv();
        }

        UpdateLoincCodesForExistingRecords();
    }


    private void UpdateLoincCodesForExistingRecords()
    {
        var asapEntries = ObjectSpace.GetObjectsQuery<AsapEntryRecord>().ToList();

        foreach (var entry in asapEntries)
        {
            entry.UpdateLoincCode();
        }
        ObjectSpace.CommitChanges();
    }

    private void ImportAsapEntriesFromCsv()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "iData.Dp.AsapRecords.Bootstrap.ASAPDatabase.xlsx";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheetC6888 = workbook.GetSheetAt(0); 

            for (int row = 2; row <= sheetC6888.LastRowNum; row++)
            {
                NPOI.SS.UserModel.IRow currentRow = sheetC6888.GetRow(row);
                if (currentRow != null)
                {
                    if (!string.IsNullOrEmpty(currentRow.GetCell(2)?.ToString()))
                    {
                        var ExistingLifeCycleTeam = ObjectSpace.FindObject<AsapLifeCycleTeam>(new BinaryOperator("LifeCycleTeam", currentRow.GetCell(0)?.ToString()));

                        AsapLifeCycleTeam LifeCycleTeam;

                        if (ExistingLifeCycleTeam is null)
                        {
                            LifeCycleTeam = ObjectSpace.CreateObject<AsapLifeCycleTeam>();
                            LifeCycleTeam.LifeCycleTeam = currentRow.GetCell(0)?.ToString();
                        }
                        else
                            LifeCycleTeam = ExistingLifeCycleTeam;

                        var ExistingAssay = ObjectSpace.FindObject<AsapAssay>(new BinaryOperator("Assay", currentRow.GetCell(1)?.ToString()));

                        AsapAssay Assay;

                        if (ExistingAssay is null)
                        {
                            Assay = ObjectSpace.CreateObject<AsapAssay>();
                            Assay.Assay = currentRow.GetCell(1)?.ToString();
                        }
                        else
                            Assay = ExistingAssay;

                        

                        AsapMappingTerm MappingTerm = ObjectSpace.CreateObject<AsapMappingTerm>();
                        MappingTerm.Term = currentRow.GetCell(2)?.ToString();
                        MappingTerm.IsPrimary = true;

                        var ExistingAsapEntry = ObjectSpace.FindObject<AsapEntryRecord>(new BinaryOperator("AsapName", currentRow.GetCell(2)?.ToString()));

                        AsapEntryRecord AsapEntry;

                        if (ExistingAsapEntry is null)
                        {
                            AsapEntry = ObjectSpace.CreateObject<AsapEntryRecord>();
                            AsapEntry.AsapName = currentRow.GetCell(2)?.ToString();
                            AsapEntry.LifeCycleTeam = LifeCycleTeam;
                            AsapEntry.Assay = Assay;
                            AsapEntry.SystemType = "Cobas 6800;Cobas 8800";
                            AsapEntry.USI = currentRow.GetCell(3)?.ToString();
                            AsapEntry.MappingTerms.Add(MappingTerm);
                        }
                        else
                            AsapEntry = ExistingAsapEntry;

                        AsapVersion Version;
                        Version = ObjectSpace.CreateObject<AsapVersion>();
                        Version.Version = currentRow.GetCell(4)?.ToString();
                        Version.MaterialNumber = currentRow.GetCell(5)?.ToString();
                        Version.AsapEntry = AsapEntry;

                        AsapEntry.Versions.Add(Version);

                        ObjectSpace.CommitChanges();
                    }
                }
            }

            ISheet sheetC5800 = workbook.GetSheetAt(1);

            for (int row = 2; row <= sheetC5800.LastRowNum; row++)
            {
                NPOI.SS.UserModel.IRow currentRow = sheetC5800.GetRow(row);
                if (currentRow != null)
                {
                    if (!string.IsNullOrEmpty(currentRow.GetCell(7)?.ToString()))
                    {
                        var ExistingLifeCycleTeam = ObjectSpace.FindObject<AsapLifeCycleTeam>(new BinaryOperator("LifeCycleTeam", currentRow.GetCell(0)?.ToString()));

                        AsapLifeCycleTeam LifeCycleTeam;

                        if (ExistingLifeCycleTeam is null)
                        {
                            LifeCycleTeam = ObjectSpace.CreateObject<AsapLifeCycleTeam>();
                            LifeCycleTeam.LifeCycleTeam = currentRow.GetCell(0)?.ToString();
                        }
                        else
                            LifeCycleTeam = ExistingLifeCycleTeam;

                        var ExistingAssay = ObjectSpace.FindObject<AsapAssay>(new BinaryOperator("Assay", currentRow.GetCell(1)?.ToString()));

                        AsapAssay Assay;

                        if (ExistingAssay is null)
                        {
                            Assay = ObjectSpace.CreateObject<AsapAssay>();
                            Assay.Assay = currentRow.GetCell(1)?.ToString();
                        }
                        else
                            Assay = ExistingAssay;

                        AsapMappingTerm MappingTermPrimary = ObjectSpace.CreateObject<AsapMappingTerm>();
                        MappingTermPrimary.Term = currentRow.GetCell(7)?.ToString();
                        MappingTermPrimary.IsPrimary = true;

                        string SWCobas5800 = currentRow.GetCell(7)?.ToString().Substring(0, 13);
                        string SWCobas = currentRow.GetCell(7)?.ToString().Substring(0, 9);
                        string ASAP = currentRow.GetCell(7)?.ToString().Substring(currentRow.GetCell(7).ToString().Length - 4, 4);
                        string TermName = string.Empty;
                        string c5800Term = string.Empty;

                        if (SWCobas5800 == "SW cobas 5800")
                        {
                            if (ASAP == "ASAP")
                            {
                                TermName = currentRow.GetCell(7)?.ToString().Remove(0, 14);
                                TermName = TermName.Remove(TermName.Length - 4);

                                c5800Term = string.Format("c5800 {0}", TermName);

                            }
                            else
                            {
                                TermName = currentRow.GetCell(7)?.ToString().Remove(0, 14);

                                c5800Term = string.Format("c5800 {0}", TermName);

                            }
                            
                        }
                        else
                        {
                            if (SWCobas5800 == "cobas 5800" || SWCobas5800 == "Cobas 5800")
                            {
                                if (ASAP == "ASAP")
                                {
                                    TermName = currentRow.GetCell(7)?.ToString().Remove(0, 10);
                                    TermName = TermName.Remove(TermName.Length - 4);

                                    c5800Term = string.Format("c5800 {0}", TermName);
                                }
                                else
                                {
                                    TermName = currentRow.GetCell(7)?.ToString().Remove(0, 10);

                                    c5800Term = string.Format("c5800 {0}", TermName);
                                }
                                
                            }
                            else
                            {
                                if (ASAP == "ASAP")
                                {
                                    TermName = currentRow.GetCell(7)?.ToString().Remove(currentRow.GetCell(7).ToString().Length - 5);

                                    c5800Term = string.Format("c5800 {0}", TermName);
                                }
                                    
                            }
                        }

                        AsapMappingTerm MappingTermSecondary = ObjectSpace.CreateObject<AsapMappingTerm>();
                        MappingTermSecondary.Term = TermName;
                        MappingTermSecondary.IsPrimary = false;

                        AsapMappingTerm MappingTermc5800 = ObjectSpace.CreateObject<AsapMappingTerm>();
                        MappingTermc5800.Term = c5800Term;
                        MappingTermc5800.IsPrimary = false;

                        var ExistingAsapEntry = ObjectSpace.FindObject<AsapEntryRecord>(new BinaryOperator("AsapName", currentRow.GetCell(7)?.ToString()));

                        AsapEntryRecord AsapEntry;

                        if (ExistingAsapEntry is null)
                        {
                            AsapEntry = ObjectSpace.CreateObject<AsapEntryRecord>();
                            AsapEntry.AsapName = currentRow.GetCell(7)?.ToString();
                            AsapEntry.LifeCycleTeam = LifeCycleTeam;
                            AsapEntry.Assay = Assay;
                            AsapEntry.SystemType = "Cobas 5800";
                            AsapEntry.USI = currentRow.GetCell(8)?.ToString();
                            AsapEntry.MappingTerms.Add(MappingTermPrimary);
                            AsapEntry.MappingTerms.Add(MappingTermSecondary);
                            AsapEntry.MappingTerms.Add(MappingTermc5800);
                        }
                        else
                            AsapEntry = ExistingAsapEntry;

                        AsapVersion Version = ObjectSpace.CreateObject<AsapVersion>();
                        Version.Version = currentRow.GetCell(9)?.ToString();
                        Version.MaterialNumber = currentRow.GetCell(10)?.ToString();
                        Version.AsapEntry = AsapEntry;

                        AsapEntry.Versions.Add(Version);

                        ObjectSpace.CommitChanges();
                    }  
                }
            }
        }

        
    }

    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
    }
}
