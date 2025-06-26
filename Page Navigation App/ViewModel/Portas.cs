using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO.Ports;



namespace Page_Navigation_App.ViewModel
{
    public partial class Portas : ObservableObject
    {

        



        public Portas()
        {
            
            AtualizarPortasDisponiveis();
            alarmes = new ObservableCollection<string>();
            AtualizaValores();

        }


        // Leituras
        [ObservableProperty] private ObservableCollection<string> portasDisponiveis = new();
        [ObservableProperty] private string portaSelecionada;
        [ObservableProperty] private double refTensao;
        [ObservableProperty] private ObservableCollection<string> alarmes = new();

        // Escritas
        [ObservableProperty] private string valor1;


        private void AtualizarPortasDisponiveis()
        {
            

            PortasDisponiveis.Clear();
            foreach (var porta in SerialPort.GetPortNames())
                
                PortasDisponiveis.Add(porta);

            // Seleciona automaticamente a primeira porta (opcional)
            if (PortasDisponiveis.Count > 0)
                PortaSelecionada = PortasDisponiveis[0];
        }


        


        private void AtualizaValores()
        {
            Valor1 = (RefTensao).ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        }




    }
}
