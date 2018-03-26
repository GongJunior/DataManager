using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace DataManager
{
    public class SurveyUtilities
    {
        #region METHODS
        
        public static DataTable GetDTfromExcel(FileInfo xlFile, int startRow, string sheetName)
        {
            DataTable dt = new DataTable();
            using (ExcelPackage pck = new ExcelPackage(xlFile))
            {
                //get first sheet & pull into data table
                ExcelWorksheet ws = pck.Workbook.Worksheets[sheetName]; //1 is the first worksheet in list
                
                //find beginning & end of sheet
                var sr = ws.Dimension.Start.Row;
                var sc = ws.Dimension.Start.Column;
                var er = ws.Dimension.End.Row;
                var ec = ws.Dimension.End.Column;

                //define columns for datatable Cells[start row, start column, max Row, max Column]
                dt.TableName = xlFile.Name + ws.Name;

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

                int i = 1;
                foreach (var header in ws.Cells[startRow, 1, startRow, ec])
                {
                    try
                    {
                        DataColumn newCol = new DataColumn();
                        dt.Columns.Add((string)header.Text);
                    }
                    catch (System.Data.DuplicateNameException) //manages repeate columns in same table
                    {
                        DataColumn newCol = new DataColumn();
                        dt.Columns.Add((string)header.Text + "." + i.ToString());
                        i++;
                    }
                        
                        
                }

                //add rows in the same order as column titles
                //shifted columns +2 to account for tag columns
                int dtEnd = dt.Columns.Count-1;
                for (int rowNum = startRow + 1; rowNum <= er; rowNum++)
                {
                    DataRow row = dt.NewRow();
                    foreach (var cell in ws.Cells[rowNum, 1, rowNum, dtEnd]) //switched to dtEnd dut to issue where cell ec goes further than column headers created
                    {
                        try
                        {
                            row[cell.Start.Column + 1] = cell.Value; // row[index] starts at zero
                        }
                        catch(ArgumentException)
                        {
                            break;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            //added due to inconsistency of column range
                            break;
                        }

                    }
                    dt.Rows.Add(row);
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
                return allSheets;
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
