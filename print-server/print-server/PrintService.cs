using System;
using System.ServiceProcess;
using System.IO;
using Core;

namespace print_server
{
    public partial class PrintService : ServiceBase
    {
        public PrintService()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            HttpProvider http = new HttpProvider();
        }

        protected override void OnStop()
        {
            
        }
    }
}