using FigureSandbox.Entities.Figures;
using System;

namespace FigureSandbox.Events;

public class IntersectionEventArgs : EventArgs
{
    public Figure Figure1 { get; }
    public Figure Figure2 { get; }
    public double X { get; }
    public double Y { get; }

    public IntersectionEventArgs(Figure figure1, Figure figure2, double x, double y)
    {
        Figure1 = figure1;
        Figure2 = figure2;
        X = x;
        Y = y;
    }
}

