using DMCoreLibrary.Models;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System;
using DMCoreLibrary.Connections;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace DMCoreLibraryTests
{
    public class SpreadsheetCollectionTests
    {
        [Fact]
        public void ProveFailure_Failing_ReturnFailedTest()
        {
            Assert.True(false);
        }

        [Fact]
        public void DataFileInfo_IdentifyFileExtensions_ReturnFileTypes()
        {
            //Arrange
            var stubFiles = new List<string>() {"file.xlsx", "file.xlsb", "file.xls", "file.csv", "file.txt", "file.xlsx.xl", "filexlsx", "file.xxlsx"};
            var validFiles = new List<string>() {"file.xlsx", "file.xlsb", "file.xls", "file.csv", "file.txt" };
            var csvFiles = new List<string>() { "file.csv", "file.txt" };
            var stubSheets = new List<string>() {"sheet1", "sheet2", "sheet3" };

            //Act
            var infoHolders = new List<DataFileInfo>();
            foreach (var file in stubFiles)
            {
                infoHolders.Add(new DataFileInfo(file, stubSheets));
            }

            var actualValidExts = infoHolders.Where(p => p.FileExtension != FileType.Invalid).Select(p => p.SourceFile.Name).ToList();
            var actualCsvExts = infoHolders.Where(p => p.FileExtension == FileType.CSV).Select(p => p.SourceFile.Name).ToList();

            //Assert
            Assert.Equal(validFiles, actualValidExts);
            Assert.Equal(csvFiles, actualCsvExts);

        }

        [Fact]
        public void GetFileDetails_SmallFiles_ReturnDetails()
        {
            //Arrange
            var stubFiles = new List<string>() { "resources/t1.xlsx", "resources/t2.xlsx", "resources/t3.xlsx" };
            var expectedSheetCounts = new Dictionary<string, int>() { {"t1.xlsx", 1 }, {"t2.xlsx", 3 }, {"t3.xlsx", 4 }};
            var stubOptions = new SpreadsheetCollectionOptions() { Files = stubFiles };
            var fakeCollection = new SpreadsheetCollection(stubOptions);

            //Act
            var actualCollectionDetails = fakeCollection.GetFileDetails();
            var actualFilesLoaded = actualCollectionDetails.Count;

            //Assert
            Assert.Equal(3,actualFilesLoaded);
            foreach (var sheetCount in expectedSheetCounts)
            {
                var expectedCount = sheetCount.Value;
                var matches = actualCollectionDetails.Where(p => p.SourceFile.Name == sheetCount.Key).ToList();
                var actualCount = matches[0].Sheets.Count;
                Assert.Equal(expectedCount, actualCount);
            }
        }

        [Fact]
        public void MergeDTfromFiles_PasswordOptions_ReturnsMergedFile()
        {
            throw new NotImplementedException();
        }
    }
}
