using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GRTDappWpf.Models;

using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Page_Navigation_App.ViewModel
{
    public partial class InterfaceVM : ObservableObject
    {
        private readonly IModbusService _modbusService;
        private readonly INavigationService _navigationService;

        // Comunicação
        private Timer _leituraTimer;
        private bool _emLeitura = false;
        private bool _leituraConcluida;
        private CancellationTokenSource _leituraCts;

        public enum CommMode { Tcp, Rtu }
        private CommMode _mode;

        private byte _slaveId = 1;
        private string _ip = "127.0.0.1";
        private int _tcpPort = 502;

        private int _baud;
        private Parity _parity = Parity.None;
        private StopBits _stopBits = StopBits.One;

        // Construtor
        public InterfaceVM(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _modbusService = new ModbusService();

            alarmes = new ObservableCollection<string>();
            AtualizarPortasDisponiveis();
            _leituraConcluida = false;
        }

        // Propriedades ObservableProperty geradas automaticamente pelo Source Generator
        [ObservableProperty]
        private ObservableCollection<string> alarmes;

        [ObservableProperty]
        private string mensagem = "Desconectado";

        [ObservableProperty]
        private bool usaTcp = true;

        [ObservableProperty]
        private int setBaudRate;

        [ObservableProperty]
        private ObservableCollection<string> portasDisponiveis = new();

        [ObservableProperty]
        private string portaSelecionada;

        // Propriedades do Modbus (exemplos)
        [ObservableProperty]
        private double id;

        [ObservableProperty]
        private double baudRate;

        [ObservableProperty]
        private double canId;

        [ObservableProperty]
        private double refTensao;

        [ObservableProperty]
        private double kp;

        [ObservableProperty]
        private double ki;

        [ObservableProperty]
        private double kd;

        // Propriedades para parâmetros de escrita
        [ObservableProperty]
        private string paramRefTensao;

        [ObservableProperty]
        private string paramGanhoKP;

        [ObservableProperty]
        private string paramGanhoKI;

        [ObservableProperty]
        private string paramGanhoKD;

        // Comandos de navegação
        [RelayCommand]
        private void NavegarPage2()
        {
            _navigationService.NavigateTo("Page2");
        }

        [RelayCommand]
        private void NavegarPage1()
        {
            _navigationService.NavigateTo("Page1");
        }

        // Comandos de conexão
        [RelayCommand]
        private void Conectar()
        {
            _mode = UsaTcp ? CommMode.Tcp : CommMode.Rtu;

            try
            {
                if (UsaTcp)
                {
                    _modbusService.ConnectTcp(_ip, _tcpPort);
                    Mensagem = "Conectado via TCP";
                }
                else
                {
                    _modbusService.ConnectRtu(PortaSelecionada, setBaudRate, _parity, _stopBits);
                    Mensagem = "Conectado via RTU";
                }

                Mensagem = $"Conectado com Slave ID {_slaveId}, modo {_mode}";
                IniciarLeituraPeriodica();
            }
            catch (Exception ex)
            {
                Mensagem = "Erro na conexão: " + ex.Message;
            }
        }

        [RelayCommand]
        private void Desconectar()
        {
            _modbusService.Disconnect();
            Mensagem = "Desconectado";

            _leituraTimer?.Dispose();
            _leituraTimer = null;
            _leituraCts?.Cancel();
            _leituraConcluida = false;
        }

        // Exemplos de comandos para escrita com validação
        [RelayCommand]
        private void EnterPressedRefTensao()
        {
            if (float.TryParse(ParamRefTensao?.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out float numero))
            {
                EscreverValor(0x0003, numero * 10); // Endereço refTensao
                Mensagem = $"Valor escrito: {numero}";
            }
            else
            {
                Mensagem = "Número inválido.";
            }
        }

        [RelayCommand]
        private void EnterPressedParamKP()
        {
            if (float.TryParse(ParamGanhoKP?.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out float numero))
            {
                EscreverValor(0x0004, numero); // Endereço KP
                Mensagem = $"Valor escrito: {numero}";
            }
            else
            {
                Mensagem = "Número inválido.";
            }
        }

        [RelayCommand]
        private void EnterPressedParamKI()
        {
            if (float.TryParse(ParamGanhoKI?.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out float numero))
            {
                EscreverValor(0x0005, numero); // Endereço KI
                Mensagem = $"Valor escrito: {numero}";
            }
            else
            {
                Mensagem = "Número inválido.";
            }
        }

        [RelayCommand]
        private void EnterPressedParamKD()
        {
            if (float.TryParse(ParamGanhoKD?.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out float numero))
            {
                EscreverValor(0x0006, numero); // Endereço KD
                Mensagem = $"Valor escrito: {numero}";
            }
            else
            {
                Mensagem = "Número inválido.";
            }
        }

        // Atualiza lista de portas seriais disponíveis
        private void AtualizarPortasDisponiveis()
        {
            PortasDisponiveis.Clear();
            foreach (var porta in SerialPort.GetPortNames())
                PortasDisponiveis.Add(porta);

            if (PortasDisponiveis.Count > 0)
                PortaSelecionada = PortasDisponiveis[0];
        }

        private async void IniciarLeituraPeriodica()
        {
            _leituraCts?.Cancel();
            _leituraCts = new CancellationTokenSource();
            var token = _leituraCts.Token;

            _ = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await LeituraCicloAsync();

                    if (_mode == CommMode.Rtu)
                        await Task.Delay(2000, token);
                    else
                        await Task.Delay(200, token);
                }
            }, token);
        }

        private async Task LeituraCicloAsync()
        {
            if (_emLeitura) return;
            _emLeitura = true;

            try
            {
                var resultado = await _modbusService.ReadAllRegistersAsync(_slaveId, 0x0209, 0x0000); // Exemplo endereços
                var dados = resultado.RawRegisters;

                // Atualiza propriedades bindadas
                Id = dados.Length > 0 ? dados[0] : 0;
                BaudRate = dados.Length > 1 ? dados[1] : 0;
                CanId = dados.Length > 2 ? dados[2] : 0;
                RefTensao = dados.Length > 3 ? dados[3] / 10.0 : 0;
                Kp = dados.Length > 4 ? dados[4] : 0;
                Ki = dados.Length > 5 ? dados[5] : 0;
                Kd = dados.Length > 6 ? dados[6] : 0;

                var alarmesAtivos = DecodificarAlarmes(resultado.AlarmRegister);

                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmes.Clear();
                    foreach (var alarme in alarmesAtivos)
                        Alarmes.Add(alarme);
                });

                if (!_leituraConcluida)
                    AtualizaValores();

                _leituraConcluida = true;
            }
            catch (Exception ex)
            {
                Mensagem = "Erro na leitura: " + ex.Message;
            }
            finally
            {
                _emLeitura = false;
            }
        }

        private System.Collections.Generic.List<string> DecodificarAlarmes(ushort bitmask)
        {
            string[] mensagens = {
                "Sobretensão do gerador",
                "Sobrecorrente do gerador",
                "Sobretensão de excitação",
                "Sobrecorrente de excitação",
                "U/F atuado",
                "Limite térmico atuado",
                "Falha de diodo girante",
                "Fator de potência baixo",
                "Falha no microcontrolador RDAC",
                "Falha na comunicação I2C",
                "Falha de realimentação",
                "Falha de rampa de realimentação",
                "Reservado",
                "Reservado",
                "Reservado",
                "Reservado"
            };

            var alarmesAtivos = new System.Collections.Generic.List<string>();
            for (int i = 0; i < 16; i++)
            {
                if (((bitmask >> i) & 1) == 1)
                    alarmesAtivos.Add(mensagens[i]);
            }

            return alarmesAtivos;
        }

        private void EscreverValor(int endereco, float valor)
        {
            try
            {
                _modbusService.WriteSingleRegister(_slaveId, (ushort)endereco, (ushort)valor);
                Mensagem = $"Valor no endereço {endereco} escrito: {valor}";
            }
            catch (Exception ex)
            {
                Mensagem = "Erro na escrita: " + ex.Message;
            }
        }

        private void AtualizaValores()
        {
            ParamRefTensao = RefTensao.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
            ParamGanhoKP = Kp.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
            ParamGanhoKI = Ki.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
            ParamGanhoKD = Kd.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
