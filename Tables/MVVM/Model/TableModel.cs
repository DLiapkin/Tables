using System;
using System.IO;
using System.Collections.Generic;

namespace Tables.MVVM.Model
{
    class TableModel
    {
        public string TableName { get; set; }
        public List<string> ColumnHeaders { get; set; }
        public List<DataRowModel> TableData { get; set; }

        public TableModel(string tableName, List<string> columnHeaders, List<DataRowModel> tableData)
        {
            TableName = tableName;
            ColumnHeaders = columnHeaders;
            TableData = tableData;
        }

        public TableModel(string filePath)
        {
            string[] path = filePath.Split('/');
            TableName = path[-0].Replace(".csv", "");
            ColumnHeaders = new List<string>() {"Date", "Name", "LastName", "Surname", "City", "Country"};
            TableData = LoadCSV(filePath);
        }

        private List<DataRowModel> LoadCSV(string filePath)
        {
            List<DataRowModel> rows = new List<DataRowModel>();
            var rawData = File.ReadAllLines(filePath);
            foreach (string line in rawData)
            {
                string[] data = line.Split(';');
                DataRowModel row = new DataRowModel();
                row.Date = DateTime.Parse(data[0]);
                row.Name = data[1];
                row.LastName = data[2];
                row.Surname = data[3];
                row.City = data[4];
                row.Country = data[5];
                rows.Add(row);
            }
            return rows;
        }
    }
}
