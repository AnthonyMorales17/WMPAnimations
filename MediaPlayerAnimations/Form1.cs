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
        private CMediaPlayer player;
        private string[] tracks = new string[3];
        private int trackIndex = 0;
        private bool trackLoaded = false;
        private bool trackStarted = false;

        public FrmMediaPlayer()
        {
            InitializeComponent();
            timer1.Interval = 1000; // Actualiza cada segundo
        }

        private void FrmMediaPlayer_Load(object sender, EventArgs e)
        {
            player = new CMediaPlayer(axWindowsMediaPlayer1, timerAnimacion, picCanvas);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!trackLoaded) return;

            if (!trackStarted)
            {
                player.LoadTrack(tracks[trackIndex], trackIndex);
                trackStarted = true;
            }

            player.Play();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                double duration = axWindowsMediaPlayer1.currentMedia.duration;
                double current = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;

                if (duration > 0)
                {
                    int progress = (int)(current / duration * 100);
                    pgBar.Value = Math.Min(progress, 100);

                    TimeSpan currentTime = TimeSpan.FromSeconds(current);
                    TimeSpan totalTime = TimeSpan.FromSeconds(duration);

                    lblTimer.Text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
                }
            }
            catch
            {
                // Silenciar errores en reproducción
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            player.Pause();
            timer1.Stop();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            player.Stop();
            timer1.Stop();
            trackStarted = false;
            pgBar.Value = 0;
            lblTimer.Text = "00:00 / 00:00";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (!trackLoaded) return;

            trackIndex = (trackIndex - 1 + 3) % 3;
            LoadCurrentTrack();
            btnPlay.PerformClick();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (!trackLoaded) return;

            trackIndex = (trackIndex + 1) % 3;
            LoadCurrentTrack();
            btnPlay.PerformClick();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.wav"
            };

            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileNames.Length > 0)
            {
                for (int i = 0; i < Math.Min(ofd.FileNames.Length, 3); i++)
                {
                    tracks[i] = ofd.FileNames[i];
                }

                trackIndex = 0;
                LoadCurrentTrack();
            }
        }
        private void LoadCurrentTrack()
        {
            if (!string.IsNullOrEmpty(tracks[trackIndex]))
            {
                player.LoadTrack(tracks[trackIndex], trackIndex); // Cargar con estilo según pista
                trackLoaded = true;
                trackStarted = false;
                lblTimer.Text = "00:00 / 00:00";
                pgBar.Value = 0;
            }
        }
    }
}
