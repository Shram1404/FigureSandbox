using FigureSandbox.Entities.Figures;
using FigureSandbox.Entities.Figures.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace FigureSandbox.Services;

public static class FigureMenuManager
{
    private static MainWindow? mainWindow;

    public static void OpenFiguresFromFile(
        MainWindow instance,
        Canvas canvas,
        ref List<Figure> figures,
        ref List<Figure> stoppedFigures,
        ref Dictionary<string, TreeViewItem> figureNodes)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            string fileName = openFileDialog.FileName;
            string extension = Path.GetExtension(fileName);
            FigureData data = LoadFigureDataFromFile(fileName, extension);

            mainWindow = instance;
            mainWindow.ClearFigures();
            AddFiguresToList(canvas, ref figures, ref stoppedFigures, data);
        }
    }

    public static void SaveAsBin(List<Figure> figures, List<Figure> stoppedFigures)
    {
        SaveData(figures, stoppedFigures, "Binary files (*.bin)|*.bin", ".bin", (stream, data) =>
        {
            BinaryFormatter formatter = new();
#pragma warning disable SYSLIB0011 // Тип или член устарел
            formatter.Serialize(stream, data);
#pragma warning restore SYSLIB0011 // Тип или член устарел
        });
    }

    public static void SaveAsJson(List<Figure> figures, List<Figure> stoppedFigures)
    {
        SaveData(figures, stoppedFigures, "JSON files (*.json)|*.json", ".json", (stream, data) =>
        {
            DataContractJsonSerializer jsonFormatter = new(typeof(List<Figure>));
            jsonFormatter.WriteObject(stream, data);
        });
    }

    public static void SaveAsXml(List<Figure> figures, List<Figure> stoppedFigures)
    {
        SaveData(figures, stoppedFigures, "XML files (*.xml)|*.xml", ".xml", (stream, data) =>
        {
            XmlSerializer serializer = new(typeof(FigureData));
            serializer.Serialize(stream, data);
        });
    }

    private static FigureData LoadFigureDataFromFile(string fileName, string extension)
    {
        using (var stream = File.OpenRead(fileName))
        {
            switch (extension)
            {
                case ".json":
                    return LoadFromJson(stream);
                case ".xml":
                    return LoadFromXml(stream);
                case ".bin":
                    return LoadFromBin(stream);
                default:
                    throw new ArgumentException("Unsupported file type");
            }
        }
    }

    private static FigureData LoadFromJson(Stream stream)
    {
        DataContractJsonSerializer jsonSerializer = new(typeof(FigureData));
        return (FigureData)jsonSerializer.ReadObject(stream) ?? throw new ArgumentException("Failed to deserialize file");
    }

    private static FigureData LoadFromXml(Stream stream)
    {
        XmlSerializer xmlSerializer = new(typeof(FigureData));
        return (FigureData)xmlSerializer.Deserialize(stream) ?? throw new ArgumentException("Failed to deserialize file");
    }

    private static FigureData LoadFromBin(Stream stream)
    {
        BinaryFormatter binaryFormatter = new();
#pragma warning disable SYSLIB0011 // Тип или член устарел
        return (FigureData)binaryFormatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Тип или член устарел
    }

    private static void AddFiguresToList(Canvas canvas, ref List<Figure> figures, ref List<Figure> stoppedFigures, FigureData data)
    {
        foreach (var figureData in data.Figures)
        {
            var figure = CreateFigure(canvas, figureData);
            figures.Add(figure);
            mainWindow.AddTreeNode(figure);
        }

        foreach (var figureData in data.StopedFigures)
        {
            var figure = CreateFigure(canvas, figureData);
            stoppedFigures.Add(figure);
            mainWindow.AddTreeNode(figure);
        }
    }

    private static Figure CreateFigure(Canvas canvas, Figure figureData)
    {
        Figure figure;
        switch (figureData.Type)
        {
            case "Circle":
                figure = new Circle(canvas, (int)figureData.PosX, (int)figureData.PosY, figureData.Speed);
                break;
            case "Rectangle":
                figure = new Rectangle(canvas, (int)figureData.PosX, (int)figureData.PosY, figureData.Speed);
                break;
            case "Triangle":
                figure = new Triangle(canvas, (int)figureData.PosX, (int)figureData.PosY, figureData.Speed);
                break;
            default:
                throw new InvalidOperationException("Unknown figure type");
        }
        return figure;
    }

    private static void SaveData(List<Figure> figures, List<Figure> stoppedFigures, string filter, string defaultExt, Action<Stream, FigureData> saveAction)
    {
        SaveFileDialog saveFileDialog = new();
        saveFileDialog.Filter = filter;
        saveFileDialog.DefaultExt = defaultExt;

        if (saveFileDialog.ShowDialog() == true)
        {
            string fileName = saveFileDialog.FileName;
            FigureData data = new(figures, stoppedFigures);

            using (FileStream fs = new(fileName, FileMode.OpenOrCreate))
                saveAction(fs, data);

        }
    }
}