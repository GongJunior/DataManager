using DMCoreLibrary.Models;
using DMCoreLibrary.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace DMCoreLibrary.Connections
{
    public class SpreadsheetCollection
    {
        private SpreadsheetCollectionOptions Options { get; }

        public SpreadsheetCollection(SpreadsheetCollectionOptions collectionOptions)
        {
            Options = collectionOptions;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public void MergeDTfromFiles(IProgress<SpreadsheetCollectionProgressModel> progress, CancellationToken cancellationToken)
        {
            SpreadsheetCollectionProgressModel report = new SpreadsheetCollectionProgressModel();
            List<FileInfo> files = StringToFileInfo(Options.Files);
            string dir = files[0].DirectoryName;

            List<string> sheets = GenerateSheets();

            List<DataTable> tables = new List<DataTable>();
            DataTable errorTable = new DataTable();

            foreach (FileInfo file in files)
            {
                report.steps.Add($"Loading {file.Name}...");
                progress.Report(report);

                var package = SpreadsheetUtilities.GetDTfromExcel(file, Convert.ToInt32(Options.StartRow), sheets, Options.Passwords);
                tables.AddRange(package.data);
                errorTable.Merge(package.errors);
                cancellationToken.ThrowIfCancellationRequested();
            }

            //Combine all tables into first table in list
            for (int i = 1; i < tables.Count; i++)
            {
                report.steps.Add($"Merging {tables[i].TableName}...");
                progress.Report(report);
                tables[0].Merge(tables[i], false, MissingSchemaAction.Add);
                tables[i] = null;
                cancellationToken.ThrowIfCancellationRequested();
            }

            report.IsCancellable = false;
            report.steps.Add("Sending to Excel...");
            progress.Report(report);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                SpreadsheetUtilities.DTtoExcel(tables[0], dir, errorTable);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Sheet(s) not found in any files!", e);
            }
            catch (OperationCanceledException)
            {
                throw;
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
    }
}
