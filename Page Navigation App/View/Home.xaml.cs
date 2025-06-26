using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Page_Navigation_App.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>

    public partial class Home : UserControl
    {
        public ObservableCollection<Penas> ListaPortas { get; set; }
        public Home()
        {

            InitializeComponent();

            ListaPortas = new ObservableCollection<Penas>
            {
                new Penas { Id = 1, Nome = "Com 1" },
                new Penas { Id = 2, Nome = "Com 2" },
                new Penas { Id = 3, Nome = "Com 3" },
                new Penas { Id = 4, Nome = "Com 4" },
                new Penas { Id = 5, Nome = "Com 5" },

            };

            DataContext = this;





        }

       
    }
}
