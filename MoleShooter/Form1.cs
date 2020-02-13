#define MyDebug

using MoleShooter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace MoleShooter
{
    public partial class MoleShooter : Form
    {
        const int FrameNum = 8;
        const int SplatNum = 3;
        bool splat = false;
        int _cursX = 0, _cursY = 0;
        int _gameFrame = 0;
        int _SplatTime = 0;

        // Initialisation des données du tableau
        int _hits = 0;
        int _misses = 0;
        int _totalShots = 0;
        double _averageHits = 0;

        CMole _mole;
        CSplat _splat;
        CSign _sign;
        CGrass _grass_one;
        CGrass _grass_two;
        CScoreFrame _scoreframe;
    
        // WMP
        WindowsMediaPlayer Game_Music;

        Random rnd = new Random();

        public MoleShooter()
        {
            InitializeComponent();

            Bitmap b = new Bitmap(Resources.Cible); //*** Créer le curseur cible
            this.Cursor = CustomCursor.CreateCursor(b, b.Height / 2, b.Width / 2);//***

            _scoreframe = new CScoreFrame() { Left = 10, Top = 10 };
            _sign = new CSign() { Left = 700, Top = 90 };
            _splat = new CSplat();
            _mole = new CMole() { Left = 250, Top = 500 };
            _grass_one = new CGrass() { Left = 450, Top = 245 };
            _grass_two = new CGrass() { Left = 50, Top = 245 };
           

            Game_Music = new WindowsMediaPlayer();
            Game_Music.URL = "sounds\\David_Renda_FesliyanStudios.com.mp3";
            Game_Music.settings.setMode("loop", true);
            Game_Music.settings.volume = 8;
            Game_Music.controls.stop();

              
        }

        private void timerGameLoop_Tick(object sender, EventArgs e)
        {

            if (_gameFrame >= 5)
            {
                UpdateMole();
                _gameFrame = 0;
            }

            if (splat)
            {
                if (_SplatTime >= SplatNum)
                {
                    splat = false;
                    _SplatTime = 0;
                    UpdateMole();
                }
                _SplatTime++;
            }

            _gameFrame++;

            this.Refresh();
        }

        private void UpdateMole()
        {
            _mole.Update(
                rnd.Next(Resources.Mole.Width, this.Width - Resources.Mole.Width),
                rnd.Next(this.Height / 2, this.Height - Resources.Mole.Height * 2)
                );
        }

        private void MoleShooter_MouseMove(object sender, MouseEventArgs e)
        {
            _cursX = e.X;
            _cursY = e.Y;

            this.Refresh();
        }

        private void MoleShooter_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 750 && e.X < 933 && e.Y > 147 && e.Y < 170 ) // Débute le jeux
            {
                timerGameLoop.Start();
                Game_Music.controls.play(); 
            }

           else if (e.X > 750 && e.X < 929 && e.Y > 192 && e.Y < 219) // Stop le jeux
            {
                timerGameLoop.Stop();
                Game_Music.controls.stop();
            }

           else if (e.X > 742 && e.X < 941 && e.Y > 241 && e.Y < 259) // Reset le jeux
            {
                _hits = 0;
                _misses = 0;
                _totalShots = 0;
                _averageHits = 0;

                timerGameLoop.Stop();
                Game_Music.controls.stop();
            }
            else if (e.X > 750 && e.X < 940 && e.Y > 282 && e.Y < 303) // Quitte le jeux
            {
                timerGameLoop.Stop();
                Close();
            }
            else
            {
                if (_mole.Hit(e.X,e.Y))
                {
                     splat = true;
                    _splat.Left = _mole.Left - Resources.Splat.Width / 3;
                    _splat.Top = _mole.Top - Resources.Splat.Height / 3;

                    _hits++;
                }
                else
                {
                    _misses++;
                }
                
                _totalShots = _hits + _misses;
                _averageHits = (double)_hits / _totalShots * 100.0;
            }

            FireGun();
        }

        private void FireGun()
        {
            // Son d'un pistolet silencieux
            SoundPlayer simpleSound = new SoundPlayer(Resources.Gun);

            simpleSound.Play();

        }

        private void MoleShooter_Load(object sender, EventArgs e)
        {
           
        }

        private void SplashScreenTimer_Tick(object sender, EventArgs e)
        {

        }

        private void MoleShooter_FormClosed(object sender, FormClosedEventArgs e)
        {
            //exit application when form is closed

            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics dc = e.Graphics;
           
          
         //   _splash_screen.DrawImage(dc);
         
            _grass_one.DrawImage(dc);
            _grass_two.DrawImage(dc);
            _sign.DrawImage(dc);

            if ( splat == true )
            {
                _splat.DrawImage(dc);
            }
            else
            {
                _mole.DrawImage(dc);
            }

    
            _scoreframe.DrawImage(dc);

            //_mole.DrawImage(dc);

            TextFormatFlags flags = TextFormatFlags.Left;
            Font _font = new System.Drawing.Font("Stencil", 12, FontStyle.Regular);
            TextRenderer.DrawText(e.Graphics, "Tirs: " + _totalShots.ToString(), _font, new Rectangle(70 ,132 ,120 ,20 ), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Touché: " + _hits.ToString(), _font, new Rectangle(70 ,152 ,120 ,20 ), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Raté: " + _misses.ToString(), _font, new Rectangle(70, 172, 120, 20), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Score Max: " + _averageHits.ToString("F0") + "%", _font, new Rectangle(70, 192, 140, 40), SystemColors.ControlText, flags);

            base.OnPaint(e);
 #if MyDebug

         //   TextFormatFlags flags = TextFormatFlags.Left| TextFormatFlags.EndEllipsis;
         //   Font _font = new System.Drawing.Font("Stencil", 12, FontStyle.Regular);
            TextRenderer.DrawText(dc, "X=" + _cursX.ToString() + ":" + "Y=" + _cursY.ToString(), _font,
            new Rectangle(0, 0, 120, 20), SystemColors.ControlText, flags);
 #endif

        }
    }
}
