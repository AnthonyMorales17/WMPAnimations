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
            axWindowsMediaPlayer1.PlayStateChange += axWindowsMediaPlayer1_PlayStateChange;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!trackLoaded)
            {
                MessageBox.Show("Debe cargar una pista antes de reproducir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
            lblTrackName.Text = "";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (!trackLoaded) return;

            trackIndex = (trackIndex - 1 + tracks.Length) % tracks.Length;
            LoadCurrentTrack();
            UpdateNameTrack();
            btnPlay.PerformClick();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (!trackLoaded)
            {
                MessageBox.Show("No hay pistas cargadas.");
                return;
            }

            trackIndex = (trackIndex + 1) % tracks.Length;
            LoadCurrentTrack();
            UpdateNameTrack();
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
                for (int i = 0; i < Math.Min(ofd.FileNames.Length, tracks.Length); i++)
                {
                    tracks[i] = ofd.FileNames[i];
                }

                trackIndex = 0;
                LoadCurrentTrack();
                UpdateNameTrack();
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

        private void UpdateNameTrack()
        {
            if (trackLoaded && !string.IsNullOrEmpty(tracks[trackIndex]))
            {
                string nameFile = System.IO.Path.GetFileName(tracks[trackIndex]);
                lblTrackName.Text = $"{nameFile}";
            }
            else
            {
                lblTrackName.Text = "Ninguna pista cargada";
            }
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            // 8 = MediaEnded
            if (e.newState == 8)
            {
                trackIndex = (trackIndex + 1) % tracks.Length;

                // Verificamos si la siguiente pista está cargada
                if (!string.IsNullOrEmpty(tracks[trackIndex]))
                {
                    //Se usa Invoke para asegurarse que los cambios se hagan en el hilo de la interfaz gràfica.
                    Invoke(new Action(() =>
                    {
                        LoadCurrentTrack();
                        UpdateNameTrack();
                        // Reproducir automáticamente
                        trackStarted = true;
                        player.Play();
                        timer1.Start();
                    }));
                }
            }
        }
    }
}
