using FigureSandbox.Tools;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FigureSandbox.Entities;

public class Circle : Figure
{
    public Circle(Canvas canvas) => Draw(canvas);

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