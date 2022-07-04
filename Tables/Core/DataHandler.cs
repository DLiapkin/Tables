using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Tables.MVVM.Model;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Tables.Core
{
    /// <summary>
    /// Class for Serialization and data manipulations
    /// </summary>
    class DataHandler
    {
        /// <summary>
        /// Serializes selected employees to a XML file located in filePath
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="employees">A collection of selected employes</param>
        public void SerializeXml(string filePath, string tableName, IEnumerable<Employee> employees)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Incorret file path!", "Serialization error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (String.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Incorret table name!", "Serialization error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (employees == null)
            {
                MessageBox.Show("Incorret collection of employees!", "Serialization error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<Employee>rowsToSave = new List<Employee>(employees);
            XDocument document = new XDocument();
            XElement records = new XElement(tableName);
            foreach (Employee row in rowsToSave)
            {
                XElement record = new XElement("record", new XAttribute("id", rowsToSave.IndexOf(row)));
                record.Add(new XElement("Date", row.Date.ToString()));
                record.Add(new XElement("FirstName", row.Name));
                record.Add(new XElement("LastName", row.LastName));
                record.Add(new XElement("Surname", row.Surname));
                record.Add(new XElement("City", row.City));
                record.Add(new XElement("Country", row.Country));
                records.Add(record);
            }
            document.Add(records);
            document.Save(filePath);
        }

        /// <summary>
        /// Serializes selected employees to a Excel file located in filePath
        /// </summary>
        /// <param name="filePath">Location of a file </param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="employees">A collection of selected employes</param>
        public void SerializeExcel(string filePath, string tableName, IEnumerable<Employee> employees)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Incorret file path!", "Serialization error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (String.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Incorret table name!", "Serialization error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (employees == null)
            {
                MessageBox.Show("Incorret collection of employees!", "Serialization error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataTable epmployeesTable = new DataTable();
            epmployeesTable.TableName = tableName;
            epmployeesTable.Columns.Add("Date", typeof(DateTime));
            epmployeesTable.Columns.Add("Name", typeof(string));
            epmployeesTable.Columns.Add("Lastname", typeof(string));
            epmployeesTable.Columns.Add("Surname", typeof(string));
            epmployeesTable.Columns.Add("City", typeof(string));
            epmployeesTable.Columns.Add("Country", typeof(string));
            foreach (Employee employee in employees)
            {
                epmployeesTable.Rows.Add(new object[] 
                { 
                    employee.Date, 
                    employee.Name,
                    employee.LastName,
                    employee.Surname,
                    employee.City,
                    employee.Country
                });
            }

            try
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    workbookPart.Workbook.Sheets = new Sheets();

                    var sheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbookPart.GetIdOfPart(sheetPart);
                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }
                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = epmployeesTable.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();
                    List<String> columns = new List<string>();
                    foreach (DataColumn column in epmployeesTable.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);
                    foreach (DataRow dsrow in epmployeesTable.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't create Excel file.\r\nException: " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Select employees by given sample entity
        /// </summary>
        /// <param name="sample">Sample employee for query</param>
        /// <returns>Collection of employees.</returns>
        public IEnumerable<Employee> GetEmployeesBySample(Employee sample)
        {
            if (sample == null)
            {
                MessageBox.Show("Filter is invalid!", "Filter error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return Enumerable.Empty<Employee>();
            }

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

        /// <summary>
        /// Generates sample employee entity by given filter
        /// </summary>
        /// <param name="filter">Filter for export query</param>
        /// <returns>Employee if the <paramref ref="filter"/> parameter was valid; otherwise, null.</returns>
        public Employee? ProccesFilter(Filter filter)
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
            return employee;
        }
    }
}
