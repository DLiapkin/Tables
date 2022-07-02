using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Tables.MVVM.Model;

namespace Tables.Core
{
    class DataHandler
    {
        public void Serialize(string filePath, string tableName, ObservableCollection<Employee> rowsToSave)
        {
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
    }
}
