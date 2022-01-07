namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that renders a box.
  /// </summary>
  /// <remarks>
  /// The box is aligned with the local X, Y and Z coordinate system
  /// Use a transform to orient the box in other directions.
  /// </remarks>
  public class BoxVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets a value indicating whether to include the bottom face.
    /// </summary>
    public bool BottomFace
    {
      get => (bool)GetValue(BottomFaceProperty);
      set => SetValue(BottomFaceProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BottomFace"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BottomFaceProperty = DependencyProperty.Register(
      nameof(BottomFace), typeof(bool), typeof(BoxVisual3D), new UIPropertyMetadata(true, GeometryChanged));

    /// <summary>
    /// Gets or sets the center of the box.
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
      nameof(Center), typeof(Point3D), typeof(BoxVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

    /// <summary>
    /// Gets or sets the height (along local z-axis).
    /// </summary>
    /// <value>The height.</value>
    public double Height
    {
      get => (double)GetValue(HeightProperty);
      set => SetValue(HeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
      nameof(Height), typeof(double), typeof(BoxVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the length of the box (along local x-axis).
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
      nameof(Length), typeof(double), typeof(BoxVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets a value indicating whether to include the top face.
    /// </summary>
    public bool TopFace
    {
      get => (bool)GetValue(TopFaceProperty);
      set => SetValue(TopFaceProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TopFace"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TopFaceProperty = DependencyProperty.Register(
      nameof(TopFace), typeof(bool), typeof(BoxVisual3D), new UIPropertyMetadata(true, GeometryChanged));

    /// <summary>
    /// Gets or sets the width of the box (along local y-axis).
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
      nameof(Width), typeof(double), typeof(BoxVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>The mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var b = new MeshBuilder(false);
      b.AddCubeFace(Center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), Length, Width, Height);
      b.AddCubeFace(Center, new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), Length, Width, Height);
      b.AddCubeFace(Center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), Width, Length, Height);
      b.AddCubeFace(Center, new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), Width, Length, Height);
      if(TopFace)
      {
        b.AddCubeFace(Center, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), Height, Length, Width);
      }

      if(BottomFace)
      {
        b.AddCubeFace(Center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), Height, Length, Width);
      }
      return b.ToMesh();
    }
  }
}