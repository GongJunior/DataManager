using DMCoreLibrary.Connections;
using System.Collections.Generic;
using System.IO;

namespace DMCoreLibrary.Models
{
    public class DataFileInfo
    {
        public FileInfo SourceFile { get; }
        public List<string> Sheets {get;}
        public FileType FileExtension { get; }


        public DataFileInfo(string src, List<string> sheets)
        {
            SourceFile = new FileInfo(src);
            Sheets = sheets;
            FileExtension = SpreadsheetCollection.DetermineExtension(src.ToLower());
        }
        public DataFileInfo(string src, List<string> sheets, FileType type)
        {
            SourceFile = new FileInfo(src);
            Sheets = sheets;
            FileExtension = type;
        }

    }
}