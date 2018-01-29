using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace BatteryNotify.WindowsFormsApp
{
    public partial class Form1 : Form {
        private readonly int IconTooltipMaxLength = 64;
        private readonly WebClient _webClient = new WebClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(0, 0);
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Hide();
            
            ThreadPool.QueueUserWorkItem(state => {
                var tempFileName = Path.GetTempFileName();
                while (true) {
                    try {
                        var batteryRemainingPercentage = (int)(SystemInformation.PowerStatus.BatteryLifePercent*100);
                        batteryRemainingPercentage = batteryRemainingPercentage < 100 ? batteryRemainingPercentage : 100;
                        batteryRemainingPercentage = batteryRemainingPercentage > 0 ? batteryRemainingPercentage : 0;
                        _webClient.DownloadFile($"http://home.digitalcreations.cc:8468/NumberDisplayer/{batteryRemainingPercentage}", tempFileName);
                        notifyIcon.Text = $@"{DateTime.Now}";
                    }
                    catch (Exception exception) {
                        notifyIcon.Text = exception.Message.Substring(0, exception.Message.Length < IconTooltipMaxLength ? exception.Message.Length : IconTooltipMaxLength - 1);
                    }
                    Thread.Sleep(60000);
                }
                // ReSharper disable once FunctionNeverReturns
            });
        }

        private void exitContextMenuItem_Click(object sender, EventArgs e)
        {
            _webClient.Dispose();
            Close();
        }
    }
}
