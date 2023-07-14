using FigureSandbox.Tools;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FigureSandbox.Entities.Figures;

[Serializable]
public class Rectangle : Figure
{
    static PointCollection _points = new PointCollection();

    static Rectangle()
    {
        _points.Add(new System.Windows.Point(0, 0));
        _points.Add(new System.Windows.Point(0, 100));
        _points.Add(new System.Windows.Point(100, 100));
        _points.Add(new System.Windows.Point(100, 0));
    }

    public Rectangle() { }
    public Rectangle(Canvas canvas)
    {
        Draw(canvas);
        Type = "Rectangle";
    }

    public Rectangle(Canvas canvas, int x, int y, int[] speed)
    {
        Type = "Rectangle";
        PosX = x;
        PosY = y;
        Speed = speed;
        Draw(canvas);
        Canvas.SetLeft(shape, x);
        Canvas.SetTop(shape, y);
    }

    public override void Draw(Canvas canvas)
    {
        Polygon polygon = new Polygon()
        {
            Points = _points,
            Fill = Brushes.Transparent,
            Stroke = Brushes.DarkBlue,
            StrokeThickness = 3,
        };

        canvas.Children.Add(polygon);
        shape = polygon;

        FigureRandomProperty.SetRandomFigurePosition(shape, canvas);
    }
}
