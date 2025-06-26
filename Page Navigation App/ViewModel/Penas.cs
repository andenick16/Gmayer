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




namespace Page_Navigation_App
{
    
    public class  Penas : INotifyPropertyChanged
    {
        // Define as propriedades para as classes

        private int _id; 
        private string _nome;


        public int Id 
        { get => _id;
            set 
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }

        }

       public string Nome 
        { 
            get => _nome;
             set
             {
                if (_nome != value)
                {
                    _nome = value;
                    NotifyPropertyChanged("Nome");
                }
             }

        }

       public event PropertyChangedEventHandler? PropertyChanged;
       public void NotifyPropertyChanged ( string? propertyName = null)
       {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs
                (propertyName));
       }
    }

    }