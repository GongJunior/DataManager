using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using OfficeOpenXml;

namespace DataManager
{
    public class SurveyUtilities
    {
        #region FromFileToDT
        
        public static List<DataTable> GetDTfromExcel(FileInfo xlFile, int startRow, List<string> requsetedSheets)
        {
            List<DataTable> tables = new List<DataTable>();
            DataTable errorTable = MakeErrorTable();
            List<string> actualSheets = new List<string>();
            bool UseAllSheets;
            UseAllSheets = (requsetedSheets[0] == "allsheets") ? true : false;

            using (var stream = File.Open(xlFile.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        if (!UseAllSheets && !requsetedSheets.Contains(reader.Name)) continue;
                        
                        DataTable table = new DataTable();
                        bool headerRow = true;
                        int startRow_check = 1;
                        int BlankRow = 0;

                        while (reader.Read())
                        {
                            if (startRow_check < startRow)
                            {
                                startRow_check++;
                                continue;
                            }

                            IDataRecord rowData = reader;
                            if (headerRow == true)
                            {
                                // make column headers for datatable
                                table = CreateTable(table, rowData);
                                headerRow = false;
                                continue;
                            }

                            // add blank or actual data rows
                            if (RowIsNull(rowData))
                            {
                                BlankRow++;
                                continue;
                            }
                            for(int i = 0; i< BlankRow; i++)
                            {
                                table.Rows.Add(table.NewRow());
                            }
                            BlankRow = 0;

                            table = AddToTable(table, rowData, xlFile.Name, reader.Name);
                        }
                        tables.Add(table);
                        actualSheets.Add(reader.Name);
                    } while (reader.NextResult());
                }
            }
            errorTable = RequestedSheetsCheck(errorTable, actualSheets, requsetedSheets, xlFile.Name);
            tables.Add(errorTable);
            return tables;
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

        #region Helpers
        private static DataTable RequestedSheetsCheck(DataTable errorTable, List<string> actualSheets, List<string> requsetedSheets, string filename)
        {
            foreach (string sheet in requsetedSheets)
            {
                if (!actualSheets.Contains(sheet))
                {
                    DataRow tableRow = errorTable.NewRow();
                    tableRow["File Name"] = filename;
                    tableRow["Sheet Name"] = sheet;
                    tableRow["Message"] = "Sheet not found";
                    errorTable.Rows.Add(tableRow);
                }
            }
            return errorTable;
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

        private static bool RowIsNull(IDataRecord row)
        {
            for(int i = 0; i < row.FieldCount; i++)
            {
                if (row.GetValue(i) != null) return false;
            }
            return true;
        }

        private static DataTable AddToTable(DataTable table, IDataRecord row, string filename, string sheetname)
        {
            DataRow tableRow = table.NewRow();
            tableRow["File_tag"] = filename;
            tableRow["Sheet_tag"] = sheetname;
            for (int i = 2; i < table.Columns.Count; i++)
            {
                tableRow[i] = row.GetValue(i - 2);
            }
            table.Rows.Add(tableRow);

            return table;
        }

        private static DataTable CreateTable(DataTable table, IDataRecord row)
        {
            DataColumn fileColumn = new DataColumn("File_Tag", typeof(string));
            table.Columns.Add(fileColumn);
            DataColumn sheetColumn = new DataColumn("Sheet_Tag", typeof(string));
            table.Columns.Add(sheetColumn);

            for (int i = 0; i < row.FieldCount; i++)
            {
                int dupNameCounter = 1;

                try
                {
                    DataColumn column = new DataColumn(row.GetString(i), typeof(object));
                    table.Columns.Add(column);
                }
                catch (ArgumentException)
                {
                    //should catch null returned for column name
                    DataColumn column = new DataColumn("Column" + i.ToString(), typeof(object));
                    table.Columns.Add(column);
                }
                catch (DuplicateNameException)
                {
                    DataColumn column = new DataColumn(row.GetString(i) + dupNameCounter.ToString(), typeof(object));
                    table.Columns.Add(column);
                    dupNameCounter++;
                }


            }

            return table;
        }
        #endregion
    }


}
