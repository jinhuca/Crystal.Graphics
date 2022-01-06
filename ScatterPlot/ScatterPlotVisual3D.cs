using System.Globalization;
using Crystal.Graphics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ScatterPlot
{
  public class ScatterPlotVisual3D : ModelVisual3D
  {
    #region Dependency Properties

    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
      nameof(Points), typeof(Point3D[]), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(null, ModelChanged));

    public Point3D[] Points
    {
      get => (Point3D[])GetValue(PointsProperty);
      set => SetValue(PointsProperty, value);
    }

    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
      nameof(Values), typeof(double[]), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(null, ModelChanged));

    public double[] Values
    {
      get => (double[])GetValue(ValuesProperty);
      set => SetValue(ValuesProperty, value);
    }

    public static readonly DependencyProperty SurfaceBrushProperty = DependencyProperty.Register(
      nameof(SurfaceBrush), typeof(Brush), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(null, ModelChanged));

    public Brush SurfaceBrush
    {
      get => (Brush)GetValue(SurfaceBrushProperty);
      set => SetValue(SurfaceBrushProperty, value);
    }

    public double IntervalX
    {
      get => (double)GetValue(IntervalXNewProperty);
      set => SetValue(IntervalXNewProperty, value);
    }

    public static readonly DependencyProperty IntervalXNewProperty = DependencyProperty.Register(
      nameof(IntervalX), typeof(double), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double IntervalY
    {
      get => (double)GetValue(IntervalYProperty);
      set => SetValue(IntervalYProperty, value);
    }

    public static readonly DependencyProperty IntervalYProperty = DependencyProperty.Register(
      nameof(IntervalY), typeof(double), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double IntervalZ
    {
      get => (double)GetValue(IntervalZProperty);
      set => SetValue(IntervalZProperty, value);
    }

    public static readonly DependencyProperty IntervalZProperty = DependencyProperty.Register(
      nameof(IntervalZ), typeof(double), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double FontSize
    {
      get => (double)GetValue(FontSizeProperty);
      set => SetValue(FontSizeProperty, value);
    }

    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
      nameof(FontSize), typeof(double), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(.1d, ModelChanged));

    public double SphereSize
    {
      get => (double)GetValue(SphereSizeProperty);
      set => SetValue(SphereSizeProperty, value);
    }

    public static readonly DependencyProperty SphereSizeProperty = DependencyProperty.Register(
      nameof(SphereSize), typeof(double), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(.05d, ModelChanged));

    public double LineThickness
    {
      get => (double)GetValue(LineThicknessProperty);
      set => SetValue(LineThicknessProperty, value);
    }

    public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(
      nameof(LineThickness), typeof(double), typeof(ScatterPlotVisual3D), new UIPropertyMetadata(0.03d, ModelChanged));

    #endregion Dependency Properties

    private readonly ModelVisual3D visualChild;

    public ScatterPlotVisual3D()
    {
      visualChild = new ModelVisual3D();
      Children.Add(visualChild);
    }

    private static void ModelChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
    {
      ((ScatterPlotVisual3D)dpObj).UpdateModel();
    }

    private void UpdateModel()
    {
      visualChild.Content = CreateModel();
    }

    private Model3D CreateModel()
    {
      var plotModel = new Model3DGroup();
      if(Points == null || Values == null) return plotModel;

      var minX = Points.Min(p => p.X);
      var maxX = Points.Max(p => p.X);
      var minY = Points.Min(p => p.Y);
      var maxY = Points.Max(p => p.Y);
      var minZ = Points.Min(p => p.Z);
      var maxZ = Points.Max(p => p.Z);
      var minValue = Values.Min();
      var maxValue = Values.Max();

      var valueRange = maxValue - minValue;
      var scatterMeshBuilder = new MeshBuilder(true);
      var oldTCCount = 0;

      for(var i = 0; i < Points.Length; ++i)
      {
        scatterMeshBuilder.AddSphere(Points[i], SphereSize, 4, 4);
        var u = (Values[i] - minValue) / valueRange;
        var newTCCount = scatterMeshBuilder.TextureCoordinates.Count;

        for(var j = oldTCCount; j < newTCCount; ++j)
        {
          scatterMeshBuilder.TextureCoordinates[j] = new Point(u, u);
        }
        oldTCCount = newTCCount;
      }

      var scatterModel = new GeometryModel3D(scatterMeshBuilder.ToMesh(), MaterialHelper.CreateMaterial(SurfaceBrush, null, null, 1, 0));
      scatterModel.BackMaterial = scatterModel.Material;

      // create bounding box with axes indications
      var axesMeshBuilder = new MeshBuilder();
      for(var x = minX; x <= maxX; x += IntervalX)
      {
        var label = TextCreator.CreateTextLabelModel3D(
          x.ToString(CultureInfo.InvariantCulture), Brushes.Black, true, FontSize, new Point3D(x, minY - FontSize * 2.5, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }

      {
        var label = TextCreator.CreateTextLabelModel3D(
          "X-axis", Brushes.Black, true, FontSize, new Point3D((minX + maxX) * 0.5, minY - FontSize * 6, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }

      for(var y = minY; y <= maxY; y += IntervalY)
      {
        var label = TextCreator.CreateTextLabelModel3D(
          y.ToString(CultureInfo.InvariantCulture), Brushes.Black, true, FontSize, new Point3D(minX - FontSize * 3, y, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }
      {
        var label = TextCreator.CreateTextLabelModel3D(
          "Y-axis", Brushes.Black, true, FontSize, new Point3D(minX - FontSize * 10, (minY + maxY) * 0.5, minZ), new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0));
        plotModel.Children.Add(label);
      }
      
      var z0 = (int)(minZ / IntervalZ) * IntervalZ;
      for(var z = z0; z <= maxZ + double.Epsilon; z += IntervalZ)
      {
        var label = TextCreator.CreateTextLabelModel3D(
          z.ToString(CultureInfo.InvariantCulture), Brushes.Black, true, FontSize, new Point3D(minX - FontSize * 3, maxY, z), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
        plotModel.Children.Add(label);
      }
      
      {
        var label = TextCreator.CreateTextLabelModel3D(
          "Z-axis", Brushes.Black, true, FontSize, new Point3D(minX - FontSize * 10, maxY, (minZ + maxZ) * 0.5), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
        plotModel.Children.Add(label);
      }

      var bb = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
      axesMeshBuilder.AddBoundingBox(bb, LineThickness);

      var axesModel = new GeometryModel3D(axesMeshBuilder.ToMesh(), Materials.Black);
      plotModel.Children.Add(scatterModel);
      plotModel.Children.Add(axesModel);

      return plotModel;
    }
  }
}
