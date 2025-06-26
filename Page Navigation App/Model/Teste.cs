using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRTDappWpf.Models
{
    class Teste
    {
        public (int valor1, int valor2) teste1()
        {  
            int valor1 = 10;
            int valor2 = 5;

            return(valor1, valor2);
        }
    }
}
