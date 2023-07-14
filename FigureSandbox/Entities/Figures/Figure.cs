using FigureSandbox.Tools;
using System;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace FigureSandbox.Entities.Figures;

[KnownType(typeof(Circle))]
[KnownType(typeof(Rectangle))]
[KnownType(typeof(Triangle))]
[XmlInclude(typeof(Triangle))]
[XmlInclude(typeof(Circle))]
[XmlInclude(typeof(Rectangle))]
[DataContract]
[Serializable]
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

    public abstract void Draw(Canvas canvas);

    public void Move(Canvas canvas)
    {
        double x = Canvas.GetLeft(shape);
        double y = Canvas.GetTop(shape);

        if (x <= 0 || x >= canvas.ActualWidth - shape.ActualWidth)
            Speed[0] = -Speed[0];
        if (y <= 0 || y >= canvas.ActualHeight - shape.ActualHeight)
            Speed[1] = -Speed[1];

        x += Speed[0];
        y += Speed[1];

        FreeFigureOutsideBounds(canvas, ref x, ref y);

        PosX = x;
        PosY = y;

        Canvas.SetLeft(shape, x);
        Canvas.SetTop(shape, y);
    }

    private void FreeFigureOutsideBounds(Canvas canvas, ref double x, ref double y)
    {
        x = Math.Clamp(x, 0, canvas.ActualWidth - shape.ActualWidth);
        y = Math.Clamp(y, 0, canvas.ActualHeight - shape.ActualHeight);
    }
}
