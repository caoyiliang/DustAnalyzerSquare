using Communication;
using Communication.Bus.PhysicalPort;
using Communication.Exceptions;
using LogInterface;
using Modbus;
using Modbus.Parameter;
using TopPortLib.Interfaces;
using Utils;

namespace DustAnalyzerSquare
{
    public class DustAnalyzerSquare : IDustAnalyzerSquare
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<DustAnalyzerSquare>();
        private readonly IModBusMaster _modBusMaster;
        private bool _isConnect = false;
        public bool IsConnect => _isConnect;

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _modBusMaster.OnDisconnect += value; remove => _modBusMaster.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _modBusMaster.OnConnect += value; remove => _modBusMaster.OnConnect -= value; }

        public DustAnalyzerSquare(SerialPort serialPort, int defaultTimeout = 5000)
        {
            _modBusMaster = new ModBusMaster(serialPort, ModbusType.RTU, defaultTimeout)
            {
                IsHighByteBefore_Req = true,
                IsHighByteBefore_Rsp = false
            };
            _modBusMaster.OnSentData += CrowPort_OnSentData;
            _modBusMaster.OnReceivedData += CrowPort_OnReceivedData;
            _modBusMaster.OnConnect += CrowPort_OnConnect;
            _modBusMaster.OnDisconnect += CrowPort_OnDisconnect;
        }

        public DustAnalyzerSquare(ICrowPort crowPort)
        {
            _modBusMaster = new ModBusMaster(crowPort)
            {
                IsHighByteBefore_Req = true,
                IsHighByteBefore_Rsp = false
            };
            _modBusMaster.OnConnect += CrowPort_OnConnect;
            _modBusMaster.OnDisconnect += CrowPort_OnDisconnect;
        }

        private async Task CrowPort_OnDisconnect()
        {
            _isConnect = false;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnConnect()
        {
            _isConnect = true;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnReceivedData(byte[] data)
        {
            _logger.Trace($"DustAnalyzerSquare Rec:<-- {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnSentData(byte[] data)
        {
            _logger.Trace($"DustAnalyzerSquare Send:--> {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OpenAsync()
        {
            _isConnect = _modBusMaster.IsConnect;
            return _modBusMaster.OpenAsync();
        }

        /// <inheritdoc/>
        public async Task CloseAsync(bool closePhysicalPort)
        {
            if (closePhysicalPort) await _modBusMaster.CloseAsync(closePhysicalPort);
        }

        public async Task<Dictionary<string, string>?> Read(string addr, int tryCount = 0, CancellationToken cancelToken = default)
        {
            if (!_isConnect) throw new NotConnectedException();
            var b = new BlockList();
            b.Add(new PDustAnalyzerSquare());
            Func<Task<PDustAnalyzerSquare>> func = () => _modBusMaster.GetAsync<PDustAnalyzerSquare>(addr, b);
            var rs = await func.ReTry(tryCount, cancelToken);
            return rs == null ? null : new Dictionary<string, string>()
            {
                { "k", rs.k.ToString() },
                { "b", rs.b.ToString() },
                { "85", rs.浓度.ToString() },
                { "状态", rs.状态 switch
                {
                    0 => "N",
                    1 => "M",
                    2 => "C",
                    3 => "C",
                    4 => "M",
                    5 => "D",
                    6 => "N",
                    7 => "P",
                    8 => "M",
                    _ => "N"
                }},
                { "01-FullRange", rs.量程.ToString() }
            };
        }
    }
}
