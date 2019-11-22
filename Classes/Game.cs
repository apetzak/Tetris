using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;

namespace Tetris
{
    public class Game
    {
        public MainWindow MW;
        public DispatcherTimer Timer = new DispatcherTimer();
        public Random Random = new Random();
        public Shape Shape;
        public Shape NextShape;
        public List<Block> Blocks = new List<Block>();
        public Score Score = new Score();
        public bool IsActive = false;
        public bool IsPaused = false;
        public bool Next;
        public int TimeTicks;
        public int TickCount;
        public bool RotateDown = false;
        public int MovingDirection = 0;
        public bool RightDown = false;
        public bool LeftDown = false;
        
        public Game()
        {
            Timer.Interval = new TimeSpan(100000); // .1 seconds
            Timer.Tick += Timer_Tick;
        }

        public void Start()
        {
            ResetStartingVariables();
            MW.cvsMain.Children.Clear();
            Shape = new Shape(Random.Next(0, 7));
            SetNextShape();
            IsActive = true;
            Timer.Start();
        }

        public void ResetStartingVariables()
        {
            Blocks.Clear();
            Score.Reset();
            TimeTicks = 0;
            TickCount = 0;
        }

        public void SetNextShape()
        {
            Shape.MoveToStartingLocation();
            NextShape = new Shape(Random.Next(0, 7));
            NextShape.PaintNext(MW.cvsNext);
            Next = false;
        }

        public void Pause()
        {
            if (IsPaused == true)
            {
                Timer.Start();
                IsPaused = false;
            }
            else
            {
                Timer.Stop();
                IsPaused = true;
            }
        }

        public void IncrementTime()
        {
            if (TimeTicks++ == 100)
            {
                TimeTicks = 0;
                Score.UpdateLabel(MW.lblTime, Score.Time++);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Shift();
            IncrementTime();
            TickCount++;
            if (TickCount < Score.DelayTicks)
                return;
            TickCount = 0;

            if (Next)
                AddNext();

            if (GetSpacesBelow() == 0)
            {
                Next = true;
                return;
            }              

            Shape.Drop(1);
            Paint();
        }

        public void Paint()
        {
            MW.cvsMain.Children.Clear();
            foreach (Block b in Blocks)
                b.Paint(MW.cvsMain);
            Shape.Paint(MW.cvsMain);
            PaintOutline();
        }

        public void PaintOutline()
        {
            int spaces = GetSpacesBelow();
            List<Block> list = new List<Block>();
            foreach (Block b in Shape.Blocks)
                list.Add(new Block(b.X, b.Y, Colors.Black));
            foreach (Block b in list)
            {
                b.Y -= spaces;
                b.PaintOutline(MW.cvsMain);
            }          
        }

        public bool GameOver()
        {
            if (!IsActive)
                return true;

            foreach (Block b in Blocks)
            {
                if (b.Y >= 19)
                {
                    Timer.Stop();
                    IsActive = false;
                    Score.Save();
                    return true;
                }
            }              
            return false;
        }

        public void AddNext() 
        {
            foreach (Block b in Shape.Blocks)
                Blocks.Add(b);
            Shape = NextShape;
            SetNextShape();
            ClearLines();
        }

        public void ClearLines()
        {
            int linesCleared = 0;
            for (int i = 0; i < 20; i++)
            {
                if (Blocks.Where(b => b.Y == i).Count() == 10)
                {
                    linesCleared++;
                    Blocks.RemoveAll(b => b.Y == i);
                    foreach (Block b in Blocks.Where(b => b.Y > i))
                        b.Y -= 1;
                    i -= 1;
                }
            }
            Score.Update(linesCleared);
            Paint();
        }

        public void Floor()
        {
            Shape.Drop(GetSpacesBelow());
            Shape.HasLanded = true;
            Paint();
            Next = true;
        }
        
        public void Drop()
        {
            if (GetSpacesBelow() != 0)
                Shape.Drop(1);
            else
                Shape.HasLanded = true;
            Paint();
        }

        public int GetSpacesBelow()
        {
            if (GameOver() || Shape.IsFloored())
                return 0;
            int spaces = GetSpaceBetweenBlocks();
            if (spaces != 19)
                return spaces;
            else
                return Shape.GetBottomHeight();
        }

        public int GetSpaceBetweenBlocks()
        {
            int spaces = 20;
            foreach (Block b in Shape.GetBottomBlocks())
            {
                foreach (Block _b in Blocks.Where(_b => _b.X == b.X))
                {
                    if (b.Y - _b.Y < spaces && b.Y > _b.Y)
                        spaces = b.Y - _b.Y;
                }
            }
            return spaces - 1; // return amount of space between bottom blocks and resting blocks
        }

        public void Rotate()
        {
            if (RotateDown)
                return;
            RotateDown = true;
            Shape.Rotate(Blocks);
            if (GetSpacesBelow() == 0)
                Shape.HasLanded = true;
            Paint();
        }

        public void Shift()
        {
            if (Next || MovingDirection == 0)
                return;
            Shape.Shift(Blocks, MovingDirection);
            Paint();
        }
    }
}
