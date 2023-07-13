using System;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FigureSandbox.Tools;

public static class FigureRandomProperty
{
    private static readonly int maxSpeed = 5;

    public static void SetRandomSpeed(int[] speed)
    {
        do
        {
            speed[0] = new Random().Next(-maxSpeed, maxSpeed);
            speed[1] = new Random().Next(-maxSpeed, maxSpeed);
        }
        while (speed[0] == 0 || speed[1] == 0);
    }

    public static void SetRandomFigurePosition(Shape shape, Canvas canvas)
    {
        Canvas.SetLeft(shape, new Random().Next(0, (int)canvas.ActualWidth - (int)shape.ActualWidth));
        Canvas.SetTop(shape, new Random().Next(0, (int)canvas.ActualHeight - (int)shape.ActualHeight));
    }
}
