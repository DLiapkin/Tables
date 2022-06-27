using Tables.Core;
using Tables.MVVM.Model;

namespace Tables.MVVM.ViewModel
{
    class BaseViewModel : ObservableObject
    {
        public TableModel Table { get; set; }

        public BaseViewModel()
        {

        }
    }
}
