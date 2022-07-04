using System;
using Tables.Core;
using Microsoft.Win32;
using Tables.MVVM.Model;

namespace Tables.MVVM.ViewModel
{
    /// <summary>
    /// Represents View Model that controls Table UserControl
    /// </summary>
    class TableViewModel : BaseViewModel
    {
        private bool isCollapsed = true;
        private Filter filter;

        public Filter Filter 
        {
            get => filter; 
            set
            {
                filter = value;
                OnPropertyChanged();
            }
        }

        public bool IsCollapsed
        {
            get 
            { 
                return isCollapsed; 
            }
            set 
            { 
                isCollapsed = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ChangeVisibility { get; set; }
        public RelayCommand ExportXml { get; set; }
        public RelayCommand ExportExcel { get; set; }

        public TableViewModel()
        {
            Table = new TableModel();
            Filter = new Filter();
            ChangeVisibility = new RelayCommand(o =>
            {
                if (isCollapsed)
                {
                    IsCollapsed = false;
                }
                else
                {
                    IsCollapsed = true;
                }
            });
            ExportXml = new RelayCommand(o =>
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "XML Files (*.xml)|*.xml";
                saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (saveDialog.ShowDialog() == true)
                {
                    Table.SaveToXml(saveDialog.FileName, Filter);
                }
            });
            ExportExcel = new RelayCommand(o =>
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (saveDialog.ShowDialog() == true)
                {
                    Table.SaveToExcel(saveDialog.FileName, Filter);
                }
            });
        }
    }
}
