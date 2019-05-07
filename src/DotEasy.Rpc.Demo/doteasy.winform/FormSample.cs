using System;
using System.Windows.Forms;
using DotEasy.Rpc.Consul.Entry;
using doteasy.rpc.interfaces;
using Newtonsoft.Json;

namespace doteasy.winform
{
    /// <summary>
    /// 长连接通信测试
    /// </summary>
    public partial class FormSample : Form
    {
        public FormSample()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var proxy = ClientProxy.Generate<IProxyCommpoundService>(new Uri("http://127.0.0.1:8500")))
            {
                Console.WriteLine($@"{JsonConvert.SerializeObject(proxy.GetCurrentObject(new CompoundObject()))}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}