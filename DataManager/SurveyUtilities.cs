using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace DataManager
{
    public class SurveyUtilities
    {
        #region METHODS
        //get DataTable from excel example
        public static DataTable GetDTfromExcel(FileInfo xlFile, int startRow, string sheetName)
        {
            //open workbook!
            //FileInfo xlFile = new FileInfo(loc);
            DataTable dt = new DataTable();
            using (ExcelPackage pck = new ExcelPackage(xlFile))
            {
                //get first sheet & pull into data table
                ExcelWorksheet ws = pck.Workbook.Worksheets[sheetName]; //1 is the first worksheet in list - Add to args list to take user input

                //Create table columns using for loop from user defined start row
                //find beginning & end of sheet
                var sr = ws.Dimension.Start.Row;
                var sc = ws.Dimension.Start.Column;
                var er = ws.Dimension.End.Row;
                var ec = ws.Dimension.End.Column;

                //define columns for datatable Cells[start row, start column, max Row, max Column]
                dt.TableName = xlFile.Name;
                foreach (var header in ws.Cells[startRow, 1, 1, ec])
                {
                    DataColumn newCol = new DataColumn();
                    if (NumCheck(header.Text.ToLower()))
                    {
                        newCol.DataType = System.Type.GetType("System.Double"); //Added to guess datatype based on header text
                    }
                    newCol.ColumnName = (string)header.Text;
                    dt.Columns.Add(newCol);
                    //dt.Columns.Add((string)header.Text); //working original
                }

                //adds column reflecting file/sheet name
                DataColumn tagColumn = new DataColumn
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = "File ID Tag",
                    DefaultValue = xlFile.Name
                };
                dt.Columns.Add(tagColumn);
                DataColumn tagColumnSheet = new DataColumn
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = "Sheet ID Tag",
                    DefaultValue = sheetName
                };
                dt.Columns.Add(tagColumnSheet);



                //add rows in the same order as column titles
                for (int rowNum = startRow + 1; rowNum <= er; rowNum++)
                {
                    DataRow row = dt.NewRow();
                    foreach (var cell in ws.Cells[rowNum, 1, rowNum, ec])
                    {
                        try
                        {
                            row[cell.Start.Column - 1] = cell.Value; // row[index] starts at zero
                        }
                        catch(ArgumentException)
                        {
                            break;
                        }
                    }
                    dt.Rows.Add(row);  //necessary to add for after all fields are set
                }
            }
            return dt;
        }
        
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

        // Generates list of files in a given directory
        public static List<FileInfo> GetFiles(DirectoryInfo dir)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Name.EndsWith(".xlsx")) files.Add(file);
            }
            return files;
        }
        
        //Generates list of sheets in a given file
        public static List<string> GetSheets(FileInfo file)
        {
            using (ExcelPackage pkg = new ExcelPackage(file))
            {
                var ws = pkg.Workbook.Worksheets;
                List<string> allSheets = new List<string>();
                foreach (var s in ws) allSheets.Add(s.ToString());
                //for (int i = 1; i <= ws.Count; i++) Console.WriteLine($"{ws[i].Name}");
                return allSheets;
            }
        }

        public static bool NumCheck(string header)
        {
            //check if columnn should be changed to number
            string regexString = @"\w*\s*([1-95]|av|med|min|mid|max|elig|rec|%|per|exempt|obs|emp)\w*\s*";
            RegexStringValidator regexTest = new RegexStringValidator(regexString);
            try
            {
                regexTest.Validate(header);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static DataTable MakeErrorTable()
        {
            DataTable table = new DataTable("ERRORS");
            DataColumn column;

            List<string> columnNames = new List<string> { "File Name", "Sheet Name", "Message" };

            foreach (string name in columnNames)
            {
                column = new DataColumn
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = name
                };
                table.Columns.Add(column);
            }
            return table;
        }

        #endregion
    }

    
}
