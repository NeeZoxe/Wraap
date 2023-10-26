using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Threading;

namespace NeeZoxeClicker
{
    public partial class Form2 : Form
    {
        private bool videoEnded = false;
        private bool pseudoEnded = false;

        public Form2()
        {
            InitializeComponent();
            progressbar1.Value = 0;
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(axWindowsMediaPlayer1_PlayStateChange);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string videoPath = Path.GetTempFileName() + ".mp4";
            using (var videoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NeeZoxeClicker.Resources.Loading.mp4"))
            using (var fileStream = File.Create(videoPath))
            {
                videoStream.CopyTo(fileStream);
            }

            axWindowsMediaPlayer1.URL = "";
            axWindowsMediaPlayer1.currentPlaylist.clear();
            axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(videoPath));
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8)
            {
                videoEnded = true;
            }
            else
            {
                videoEnded = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                videoEnded = true;
            }
            if (videoEnded == true)
            {
                label3.Text = "HELLO " + Environment.UserName;
                label3.ForeColor = Color.White;
                pseudoEnded = true;
                label3.BringToFront();
                // Démarrez le timer2 avec une intervalle de 1 seconde
                timer2.Interval = 1000;
                timer2.Start();
            }
        }

        private void progressbar1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
                // Arrêtez les deux timers
                timer1.Stop();
                timer2.Stop();

                // Créez une nouvelle instance de Form1 et affichez-la
                Form1 se_form = new Form1();
                se_form.Show();

                // Cachez la Form2 actuelle
                this.Hide();
            
        }
    }
}
