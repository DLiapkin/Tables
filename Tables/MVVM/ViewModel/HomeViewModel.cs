using System;
using Tables.Core;
using Microsoft.Win32;
using Tables.MVVM.Model;

namespace Tables.MVVM.ViewModel
{
    /// <summary>
    /// Represents View Model that controls Home UserControl
    /// </summary>
    class HomeViewModel : BaseViewModel
    {
        public RelayCommand OpenCommand { get; set; }
        public RelayCommand CreateCommand { get; set; }

        public HomeViewModel()
        {
            OpenCommand = new RelayCommand(o =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    Table = new TableModel(openFileDialog.FileName);
                }
            });

            CreateCommand = new RelayCommand(o =>
            {
                Table = new TableModel();
                Table.ClearDb();
            });
        }
    }
}
