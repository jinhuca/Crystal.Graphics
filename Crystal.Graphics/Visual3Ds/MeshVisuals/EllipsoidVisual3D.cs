namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows an axis aligned ellipsoid.
  /// </summary>
  public class EllipsoidVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the center of the ellipsoid (this will set the transform of the element).
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
      nameof(Center), typeof(Point3D), typeof(EllipsoidVisual3D), new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Gets or sets the number of divisions in the phi direction (from "top" to "bottom").
    /// </summary>
    /// <value>The number of divisions.</value>
    public int PhiDiv
    {
      get => (int)GetValue(PhiDivProperty);
      set => SetValue(PhiDivProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="PhiDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PhiDivProperty = DependencyProperty.Register(
      nameof(PhiDiv), typeof(int), typeof(EllipsoidVisual3D), new PropertyMetadata(30, GeometryChanged));

    /// <summary>
    /// Gets or sets the X equatorial radius of the ellipsoid.
    /// </summary>
    /// <value>The radius.</value>
    public double RadiusX
    {
      get => (double)GetValue(RadiusXProperty);
      set => SetValue(RadiusXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RadiusX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register(
      nameof(RadiusX), typeof(double), typeof(EllipsoidVisual3D), new PropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the Y equatorial radius of the ellipsoid.
    /// </summary>
    /// <value>The radius.</value>
    public double RadiusY
    {
      get => (double)GetValue(RadiusYProperty);
      set => SetValue(RadiusYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RadiusY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register(
      nameof(RadiusY), typeof(double), typeof(EllipsoidVisual3D), new PropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the polar radius of the ellipsoid.
    /// </summary>
    /// <value>The radius.</value>
    public double RadiusZ
    {
      get => (double)GetValue(RadiusZProperty);
      set => SetValue(RadiusZProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RadiusZ"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusZProperty = DependencyProperty.Register(
      nameof(RadiusZ), typeof(double), typeof(EllipsoidVisual3D), new PropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the number of divisions in the theta direction (around the sphere).
    /// </summary>
    /// <value>The number of divisions.</value>
    public int ThetaDiv
    {
      get => (int)GetValue(ThetaDivProperty);
      set => SetValue(ThetaDivProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
      nameof(ThetaDiv), typeof(int), typeof(EllipsoidVisual3D), new PropertyMetadata(60, GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var builder = new MeshBuilder(false);
      builder.AddEllipsoid(Center, RadiusX, RadiusY, RadiusZ, ThetaDiv, PhiDiv);
      return builder.ToMesh();
    }
  }
}