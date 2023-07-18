using System;
using FigureSandbox.Entities.Figures;

namespace FigureSandbox.Exceptions;

public class FigureOutOfBoundsException : Exception
{
    public Figure Figure { get; }
    public FigureOutOfBoundsException(Figure figure)
        : base($"Figure of type {figure.Type} is out of bounds.") => Figure = figure;
}
