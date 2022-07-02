using Microsoft.Win32;
using System;
using Tables.Core;
using Tables.MVVM.Model;

namespace Tables.MVVM.ViewModel
{
    class TableViewModel : BaseViewModel
    {
        public RelayCommand SaveCommand { get; set; }

        public TableViewModel()
        {
            //Table = new Model.TableModel(@"D:\IBAtraining\Task 3\harder.csv");
            Table = new TableModel();
            SaveCommand = new RelayCommand(o =>
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "XML Files (*.xml)|*.xml";
                saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (saveDialog.ShowDialog() == true)
                {
                    Table.SaveToXml(saveDialog.FileName);
                }
            });
        }

        //public void InitializeTable()
        //{
        //    Table = new TableModel();
        //}
    }
}
