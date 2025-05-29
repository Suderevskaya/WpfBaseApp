using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfBaseApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Points> PointsCollection;
        float cfx, cfy, ccc;
        public MainWindow()
        {
            InitializeComponent();
            PointsCollection = new ObservableCollection<Points>()
            {
                new Points(0f, 0f),
                new Points(1f, 1f),
                new Points(2f, 4f)
            };
            PointsTable.ItemsSource = PointsCollection;
            cfx = 10.0f;
            cfy = 10.0f;
            paint();
        }
        public void paint()
        {
            List<PathFigure> lines = new List<PathFigure>();
            var pathFigure = new PathFigure();
            pathFigure.StartPoint = new System.Windows.Point(cfx * PointsCollection[0].X + 20, cfy * PointsCollection[0].Y + 25);
            for (int i = 1; i < PointsCollection.Count; i++)
            {
                var point = new System.Windows.Point(cfx * PointsCollection[i].X + 20, cfy * PointsCollection[i].Y + 25);
                pathFigure.Segments.Add(new LineSegment(point, true));
            }
            lines.Add(pathFigure);
            Draw.Figures = new PathFigureCollection(lines);
            //AsixX
            DrawingGroup drawingGroupX = new DrawingGroup();
            drawingGroupX.Children.Add(new GeometryDrawing(System.Windows.Media.Brushes.White,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.White, 1),
                        new RectangleGeometry(new Rect(0, 0, 530, 16), 0, 0)));
            for (int i = 1; i < 25; i++)
            {
                FormattedText formattedText = new FormattedText(
                    Math.Round(i * 20f / Math.Round(cfx)).ToString(),
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface(new System.Windows.Media.FontFamily("Arial"), FontStyle, FontWeight, FontStretch),
                    FontSize,
                    System.Windows.Media.Brushes.Black);
                if (drawingGroupX.Children.Count() == 1)
                {
                    drawingGroupX.Children.Add(new GeometryDrawing(System.Windows.Media.Brushes.Black,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1),
                        formattedText.BuildGeometry(new System.Windows.Point(Math.Round(i * 20f / Math.Round(cfx)) * cfx, 0))));
                }
                else
                if ((drawingGroupX.Children.Last() as GeometryDrawing).Bounds.Right + 2 <= Math.Round(i * 20f / Math.Round(cfx)) * cfx)
                {
                    drawingGroupX.Children.Add(new GeometryDrawing(System.Windows.Media.Brushes.Black,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1),
                        formattedText.BuildGeometry(new System.Windows.Point(Math.Round(i * 20f / Math.Round(cfx)) * cfx, 0))));
                }
            }
            AxisX.Stretch = Stretch.None;
            DrawingImage axisX = new DrawingImage(drawingGroupX);
            AxisX.Source = axisX;
            //AsixY
            DrawingGroup drawingGroupY = new DrawingGroup();
            drawingGroupY.Children.Add(new GeometryDrawing(System.Windows.Media.Brushes.White,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.White, 1),
                        new RectangleGeometry(new Rect(0, 0, 17, 480), 0, 0)));
            for (int i = 1; i < 27; i++)
            {
                FormattedText formattedText = new FormattedText(
                    Math.Round(i * 20f / Math.Round(cfy)).ToString(),
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface(new System.Windows.Media.FontFamily("Arial"), FontStyle, FontWeight, FontStretch),
                    FontSize,
                    System.Windows.Media.Brushes.Black);
                if (drawingGroupY.Children.Count() == 1)
                {
                    drawingGroupY.Children.Add(new GeometryDrawing(System.Windows.Media.Brushes.Black,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1),
                        formattedText.BuildGeometry(new System.Windows.Point(0, Math.Round(i * 20f / Math.Round(cfy)) * cfy))));
                }
                else
                if ((drawingGroupY.Children.Last() as GeometryDrawing).Bounds.Bottom + 1 <= Math.Round(i * 20f / Math.Round(cfy)) * cfy)
                {
                    drawingGroupY.Children.Add(new GeometryDrawing(System.Windows.Media.Brushes.Black,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1),
                        formattedText.BuildGeometry(new System.Windows.Point(0, Math.Round(i * 20f / Math.Round(cfy)) * cfy))));
                }
            }
            AxisY.Stretch = Stretch.None;
            DrawingImage axisY = new DrawingImage(drawingGroupY);
            AxisY.Source = axisY;
        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                cfx += 1f;
                cfy += 1f;
            }
            else if (cfx > 1 && cfy > 1)
            {
                cfx -= 1f;
                cfy -= 1f;
            }
            paint();
        }
        private void saveClick(object sender, RoutedEventArgs e)
        {
            List<string> output = new List<string>();
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.InitialDirectory = "d:\\";
            foreach (Points pt in PointsCollection)
                output.Add(pt.X.ToString() + " " + pt.Y.ToString());
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                System.IO.File.WriteAllLines(filename, output);
                MessageBox.Show("Файл сохранен");
            }
        }
        private void loadClick(object sender, RoutedEventArgs e)
        {
            List<string> input = new List<string>();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                input = System.IO.File.ReadAllLines(filename).ToList();
                PointsCollection.Clear();
                foreach (var str in input)
                {
                    string[] pt = str.Split(' ');
                    PointsCollection.Add(new Points() { X = float.Parse(pt[0]), Y = float.Parse(pt[1]) });
                }
                paint();
            }
        }

        private void PointsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            paint();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            float xmax = 0f, ymax = 0f;
            for (int i = 0; i < PointsCollection.Count; i++)
            {if (PointsCollection[i].X > xmax)
                        xmax = PointsCollection[i].X;
                    if (PointsCollection[i].Y > ymax)
                        ymax = PointsCollection[i].Y;
            }
            float cf;
            if ((Canvas.Width - 50) / xmax > (Canvas.Height - 50) / ymax)
                cf = (float)((Canvas.Height - 50) / ymax);
            else
                cf = (float)((Canvas.Width - 50) / xmax);
            cfx = cfy = cf;
            paint();
        }

        public class Points: INotifyPropertyChanged
        {
            public float x, y;
            public Points()
            {
                x = 0f; y = 0f;
            }
            public Points(float p_x, float p_y)
            {
                x = p_x; y = p_y;
            }
            public float X
            {
                get { return x; }
                set { x = value; OnPropertyChanged(); }
            }
            public float Y
            {
                get { return y; }
                set { y = value; OnPropertyChanged(); }
            }
            private void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
