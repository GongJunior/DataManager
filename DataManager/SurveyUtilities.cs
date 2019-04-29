using System.Collections.Generic;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace DataManager
{
    public class SurveyUtilities
    {
        #region FromFileToDT
        
        public static (List<DataTable> data, DataTable errors) GetDTfromExcel(FileInfo fullFilePath, int startRow, List<string> requsetedSheets)
        {
            DataSource data = ValidateDataSource(fullFilePath, startRow, requsetedSheets);
            DataSourceInterpreter interprety = new DataSourceInterpreter(data);
            return interprety.ReadData();
        }

        private static DataSource ValidateDataSource(FileInfo fullFilePath, int startRow, List<string> requsetedSheets)
        {
            if (fullFilePath.Name.ToLower().EndsWith(".csv"))
            {
                return new CsvDataSource(fullFilePath, startRow);
            }
            
            return new ExcelDataSource(fullFilePath, requsetedSheets, startRow);
        }
        #endregion

        #region WriteToFile
        //add DataTable to file
        public static void DTtoExcel(DataTable dt, string loc)
        {
            using (ExcelPackage pkg = new ExcelPackage())
            {
                ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Merge Output");

                ws.Cells["A1"].LoadFromDataTable(dt, true);


                //should join dir + filename for finFile
                string tempFile = Path.Combine(loc, "RESULT.xlsx");
                FileInfo finFile = new FileInfo(tempFile);
                if (finFile.Exists) finFile.Delete();
                pkg.SaveAs(finFile);
            }
        }
        //add DataTable to file with error sheet
        public static void DTtoExcel(DataTable dt, string loc, DataTable dtErrors)
        {
            using (ExcelPackage pkg = new ExcelPackage())
            {
                ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Merge Output");
                ExcelWorksheet wsErrors = pkg.Workbook.Worksheets.Add("ERRORS");

                ws.Cells["A1"].LoadFromDataTable(dt, true);
                wsErrors.Cells["A1"].LoadFromDataTable(dtErrors, true);

                //should join dir + filename for finFile
                string tempFile = Path.Combine(loc, "RESULT.xlsx");
                FileInfo finFile = new FileInfo(tempFile);
                if (finFile.Exists) finFile.Delete();
                pkg.SaveAs(finFile);
            }
        }
        #endregion

    }


}
