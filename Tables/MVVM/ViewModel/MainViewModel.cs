using Tables.Core;

namespace Tables.MVVM.ViewModel
{
    /// <summary>
    /// Represents View Model that controls Main Window
    /// </summary>
    class MainViewModel : BaseViewModel
    {
        public HomeViewModel HomeViewModel { get; set; }
        public TableViewModel TableViewModel { get; set; }

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
            HomeViewModel = new HomeViewModel();
            TableViewModel = new TableViewModel();
            CurrentView = HomeViewModel;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeViewModel;
            });
            TableViewCommand = new RelayCommand(o =>
            {
                CurrentView = TableViewModel;
            });
        }
    }
}
