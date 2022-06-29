using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tables.Core;

namespace Tables.MVVM.ViewModel
{
    class TableViewModel : BaseViewModel
    {
        public RelayCommand SaveCommand { get; set; }

        public TableViewModel()
        {
            Table = new Model.TableModel(@"D:\IBAtraining\Task 3\harder.csv");
            SaveCommand = new RelayCommand(o =>
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "XML Files (*.xml)|*.xml";
                saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (saveDialog.ShowDialog() == true)
                {
                    Table.Save(saveDialog.FileName);
                }
            });
        }
    }
}
