//Bibliotecas
using FluentModbus;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

//Nomespace
namespace GRTDappWpf.Models
{
    // Implementa a interface IModbusService ao ModbusService
    public class ModbusService : IModbusService, IDisposable
    {
        // Armazena os dois tipos de clientes suportados.
        // _modoAtual guarda o tipo da conexão ativa (TCP ou RTU).
        private ModbusTcpClient _tcpClient;
        private ModbusRtuClient _rtuClient;
        private CommMode _modoAtual;

        // Método para conexão em TCP
        public void ConnectTcp(string ip, int port)
        {
            Disconnect();

            _tcpClient = new ModbusTcpClient();
            _tcpClient.Connect(ip, ModbusEndianness.BigEndian);

            // Define tempo máximo de espera para leitura/escrita
            _tcpClient.ReadTimeout = 2000;
            _tcpClient.WriteTimeout = 2000;

            // Armazena o modo em que está conectado
            _modoAtual = CommMode.Tcp;

        }

        // Método para conexão em RTU
        public void ConnectRtu(string port, int baud, Parity parity, StopBits stopBits)
        {
            Disconnect();

            // Define tempo de espera para leitura/escrita
            _rtuClient = new ModbusRtuClient
            {
                BaudRate = baud,
                Parity = parity,
                StopBits = stopBits
            };
            _rtuClient.Connect(port, ModbusEndianness.BigEndian);

            _rtuClient.ReadTimeout = 5000;  // Timeout maior para evitar erros
            _rtuClient.WriteTimeout = 5000; // Timeout maior para evitar erros

            // Armazena o modo em que está conectado
            _modoAtual = CommMode.Rtu;
        }

        // Método para desconexão
        public void Disconnect()
        {
            _tcpClient?.Dispose();
            _tcpClient = null;

            _rtuClient?.Dispose();
            _rtuClient = null;
        }

        // Método Dispose
        public void Dispose()
        {
            Disconnect();
        }

        // Método para leitura dos registradores
        public async Task<ModbusReadResult> ReadAllRegistersAsync(byte slaveId, ushort addrAlarmes, ushort addrReadRegisters)
        {
            if (_modoAtual == CommMode.Tcp && _tcpClient != null)
            {
                return new ModbusReadResult
                {
                    AlarmRegister = _tcpClient.ReadHoldingRegisters<ushort>(slaveId, addrAlarmes, 1)[0],
                    RawRegisters = _tcpClient.ReadHoldingRegisters<ushort>(slaveId, addrReadRegisters, 7).ToArray()
                };
            }
            else if (_modoAtual == CommMode.Rtu && _rtuClient != null)
            {
                // Leitura dos demais registradores
                var data = await Task.Run(() => _rtuClient.ReadHoldingRegisters<ushort>(slaveId, addrReadRegisters, 7).ToArray());

                // Aguardar para evitar colisão na linha serial
                await Task.Delay(500);

                // Leitura do alarme
                var alarm = await Task.Run(() => _rtuClient.ReadHoldingRegisters<ushort>(slaveId, addrAlarmes, 1)[0]);

                return new ModbusReadResult
                {
                    AlarmRegister = alarm,
                    RawRegisters = data
                };
            }
            else
            {
                throw new InvalidOperationException("Nenhum cliente Modbus conectado.");
            }
        }

        // Método para escrita dos registradores
        public void WriteSingleRegister(byte slaveId, ushort registerAddress, ushort value)
        {
            if (_modoAtual == CommMode.Tcp && _tcpClient != null)
            {
                _tcpClient.WriteSingleRegister(slaveId, registerAddress, value);
            }
            else if (_modoAtual == CommMode.Rtu && _rtuClient != null)
            {
                _rtuClient.WriteSingleRegister(slaveId, registerAddress, value);
            }
            else
            {
                throw new InvalidOperationException("Nenhum cliente Modbus conectado.");
            }
        }

        // Classe interna
        public class ModbusReadResult
        {
            public ushort AlarmRegister { get; set; }
            public ushort[] RawRegisters { get; set; } = new ushort[7];
        }

    }
}

