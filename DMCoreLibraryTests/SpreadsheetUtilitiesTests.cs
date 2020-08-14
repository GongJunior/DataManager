using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using DMCoreLibrary.Models;
using System.Linq;
using DMCoreLibrary.Connections;
using DMCoreLibrary.Spreadsheets;
using System.IO;

namespace DMCoreLibraryTests
{
    public class SpreadsheetUtilitiesTests
    {
        [Theory]
        [InlineData("NumbersAsColumnNames")]
        [InlineData("StringsAsColumnNames")]
        [InlineData("ColumnNameswBlanks")]
        [InlineData("BlanksAsColumnNames")]
        public void GetDTFromExcel_HeaderPatterns_ReturnTables(string file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var mockFile = new FileInfo($"resources/{file}.xlsx");
            var headerRow = 1;
            var sheets = new List<string>(){ "Sheet1" };
            var passwords = new List<string>();

            var (data, err) = SpreadsheetUtilities.GetDTfromExcel(mockFile, headerRow, sheets, passwords);

            Assert.Equal(0, err.Rows.Count);
            Assert.Single(data);
            Assert.Equal(8, data[0].Columns.Count);
            Assert.Equal(10, data[0].Rows.Count);
        }
    }
}
