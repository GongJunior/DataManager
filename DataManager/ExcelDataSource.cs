using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace DataManager
{
    class ExcelDataSource : DataSource
    {
        private FileInfo location;
        private List<string> actualSheets = new List<string>();
        private List<string> requestedSheets = new List<string>();
        private bool UseAllSheets;

        public ExcelDataSource(FileInfo location, List<string> requestedSheets,int startingRow):base()
        {
            this.startingRow = startingRow;
            this.location = location;
            this.requestedSheets = requestedSheets;
            UseAllSheets = (requestedSheets[0] == "allsheets") ? true : false;
        }

        public override (List<DataTable> data, DataTable errors) ParseData()
        {
            using (var stream = File.Open(location.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        if (!UseAllSheets && !requestedSheets.Contains(reader.Name)) continue;

                        DataTable table = new DataTable();
                        bool headerRow = true;
                        int startRow_check = 1;
                        int BlankRow = 0;

                        while (reader.Read())
                        {
                            if (startRow_check < startingRow)
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
                            for (int i = 0; i < BlankRow; i++)
                            {
                                table.Rows.Add(table.NewRow());
                            }
                            BlankRow = 0;

                            table = AddToTable(table, rowData, location.Name, reader.Name);
                        }
                        tables.Add(table);
                        actualSheets.Add(reader.Name);
                    } while (reader.NextResult());
                }
            }
            errorTable = RequestedSheetsCheck(errorTable, actualSheets, requestedSheets, location.Name);
            return (tables, errorTable);
        }
    }
}
