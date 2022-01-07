namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a flat pie slice defined by center, normal, up vectors, inner and outer radius, start and end angles.
  /// </summary>
  public class PieSliceVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the center.
    /// </summary>
    /// <value>The center.</value>
    public Point3D Center
    {
      get => (Point3D)GetValue(CenterProperty);
      set => SetValue(CenterProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Center"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
      nameof(Center), typeof(Point3D), typeof(PieSliceVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

    /// <summary>
    /// Gets or sets the end angle.
    /// </summary>
    /// <value>The end angle.</value>
    public double EndAngle
    {
      get => (double)GetValue(EndAngleProperty);
      set => SetValue(EndAngleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="EndAngle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
      nameof(EndAngle), typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(90.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the inner radius.
    /// </summary>
    /// <value>The inner radius.</value>
    public double InnerRadius
    {
      get => (double)GetValue(InnerRadiusProperty);
      set => SetValue(InnerRadiusProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="InnerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
      nameof(InnerRadius) , typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(0.5, GeometryChanged));

    /// <summary>
    /// Gets or sets the normal.
    /// </summary>
    /// <value>The normal.</value>
    public Vector3D Normal
    {
      get => (Vector3D)GetValue(NormalProperty);
      set => SetValue(NormalProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Normal"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
      nameof(Normal), typeof(Vector3D), typeof(PieSliceVisual3D), new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

    /// <summary>
    /// Gets or sets the outer radius.
    /// </summary>
    /// <value>The outer radius.</value>
    public double OuterRadius
    {
      get => (double)GetValue(OuterRadiusProperty);
      set => SetValue(OuterRadiusProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="OuterRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register(
      nameof(OuterRadius), typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the start angle.
    /// </summary>
    /// <value>The start angle.</value>
    public double StartAngle
    {
      get => (double)GetValue(StartAngleProperty);
      set => SetValue(StartAngleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="StartAngle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
        nameof(StartAngle), typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the number of angular divisions of the slice.
    /// </summary>
    /// <value>The theta div.</value>
    public int ThetaDiv
    {
      get => (int)GetValue(ThetaDivProperty);
      set => SetValue(ThetaDivProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
        nameof(ThetaDiv), typeof(int), typeof(PieSliceVisual3D), new UIPropertyMetadata(20, GeometryChanged));

    /// <summary>
    /// Gets or sets up vector.
    /// </summary>
    /// <value>Up vector.</value>
    public Vector3D UpVector
    {
      get => (Vector3D)GetValue(UpVectorProperty);
      set => SetValue(UpVectorProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="UpVector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpVectorProperty = DependencyProperty.Register(
        nameof(UpVector), typeof(Vector3D), typeof(PieSliceVisual3D), new UIPropertyMetadata(new Vector3D(0, 1, 0), GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var pts = new List<Point3D>();
      var right = Vector3D.CrossProduct(UpVector, Normal);
      for(var i = 0; i < ThetaDiv; i++)
      {
        var angle = StartAngle + ((EndAngle - StartAngle) * i / (ThetaDiv - 1));
        var angleRad = angle / 180 * Math.PI;
        var dir = (right * Math.Cos(angleRad)) + (UpVector * Math.Sin(angleRad));
        pts.Add(Center + (dir * InnerRadius));
        pts.Add(Center + (dir * OuterRadius));
      }

      var b = new MeshBuilder(false, false);
      b.AddTriangleStrip(pts);
      return b.ToMesh();
    }
  }
}