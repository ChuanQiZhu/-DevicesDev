using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NT_RFIDService
{
    public partial class NT_RFIDService : ServiceBase
    {
        Reader reader = null;
        public NT_RFIDService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            reader.Start();
        }

        protected override void OnStop()
        {
            reader.Stop();
        }
    }
}
