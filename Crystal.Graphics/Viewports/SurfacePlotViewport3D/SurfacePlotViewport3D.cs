namespace Crystal.Graphics
{
  public class SurfacePlotViewport3D : PlotViewport3D
  {
    #region Private fields

    private readonly ModelVisual3D visualChild;

    #endregion Private fields

    #region Data Dependency Properties

    /// <summary>
    /// Gets or sets the points defining the surface.
    /// </summary>
    public Point3D[,] Points
    {
      get => (Point3D[,])GetValue(PointsProperty);
      set => SetValue(PointsProperty, value);
    }

    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
      nameof(Points), typeof(Point3D[,]), typeof(SurfacePlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    /// <summary>
    /// Gets or sets the color values corresponding to the Points array.
    /// The color values are used as Texture coordinates for the surface.
    /// Remember to set the SurfaceBrush, e.g. by using the BrushHelper.CreateGradientBrush method.
    /// If this property is not set, the z-value of the Points will be used as color value.
    /// </summary>
    public double[,] ColorValues
    {
      get => (double[,])GetValue(ColorValuesProperty);
      set => SetValue(ColorValuesProperty, value);
    }

    public static readonly DependencyProperty ColorValuesProperty = DependencyProperty.Register(
      nameof(ColorValues), typeof(double[,]), typeof(SurfacePlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    /// <summary>
    /// Gets or sets the brush used for the surface.
    /// </summary>
    public Brush SurfaceBrush
    {
      get => (Brush)GetValue(SurfaceBrushProperty);
      set => SetValue(SurfaceBrushProperty, value);
    }

    public static readonly DependencyProperty SurfaceBrushProperty = DependencyProperty.Register(
      nameof(SurfaceBrush), typeof(Brush), typeof(SurfacePlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    #endregion Data Dependency Properties

    #region Constructors

    static SurfacePlotViewport3D()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SurfacePlotViewport3D), new FrameworkPropertyMetadata(typeof(SurfacePlotViewport3D)));
    }

    public SurfacePlotViewport3D()
    {
      visualChild = new ModelVisual3D();
      Children.Add(visualChild);
    }

    #endregion Constructors

    #region Private Methods

    private new static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SurfacePlotViewport3D)d).UpdateModel();
    }

    private void UpdateModel()
    {
      visualChild.Content = CreateModel();
    }

    private Model3D CreateModel()
    {
      var plotModel = new Model3DGroup();

      var rows = Points.GetUpperBound(0) + 1;
      var columns = Points.GetUpperBound(1) + 1;
      var minX = double.MaxValue;
      var maxX = double.MinValue;
      var minY = double.MaxValue;
      var maxY = double.MinValue;
      var minZ = double.MaxValue;
      var maxZ = double.MinValue;
      var minColorValue = double.MaxValue;
      var maxColorValue = double.MinValue;
      for(var i = 0; i < rows; i++)
        for(var j = 0; j < columns; j++)
        {
          var x = Points[i, j].X;
          var y = Points[i, j].Y;
          var z = Points[i, j].Z;
          maxX = Math.Max(maxX, x);
          maxY = Math.Max(maxY, y);
          maxZ = Math.Max(maxZ, z);
          minX = Math.Min(minX, x);
          minY = Math.Min(minY, y);
          minZ = Math.Min(minZ, z);
          if(ColorValues != null)
          {
            maxColorValue = Math.Max(maxColorValue, ColorValues[i, j]);
            minColorValue = Math.Min(minColorValue, ColorValues[i, j]);
          }
        }

      // make color value 0 at texture coordinate 0.5
      if(Math.Abs(minColorValue) < Math.Abs(maxColorValue))
        minColorValue = -maxColorValue;
      else
        maxColorValue = -minColorValue;

      // set the texture coordinates by z-value or ColorValue
      var texcoords = new Point[rows, columns];
      for(var i = 0; i < rows; i++)
        for(var j = 0; j < columns; j++)
        {
          var u = (Points[i, j].Z - minZ) / (maxZ - minZ);
          if(ColorValues != null)
            u = (ColorValues[i, j] - minColorValue) / (maxColorValue - minColorValue);
          texcoords[i, j] = new Point(u, u);
        }

      var surfaceMeshBuilder = new MeshBuilder();
      surfaceMeshBuilder.AddRectangularMesh(Points, texcoords);

      var surfaceModel = new GeometryModel3D(surfaceMeshBuilder.ToMesh(), MaterialHelper.CreateMaterial(SurfaceBrush, null, null, 1, 0));
      surfaceModel.BackMaterial = surfaceModel.Material;
      plotModel.Children.Add(surfaceModel);

      var axesMeshBuilder = new MeshBuilder();
      for(var x = minX; x <= maxX; x += IntervalX)
      {
        var j = (x - minX) / (maxX - minX) * (columns - 1);
        var path = new List<Point3D> { new(x, minY, minZ) };
        for(var i = 0; i < rows; i++)
        {
          path.Add(BilinearInterpolation(Points, i, j));
        }
        path.Add(new Point3D(x, maxY, minZ));

        axesMeshBuilder.AddTube(path, BoundingBoxThickness, 9, false);
        
        var label = TextCreator.CreateTextLabelModel3D($"{x:0}", XAxisLabelBrush, true, XAxisLabelFontSize, new Point3D(x, minY - FontSize * 2.5, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }
      {
        var label = TextCreator.CreateTextLabelModel3D(
          XAxisTitleContent, XAxisTitleBrush, true, XAxisTitleFontSize, new Point3D((minX + maxX) * 0.5, minY - FontSize * 10, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }

      for(var y = minY; y <= maxY; y += IntervalY)
      {
        var i = (y - minY) / (maxY - minY) * (rows - 1);
        var path = new List<Point3D> { new(minX, y, minZ) };
        for(var j = 0; j < columns; j++)
        {
          path.Add(BilinearInterpolation(Points, i, j));
        }
        path.Add(new Point3D(maxX, y, minZ));
        axesMeshBuilder.AddTube(path, BoundingBoxThickness, 9, false);
        var label = TextCreator.CreateTextLabelModel3D(
          $"{y:0}", YAxisLabelBrush, true, YAxisLabelFontSize, new Point3D(minX - FontSize * 3, y, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }
      {
        var label = TextCreator.CreateTextLabelModel3D(
          YAxisTitleContent, YAxisTitleBrush, true, YAxisTitleFontSize, new Point3D(minX - FontSize * 10,(minY + maxY) * 0.5, minZ), new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0));
        plotModel.Children.Add(label);
      }

      var z0 = (int)(minZ / IntervalZ) * IntervalZ;
      for(var z = z0; z <= maxZ + double.Epsilon; z += IntervalZ)
      {
        var label = TextCreator.CreateTextLabelModel3D(
          $"{z:0}", ZAxisLabelBrush, true, ZAxisLabelFontSize, new Point3D(minX - FontSize * 3, maxY, z), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
        plotModel.Children.Add(label);
      }
      {
        var label = TextCreator.CreateTextLabelModel3D(
          ZAxisTitleContent, ZAxisTitleBrush, true, ZAxisTitleFontSize, new Point3D(minX - FontSize * 10, maxY, (minZ + maxZ) * 0.5), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
        plotModel.Children.Add(label);
      }

      #region Bounding box
      
      var axisBoundingBox = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
      axesMeshBuilder.AddBoundingBox(axisBoundingBox, BoundingBoxThickness/2);

      #endregion Bounding box

      #region GridLine

      /*
      if (ShowGridLine)
      {
        for (int ix = (int)minX; ix < maxX; ix++)
        {
          var axis1 = new Rect3D(minX + ix, minY, minZ, maxX - minX - ix, maxY - minY, maxZ - minZ);
          axesMeshBuilder.AddBoundingBox(axis1, BoundingBoxThickness / 4);
        }
        
        for (int iy = minY; iy < maxY; iy++)
        {
          var axis1 = new Rect3D(minX, minY + iy, minZ, maxX - minX, maxY - minY - iy, maxZ - minZ);
          axesMeshBuilder.AddBoundingBox(axis1, BoundingBoxThickness / 4);
        }

        for (int iz = minZ; iz < maxZ; iz++)
        {
          var axis1 = new Rect3D(minX, minY, minZ + iz, maxX - minX, maxY - minY, maxZ - minZ - iz);
          axesMeshBuilder.AddBoundingBox(axis1, BoundingBoxThickness / 4);
        }
      }
      */

      #endregion GridLine

      var axesModel = new GeometryModel3D(axesMeshBuilder.ToMesh(), BoundingBoxMaterial);
      plotModel.Children.Add(axesModel);
      
      return plotModel;
    }

    private static Point3D BilinearInterpolation(Point3D[,] p, double i, double j)
    {
      var n = p.GetUpperBound(0);
      var m = p.GetUpperBound(1);
      var i0 = (int)i;
      var j0 = (int)j;
      if(i0 + 1 >= n) i0 = n - 2;
      if(j0 + 1 >= m) j0 = m - 2;

      if(i < 0) i = 0;
      if(j < 0) j = 0;
      var u = i - i0;
      var v = j - j0;
      var v00 = p[i0, j0].ToVector3D();
      var v01 = p[i0, j0 + 1].ToVector3D();
      var v10 = p[i0 + 1, j0].ToVector3D();
      var v11 = p[i0 + 1, j0 + 1].ToVector3D();
      var v0 = v00 * (1 - u) + v10 * u;
      var v1 = v01 * (1 - u) + v11 * u;
      return (v0 * (1 - v) + v1 * v).ToPoint3D();
    }

    #endregion Private Methods
  }
}
