using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tables.Core
{
    class Filter : ObservableObject
    {
        private string date = String.Empty;
        private string name = String.Empty;
        private string lastName = String.Empty;
        private string surname = String.Empty;
        private string city = String.Empty;
        private string country = String.Empty;

        public string Date
        {
            get 
            { 
                return date; 
            }
            set 
            { 
                date = value; 
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get 
            { 
                return name; 
            }
            set 
            { 
                name = value; 
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get 
            { 
                return lastName; 
            }
            set 
            { 
                lastName = value;
                OnPropertyChanged();
            }
        }

        public string Surname
        {
            get 
            { 
                return surname; 
            }
            set 
            {
                surname = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get 
            { 
                return city; 
            }
            set
            { 
                city = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get 
            { 
                return country; 
            }
            set 
            { 
                country = value;
                OnPropertyChanged();
            }
        }
    }
}
