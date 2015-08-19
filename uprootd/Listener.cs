using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Uprootd
{
    internal class Listener
    {
        internal HttpListener listener;

        internal string Server;

        internal int Port;

        internal string URI;

        internal Syslog Forwarder;

        private Thread listenThread1;

        private bool stop;

        internal Listener(string uri, Syslog f)
        {
            this.Server = uri.Split(new char[]
			{
				'/'
			})[2].Split(new char[]
			{
				':'
			})[0];
            this.Port = Convert.ToInt32(uri.Split(new char[]
			{
				':'
			})[2].Split(new char[]
			{
				'/'
			})[0]);
            this.URI = uri;
            this.Forwarder = f;
        }

        internal bool Start()
        {
            bool result;
            try
            {
                this.listener = new HttpListener();
                this.listener.Prefixes.Add(this.URI);
                this.listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                this.listener.Start();
                this.listenThread1 = new Thread(new ParameterizedThreadStart(this.startlistener));
                this.listenThread1.Start();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        internal void startlistener(object s)
        {
            while (!this.stop)
            {
                this.ProcessRequest();
            }
        }

        internal void ProcessRequest()
        {
            while (this.listener.IsListening)
            {
                try
                {
                    IAsyncResult asyncResult = this.listener.BeginGetContext(new AsyncCallback(this.ListenerCallback), this.listener);
                    asyncResult.AsyncWaitHandle.WaitOne();
                }
                catch
                {
                }
            }
        }

        internal void ListenerCallback(IAsyncResult result)
        {
            try
            {
                if (this.listener.IsListening)
                {
                    HttpListenerContext httpListenerContext = this.listener.EndGetContext(result);
                    string message = new StreamReader(httpListenerContext.Request.InputStream, httpListenerContext.Request.ContentEncoding).ReadToEnd();
                    if (this.Forwarder != null)
                    {
                        this.Forwarder.Send(DateTime.UtcNow, httpListenerContext.Request.RemoteEndPoint.Address.ToString(), message);
                    }
                    httpListenerContext.Response.StatusCode = 200;
                    httpListenerContext.Response.StatusDescription = "OK";
                    httpListenerContext.Response.Close();
                }
            }
            catch
            {
            }
        }

        internal void Stop()
        {
            this.listener.Stop();
            this.listener.Close();
            this.stop = true;
        }
    }
}