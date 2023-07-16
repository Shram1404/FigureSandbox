using FigureSandbox.Entities.Figures;
using FigureSandbox.Events;
using System;
using System.Threading.Tasks;

namespace FigureSandbox.Services;

public class FigureEventManager
{
    private readonly Figure selectedFigure;

    public FigureEventManager(Figure selectedFigure)
    {
        this.selectedFigure = selectedFigure;
    }

    public void AddIntersectionHandler() => selectedFigure.Intersection += OnIntersection;
    public void RemoveIntersectionHandler() => selectedFigure.Intersection -= OnIntersection;

    private async void OnIntersection(object sender, IntersectionEventArgs e)
    {
        await Task.Run(() => Console.Beep(2000, 2));
        Console.WriteLine($"Intersection at ({e.X}, {e.Y})");
    }
}
