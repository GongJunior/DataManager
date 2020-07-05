using DMCoreLibrary.Events;
using DMCoreLibrary.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DMCoreLibrary.Connections
{
    partial class SpreadsheetCollection
    {
        private SpreadsheetCollectionOptions Options { get; }
        public event EventHandler<CancelEventArgs>? CheckCancel;
        public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;

        public SpreadsheetCollection(SpreadsheetCollectionOptions collectionOptions)
        {
            Options = collectionOptions;
        }

        public void MergeDTfromFiles()
        {
            //Convert filesList to FileInfo & find cwd
            List<FileInfo> files = StringToFileInfo(Options.Files);
            string dir = files[0].DirectoryName;

            //generates sheet list if necessary
            List<string> sheets = GenerateSheets();

            //Generate list of DataTables from excel data
            List<DataTable> tables = new List<DataTable>();
            DataTable errorTable = new DataTable();
            foreach (FileInfo file in files)
            {
                OnProgressChanged(1, $"Loading {file.Name}...");

                var package = SpreadsheetUtilities.GetDTfromExcel(file, Convert.ToInt32(Options.StartRow), sheets, Options.Passwords);
                tables.AddRange(package.data);
                errorTable.Merge(package.errors);
                if (OnCheckCancel())
                {
                    return;
                }
            }

            //Combine all tables into first table in list
            for (int i = 1; i < tables.Count; i++)
            {
                OnProgressChanged(2, $"Merging {tables[i].TableName}...");
                tables[0].Merge(tables[i], false, MissingSchemaAction.Add);
                if (OnCheckCancel())
                {
                    return;
                }
                tables[i] = null;
            }

            OnProgressChanged(3, "Sending to Excel...");
            try
            {
                SpreadsheetUtilities.DTtoExcel(tables[0], dir, errorTable);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new System.ArgumentOutOfRangeException("Sheet(s) not found in any files!", e);
            }

        }


        private List<string> GenerateSheets()
        {
            if (Options.Sheets.ToLower() == "allsheets")
            {
                List<string> sheets = new List<string>();
                sheets.Add("allsheets");
                return sheets;
            }
            else
            {
                List<string> sheets = Splitter();
                return sheets;
            }
        }
        private List<FileInfo> StringToFileInfo(List<string> stringList)
        {

            List<FileInfo> files = new List<FileInfo>();
            foreach (string file in stringList)
            {
                FileInfo f = new FileInfo(file);
                files.Add(f);
            }
            return files;
        }
        private List<string> Splitter()
        {
            Char delimiter = ',';
            String[] extractedSheets = Options.Sheets.Split(delimiter);
            List<string> sheetList = new List<string>();
            foreach (string sheet in extractedSheets)
            {
                sheetList.Add(sheet.Trim());
            }
            return sheetList;
        }

        protected virtual bool OnCheckCancel()
        {
            EventHandler<CancelEventArgs>? handler = CheckCancel;
            if (handler != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                handler(this, e);
                return e.Cancel;
            }
            return false;
        }


        protected virtual void OnProgressChanged(int part, string message)
        {
            EventHandler<ProgressChangedEventArgs>? handler = ProgressChanged;
            if (ProgressChanged != null)
            {
                handler(this, new ProgressChangedEventArgs(part, message));
            }
        }

    }
}
