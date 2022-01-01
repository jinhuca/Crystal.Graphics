namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a 3D rectangle defined by origin, normal, length and width.
  /// </summary>
  public class RectangleVisual3D : MeshElement3D
  {
    /// <summary>
    /// Identifies the <see cref="DivLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DivLengthProperty = DependencyProperty.Register(
        "DivLength", typeof(int), typeof(RectangleVisual3D), new UIPropertyMetadata(10, GeometryChanged, CoerceDivValue));

    /// <summary>
    /// Identifies the <see cref="DivWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DivWidthProperty = DependencyProperty.Register(
        "DivWidth", typeof(int), typeof(RectangleVisual3D), new UIPropertyMetadata(10, GeometryChanged, CoerceDivValue));

    /// <summary>
    /// Identifies the <see cref="LengthDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthDirectionProperty =
        DependencyProperty.Register(
            "LengthDirection",
            typeof(Vector3D),
            typeof(RectangleVisual3D),
            new PropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Length"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        "Length", typeof(double), typeof(RectangleVisual3D), new PropertyMetadata(10.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Normal"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
        "Normal",
        typeof(Vector3D),
        typeof(RectangleVisual3D),
        new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Origin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
        "Origin",
        typeof(Point3D),
        typeof(RectangleVisual3D),
        new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
        "Width", typeof(double), typeof(RectangleVisual3D), new PropertyMetadata(10.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the number of divisions in the 'length' direction.
    /// </summary>
    /// <value>The number of divisions.</value>
    public int DivLength
    {
      get => (int)GetValue(DivLengthProperty);

      set => SetValue(DivLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of divisions in the 'width' direction.
    /// </summary>
    /// <value>The number of divisions.</value>
    public int DivWidth
    {
      get => (int)GetValue(DivWidthProperty);

      set => SetValue(DivWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length.</value>
    public double Length
    {
      get => (double)GetValue(LengthProperty);

      set => SetValue(LengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the length direction.
    /// </summary>
    /// <value>The length direction.</value>
    public Vector3D LengthDirection
    {
      get => (Vector3D)GetValue(LengthDirectionProperty);

      set => SetValue(LengthDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the normal vector of the plane.
    /// </summary>
    /// <value>The normal.</value>
    public Vector3D Normal
    {
      get => (Vector3D)GetValue(NormalProperty);

      set => SetValue(NormalProperty, value);
    }

    /// <summary>
    /// Gets or sets the center point of the plane.
    /// </summary>
    /// <value>The origin.</value>
    public Point3D Origin
    {
      get => (Point3D)GetValue(OriginProperty);

      set => SetValue(OriginProperty, value);
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public double Width
    {
      get => (double)GetValue(WidthProperty);

      set => SetValue(WidthProperty, value);
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D Tessellate()
    {
      Vector3D u = LengthDirection;
      Vector3D w = Normal;
      Vector3D v = Vector3D.CrossProduct(w, u);
      u = Vector3D.CrossProduct(v, w);

      u.Normalize();
      v.Normalize();
      w.Normalize();

      double le = Length;
      double wi = Width;

      var pts = new List<Point3D>();
      for(int i = 0; i < DivLength; i++)
      {
        double fi = -0.5 + ((double)i / (DivLength - 1));
        for(int j = 0; j < DivWidth; j++)
        {
          double fj = -0.5 + ((double)j / (DivWidth - 1));
          pts.Add(Origin + (u * le * fi) + (v * wi * fj));
        }
      }

      var builder = new MeshBuilder(false, true);
      builder.AddRectangularMesh(pts, DivWidth);

      return builder.ToMesh();
    }

    /// <summary>
    /// Coerces the division value.
    /// </summary>
    /// <param name="d">The sender.</param>
    /// <param name="baseValue">The base value.</param>
    /// <returns>A value not less than 2.</returns>
    private static object CoerceDivValue(DependencyObject d, object baseValue)
    {
      return Math.Max(2, (int)baseValue);
    }
  }
}