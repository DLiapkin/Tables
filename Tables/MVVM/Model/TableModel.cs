using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tables.Core;
using System.Linq;

namespace Tables.MVVM.Model
{
    class TableModel
    {
        public string TableName { get; set; }
        public List<string> ColumnHeaders { get; set; }
        public ObservableCollection<Employee> TableData { get; set; }

        public TableModel(string tableName, List<string> columnHeaders, ObservableCollection<Employee> tableData)
        {
            TableName = tableName;
            ColumnHeaders = columnHeaders;
            TableData = tableData;
        }

        public TableModel(string filePath)
        {
            string[] path = filePath.Split('\\');
            TableName = path[^1].Replace(".csv", "");
            ColumnHeaders = new List<string>() {"Id", "Date", "Name", "LastName", "Surname", "City", "Country"};
            TableData = LoadCsv(filePath);
            SaveToDb();
        }

        public TableModel()
        {
            TableName = "";
            ColumnHeaders = new List<string>() { "Id", "Date", "Name", "LastName", "Surname", "City", "Country" };
            TableData = new ObservableCollection<Employee>();
            LoadFromDb();
        }

        private ObservableCollection<Employee> LoadCsv(string filePath)
        {
            var employeeList = fastCSV.ReadFile<Employee>(
                filePath,          // filename
                true,              // has header
                ';',               // delimiter
                (o, c) =>          // to object function o : employee object, c : columns array read
                {
                    o.Date = DateTime.Parse(c[0]);
                    o.Name = c[1];
                    o.LastName = c[2];
                    o.Surname = c[3];
                    o.City = c[4];
                    o.Country = c[5];
                    // add to list
                    return true;
                });
            ObservableCollection<Employee> rows = new ObservableCollection<Employee>(employeeList);
            return rows;
        }

        public void SaveToDb()
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                foreach (Employee employee in TableData)
                {
                    //var result = context.Employees.FirstOrDefault(
                    //    e => e.Name == employee.Name 
                    //    && e.LastName == employee.LastName 
                    //    && e.Surname == employee.Surname);
                    //if(result == null)
                    //{
                        context.Employees.Add(employee);
                    //}
                }
                context.SaveChanges();
            }
        }

        public void LoadFromDb()
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                TableData = new ObservableCollection<Employee>(context.Employees.ToList());
            }
        }

        public void UpdateDb(Employee employee)
        {
            // TODO: implement update function
            using (DataBaseContext context = new DataBaseContext())
            {
                //context.Employees.Update();
            }
        }

        public void ClearDb()
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Employees.RemoveRange(context.Employees);
                context.SaveChanges();
            }
        }

        public void SaveToXml(string filePath)
        {
            DataHandler handler = new DataHandler();
            handler.Serialize(filePath, TableName, TableData);
        }
    }
}
