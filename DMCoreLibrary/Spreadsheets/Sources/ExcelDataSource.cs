using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using ExcelDataReader.Exceptions;

namespace DMCoreLibrary.Spreadsheets.Sources
{
    class ExcelDataSource : DataSource
    {
        private FileInfo location;
        private List<string> actualSheets = new List<string>();
        private List<string> requestedSheets = new List<string>();
        private bool UseAllSheets;
        private List<string> possiblePasswords = new List<string>();

        public ExcelDataSource(FileInfo location, List<string> requestedSheets,int startingRow):base()
        {
            this.startingRow = startingRow;
            this.location = location;
            this.requestedSheets = requestedSheets;
            UseAllSheets = (requestedSheets[0] == "allsheets") ? true : false;
        }
        public ExcelDataSource(FileInfo location, List<string> requestedSheets,int startingRow, List<string> passwords):base()
        {
            this.startingRow = startingRow;
            this.location = location;
            this.requestedSheets = requestedSheets;
            UseAllSheets = (requestedSheets[0] == "allsheets") ? true : false;
            possiblePasswords = passwords;
        }

        public override (List<DataTable> data, DataTable errors) ParseData()
        {
            var config = TryPossiblePasswords();
            using (var stream = File.Open(location.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream,config))
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
                            // TODO: Resolve issue of file is active blank columns to end of sheet (xlsx max col: 16,384)
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
        private ExcelReaderConfiguration TryPossiblePasswords()
        {
            foreach (var pw in possiblePasswords)
            {
                try
                {
                    var config = new ExcelReaderConfiguration() { Password = pw };
                    using (var stream = File.Open(location.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream, config))
                        {

                        }
                    }
                    return config;
                }
                catch (InvalidPasswordException)
                {

                    continue;
                }
            }
            return new ExcelReaderConfiguration();
        }
    }
}
