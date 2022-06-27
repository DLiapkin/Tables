using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tables.MVVM.Model
{
    class TableModel
    {
        public string TableName { get; set; }
        public List<string> ColumnHeaders { get; set; }
        public ObservableCollection<DataRowModel> TableData { get; set; }

        public TableModel(string tableName, List<string> columnHeaders, ObservableCollection<DataRowModel> tableData)
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

        private ObservableCollection<DataRowModel> LoadCSV(string filePath)
        {
            ObservableCollection<DataRowModel> rows = new ObservableCollection<DataRowModel>();
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
