using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uprootd
{
    internal class Syslog
    {
        internal int Facility;

        internal int Severity;

        internal string Server;

        internal int Port;

        internal Syslog(int facility, int severity, string server, int port)
        {
            this.Facility = facility;
            this.Severity = severity;
            this.Server = server;
            this.Port = port;
        }

        internal int Send(DateTime timestamp, string hostname, string message)
        {
            string s = string.Format("<{0}>1 {1} {2} {3}", new object[]
			{
				this.Facility * 8 + this.Severity,
				timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
				hostname,
				message
			});
            int result;
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.Server), this.Port);
                UdpClient udpClient = new UdpClient();
                udpClient.Connect(endPoint);
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                udpClient.Send(bytes, bytes.Length);
                result = 0;
            }
            catch
            {
                result = 1;
            }
            return result;
        }
    }
}
