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
        private Timer animationTimer;
        private int progress;
        private PictureBox canvas;
        private Pen pen = new Pen(Color.MediumVioletRed, 2);
        private int offset = 0;
        private int style = 0;

        public CMediaPlayer(AxWMPLib.AxWindowsMediaPlayer mediaPlayer, Timer timer, PictureBox canvas)
        {
            this.player = mediaPlayer;
            this.animationTimer = timer;
            this.canvas = canvas;

            this.animationTimer.Tick += Timer_Tick;
        }

        public void LoadTrack(string path, int styleIndex)
        {
            player.URL = path;
            SetStyle(styleIndex);
        }

        public void SetStyle(int styleIndex)
        {
            style = styleIndex;
            switch (style)
            {
                case 0:
                    pen = new Pen(Color.Cyan, 2);
                    break;
                case 1:
                    pen = new Pen(Color.MediumVioletRed, 2);
                    break;
                case 2:
                    pen = new Pen(Color.OrangeRed, 2);
                    break;
            }
        }

        public void Play()
        {
            player.Ctlcontrols.play();
            animationTimer.Start();
        }

        public void Pause()
        {
            player.Ctlcontrols.pause();
            animationTimer.Stop();
        }

        public void Stop()
        {
            player.Ctlcontrols.stop();
            animationTimer.Stop();
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

                switch (style)
                {
                    case 0:
                        DrawLines(g, centerX, centerY, radius);
                        break;
                    case 1:
                        DrawArcs(g, centerX, centerY, radius);
                        break;
                    case 2:
                        DrawBezierCurves(g, centerX, centerY, radius);
                        break;
                }
            }

            canvas.Image?.Dispose(); // Libera memoria
            canvas.Image = bmp;

            offset += 3; 
        }

        private void DrawLines(Graphics g, int cx, int cy, int r)
        {
            for (int angle = 0; angle < 360; angle += 15)
            {
                double rad = (angle + offset) * Math.PI / 180;
                int x = cx + (int)(r * Math.Cos(rad));
                int y = cy + (int)(r * Math.Sin(rad));
                g.DrawLine(pen, cx, cy, x, y);
            }
        }

        private void DrawArcs(Graphics g, int cx, int cy, int r)
        {
            for (int i = 0; i < 360; i += 30)
            {
                double rad = (i + offset) * Math.PI / 180;
                int x = cx + (int)(r * Math.Cos(rad));
                int y = cy + (int)(r * Math.Sin(rad));
                g.DrawArc(pen, x - 20, y - 20, 40, 40, 0, 180);
            }
        }

        private void DrawBezierCurves(Graphics g, int cx, int cy, int r)
        {
            for (int i = 0; i < 360; i += 30)
            {
                double rad = (i + offset) * Math.PI / 180;
                int x = cx + (int)(r * Math.Cos(rad));
                int y = cy + (int)(r * Math.Sin(rad));
                g.DrawBezier(pen, cx, cy, cx + 20, cy - 40, x - 20, y + 40, x, y);
            }
        }

    }
}
