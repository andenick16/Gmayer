﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page_Navigation_App.Model;

namespace Page_Navigation_App.ViewModel
{
    class EstabilidadeVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public decimal TransactionAmount
        {
            get { return _pageModel.TransactionValue; }
            set { _pageModel.TransactionValue = value; OnPropertyChanged(); }
        }

        public EstabilidadeVM()
        {
            _pageModel = new PageModel();
            TransactionAmount = 5638;
        }
    }
}
