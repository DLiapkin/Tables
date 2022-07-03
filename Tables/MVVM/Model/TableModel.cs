using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tables.Core;
using System.Linq;
using System.Windows;

namespace Tables.MVVM.Model
{
    /// <summary>
    /// Represents table model that manipulate employees collection
    /// </summary>
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
            TableName = "FromDataBase";
            ColumnHeaders = new List<string>() { "Id", "Date", "Name", "LastName", "Surname", "City", "Country" };
            TableData = new ObservableCollection<Employee>();
            LoadFromDb();
        }

        /// <summary>
        /// Loads employees from file in location of filePath
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        /// <returns>Observable collection of employees.</returns>
        private ObservableCollection<Employee> LoadCsv(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Incorret file path!", "Loading error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Employee>();
            }

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

        /// <summary>
        /// Saves employees to database
        /// </summary>
        public void SaveToDb()
        {
            if (TableData.Count == 0)
            {
                MessageBox.Show("Collection of employees is empty!", "Saving error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (DataBaseContext context = new DataBaseContext())
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                try
                {
                    for (int i = 0; i < TableData.Count; i++)
                    {
                        var employee = TableData[i];
                        context.Employees.Add(employee);

                        if (i % 5000 == 0)
                        {
                            context.ChangeTracker.DetectChanges();
                            context.SaveChanges();
                        }
                    }
                }
                finally
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = true;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Loads employees from database
        /// </summary>
        public void LoadFromDb()
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                TableData = new ObservableCollection<Employee>(context.Employees.Take(5000).ToList());
            }
        }

        /// <summary>
        /// Clears database
        /// </summary>
        public void ClearDb()
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                context.Employees.RemoveRange(context.Employees);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Saves employees to XML file in location of filePath by given filter
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        /// <param name="filter">Filter for export query</param>
        public void SaveToXml(string filePath, Filter filter)
        {
            DataHandler handler = new DataHandler();
            Employee? sample = handler.ProccesFilter(filter);
            if (sample != null)
            {
                IEnumerable<Employee> selectedEmployees = handler.GetEmployeesBySample(sample);
                handler.SerializeXml(filePath, TableName, selectedEmployees);
            }
        }

        /// <summary>
        /// Saves employees to Excel file in location of filePath by given filter
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        /// <param name="filter">Filter for export query</param>
        public void SaveToExcel(string filePath, Filter filter)
        {
            DataHandler handler = new DataHandler();
            Employee? sample = handler.ProccesFilter(filter);
            if (sample != null)
            {
                IEnumerable<Employee> selectedEmployees = handler.GetEmployeesBySample(sample);
                handler.SerializeExcel(filePath, TableName, selectedEmployees);
            }
        }
    }
}
