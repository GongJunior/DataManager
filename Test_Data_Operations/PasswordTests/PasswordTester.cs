using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager;
using DataManager.ViewModels;

namespace Test_Data_Operations.PasswordTests
{
    class PasswordTester
    {
        private class Sample
        {
            public StandardViewModel model { get; set; }
            public string samp1 { get; set; }
            public string samp2 { get; set; }

            public Sample(StandardViewModel m, string s1, string s2)
            {
                model = m;
                samp1 = s1;
                samp2 = s2;
            }
        }

        public static void RunPasswordTest()
        {
            //use to test functions in surveysutilitiesmanager
            var managers = new List<SurveyUtilitiesManager>();
            var samples = new List<Sample>()
            {
                new Sample(new StandardViewModel(),"pw_test1.xlsx", "pw_test2.xlsx"),
                new Sample(new StandardViewModel(),"test1.csv", "test2.csv"),
                new Sample(new StandardViewModel(),"test1.xlsx", "test2.xlsx")
            };

            foreach (var sample in samples)
            {
                managers.Add(new SurveyUtilitiesManager(SetPasswordVars(sample.model, sample.samp1, sample.samp2)));
            }

            foreach (var manager in managers)
            {
                Console.WriteLine("starting merge....");
                manager.MergeDTfromFiles();
            }
            Console.WriteLine("Complete, press the any key to contiue");
            Console.ReadKey();

        }
        private static StandardViewModel SetPasswordVars(StandardViewModel vm, string f1, string f2)
        {
            var files = new List<string>()
            {
                $@"test_resources\{ f1 }",
                $@"test_resources\{ f2 }"
            };
            var passwords = new ObservableCollection<string>()
            {
                @"password",
                @"test",
                @"invalidpassword"
            };
            var sheets = "test1, test2";
            var startrow = "1";

            vm.File_list = files;
            vm.PW_list = passwords;
            vm.Sheetname_text = sheets;
            vm.Startrow_text = startrow;
            return vm;
        }
        private static StandardViewModel SetVarsNoPW(StandardViewModel vm, string f1, string f2)
        {
            var files = new List<string>()
            {
                $@"test_resources\{ f1 }",
                $@"test_resources\{ f2 }"
            };
            var sheets = "test1, test2";
            var startrow = "1";

            vm.File_list = files;
            vm.Sheetname_text = sheets;
            vm.Startrow_text = startrow;
            return vm;
        }

    }
}
