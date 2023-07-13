﻿using FigureSandbox.Tools;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FigureSandbox.Entities;

public class Triangle : Figure
{
    static PointCollection _points = new PointCollection();

    static Triangle()
    {
        _points.Add(new System.Windows.Point(50, 0));
        _points.Add(new System.Windows.Point(0, 100));
        _points.Add(new System.Windows.Point(100, 100));
    }

    public Triangle(Canvas canvas) => Draw(canvas);

    public override void Draw(Canvas canvas)
    {
        Polygon polygon = new Polygon()
        {
            Points = _points,
            Fill = Brushes.Transparent,
            Stroke = Brushes.Black,
            StrokeThickness = 2,
        };

        canvas.Children.Add(polygon);
        shape = polygon;

        FigureRandomProperty.SetRandomFigurePosition(shape, canvas);
    }
}
