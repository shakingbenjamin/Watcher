using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PUtilsLibrary;
using System.Timers;

namespace WFVersion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Reveal(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            var start = new StartUp();
            switch (StartBtn.Text)
            {
                case "Stop":
                    start.StopTime();
                    StartBtn.Text = "Start";
                    break;
                case "Start":
                    start.GoTimeWeb();
                    StartBtn.Text = "Stop";
                break;
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ToTrayBtn_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            Hide();
        }
    }
}
