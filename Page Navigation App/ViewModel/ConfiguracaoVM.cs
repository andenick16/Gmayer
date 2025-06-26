using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page_Navigation_App.Model;

namespace Page_Navigation_App.ViewModel
{
    class ConfiguracaoVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public int CustomerID
        {
            get { return _pageModel.ConfiguracaoCount; }
            set { _pageModel.ConfiguracaoCount = value; OnPropertyChanged(); }
        }

        public ConfiguracaoVM()
        {
            _pageModel = new PageModel();
            CustomerID = 100528;
        }
    }
}
