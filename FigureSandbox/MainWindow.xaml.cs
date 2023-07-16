using FigureSandbox.Entities.Figures;
using FigureSandbox.Resources;
using FigureSandbox.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FigureSandbox;

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; } = null!;

    private readonly DispatcherTimer _timer;
    private Dictionary<string, TreeViewItem> _figureNodes = new();
    private FigureEventManager? figureEventManager;

    private List<Figure> _allFigures = new();
    private List<Figure> _movedFigures = new();
    private List<Figure> _stoppedFigures = new();

    private Figure? _selectedFigure => (FiguresTree.SelectedItem as TreeViewItem)?.Tag as Figure;
    private HashSet<Tuple<Figure, Figure>> _intersectedFigures = new();

    private string _defaultCultureName = "en-US";
    private int _figuresWithEventsCount = 0;

    public MainWindow()
    {
        InitializeComponent();
        ConsoleTool.ShowConsole();

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(_defaultCultureName);
        Resource.Culture = new CultureInfo(_defaultCultureName);

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(20);
        _timer.Tick += Timer_Tick;
        _timer.Start();

        Instance = this;
        DataContext = this;
    }

    public void AddFigure(Figure figure)
    {
        _movedFigures.Add(figure);
        _allFigures.Add(figure);
        AddTreeNode(figure);
    }
    public void ClearFigures()
    {
        _movedFigures = new List<Figure>();
        _stoppedFigures = new List<Figure>();
        FiguresCanvas.Children.Clear();
        FiguresTree.Items.Clear();
        _figureNodes.Clear();
    }
    public void AddTreeNode(Figure figure)
    {
        string figureType = figure.GetType().Name;
        string localizedFigureType = Resource.ResourceManager.GetString(figureType);

        if (!_figureNodes.ContainsKey(figureType))
        {
            TreeViewItem item = new TreeViewItem() { Header = localizedFigureType };
            FiguresTree.Items.Add(item);
            _figureNodes[figureType] = item;
        }

        TreeViewItem node = new TreeViewItem() { Header = localizedFigureType, Tag = figure };
        _figureNodes[figureType].Items.Add(node);
    }

    private void OpenFileMenu_Click(object sender, RoutedEventArgs e) => FigureFileMenuManager.OpenFiguresFromFile(Instance, FiguresCanvas, ref _movedFigures, ref _stoppedFigures);
    private void SaveAsBinaryMenu_Click(object sender, RoutedEventArgs e) => FigureFileMenuManager.SaveAsBin(_movedFigures, _stoppedFigures);
    private void SaveAsJsonMenu_Click(object sender, RoutedEventArgs e) => FigureFileMenuManager.SaveAsJson(_movedFigures, _stoppedFigures);
    private void SaveAsXmlMenu_Click(object sender, RoutedEventArgs e) => FigureFileMenuManager.SaveAsXml(_movedFigures, _stoppedFigures);

    private void AddRectangle_Click(object sender, RoutedEventArgs e) => AddFigure(new Rectangle(FiguresCanvas));
    private void AddCircle_Click(object sender, RoutedEventArgs e) => AddFigure(new Circle(FiguresCanvas));
    private void AddTriangle_Click(object sender, RoutedEventArgs e) => AddFigure(new Triangle(FiguresCanvas));
    private void ToggleMove_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedFigure != null)
        {
            ToggleMoveForFigure(_selectedFigure);
            ToggleMoveButtonName(sender, null);
        }
    }

    private void AddHandlerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedFigure != null)
        {
            figureEventManager = new FigureEventManager(_selectedFigure);
            figureEventManager.AddIntersectionHandler();
        }
        _figuresWithEventsCount++;
    }
    private void RemoveHandlerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedFigure != null && figureEventManager != null)
        {
            figureEventManager.RemoveIntersectionHandler();
            figureEventManager = null;
        }
        _figuresWithEventsCount--;
    }

    private void ToggleMoveForFigure(Figure figure)
    {
        if (_movedFigures.Contains(figure))
        {
            _movedFigures.Remove(figure);
            _stoppedFigures.Add(figure);
        }
        else if (_stoppedFigures.Contains(figure))
        {
            _stoppedFigures.Remove(figure);
            _movedFigures.Add(figure);
        }
    }
    private void CheckIntersections()
    {
        var allFigures = _movedFigures.Concat(_stoppedFigures);
        var newIntersectedFigures = new HashSet<Tuple<Figure, Figure>>();

        foreach (Figure figure in allFigures)
        {
            foreach (Figure figure2 in allFigures)
            {
                if (figure != figure2 && figure.GetType() == figure2.GetType() && figure.IntersectsWith(figure2))
                {
                    var figuresTuple = new Tuple<Figure, Figure>(figure, figure2);
                    var figuresTupleReversed = new Tuple<Figure, Figure>(figure2, figure);

                    if (!_intersectedFigures.Contains(figuresTuple) && !_intersectedFigures.Contains(figuresTupleReversed))
                        figure.OnIntersection(figure2, figure.PosX, figure.PosY);

                    newIntersectedFigures.Add(figuresTuple);
                }
            }
        }

        _intersectedFigures = newIntersectedFigures;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        foreach (Figure figure in _movedFigures)
            figure.Move(FiguresCanvas);


        if (_allFigures.Count > 1 && _figuresWithEventsCount > 0)
            CheckIntersections();
    }

    private void ToggleMoveButtonName(object sender, RoutedPropertyChangedEventArgs<object>? e)
    {
        if (_selectedFigure != null)
        {
            if (_movedFigures.Contains(_selectedFigure))
                ToggleMoveButton.Content = Resource.StopFigureButton;
            else if (_stoppedFigures.Contains(_selectedFigure))
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

        foreach (var figureNode in _figureNodes)
        {
            figureNode.Value.Header = Resource.ResourceManager.GetString(figureNode.Key);

            foreach (TreeViewItem item in figureNode.Value.Items)
                item.Header = Resource.ResourceManager.GetString(item.Tag.GetType().Name);
        }
    }
}