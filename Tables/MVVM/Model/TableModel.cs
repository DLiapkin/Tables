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
        public ObservableCollection<Employee> TableData { get; set; }

        public TableModel(string tableName, ObservableCollection<Employee> tableData)
        {
            TableName = tableName;
            TableData = tableData;
        }

        public TableModel(string filePath)
        {
            string[] path = filePath.Split('\\');
            TableName = path[^1].Replace(".csv", "");
            TableData = new ObservableCollection<Employee>();
            SaveToDb(filePath);
        }

        public TableModel()
        {
            TableName = "FromDataBase";
            TableData = new ObservableCollection<Employee>();
            LoadFromDb();
        }

        /// <summary>
        /// Loads employees from file in location of filePath
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        /// <returns>Collection of employees.</returns>
        private async IAsyncEnumerable<Employee> LoadCsv(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                string line = "";
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] data = line.Split(';');
                    yield return new Employee()
                    {
                        Date = DateTime.Parse(data[0]),
                        Name = data[1],
                        LastName = data[2],
                        Surname = data[3],
                        City = data[4],
                        Country = data[5]
                    };
                }
            }
        }

        /// <summary>
        /// Saves employees to database
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        public async void SaveToDb(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("File path is incorrect!", "Saving error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (DataBaseContext context = new DataBaseContext())
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                try
                {
                    var data = LoadCsv(filePath);
                    int i = 0;
                    await foreach (Employee employee in data)
                    {
                        context.Employees.Add(employee);

                        if (i % 5000 == 0)
                        {
                            context.ChangeTracker.DetectChanges();
                            context.SaveChanges();
                            i++;
                        }
                        else
                        {
                            i++;
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
