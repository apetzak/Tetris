using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public class Shape
    {
        public Color Color;
        public List<Block> Blocks = new List<Block>();
        public Types Type;
        public int Rotations = 0;
        public bool Rotated = false;
        public bool HasLanded = false;

        public enum Types
        {
            I = 0,
            O = 1,
            T = 2,
            J = 3,
            L = 4,
            Z = 5,
            S = 6
        }

        public Shape(int value)
        {
            Type = (Types)value;
            SetAttributes(Types.O, Colors.Red, new int[] { 0, 0, 0, 1, 1, 0, 1, 1 });
            SetAttributes(Types.I, Colors.Orange, new int[] { 0, 0, 0, 1, 0, 2, 0, 3 });
            SetAttributes(Types.T, Colors.Yellow, new int[] { 0, 0, 0, 1, 0, 2, 1, 1 });
            SetAttributes(Types.J, Colors.Green, new int[] { 0, 0, 0, 1, 0, 2, 1, 2 });
            SetAttributes(Types.L, Colors.Blue, new int[] { 1, 0, 1, 1, 1, 2, 0, 2 });
            SetAttributes(Types.Z, Colors.Indigo, new int[] { 0, 2, 0, 1, 1, 1, 1, 0 });
            SetAttributes(Types.S, Colors.Violet, new int[] { 0, 0, 0, 1, 1, 1, 1, 2 });
        }

        public void SetAttributes(Types t, Color color, int[] array)
        {
            if (t != Type)
                return;
            Color = color;
            for (int i = 0; i < array.Length; i += 2)
                Blocks.Add(new Block(array[i], array[i + 1], Color));
        }

        public void MoveToStartingLocation()
        {
            foreach (Block b in Blocks)
            {
                b.X += 4;
                b.Y += 20;
            }
        }

        public void Paint(Canvas canvas, int padding = 0)
        {
            foreach (Block b in Blocks)
                b.Paint(canvas, padding);
        }

        public void PaintNext(Canvas canvas)
        {
            canvas.Children.Clear();
            Paint(canvas, 30);
        }

        public void Drop(int spaces)
        {
            foreach (Block b in Blocks)
                b.Y -= spaces;
        }

        public void Rotate(List<Block> restingBlocks)
        {
            if (HasLanded || Type == Types.O)
                return;

            List<Block> prevBlocks = new List<Block>();
            foreach (Block b in Blocks)
                prevBlocks.Add(new Block(b.X, b.Y, b.Color));
            int prevRotation = Rotations;

            if (Type == Types.I)
            {
                RotateBlocks(Rotations == 0, 1, new int[] { 2, 2, 1, 1, 0, 0, -1, -1 }); 
                RotateBlocks(Rotations == 1, 0, new int[] { -2, -2, -1, -1, 0, 0, 1, 1 }); 
            }
            else if (Type == Types.T)
            {
                RotateBlocks(Rotations == 0, 1, new int[] { 1, 1, 0, 0, -1, -1, -1, 1 });
                RotateBlocks(Rotations == 1, 2, new int[] { -1, 1, 0, 0, 1, -1, -1, -1 });
                RotateBlocks(Rotations == 2, 3, new int[] { 1, -1, 0, 0, -1, 1, 1, -1 });
                RotateBlocks(Rotations == 3, 0, new int[] { -1, -1, 0, 0, 1, 1, 1, 1 });
            }
            else if (Type == Types.J)
            {
                RotateBlocks(Rotations == 0, 1, new int[] { 2, 1, 1, 0, 0, -1, -1, 0 });
                RotateBlocks(Rotations == 1, 2, new int[] { -1, 1, 0, 0, 1, -1, 0, -2 });
                RotateBlocks(Rotations == 2, 3, new int[] { -1, 0, 0, 1, 1, 2, 2, 1 });
                RotateBlocks(Rotations == 3, 0, new int[] { 0, -2, -1, -1, -2, 0, -1, 1 });
            }
            else if (Type == Types.L)
            {
                RotateBlocks(Rotations == 0, 1, new int[] { 1, 2, 0, 1, -1, 0, 0, -1 });
                RotateBlocks(Rotations == 1, 2, new int[] { -2, 0, -1, -1, 0, -2, 1, -1 });
                RotateBlocks(Rotations == 2, 3, new int[] { 0, -1, 1, 0, 2, 1, 1, 2 });
                RotateBlocks(Rotations == 3, 0, new int[] { 1, -1, 0, 0, -1, 1, -2, 0 });
            }
            else if (Type == Types.Z)
            {
                RotateBlocks(Rotations == 0, 1, new int[] { 0, -1, 1, 0, 0, 1, 1, 2 });
                RotateBlocks(Rotations == 1, 0, new int[] { 0, 1, -1, 0, 0, -1, -1, -2 });
            }
            else if (Type == Types.S)
            {
                RotateBlocks(Rotations == 0, 1, new int[] { 2, 1, 1, 0, 0, 1, -1, 0 });
                RotateBlocks(Rotations == 1, 0, new int[] { -2, -1, -1, 0, 0, -1, 1, 0});
            }

            Rotated = false;

            if (!CanRotate(restingBlocks)) // revert rotation if not valid
            {
                Blocks = prevBlocks;
                Rotations = prevRotation;
            }              
        }

        public void RotateBlocks(bool set, int rot, int[] arr)
        {
            if (!set || Rotated)
                return;
            for (int i = 0; i < arr.Length; i += 2)
            {
                Blocks[i / 2].X += arr[i];
                Blocks[i / 2].Y += arr[i + 1];
            }
            Rotations = rot;
            Rotated = true; // to make sure RotateBlocks() isn't called multiple times in one action
        }

        public bool CanRotate(List<Block> restingBlocks)
        {
            foreach (Block b in Blocks)
            {
                if (b.X < 0 || b.X > 9 || b.Y < 0)
                    return false;

                foreach (Block _b in restingBlocks)
                    if (b.X == _b.X && b.Y == _b.Y)
                        return false;
            }
            return true;
        }

        public void Shift(List<Block> restingBlocks, int i)
        {
            if (!CanShift(restingBlocks, i))
                return;

            foreach (Block b in Blocks)
                b.X += i;
        }

        public bool CanShift(List<Block> restingBlocks, int i)
        {
            foreach (Block b in Blocks)
            {
                if (i == -1 && b.X == 0 || i == 1 && b.X == 9)
                    return false;

                foreach (Block _b in restingBlocks)
                    if (b.Y == _b.Y && b.X == _b.X - i)
                        return false;
            }
            return true; // return true if not adjacent to the horizontal bounds or a resting block
        }

        public int GetBottomHeight()
        {
            int i = 30;
            foreach (Block b in Blocks)
                if (b.Y < i)
                    i = b.Y;
            return i; // return y coordinate of lowest block
        }

        public List<Block> GetBottomBlocks()
        {
            List<Block> list = new List<Block>();
            for (int i = 0; i < 10; i++)
            {
                int y = 30;
                foreach (Block b in Blocks.Where(b => b.X == i))
                {
                    if (b.Y < y)
                        y = b.Y;
                }
                if (y < 30)
                    list.Add(new Block(i, y, Color));
            }
            return list; // return blocks with lowest y for each x
        }

        public bool IsFloored()
        {
            foreach (Block b in Blocks)
            {
                if (b.Y == 0)
                    return true;
            }
            return false;
        }
    }
}
