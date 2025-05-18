using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaPlayerAnimations
{
    class CMediaPlayer
    {
        private AxWMPLib.AxWindowsMediaPlayer player;
        private Timer timerAnimation;
        private int progress;
        private PictureBox canvas;
        private Pen pen = new Pen(Color.MediumVioletRed, 2);
        private int offset = 0;

        public CMediaPlayer(AxWMPLib.AxWindowsMediaPlayer mediaPlayer, Timer timerAnimation, PictureBox canvas)
        {
            this.player = mediaPlayer;
            this.timerAnimation = timerAnimation;
            this.canvas = canvas;

            this.timerAnimation.Tick += Timer_Tick;
        }

        public void LoadTrack(string ruta)
        {
            player.URL = ruta;
        }

        public void Play()
        {
            player.Ctlcontrols.play();
            timerAnimation.Start();
        }

        public void Pause()
        {
            player.Ctlcontrols.pause();
            timerAnimation.Stop();
        }

        public void Stop()
        {
            player.Ctlcontrols.stop();
            timerAnimation.Stop();
            canvas.Image?.Dispose();
            canvas.Image = null;
            progress = 0;
            offset = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (canvas.Width == 0 || canvas.Height == 0) return;

            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);

                int centerX = canvas.Width / 2;
                int centerY = canvas.Height / 2;
                int radius = Math.Min(canvas.Width, canvas.Height) / 2 - 10;

                int angleStep = 15; // cada 15 grados (360 / 15 = 24 líneas)
                for (int angle = 0; angle < 360; angle += angleStep)
                {
                    double radians = (angle + offset) * Math.PI / 180;
                    int x = centerX + (int)(radius * Math.Cos(radians));
                    int y = centerY + (int)(radius * Math.Sin(radians));

                    g.DrawLine(pen, centerX, centerY, x, y);
                }
            }

            canvas.Image?.Dispose(); // Libera memoria
            canvas.Image = bmp;

            offset += 3; 
        }

    }
}
