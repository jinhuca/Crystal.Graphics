namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a sphere defined by center and radius.
  /// </summary>
  public class SphereVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Center"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
        "Center",
        typeof(Point3D),
        typeof(SphereVisual3D),
        new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="PhiDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PhiDivProperty = DependencyProperty.Register(
        "PhiDiv", typeof(int), typeof(SphereVisual3D), new PropertyMetadata(30, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Radius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
        "Radius", typeof(double), typeof(SphereVisual3D), new PropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
        "ThetaDiv", typeof(int), typeof(SphereVisual3D), new PropertyMetadata(60, GeometryChanged));

    /// <summary>
    /// Gets or sets the center of the sphere.
    /// </summary>
    /// <value>The center.</value>
    public Point3D Center
    {
      get => (Point3D)GetValue(CenterProperty);

      set => SetValue(CenterProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of divisions in the phi direction (from "top" to "bottom").
    /// </summary>
    /// <value>The phi div.</value>
    public int PhiDiv
    {
      get => (int)GetValue(PhiDivProperty);

      set => SetValue(PhiDivProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius of the sphere.
    /// </summary>
    /// <value>The radius.</value>
    public double Radius
    {
      get => (double)GetValue(RadiusProperty);

      set => SetValue(RadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of divisions in the theta direction (around the sphere).
    /// </summary>
    /// <value>The theta div.</value>
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
      var builder = new MeshBuilder(true, true);
      builder.AddSphere(Center, Radius, ThetaDiv, PhiDiv);
      return builder.ToMesh();
    }
  }
}