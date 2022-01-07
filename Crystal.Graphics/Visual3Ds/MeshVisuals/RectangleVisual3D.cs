namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a 3D rectangle defined by origin, normal, length and width.
  /// </summary>
  public class RectangleVisual3D : MeshModelVisual3D
  {
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
    /// Identifies the <see cref="DivLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DivLengthProperty = DependencyProperty.Register(
      nameof(DivLength), typeof(int), typeof(RectangleVisual3D), new UIPropertyMetadata(10, GeometryChanged, CoerceDivValue));

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
    /// Identifies the <see cref="DivWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DivWidthProperty = DependencyProperty.Register(
      nameof(DivWidth), typeof(int), typeof(RectangleVisual3D), new UIPropertyMetadata(10, GeometryChanged, CoerceDivValue));

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
    /// Identifies the <see cref="LengthDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthDirectionProperty = DependencyProperty.Register(
      nameof(LengthDirection), typeof(Vector3D), typeof(RectangleVisual3D), new PropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

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
    /// Identifies the <see cref="Length"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
      nameof(Length), typeof(double), typeof(RectangleVisual3D), new PropertyMetadata(10.0, GeometryChanged));

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
    /// Identifies the <see cref="Normal"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
      nameof(Normal), typeof(Vector3D), typeof(RectangleVisual3D), new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

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
    /// Identifies the <see cref="Origin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
      nameof(Origin), typeof(Point3D), typeof(RectangleVisual3D), new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

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
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
      nameof(Width), typeof(double), typeof(RectangleVisual3D), new PropertyMetadata(10.0, GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var u = LengthDirection;
      var w = Normal;
      var v = Vector3D.CrossProduct(w, u);
      u = Vector3D.CrossProduct(v, w);

      u.Normalize();
      v.Normalize();
      w.Normalize();

      var le = Length;
      var wi = Width;

      var pts = new List<Point3D>();
      for(var i = 0; i < DivLength; i++)
      {
        var fi = -0.5 + ((double)i / (DivLength - 1));
        for(var j = 0; j < DivWidth; j++)
        {
          var fj = -0.5 + ((double)j / (DivWidth - 1));
          pts.Add(Origin + (u * le * fi) + (v * wi * fj));
        }
      }

      var builder = new MeshBuilder(false);
      builder.AddRectangularMesh(pts, DivWidth);

      return builder.ToMesh();
    }

    /// <summary>
    /// Coerces the division value.
    /// </summary>
    /// <param name="d">The sender.</param>
    /// <param name="baseValue">The base value.</param>
    /// <returns>A value not less than 2.</returns>
    private static object CoerceDivValue(DependencyObject d, object baseValue) => Math.Max(2, (int)baseValue);
  }
}