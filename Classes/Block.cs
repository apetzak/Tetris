using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    public class Block
    {
        public int X;
        public int Y;
        public Color Color;

        public Block(int x, int y, Color c)
        {
            X = x;
            Y = y;
            Color = c;
        }

        public void Paint(Canvas canvas, int padding = 0)
        {
            if (Y > 19 || Y < 0)
                return;
            Rectangle r = new Rectangle()
            {
                Height = 30,
                Width = 30,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(Color),
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Canvas.SetLeft(r, X * 30 + padding);
            Canvas.SetBottom(r, Y * 30 + padding);
            Canvas.SetZIndex(r, 1);
            canvas.Children.Add(r);
        }

        public void PaintOutline(Canvas canvas)
        {
            if (Y > 19 || Y < 0)
                return;
            Rectangle r = new Rectangle()
            {
                Height = 28,
                Width = 28,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(Color),
                Stroke = new SolidColorBrush(Colors.White)
            };
            Canvas.SetLeft(r, X * 30 + 1);
            Canvas.SetBottom(r, Y * 30 + 1);
            Canvas.SetZIndex(r, 0);
            canvas.Children.Add(r);
        }
    }
}
