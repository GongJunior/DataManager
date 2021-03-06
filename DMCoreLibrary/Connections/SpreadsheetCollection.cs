﻿using DMCoreLibrary.Models;
using DMCoreLibrary.Spreadsheets;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
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
            SpreadsheetCollectionProgressModel report = new SpreadsheetCollectionProgressModel(ProcessState.IsRunning);
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
            finally
            {
                report.State = ProcessState.Completed;
                report.steps.Add("Process Completed!");
                progress.Report(report);
            }

        }

        public List<DataFileInfo> GetFileDetails()
        {
            var details = new List<DataFileInfo>();
            foreach (var file in Options.Files)
            {
                try
                {
                    using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
                    using var reader = ExcelReaderFactory.CreateReader(stream);
                    var sheets = new List<string>();
                    do
                    {
                        sheets.Add(reader.Name);
                    } while (reader.NextResult());
                    details.Add(new DataFileInfo(file, sheets));
                }
                catch (Exception)
                {
                    details.Add(new DataFileInfo(file, new List<string>(), FileType.Invalid));
                }            
            }
            return details;
        }

        public static FileType DetermineExtension(string file)
        {
            if (Regex.IsMatch(file, @"\.(csv|txt)$"))
            {
                return FileType.CSV;
            }
            else if (Regex.IsMatch(file, @"\.xl(s|sb|sx)$"))
            {
                return FileType.Excel;
            }
            else
            {
                return FileType.Invalid;
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
