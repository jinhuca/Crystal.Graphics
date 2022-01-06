namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a pipe between two points.
  /// </summary>
  public class PipeVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
        "Diameter", typeof(double), typeof(PipeVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="InnerDiameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InnerDiameterProperty = DependencyProperty.Register(
        "InnerDiameter", typeof(double), typeof(PipeVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point1"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
        "Point1",
        typeof(Point3D),
        typeof(PipeVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
        "Point2",
        typeof(Point3D),
        typeof(PipeVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 10), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
        "ThetaDiv", typeof(int), typeof(PipeVisual3D), new UIPropertyMetadata(36, GeometryChanged));

    /// <summary>
    /// Gets or sets the (outer) diameter.
    /// </summary>
    /// <value>The diameter. The default value is <c>1</c>.</value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);

      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner diameter.
    /// </summary>
    /// <value>The inner diameter. The default value is <c>0</c>.</value>
    public double InnerDiameter
    {
      get => (double)GetValue(InnerDiameterProperty);

      set => SetValue(InnerDiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the start point.
    /// </summary>
    /// <value>The start point. The default value is <c>0,0,0</c>.</value>
    public Point3D Point1
    {
      get => (Point3D)GetValue(Point1Property);

      set => SetValue(Point1Property, value);
    }

    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    /// <value>The end point. The default value is <c>0,0,10</c>.</value>
    public Point3D Point2
    {
      get => (Point3D)GetValue(Point2Property);

      set => SetValue(Point2Property, value);
    }

    /// <summary>
    /// Gets or sets the theta div.
    /// </summary>
    /// <value>The theta div. The default value is <c>36</c>.</value>
    public int ThetaDiv
    {
      get => (int)GetValue(ThetaDivProperty);

      set => SetValue(ThetaDivProperty, value);
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D" />.
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var builder = new MeshBuilder(false);
      builder.AddPipe(Point1, Point2, InnerDiameter, Diameter, ThetaDiv);
      return builder.ToMesh();
    }
  }
}