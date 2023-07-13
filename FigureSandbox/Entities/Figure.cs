using FigureSandbox.Tools;
using System;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FigureSandbox.Entities;

public abstract class Figure
{
    private readonly int maxSpeed = 5;
    protected int[] _speed = new int[2];
    protected Shape shape;

    protected Figure()
    {
        FigureRandomProperty.SetRandomSpeed(_speed);
    }

    public abstract void Draw(Canvas canvas);

    public void Move(Canvas canvas)
    {
        double x = Canvas.GetLeft(shape);
        double y = Canvas.GetTop(shape);

        if (x <= 0 || x >= canvas.ActualWidth - shape.ActualWidth)
            _speed[0] = -_speed[0];
        if (y <= 0 || y >= canvas.ActualHeight - shape.ActualHeight)
            _speed[1] = -_speed[1];

        x += _speed[0];
        y += _speed[1];

        FreeFigureOutsideBounds(canvas, ref x, ref y);

        Canvas.SetLeft(shape, x);
        Canvas.SetTop(shape, y);
    }

    private void FreeFigureOutsideBounds(Canvas canvas, ref double x, ref double y)
    {
        x = Math.Clamp(x, 0, canvas.ActualWidth - shape.ActualWidth);
        y = Math.Clamp(y, 0, canvas.ActualHeight - shape.ActualHeight);
    }
}
