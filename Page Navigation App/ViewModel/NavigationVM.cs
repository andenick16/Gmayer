using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page_Navigation_App.Utilities;
using System.Windows.Input;

namespace Page_Navigation_App.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand ConfiguracaoCommand { get; set; }
        public ICommand ProtecaoCommand { get; set; }
        public ICommand ReguladorCommand { get; set; }
        public ICommand EstabilidadeCommand { get; set; }
        public ICommand GraficoCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand SoftengeCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void configuracao(object obj) => CurrentView = new ConfiguracaoVM();
        private void Protecao(object obj) => CurrentView = new ProtecaoVM();
        private void Regulador(object obj) => CurrentView = new ReguladorVM();
        private void Estabilidade(object obj) => CurrentView = new EstabilidadeVM();
        private void Grafico(object obj) => CurrentView = new GraficoVM();
        private void Setting(object obj) => CurrentView = new SettingVM();
        private void Softenge(object obj) => CurrentView = new SoftengeVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            ConfiguracaoCommand = new RelayCommand(configuracao);
            ProtecaoCommand = new RelayCommand(Protecao);
            ReguladorCommand = new RelayCommand(Regulador);
            EstabilidadeCommand = new RelayCommand(Estabilidade);
            GraficoCommand = new RelayCommand(Grafico);
            SettingsCommand = new RelayCommand(Setting);
            SoftengeCommand = new RelayCommand(Softenge);

            // Startup Page
            CurrentView = new HomeVM();
        }

       
        }


    }

