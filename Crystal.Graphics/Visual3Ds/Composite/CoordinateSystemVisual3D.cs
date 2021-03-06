namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a coordinate system with arrows in the X, Y and Z directions.
  /// </summary>
  public class CoordinateSystemVisual3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="ArrowLengths"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrowLengthsProperty = DependencyProperty.Register(
        "ArrowLengths",
        typeof(double),
        typeof(CoordinateSystemVisual3D),
        new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="XAxisColor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty XAxisColorProperty = DependencyProperty.Register(
        "XAxisColor",
        typeof(Color),
        typeof(CoordinateSystemVisual3D),
        new UIPropertyMetadata(Color.FromRgb(150, 75, 75)));

    /// <summary>
    /// Identifies the <see cref="YAxisColor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty YAxisColorProperty = DependencyProperty.Register(
        "YAxisColor",
        typeof(Color),
        typeof(CoordinateSystemVisual3D),
        new UIPropertyMetadata(Color.FromRgb(75, 150, 75)));

    /// <summary>
    /// Identifies the <see cref="ZAxisColor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZAxisColorProperty = DependencyProperty.Register(
        "ZAxisColor",
        typeof(Color),
        typeof(CoordinateSystemVisual3D),
        new UIPropertyMetadata(Color.FromRgb(75, 75, 150)));

    /// <summary>
    /// Initializes a new instance of the <see cref = "CoordinateSystemVisual3D" /> class.
    /// </summary>
    public CoordinateSystemVisual3D()
    {
      OnGeometryChanged();
    }

    /// <summary>
    /// Gets or sets the arrow lengths.
    /// </summary>
    /// <value>The arrow lengths.</value>
    public double ArrowLengths
    {
      get => (double)GetValue(ArrowLengthsProperty);

      set => SetValue(ArrowLengthsProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the X axis.
    /// </summary>
    /// <value>The color of the X axis.</value>
    public Color XAxisColor
    {
      get => (Color)GetValue(XAxisColorProperty);

      set => SetValue(XAxisColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the Y axis.
    /// </summary>
    /// <value>The color of the Y axis.</value>
    public Color YAxisColor
    {
      get => (Color)GetValue(YAxisColorProperty);

      set => SetValue(YAxisColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the Z axis.
    /// </summary>
    /// <value>The color of the Z axis.</value>
    public Color ZAxisColor
    {
      get => (Color)GetValue(ZAxisColorProperty);

      set => SetValue(ZAxisColorProperty, value);
    }

    /// <summary>
    /// The geometry changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void GeometryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((CoordinateSystemVisual3D)obj).OnGeometryChanged();
    }

    /// <summary>
    /// Called when the geometry has changed.
    /// </summary>
    protected virtual void OnGeometryChanged()
    {
      Children.Clear();
      var l = ArrowLengths;
      var d = l * 0.1;

      var xaxis = new ArrowVisual3D();
      xaxis.BeginEdit();
      xaxis.Point2 = new Point3D(l, 0, 0);
      xaxis.Diameter = d;
      xaxis.Fill = new SolidColorBrush(XAxisColor);
      xaxis.EndEdit();
      Children.Add(xaxis);

      var yaxis = new ArrowVisual3D();
      yaxis.BeginEdit();
      yaxis.Point2 = new Point3D(0, l, 0);
      yaxis.Diameter = d;
      yaxis.Fill = new SolidColorBrush(YAxisColor);
      yaxis.EndEdit();
      Children.Add(yaxis);

      var zaxis = new ArrowVisual3D();
      zaxis.BeginEdit();
      zaxis.Point2 = new Point3D(0, 0, l);
      zaxis.Diameter = d;
      zaxis.Fill = new SolidColorBrush(ZAxisColor);
      zaxis.EndEdit();
      Children.Add(zaxis);

      Children.Add(new CubeVisual3D { SideLength = d, Fill = Brushes.Black });
    }

  }
}