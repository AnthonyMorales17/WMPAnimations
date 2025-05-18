using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaPlayerAnimations
{
    public partial class FrmMediaPlayer : Form
    {
        private CMediaPlayer reproductor;
        private string ruta;
        private bool pistaCargada = false;
        private bool pistaIniciada = false;

        public FrmMediaPlayer()
        {
            InitializeComponent();
            ruta = "";
            pistaCargada = false;
            pistaIniciada = false;
            timer1.Interval = 1000; // Actualiza cada segundo
        }

        private void FrmMediaPlayer_Load(object sender, EventArgs e)
        {
            reproductor = new CMediaPlayer(axWindowsMediaPlayer1, timerAnimacion, picCanvas);
        }

        int progreso = 0;

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!pistaCargada) return;

            if (!pistaIniciada)
            {
                reproductor.LoadTrack(ruta);
                pistaIniciada = true;
            }

            reproductor.Play();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                double duracion = axWindowsMediaPlayer1.currentMedia.duration;
                double actual = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;

                if (duracion > 0)
                {
                    int progreso = (int)(actual / duracion * 100);
                    pgBar.Value = Math.Min(progreso, 100);

                    TimeSpan tiempoActual = TimeSpan.FromSeconds(actual);
                    TimeSpan tiempoTotal = TimeSpan.FromSeconds(duracion);

                    lblTimer.Text = $"{tiempoActual:mm\\:ss} / {tiempoTotal:mm\\:ss}";
                }
            }
            catch
            {
                // Silenciar errores en reproducción
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            reproductor.Pause();
            timer1.Stop();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            reproductor.Stop();
            pistaIniciada = false;
            timer1.Stop();
            pgBar.Value = 0;
            lblTimer.Text = "00:00 / 00:00";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Esta función no está disponible con una sola pista.");
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Esta función no está disponible con una sola pista.");
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ruta = openFileDialog1.FileName;
                pistaCargada = true;
                pistaIniciada = false;
                pgBar.Value = 0;
                lblTimer.Text = "00:00 / 00:00";
            }
        }
    }
}
