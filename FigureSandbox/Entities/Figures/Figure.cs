using FigureSandbox.Events;
using FigureSandbox.Exceptions;
using FigureSandbox.Tools;
using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace FigureSandbox.Entities.Figures;

[DataContract]
[KnownType(typeof(Circle))]
[KnownType(typeof(Rectangle))]
[KnownType(typeof(Triangle))]
[Serializable]
[XmlInclude(typeof(Circle))]
[XmlInclude(typeof(Rectangle))]
[XmlInclude(typeof(Triangle))]
public abstract class Figure
{
    [DataMember]
    public int[] Speed { get; set; } = new int[2];
    [DataMember]
    public double PosX { get; set; }
    [DataMember]
    public double PosY { get; set; }
    [DataMember]
    public string Type { get; set; }

    [NonSerialized]
    protected Shape shape;

    protected Figure()
    {
        if (Speed[0] == 0)
            FigureRandomProperty.SetRandomSpeed(Speed);
    }

    [field: NonSerialized]
    public event EventHandler<IntersectionEventArgs> Intersection;

    public void OnIntersection(Figure other, double x, double y)
    {
        IntersectionEventArgs args = new IntersectionEventArgs(this, other, x, y);
        Intersection?.Invoke(this, args);
    }
    public bool IntersectsWith(Figure other)
    {
        if (this == other)
            return false;

        Rect rect1 = new Rect(PosX, PosY, shape.ActualWidth, shape.ActualHeight);
        Rect rect2 = new Rect(other.PosX, other.PosY, other.shape.ActualWidth, other.shape.ActualHeight);
        return rect1.IntersectsWith(rect2);
    }

    public abstract void Draw(Canvas canvas);

    public void Move(Canvas canvas)
    {
        PosX = Canvas.GetLeft(shape);
        PosY = Canvas.GetTop(shape);

        if (PosX <= 0 || PosX >= canvas.ActualWidth - (shape.ActualWidth))
            Speed[0] = -Speed[0];
        if (PosY <= 0 || PosY >= canvas.ActualHeight - (shape.ActualHeight))
            Speed[1] = -Speed[1];

        PosX += Speed[0];
        PosY += Speed[1];

        if (PosX < 0 || PosX > canvas.ActualWidth - shape.ActualWidth || PosY < 0 || PosY > canvas.ActualHeight - shape.ActualHeight)
            throw new FigureOutOfBoundsException(this);
    }

    public void UpdatePositions(Canvas canvas)
    {
        canvas.Dispatcher.BeginInvoke(new Action(() =>
        {
            Canvas.SetLeft(shape, PosX);
            Canvas.SetTop(shape, PosY);
        }));
    }

    public void FreeFigureOutsideBounds(Canvas canvas)
    {
        PosX = Math.Clamp(PosX, 0, canvas.ActualWidth - shape.ActualWidth);
        PosY = Math.Clamp(PosY, 0, canvas.ActualHeight - shape.ActualHeight);
    }
}