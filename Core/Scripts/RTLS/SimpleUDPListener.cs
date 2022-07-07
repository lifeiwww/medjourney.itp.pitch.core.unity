using System;
using System.Net;
using System.Net.Sockets;
using Serilog;

namespace dreamcube.unity.Core.Scripts.Components.RTLS
{
    public class SimpleUDPListener
    {
        private readonly UdpClient _client;
        private bool _closed;

        private IPEndPoint _remoteEp;

        public SimpleUDPListener(string localIP, string remoteIP, int port, bool isMulticast)
        {
            var localEp = new IPEndPoint(IPAddress.Parse(localIP), port);
            _remoteEp = new IPEndPoint(IPAddress.Parse(remoteIP), port);

            // Create a UDP Client
            _client = new UdpClient();

            // If multicast is enabled, allow ports to be reused.
            // See here for more information:
            // http://www.jarloo.com/c-udp-multicasting-tutorial/
            if (isMulticast)
            {
                _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _client.ExclusiveAddressUse = false;
            }

            Log.Information($"Binding UDP to local port {port}");
            _client.Client.Bind(localEp);

            // Join multicast group if applicable
            if (isMulticast)
            {
                Log.Information($"Joining UDP multicast {remoteIP}:{port}");
                var multicastaddress = IPAddress.Parse(remoteIP);
                _client.JoinMulticastGroup(multicastaddress);
            }
            else
            {
                Log.Information($"Using UDP unicast: {remoteIP}:{port}");
            }

            // use initial callback to log some info
            _client.BeginReceive(ReceiveInitialServerInfo, null);
        }

        public event EventHandler DataReceived;

        private void ReceiveInitialServerInfo(IAsyncResult result)
        {
            if (_closed) return;
            Log.Information($"Received UDP data {result} from {_remoteEp.Address}:{_remoteEp.Port.ToString()}");
            ReceiveServerInfo(result);
        }

        private void ReceiveServerInfo(IAsyncResult result)
        {
            if (_closed) return;
            try
            {
                // get data
                var receivedBytes = _client.EndReceive(result, ref _remoteEp);
                // send to callback
                DataReceived?.Invoke(receivedBytes, new EventArgs());
                // get more data
                _client.BeginReceive(ReceiveServerInfo, null);
            }
            catch (Exception e) when (!(e is ObjectDisposedException))
            {
                Log.Error("Exception occurred while trying to receive UDP message: " + e.Message);
            }
        }

        public void Close()
        {
            try
            {
                if (_client.Client.Connected)
                {
                    _client.Client.Close();
                    _client.Close();
                }

                _closed = true;
                Log.Debug("UDP connection closed");
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
        }
    }
}