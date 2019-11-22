using System;
using System.Windows;
using System.Windows.Input;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        public Game Game = new Game();

        public MainWindow()
        {
            InitializeComponent();
            Game.MW = this;
            Game.Score.MW = this;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!Game.IsActive)
                {
                    Game.Start();
                }
                else if (!Game.IsPaused)
                {
                    Game.Floor();
                }
            }
            else if (e.Key == Key.P)
            {
                Game.Pause();
            }
            else if (Game.IsPaused || !Game.IsActive)
            {
                return;
            }
            else if (e.Key == Key.S)
            {
                Game.Drop();
            }
            else if (e.Key == Key.W)
            {
                Game.Rotate();
            }
            else if (e.Key == Key.A || e.Key == Key.Left)
            {
                Game.LeftDown = true;
                if (Game.MovingDirection != 1)
                    Game.MovingDirection = -1;
            }
            else if (e.Key == Key.D || e.Key == Key.Right)
            {
                Game.RightDown = true;
                if (Game.MovingDirection != -1)
                    Game.MovingDirection = 1;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {

            }
            else if (e.Key == Key.W)
            {
                Game.RotateDown = false;
            }
            else if (e.Key == Key.A || e.Key == Key.Left)
            {
                Game.LeftDown = false;
                if (Game.RightDown == false)
                    Game.MovingDirection = 0;
            }
            else if (e.Key == Key.D || e.Key == Key.Right)
            {
                Game.RightDown = false;
                if (Game.LeftDown == false)
                    Game.MovingDirection = 0;
            }
        }

        private void btnScores_Click(object sender, RoutedEventArgs e)
        {
            ScoreWindow sw = new ScoreWindow();
            sw.Show();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            Game.Start();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            Game.Pause();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
