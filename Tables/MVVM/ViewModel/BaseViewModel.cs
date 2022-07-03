using Tables.Core;
using Tables.MVVM.Model;

namespace Tables.MVVM.ViewModel
{
    /// <summary>
    /// Base View Model that contains shared Model property
    /// </summary>
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
