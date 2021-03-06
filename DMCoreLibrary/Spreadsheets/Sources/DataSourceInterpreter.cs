﻿using System.Collections.Generic;
using System.Data;

namespace DMCoreLibrary.Spreadsheets.Sources
{
    class DataSourceInterpreter
    {
        private DataSource source;
        
        public DataSourceInterpreter(DataSource source)
        {
            this.source = source;
        }

        public (List<DataTable> data, DataTable errors) ReadData()
        {
            return source.ParseData();
        }
    }
}
