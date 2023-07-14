using FigureSandbox.Tools;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FigureSandbox.Entities.Figures;

[Serializable]
public class Circle : Figure
{
    public Circle() { }
    public Circle(Canvas canvas)
    {
        Draw(canvas);
        Type = "Circle";
    }
    public Circle(Canvas canvas, int x, int y, int[] speed)
    {
        Type = "Circle";
        PosX = x;
        PosY = y;
        Speed = speed;
        Draw(canvas);
        Canvas.SetLeft(shape, x);
        Canvas.SetTop(shape, y);
    }

    public override void Draw(Canvas canvas)
    {
        Ellipse ellipse = new Ellipse()
        {
            Width = 100,
            Height = 100,
            Fill = Brushes.Transparent,
            Stroke = Brushes.DarkRed,
            StrokeThickness = 2
        };

        canvas.Children.Add(ellipse);
        shape = ellipse;

        FigureRandomProperty.SetRandomFigurePosition(shape, canvas);
    }
}