using Microsoft.Win32;
using Tables.Core;

namespace Tables.MVVM.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        public HomeViewModel homeViewModel { get; set; }
        public TableViewModel tableViewModel { get; set; }

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand TableViewCommand { get; set; }

        private BaseViewModel _currentView;

        public BaseViewModel CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            homeViewModel = new HomeViewModel();
            tableViewModel = new TableViewModel();
            CurrentView = homeViewModel;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = homeViewModel;
            });
            TableViewCommand = new RelayCommand(o =>
            {
                CurrentView = tableViewModel;
            });
        }
    }
}
