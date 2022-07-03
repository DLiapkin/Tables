using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows;
using Tables.MVVM.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Linq;

namespace Tables.Core
{
    class DataHandler
    {
        public void SerializeXml(string filePath, string tableName, IEnumerable<Employee> employees)
        {
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

        public void SerializeExcel(string filePath, string tableName, IEnumerable<Employee> employees)
        {
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
    }
}
