using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tables.Core;
using System.Linq;
using System.Windows;

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
            TableName = "FromDataBase";
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
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                try
                {
                    for (int i = 0; i < TableData.Count; i++)
                    {
                        var employee = TableData[i];
                        //var result = context.Employees.FirstOrDefault(
                        //    e => e.Name == employee.Name
                        //    && e.LastName == employee.LastName
                        //    && e.Surname == employee.Surname);
                        //if (result == null)
                        //{
                            context.Employees.Add(employee);
                        //}

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

        public void LoadFromDb()
        {
            using (DataBaseContext context = new DataBaseContext())
            {
                TableData = new ObservableCollection<Employee>(context.Employees.Take(5000).ToList());
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

        public void SaveToXml(string filePath, Filter filter)
        {
            DataHandler handler = new DataHandler();
            Employee? sample = ProccesFilter(filter);
            if (sample != null)
            {
                IEnumerable<Employee> selectedEmployees = GetEmployeesBySample(sample);
                handler.SerializeXml(filePath, TableName, selectedEmployees);
            }
        }

        public void SaveToExcel(string filePath, Filter filter)
        {
            DataHandler handler = new DataHandler();
            Employee? sample = ProccesFilter(filter);
            if (sample != null)
            {
                IEnumerable<Employee> selectedEmployees = GetEmployeesBySample(sample);
                handler.SerializeExcel(filePath, TableName, selectedEmployees);
            }
        }

        private IEnumerable<Employee> GetEmployeesBySample(Employee sample)
        {
            List<Employee> selectedEmployees;
            using (DataBaseContext context = new DataBaseContext())
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                try
                {
                    selectedEmployees = context.Employees.ToList();
                    if (sample.Date != DateTime.MinValue)
                    {
                        selectedEmployees = selectedEmployees.Where(e => e.Date == sample.Date).ToList();
                    }
                    if (!sample.Name.Equals(String.Empty))
                    {
                        selectedEmployees = selectedEmployees.Where(e => e.Name == sample.Name).ToList();
                    }
                    if (!sample.LastName.Equals(String.Empty))
                    {
                        selectedEmployees = selectedEmployees.Where(e => e.LastName == sample.LastName).ToList();
                    }
                    if (!sample.Surname.Equals(String.Empty))
                    {
                        selectedEmployees = selectedEmployees.Where(e => e.Surname == sample.Surname).ToList();
                    }
                    if (!sample.City.Equals(String.Empty))
                    {
                        selectedEmployees = selectedEmployees.Where(e => e.City == sample.City).ToList();
                    }
                    if (!sample.Country.Equals(String.Empty))
                    {
                        selectedEmployees = selectedEmployees.Where(e => e.Country == sample.Country).ToList();
                    }
                    if (selectedEmployees.Count == context.Employees.Count() || selectedEmployees.Count == 0)
                    {
                        MessageBox.Show("No occurences", "Filter error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        selectedEmployees.Clear();
                    }
                }
                finally
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
            return selectedEmployees;
        }

        private Employee? ProccesFilter(Filter filter)
        {
            if (filter == null)
            {
                MessageBox.Show("Filter is null!", "Filter error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            Employee employee = new Employee()
            {
                Date = DateTime.MinValue,
                Name = String.Empty,
                LastName = String.Empty,
                Surname = String.Empty,
                City = String.Empty,
                Country = String.Empty
            };
            DateTime date;
            if (DateTime.TryParse(filter.Date, out date))
            {
                employee.Date = date;
            }
            if (!String.IsNullOrEmpty(filter.Name))
            {
                employee.Name = filter.Name;
            }
            if (!String.IsNullOrEmpty(filter.LastName))
            {
                employee.LastName = filter.LastName;
            }
            if (!String.IsNullOrEmpty(filter.Surname))
            {
                employee.Surname = filter.Surname;
            }
            if (!String.IsNullOrEmpty(filter.City))
            {
                employee.City = filter.City;
            }
            if (!String.IsNullOrEmpty(filter.Country))
            {
                employee.Country = filter.Country;
            }
            //MessageBox.Show("Wrong Date format!", "Data error!", MessageBoxButton.OK, MessageBoxImage.Error);
            return employee;
        }
    }
}
