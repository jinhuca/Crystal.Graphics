using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.Graphics
{
  public class PointPlotViewport3D : CrystalViewport3D
  {
    #region Constructors
    
    static PointPlotViewport3D()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PointPlotViewport3D), new FrameworkPropertyMetadata(typeof(PointPlotViewport3D)));
    }

    #endregion Constructors

    private readonly ModelVisual3D visualChild;

    public PointPlotViewport3D()
    {
      visualChild = new ModelVisual3D();
      Children.Add(visualChild);
    }

    private static void ModelChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
    {
      ((PointPlotViewport3D)dpObj).UpdateModel();
    }

    private void UpdateModel()
    {
      visualChild.Content = CreateModel();
    }

    private Model3D CreateModel()
    {
      Model3DGroup? plotModel = new();
      if(Points == null || Values == null) return plotModel;

      double minX = Points.Min(p => p.X);
      double maxX = Points.Max(p => p.X);
      double minY = Points.Min(p => p.Y);
      double maxY = Points.Max(p => p.Y);
      double minZ = Points.Min(p => p.Z);
      double maxZ = Points.Max(p => p.Z);
      double minValue = Values.Min();
      double maxValue = Values.Max();

      var valueRange = maxValue - minValue;
      var scatterMeshBuilder = new MeshBuilder(true, true);
      var oldTCCount = 0;

      for(var i = 0; i < Points.Length; ++i)
      {
        scatterMeshBuilder.AddSphere(Points[i], SphereSize, 16, 16);
        var u = (Values[i] - minValue) / valueRange;
        var newTCCount = scatterMeshBuilder.TextureCoordinates.Count;

        for(var j = oldTCCount; j < newTCCount; ++j)
        {
          scatterMeshBuilder.TextureCoordinates[j] = new Point(u, u);
        }
        oldTCCount = newTCCount;
      }

      MeshGeometry3D? mesh = scatterMeshBuilder.ToMesh(true);
      Material? temp = MaterialHelper.CreateMaterial(SurfaceBrush, null, null, 1, 0);

      var scatterModel = new GeometryModel3D(mesh, temp);

      scatterModel.BackMaterial = scatterModel.Material;

      #region Create bounding box with axes indications

      var axesMeshBuilder = new MeshBuilder();

      for(double x = minX; x <= Math.Ceiling(maxX) + 1; x += IntervalX)
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          $"{x:0}", XAxisLabelBrush, true, XAxisLabelFontSize, new Point3D(x, minY - FontSize * 2.5, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }

      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          XAxisTitleContent, XAxisTitleBrush, true, XAxisTitleFontSize, new Point3D((minX + maxX) * 0.5, minY - FontSize * 6, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }

      for(double y = minY; y <= Math.Ceiling(maxY) + 1; y += IntervalY)
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          $"{y:0}", YAxisLabelBrush, true, YAxisLabelFontSize, new Point3D(minX - FontSize * 3, y, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          YAxisTitleContent, YAxisTitleBrush, true, YAxisTitleFontSize, new Point3D(minX - (FontSize * 10), (minY + maxY) * 0.5, minZ), new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0));
        plotModel.Children.Add(label);
      }

      double z0 = (int)(minZ / IntervalZ) * IntervalZ;
      for(double z = z0; z <= Math.Ceiling(maxZ); z += IntervalZ)
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          $"{z:0}", ZAxisLabelBrush, true, ZAxisLabelFontSize, new Point3D(minX - FontSize * 3, maxY, z), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
        plotModel.Children.Add(label);
      }

      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          ZAxisTitleContent, ZAxisTitleBrush, true, ZAxisTitleFontSize, new Point3D(minX - FontSize * 10, maxY, (minZ + maxZ) * 0.5), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
        plotModel.Children.Add(label);
      }

      var axisBoundingBox = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
      axesMeshBuilder.AddBoundingBox(axisBoundingBox, BoundingBoxThickness);

      var axesModel = new GeometryModel3D(axesMeshBuilder.ToMesh(), BoundingBoxMaterial);

      #endregion Create bounding box with axes indications

      plotModel.Children.Add(scatterModel);
      plotModel.Children.Add(axesModel);

      return plotModel;
    }

    #region Dependency Properties

    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
      nameof(Points), typeof(Point3D[]), typeof(PointPlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    public Point3D[] Points
    {
      get => (Point3D[])GetValue(PointsProperty);
      set => SetValue(PointsProperty, value);
    }

    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
      nameof(Values), typeof(double[]), typeof(PointPlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    public double[] Values
    {
      get => (double[])GetValue(ValuesProperty);
      set => SetValue(ValuesProperty, value);
    }

    public static readonly DependencyProperty SurfaceBrushProperty = DependencyProperty.Register(
      nameof(SurfaceBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

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
      nameof(IntervalX), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double IntervalY
    {
      get => (double)GetValue(IntervalYProperty);
      set => SetValue(IntervalYProperty, value);
    }

    public static readonly DependencyProperty IntervalYProperty = DependencyProperty.Register(
      nameof(IntervalY), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double IntervalZ
    {
      get => (double)GetValue(IntervalZProperty);
      set => SetValue(IntervalZProperty, value);
    }

    public static readonly DependencyProperty IntervalZProperty = DependencyProperty.Register(
      nameof(IntervalZ), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public new double FontSize
    {
      get => (double)GetValue(FontSizeProperty);
      set => SetValue(FontSizeProperty, value);
    }

    public static new readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
      nameof(FontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(.1d, ModelChanged));

    public double SphereSize
    {
      get => (double)GetValue(SphereSizeProperty);
      set => SetValue(SphereSizeProperty, value);
    }

    public static readonly DependencyProperty SphereSizeProperty = DependencyProperty.Register(
      nameof(SphereSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(.15d, ModelChanged));

    #endregion Dependency Properties

    #region Bounding box with axes indications

    public double BoundingBoxThickness
    {
      get => (double)GetValue(BoundingBoxThicknessProperty);
      set => SetValue(BoundingBoxThicknessProperty, value);
    }

    public static readonly DependencyProperty BoundingBoxThicknessProperty = DependencyProperty.Register(
      nameof(BoundingBoxThickness), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.003d, ModelChanged));

    public Material BoundingBoxMaterial
    {
      get => (Material)GetValue(BoundingBoxMaterialProperty);
      set => SetValue(BoundingBoxMaterialProperty, value);
    }

    public static readonly DependencyProperty BoundingBoxMaterialProperty = DependencyProperty.Register(
      nameof(BoundingBoxMaterial), typeof(Material), typeof(PointPlotViewport3D), new UIPropertyMetadata(Materials.Black, ModelChanged));

    #endregion Bounding box with axes indications

    #region X-Axis Title Dependency Properties

    public string XAxisTitleContent
    {
      get => (string)GetValue(XAxisTitleContentProperty);
      set => SetValue(XAxisTitleContentProperty, value);
    }

    public static readonly DependencyProperty XAxisTitleContentProperty = DependencyProperty.Register(
      nameof(XAxisTitleContent), typeof(string), typeof(PointPlotViewport3D), new UIPropertyMetadata("X-axis", ModelChanged));

    public double XAxisTitleFontSize
    {
      get => (double)GetValue(XAxisTitleFontSizeProperty);
      set => SetValue(XAxisTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty XAxisTitleFontSizeProperty = DependencyProperty.Register(
      nameof(XAxisTitleFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    public Brush XAxisTitleBrush
    {
      get => (Brush)GetValue(XAxisTitleBrushProperty);
      set => SetValue(XAxisTitleBrushProperty, value);
    }

    public static readonly DependencyProperty XAxisTitleBrushProperty = DependencyProperty.Register(
      nameof(XAxisTitleBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Black, ModelChanged));

    #endregion X-Axis Title Dependency Properties

    #region X-Axis Label Dependency Properties

    public Brush XAxisLabelBrush
    {
      get => (Brush)GetValue(XAxisLabelBrushProperty);
      set => SetValue(XAxisLabelBrushProperty, value);
    }

    public static readonly DependencyProperty XAxisLabelBrushProperty = DependencyProperty.Register(
      nameof(XAxisLabelBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Blue, ModelChanged));

    public double XAxisLabelFontSize
    {
      get => (double)GetValue(XAxisLabelFontSizeProperty);
      set => SetValue(XAxisLabelFontSizeProperty, value);
    }

    public static readonly DependencyProperty XAxisLabelFontSizeProperty = DependencyProperty.Register(
      nameof(XAxisLabelFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    #endregion X-Axis Label Dependency Properties

    #region Y-Axis Title Dependency Properties

    public string YAxisTitleContent
    {
      get => (string)GetValue(YAxisTitleContentProperty);
      set => SetValue(YAxisTitleContentProperty, value);
    }

    public static readonly DependencyProperty YAxisTitleContentProperty = DependencyProperty.Register(
      nameof(YAxisTitleContent), typeof(string), typeof(PointPlotViewport3D), new UIPropertyMetadata("Y-Axis", ModelChanged));

    public double YAxisTitleFontSize
    {
      get => (double)GetValue(YAxisTitleFontSizeProperty);
      set => SetValue(YAxisTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty YAxisTitleFontSizeProperty = DependencyProperty.Register(
      nameof(YAxisTitleFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    public Brush YAxisTitleBrush
    {
      get => (Brush)GetValue(YAxisTitleBrushProperty);
      set => SetValue(YAxisTitleBrushProperty, value);
    }

    public static readonly DependencyProperty YAxisTitleBrushProperty = DependencyProperty.Register(
      nameof(YAxisTitleBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Black, ModelChanged));

    #endregion Y-Axis Title Dependency Properties

    #region Y-Axis Label Dependency Properties

    public Brush YAxisLabelBrush
    {
      get => (Brush)GetValue(YAxisLabelBrushProperty);
      set => SetValue(YAxisLabelBrushProperty, value);
    }

    public static readonly DependencyProperty YAxisLabelBrushProperty = DependencyProperty.Register(
      nameof(YAxisLabelBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Blue, ModelChanged));

    public double YAxisLabelFontSize
    {
      get => (double)GetValue(YAxisLabelFontSizeProperty);
      set => SetValue(YAxisLabelFontSizeProperty, value);
    }

    public static readonly DependencyProperty YAxisLabelFontSizeProperty = DependencyProperty.Register(
      nameof(YAxisLabelFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    #endregion Y-Axis Label Dependency Properties

    #region Z-Axis Title Dependency Properties

    public string ZAxisTitleContent
    {
      get => (string)GetValue(ZAxisTitleContentProperty);
      set => SetValue(ZAxisTitleContentProperty, value);
    }

    public static readonly DependencyProperty ZAxisTitleContentProperty = DependencyProperty.Register(
      nameof(ZAxisTitleContent), typeof(string), typeof(PointPlotViewport3D), new UIPropertyMetadata("Z-axis", ModelChanged));

    public double ZAxisTitleFontSize
    {
      get => (double)GetValue(ZAxisTitleFontSizeProperty);
      set => SetValue(ZAxisTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty ZAxisTitleFontSizeProperty = DependencyProperty.Register(
      nameof(ZAxisTitleFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    public Brush ZAxisTitleBrush
    {
      get => (Brush)GetValue(ZAxisTitleBrushProperty);
      set => SetValue(ZAxisTitleBrushProperty, value);
    }

    public static readonly DependencyProperty ZAxisTitleBrushProperty = DependencyProperty.Register(
      nameof(ZAxisTitleBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Black, ModelChanged));

    #endregion Z-Axis Title Dependency Properties

    #region Z-Axis Label Dependency Properties

    public Brush ZAxisLabelBrush
    {
      get => (Brush)GetValue(ZAxisLabelBrushProperty);
      set => SetValue(ZAxisLabelBrushProperty, value);
    }

    public static readonly DependencyProperty ZAxisLabelBrushProperty = DependencyProperty.Register(
      nameof(ZAxisLabelBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Blue, ModelChanged));

    public double ZAxisLabelFontSize
    {
      get => (double)GetValue(ZAxisLabelFontSizeProperty);
      set => SetValue(ZAxisLabelFontSizeProperty, value);
    }

    public static readonly DependencyProperty ZAxisLabelFontSizeProperty = DependencyProperty.Register(
      nameof(ZAxisLabelFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    #endregion Z-Axis Label Dependency Properties
  }
}
