using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tables.MVVM.ViewModel
{
    class TableViewModel : BaseViewModel
    {
        public TableViewModel()
        {
            Table = new Model.TableModel(@"D:\IBAtraining\Task 3\dataset.csv");
        }
    }
}
