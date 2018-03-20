using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace DataManager
{
    class SurveyUtilitiesManager : IDataErrorInfo
    {
        #region PROPERTIES
        private List<string> filesList;
        private string startrow;
        private string sheetnames;

        public List<string> FilesList
        {
            get => filesList;
            set => filesList = value;
        }
        public string Startrow
        {
            get => startrow;
            set => startrow = value;
        }
        public string Sheetnames
        {
            get => sheetnames;
            set => sheetnames = value;
        }

        #endregion

        #region VALIDATION
        //input validation
        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string name]
        {
            get
            {
                string result = null;

                if (name == "Sheetnames")
                {
                    if (string.IsNullOrEmpty(this.sheetnames))
                    {
                        result = "Tip: Type \"allsheets\" for all sheets in file(s), cannot be blank";
                    }

                }
                else if (name == "Startrow")
                {
                    if (string.IsNullOrEmpty(this.startrow) || !Regex.IsMatch(this.startrow, @"^\d+$"))
                    {
                        result = "Must provide a number for row";
                    }
                }
                return result;
            }
        }

        #endregion

        #region METHODS
        
        public void MergeDTfromFiles()
        {
            //Convert filesList to FileInfo & find cwd
            List<FileInfo> files = StringToFileInfo(filesList);
            string dir = files[0].DirectoryName;

            //generates sheet list if necessary
            List<string> sheets = GenerateSheets(files[0]);

            //create tabke for files that generate errors
            DataTable errorTable = SurveyUtilities.MakeErrorTable();
            DataRow errorRow;


            //Generate list of DataTables from excel data
            List<DataTable> tables = new List<DataTable>();
            foreach (FileInfo file in files)
            {
                foreach(string sheet in sheets)
                {
                    OnProgressChanged(1, $"Loading {file.Name}...");

                    //if sheet does not exist, add to error table
                    try
                    {
                        tables.Add(SurveyUtilities.GetDTfromExcel(file, Convert.ToInt32(startrow), sheet));
                        if (OnCheckCancel())
                        {
                            return;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        errorRow = errorTable.NewRow();
                        errorRow["File Name"] = file;
                        errorRow["Sheet Name"] = sheet;
                        errorRow["Message"] = "Sheet not found";
                        errorTable.Rows.Add(errorRow);
                    }
                }
                
            }

            //Combine all tables into first table in list
            for (int i = 1; i < tables.Count; i++)
            {
                OnProgressChanged(2,$"Merging {tables[i].TableName}...");
                tables[0].Merge(tables[i], false, MissingSchemaAction.Add);
                if (OnCheckCancel())
                {
                    return;
                }
                tables[i] = null;
            }

            OnProgressChanged(3,"Sending to Excel...");
            try
            {
                SurveyUtilities.DTtoExcel(tables[0], dir);
            }
            catch(ArgumentOutOfRangeException e)
            {
                throw new System.ArgumentOutOfRangeException("Sheet(s) not found in any files!", e);
            }
            
        }
        private List<string> GenerateSheets(FileInfo file)
        {
            if (sheetnames.ToLower() == "allsheets")
            {
                List<string> sheets = SurveyUtilities.GetSheets(file);
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
            String[] extractedSheets = sheetnames.Split(delimiter);
            List<string> sheetList = new List<string>();
            foreach (string sheet in extractedSheets)
            {
                sheetList.Add(sheet.Trim());
            }
            return sheetList;
        }

        #endregion

        #region EVENTS
        public event EventHandler<CancelEventArgs> CheckCancel;
        protected virtual bool OnCheckCancel()
        {
            EventHandler<CancelEventArgs> handler = CheckCancel;
            if (handler != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                handler(this, e);
                return e.Cancel;
            }

            return false;
        }

        public class CancelEventArgs : EventArgs
        {
            public bool Cancel { get; set; }
        }


        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        protected virtual void OnProgressChanged(int part, string message)
        {
            EventHandler<ProgressChangedEventArgs> handler = ProgressChanged;
            if (ProgressChanged != null)
            {
                handler(this, new ProgressChangedEventArgs(part, message));
            }
        }

        public class ProgressChangedEventArgs : EventArgs
        {
            public string statusMessage;
            public int part;
            public ProgressChangedEventArgs(int part, string statusMessage)
            {
                this.statusMessage = statusMessage;
                this.part = part;
            }
        }
        #endregion
    }
}
