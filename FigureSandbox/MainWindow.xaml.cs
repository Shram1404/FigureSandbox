using FigureSandbox.Entities.Figures;
using FigureSandbox.Resources;
using FigureSandbox.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FigureSandbox;

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; } = null!;

    readonly DispatcherTimer timer;
    Dictionary<string, TreeViewItem> figureNodes = new();

    List<Figure> figures = new();
    List<Figure> stoppedFigures = new();

    private Figure? SelectedFigure => (FiguresTree.SelectedItem as TreeViewItem)?.Tag as Figure;

    private string defaultCultureName = "en-US";

    public MainWindow()
    {
        InitializeComponent();

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(defaultCultureName);
        Resource.Culture = new CultureInfo(defaultCultureName);

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(20);
        timer.Tick += Timer_Tick;
        timer.Start();

        FiguresTree.SelectedItemChanged += ToggleMoveButtonName;

        Instance = this;
        DataContext = this;
    }

    public void AddFigure(Figure figure)
    {
        figures.Add(figure);
        AddTreeNode(figure);
    }
    public void ClearFigures()
    {
        figures = new List<Figure>();
        stoppedFigures = new List<Figure>();
        FiguresCanvas.Children.Clear();
        FiguresTree.Items.Clear();
        figureNodes.Clear();
    }
    public void AddTreeNode(Figure figure)
    {
        string figureType = figure.GetType().Name;
        string localizedFigureType = Resource.ResourceManager.GetString(figureType);

        if (!figureNodes.ContainsKey(figureType))
        {
            TreeViewItem item = new TreeViewItem() { Header = localizedFigureType };
            FiguresTree.Items.Add(item);
            figureNodes[figureType] = item;
        }

        TreeViewItem node = new TreeViewItem() { Header = localizedFigureType, Tag = figure };
        figureNodes[figureType].Items.Add(node);
    }

    private void OpenFileMenu_Click(object sender, RoutedEventArgs e) => FigureMenuManager.OpenFiguresFromFile(Instance, FiguresCanvas, ref figures, ref stoppedFigures);
    private void SaveAsBinaryMenu_Click(object sender, RoutedEventArgs e) => FigureMenuManager.SaveAsBin(figures, stoppedFigures);
    private void SaveAsJsonMenu_Click(object sender, RoutedEventArgs e) => FigureMenuManager.SaveAsJson(figures, stoppedFigures);
    private void SaveAsXmlMenu_Click(object sender, RoutedEventArgs e) => FigureMenuManager.SaveAsXml(figures, stoppedFigures);

    private void AddRectangle_Click(object sender, RoutedEventArgs e) => AddFigure(new Rectangle(FiguresCanvas));
    private void AddCircle_Click(object sender, RoutedEventArgs e) => AddFigure(new Circle(FiguresCanvas));
    private void AddTriangle_Click(object sender, RoutedEventArgs e) => AddFigure(new Triangle(FiguresCanvas));
    private void ToggleMove_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedFigure != null)
        {
            ToggleMoveForFigure(SelectedFigure);
            ToggleMoveButtonName(sender, null);
        }
    }

    private void ToggleMoveForFigure(Figure figure)
    {
        if (figures.Contains(figure))
        {
            figures.Remove(figure);
            stoppedFigures.Add(figure);
        }
        else if (stoppedFigures.Contains(figure))
        {
            stoppedFigures.Remove(figure);
            figures.Add(figure);
        }
    }

    private void ToggleMoveButtonName(object sender, RoutedPropertyChangedEventArgs<object>? e)
    {
        if (SelectedFigure != null)
        {
            if (figures.Contains(SelectedFigure))
                ToggleMoveButton.Content = Resource.StopFigureButton;
            else if (stoppedFigures.Contains(SelectedFigure))
                ToggleMoveButton.Content = Resource.StartFigureButton;
        }
    }
    private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string cultureName)
            UpdateLanguage(cultureName);
    }
    private void UpdateLanguage(string cultureName)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
        Resource.Culture = new CultureInfo(cultureName);

        AddRectangleButton.Content = Resource.AddRectangleButton;
        AddCircleButton.Content = Resource.AddCircleButton;
        AddTriangleButton.Content = Resource.AddTriangleButton;
        ToggleMoveButton.Content = Resource.StopFigureButton;

        foreach (var figureNode in figureNodes)
        {
            figureNode.Value.Header = Resource.ResourceManager.GetString(figureNode.Key);

            foreach (TreeViewItem item in figureNode.Value.Items)
                item.Header = Resource.ResourceManager.GetString(item.Tag.GetType().Name);
        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        foreach (Figure figure in figures)
            figure.Move(FiguresCanvas);
    }
}