using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace DataManager
{
    class CsvDataSource : DataSource
    {
        private FileInfo location;

        public CsvDataSource(FileInfo location, int startingRow):base()
        {
            this.startingRow = startingRow;
            this.location = location;
        }

        public override (List<DataTable> data, DataTable errors) ParseData()
        {
            using (var stream = File.Open(location.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    do
                    {
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
                    } while (reader.NextResult());
                }
            }
            return (tables, errorTable);
        }
    }
}
