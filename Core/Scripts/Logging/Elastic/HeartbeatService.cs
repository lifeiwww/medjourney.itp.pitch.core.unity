using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace manutd
{
    public class HeartbeatService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TcpListener _tcpListener;

        public HeartbeatService(string ip = "127.0.0.1", int port = 13000)
        {
            var ipAddress = IPAddress.Parse(ip);
            _tcpListener = new TcpListener(ipAddress, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Log.Information("Heartbeat service starting");

            _cancellationTokenSource.Token.Register(() =>
            {
                Log.Information("Heartbeat service stopping");
                _tcpListener.Stop();
            });

            Task.Run(() =>
            {
                try
                {
                    _tcpListener.Start();
                    var bytes = new byte[256]; // buffer for reading data

                    while (_cancellationTokenSource.IsCancellationRequested == false)
                    {
                        var client = _tcpListener.AcceptTcpClient();
                        var stream = client.GetStream();
                        int i;

                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var data = Encoding.ASCII.GetString(bytes, 0, i).ToUpper();
                            var msg = Encoding.ASCII.GetBytes(data);
                            stream.Write(msg, 0, msg.Length);
                        }

                        client.Close();
                    }
                }
                catch (SocketException exception)
                {
                    // ok - interrupted error is called when task is canceled since AcceptTcpClient is blocking
                    if (exception.SocketErrorCode != SocketError.Interrupted)
                        Log.Error($"Heartbeat service error: {exception}");
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}