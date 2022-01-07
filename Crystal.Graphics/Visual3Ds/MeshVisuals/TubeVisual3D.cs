namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a tube along a specified path.
  /// </summary>
  /// <remarks>
  /// The implementation will not work well if there are sharp bends in the path.
  /// </remarks>
  public class TubeVisual3D : ExtrudedVisual3D
  {
    /// <summary>
    /// Gets or sets the diameter of the tube.
    /// </summary>
    /// <value>The diameter of the tube.</value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);
      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
      nameof(Diameter), typeof(double), typeof(TubeVisual3D), new UIPropertyMetadata(1.0, SectionChanged));

    /// <summary>
    /// Gets or sets the number of divisions around the tube.
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
      nameof(ThetaDiv), typeof(int), typeof(TubeVisual3D), new UIPropertyMetadata(36, SectionChanged));

    /// <summary>
    /// Gets or sets the create Caps indicator.
    /// </summary>
    /// <value>True if Caps should be generated.</value>
    public bool AddCaps
    {
      get => (bool)GetValue(AddCapsProperty);
      set => SetValue(AddCapsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="AddCaps"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AddCapsProperty = DependencyProperty.Register(
      nameof(AddCaps), typeof(bool), typeof(TubeVisual3D), new UIPropertyMetadata(false, SectionChanged));

    /// <summary>
    /// Initializes static members of the <see cref="TubeVisual3D"/> class.
    /// </summary>
    static TubeVisual3D()
    {
      DiametersProperty.OverrideMetadata(typeof(TubeVisual3D), new UIPropertyMetadata(null, SectionChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "TubeVisual3D" /> class.
    /// </summary>
    public TubeVisual3D()
    {
      OnSectionChanged();
    }

    /// <summary>
    /// The section changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void SectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TubeVisual3D)d).OnSectionChanged();
    }

    /// <summary>
    /// Updates the section.
    /// </summary>
    protected void OnSectionChanged()
    {
      var pc = new PointCollection();
      var circle = MeshBuilder.GetCircle(ThetaDiv);

      // If Diameters is not set, create a unit circle
      // otherwise, create a circle with the specified diameter
      var r = Diameters != null ? 1 : Diameter / 2;
      for(var j = 0; j < ThetaDiv; j++)
      {
        pc.Add(new Point(circle[j].X * r, circle[j].Y * r));
      }
      Section = pc;
      OnGeometryChanged();
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
      builder.AddTube(Path, Angles, TextureCoordinates, Diameters, Section, sectionXAxis, IsPathClosed, IsSectionClosed, AddCaps, AddCaps);
      return builder.ToMesh();
    }
  }
}
