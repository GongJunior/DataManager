using System;
using System.Collections.Generic;
using System.Data;

namespace DMCoreLibrary.Spreadsheets.Sources
{
    abstract class DataSource
    {
        protected List<DataTable> tables = new List<DataTable>();
        protected int startingRow;
        protected DataTable errorTable;
        public DataSource()
        {
            errorTable = MakeErrorTable();
        }

        public abstract (List<DataTable> data, DataTable errors) ParseData();

        protected virtual DataTable RequestedSheetsCheck(DataTable errorTable, List<string> actualSheets, List<string> requsetedSheets, string filename)
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

         protected DataTable MakeErrorTable()
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

         protected virtual bool RowIsNull(IDataRecord row)
        {
            for (int i = 0; i < row.FieldCount; i++)
            {
                if (row.GetValue(i) != null) return false;
            }
            return true;
        }

         protected virtual DataTable AddToTable(DataTable table, IDataRecord row, string filename, string sheetname)
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

         protected virtual DataTable CreateTable(DataTable table, IDataRecord row)
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
                catch (InvalidCastException)
                {
                    DataColumn column = new DataColumn(row.GetValue(i).ToString(), typeof(object));
                    table.Columns.Add(column);
                }


            }

            return table;
        }
    }
}
