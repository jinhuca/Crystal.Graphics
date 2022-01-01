namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows an arrow.
  /// </summary>
  public class ArrowVisual3D : MeshElement3D
  {
    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
        "Diameter", typeof(double), typeof(ArrowVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="HeadLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeadLengthProperty = DependencyProperty.Register(
        "HeadLength", typeof(double), typeof(ArrowVisual3D), new UIPropertyMetadata(3.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point1"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
        "Point1",
        typeof(Point3D),
        typeof(ArrowVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
        "Point2",
        typeof(Point3D),
        typeof(ArrowVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 10), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
        "ThetaDiv", typeof(int), typeof(ArrowVisual3D), new UIPropertyMetadata(36, GeometryChanged));

    /// <summary>
    /// Gets or sets the diameter.
    /// </summary>
    /// <value>The diameter.</value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);

      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    /// <value>The direction.</value>
    public Vector3D Direction
    {
      get => Point2 - Point1;

      set => Point2 = Point1 + value;
    }

    /// <summary>
    /// Gets or sets the length of the head (relative to diameter of the arrow cylinder).
    /// </summary>
    /// <value>The length of the head relative to the diameter.</value>
    public double HeadLength
    {
      get => (double)GetValue(HeadLengthProperty);

      set => SetValue(HeadLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the origin.
    /// </summary>
    /// <value>The origin.</value>
    public Point3D Origin
    {
      get => Point1;

      set => Point1 = value;
    }

    /// <summary>
    /// Gets or sets the start point of the arrow.
    /// </summary>
    /// <value>The start point.</value>
    public Point3D Point1
    {
      get => (Point3D)GetValue(Point1Property);

      set => SetValue(Point1Property, value);
    }

    /// <summary>
    /// Gets or sets the end point of the arrow.
    /// </summary>
    /// <value>The end point.</value>
    public Point3D Point2
    {
      get => (Point3D)GetValue(Point2Property);

      set => SetValue(Point2Property, value);
    }

    /// <summary>
    /// Gets or sets the number of divisions around the arrow.
    /// </summary>
    /// <value>The number of divisions.</value>
    public int ThetaDiv
    {
      get => (int)GetValue(ThetaDivProperty);

      set => SetValue(ThetaDivProperty, value);
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D Tessellate()
    {
      if(Diameter <= 0)
      {
        return null;
      }

      var builder = new MeshBuilder(true, true);
      builder.AddArrow(Point1, Point2, Diameter, HeadLength, ThetaDiv);
      return builder.ToMesh();
    }
  }
}