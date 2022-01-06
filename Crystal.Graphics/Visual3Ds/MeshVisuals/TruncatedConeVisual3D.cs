namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a truncated cone defined by origin, height, normal, base- and top radius.
  /// </summary>
  public class TruncatedConeVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="BaseCap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BaseCapProperty = DependencyProperty.Register(
        "BaseCap", typeof(bool), typeof(TruncatedConeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="BaseRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BaseRadiusProperty = DependencyProperty.Register(
        "BaseRadius", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
        "Height", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(2.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Normal"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
        "Normal",
        typeof(Vector3D),
        typeof(TruncatedConeVisual3D),
        new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Origin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
        "Origin",
        typeof(Point3D),
        typeof(TruncatedConeVisual3D),
        new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
        "ThetaDiv", typeof(int), typeof(TruncatedConeVisual3D), new PropertyMetadata(35, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="TopCap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TopCapProperty = DependencyProperty.Register(
        "TopCap", typeof(bool), typeof(TruncatedConeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="TopRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TopRadiusProperty = DependencyProperty.Register(
        "TopRadius", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(0.0, GeometryChanged));

    /// <summary>
    /// Gets or sets a value indicating whether to include a base cap.
    /// </summary>
    public bool BaseCap
    {
      get => (bool)GetValue(BaseCapProperty);

      set => SetValue(BaseCapProperty, value);
    }

    /// <summary>
    /// Gets or sets the base radius.
    /// </summary>
    /// <value>The base radius.</value>
    public double BaseRadius
    {
      get => (double)GetValue(BaseRadiusProperty);

      set => SetValue(BaseRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public double Height
    {
      get => (double)GetValue(HeightProperty);

      set => SetValue(HeightProperty, value);
    }

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
    /// Gets or sets the origin.
    /// </summary>
    /// <value>The origin.</value>
    public Point3D Origin
    {
      get => (Point3D)GetValue(OriginProperty);

      set => SetValue(OriginProperty, value);
    }

    /// <summary>
    /// Gets or sets the theta div.
    /// </summary>
    /// <value>The theta div.</value>
    public int ThetaDiv
    {
      get => (int)GetValue(ThetaDivProperty);

      set => SetValue(ThetaDivProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to include a top cap.
    /// </summary>
    public bool TopCap
    {
      get => (bool)GetValue(TopCapProperty);

      set => SetValue(TopCapProperty, value);
    }

    /// <summary>
    /// Gets or sets the top radius.
    /// </summary>
    /// <value>The top radius.</value>
    public double TopRadius
    {
      get => (double)GetValue(TopRadiusProperty);

      set => SetValue(TopRadiusProperty, value);
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var builder = new MeshBuilder(false);
      builder.AddCone(
          Origin,
          Normal,
          BaseRadius,
          TopRadius,
          Height,
          BaseCap,
          TopCap,
          ThetaDiv);
      return builder.ToMesh();
    }
  }
}