using FigureSandbox.Entities.Figures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FigureSandbox.Entities.Figures.Data;

[Serializable]
[DataContract]
[KnownType(typeof(FigureData))]
public class FigureData
{
    [DataMember]
    public List<Figure> Figures { get; set; }

    [DataMember]
    public List<Figure> StopedFigures { get; set; }

    public FigureData() { }

    public FigureData(List<Figure> figures, List<Figure> stopedFigures)
    {
        Figures = figures;
        StopedFigures = stopedFigures;
    }
    
}
