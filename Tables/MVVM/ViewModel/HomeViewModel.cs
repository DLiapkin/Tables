using Tables.Core;
using Microsoft.Win32;
using Tables.MVVM.Model;
using System;

namespace Tables.MVVM.ViewModel
{
    class HomeViewModel : BaseViewModel
    {
        public RelayCommand OpenFileCommand { get; set; }

        public HomeViewModel()
        {
            OpenFileCommand = new RelayCommand(o =>
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
        }
    }
}
