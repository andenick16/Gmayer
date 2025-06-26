using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page_Navigation_App.Model;
using Page_Navigation_App.View;


namespace Page_Navigation_App.ViewModel
{
    class SoftengeVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public bool Softenge
        {
            get { return _pageModel.LocationStatus; }
            set { _pageModel.LocationStatus = value; OnPropertyChanged(); }
        }

        public SoftengeVM()
        {
            _pageModel = new PageModel();
            Softenge = true;
        }
    }
}
