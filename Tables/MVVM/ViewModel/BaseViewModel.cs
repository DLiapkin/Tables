using Tables.Core;
using Tables.MVVM.Model;

namespace Tables.MVVM.ViewModel
{
    class BaseViewModel : ObservableObject
    {
        private TableModel table;

        public TableModel Table 
        { 
            get { return table; } 
            set 
            { 
                table = value;
                OnPropertyChanged();
            }
        }
    }
}
