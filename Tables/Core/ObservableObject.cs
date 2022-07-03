using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tables.Core
{
    /// <summary>
    /// Represents object that notifies clients on changing
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
