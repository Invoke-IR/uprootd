using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace Uprootd
{
    public class Uprootd : ServiceBase
    {
        private Listener l;

        private IContainer components;

        public Uprootd()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.l = new Listener(args[0], new Syslog(3, 6, args[1], int.Parse(args[2])));
            this.l.Start();
        }

        protected override void OnStop()
        {
            this.l.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            base.ServiceName = "Uprootd";
        }
    }
}
