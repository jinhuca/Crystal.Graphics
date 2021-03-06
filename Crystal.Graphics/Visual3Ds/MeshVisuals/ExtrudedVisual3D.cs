namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that extrudes a section along a path.
  /// </summary>
  /// <remarks>
  /// The implementation will not work well if there are sharp bends in the path.
  /// </remarks>
  public class ExtrudedVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the diameters along the path.
    /// </summary>
    /// <value> The diameters. </value>
    public DoubleCollection Diameters
    {
      get => (DoubleCollection)GetValue(DiametersProperty);
      set => SetValue(DiametersProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Diameters"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiametersProperty = DependencyProperty.Register(
      nameof(Diameters), typeof(DoubleCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

    /// <summary>
    /// Gets or sets the initial alignment of the x-axis of the section into the 3D viewport.
    /// </summary>
    /// <value> The section. </value>
    public Vector3D SectionXAxis
    {
      get => (Vector3D)GetValue(SectionXAxisProperty);
      set => SetValue(SectionXAxisProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SectionXAxis"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SectionXAxisProperty = DependencyProperty.Register(
      nameof(SectionXAxis), typeof(Vector3D), typeof(ExtrudedVisual3D), new UIPropertyMetadata(GeometryChanged));

    /// <summary>
    /// Gets or sets the diameters along the path.
    /// </summary>
    /// <value> The diameters. </value>
    public DoubleCollection Angles
    {
      get => (DoubleCollection)GetValue(AnglesProperty);
      set => SetValue(AnglesProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Angles"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AnglesProperty = DependencyProperty.Register(
      nameof(Angles), typeof(DoubleCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the path is closed.
    /// </summary>
    public bool IsPathClosed
    {
      get => (bool)GetValue(IsPathClosedProperty);
      set => SetValue(IsPathClosedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsPathClosed"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsPathClosedProperty = DependencyProperty.Register(
      nameof(IsPathClosed), typeof(bool), typeof(ExtrudedVisual3D), new UIPropertyMetadata(false, GeometryChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the section is closed.
    /// </summary>
    public bool IsSectionClosed
    {
      get => (bool)GetValue(IsSectionClosedProperty);
      set => SetValue(IsSectionClosedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsSectionClosed"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSectionClosedProperty = DependencyProperty.Register(
      nameof(IsSectionClosed), typeof(bool), typeof(ExtrudedVisual3D), new UIPropertyMetadata(true, GeometryChanged));

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value> The path. </value>
    public Point3DCollection Path
    {
      get => (Point3DCollection)GetValue(PathProperty);
      set => SetValue(PathProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Path"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
      nameof(Path), typeof(Point3DCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

    /// <summary>
    /// Gets or sets the section.
    /// </summary>
    /// <value> The section. </value>
    public PointCollection Section
    {
      get => (PointCollection)GetValue(SectionProperty);
      set => SetValue(SectionProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Section"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SectionProperty = DependencyProperty.Register(
      nameof(Section), typeof(PointCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(new PointCollection(), GeometryChanged));

    /// <summary>
    /// Gets or sets the texture coordinates along the path (X only).
    /// </summary>
    /// <value> The texture coordinates. </value>
    public DoubleCollection TextureCoordinates
    {
      get => (DoubleCollection)GetValue(TextureCoordinatesProperty);
      set => SetValue(TextureCoordinatesProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TextureCoordinates"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextureCoordinatesProperty = DependencyProperty.Register(
      nameof(TextureCoordinates), typeof(DoubleCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtrudedVisual3D" /> class.
    /// </summary>
    public ExtrudedVisual3D()
    {
      Path = new Point3DCollection();
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/> .
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
      if(Path == null || Path.Count < 2)
      {
        return null;
      }

      // See also "The GLE Tubing and Extrusion Library":
      // http://linas.org/gle/
      // http://sharpmap.codeplex.com/Thread/View.aspx?ThreadId=18864
      var builder = new MeshBuilder(false, TextureCoordinates != null);
      var sectionXAxis = SectionXAxis;
      if(sectionXAxis.Length < 1e-6)
      {
        sectionXAxis = new Vector3D(1, 0, 0);
      }

      var forward = Path[1] - Path[0];
      var up = Vector3D.CrossProduct(forward, sectionXAxis);
      if(up.LengthSquared < 1e-6)
      {
        sectionXAxis = forward.FindAnyPerpendicular();
      }

      builder.AddTube(Path, Angles, TextureCoordinates, Diameters, Section, sectionXAxis, IsPathClosed, IsSectionClosed);
      return builder.ToMesh();
    }
  }
}
