using System.Threading.Tasks;

namespace GRTDappWpf.Models
{
    public interface IModbusService
    {
        void ConnectTcp(string ip, int port);
        void ConnectRtu(string port, int baud, System.IO.Ports.Parity parity, System.IO.Ports.StopBits stopBits);
        void Disconnect();
        Task<ModbusService.ModbusReadResult> ReadAllRegistersAsync(byte slaveId, ushort addrAlarmes, ushort addrReadRegisters);
        void WriteSingleRegister(byte slaveId, ushort registerAddress, ushort value);
    }
}
