namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that contains a set of line segments. The thickness of the lines is defined in screen space.
  /// </summary>
  public class LinesVisual3D : ScreenSpaceVisual3D
  {
    /// <summary>
    /// Gets or sets the thickness of the lines.
    /// </summary>
    /// <value>
    /// The thickness.
    /// </value>
    public double Thickness
    {
      get => (double)GetValue(ThicknessProperty);
      set => SetValue(ThicknessProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Thickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
      nameof(Thickness), typeof(double), typeof(LinesVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// The builder.
    /// </summary>
    private readonly LineGeometryBuilder builder;

    /// <summary>
    /// Initializes a new instance of the <see cref = "LinesVisual3D" /> class.
    /// </summary>
    public LinesVisual3D() => builder = new LineGeometryBuilder(this);

    /// <summary>
    /// Updates the geometry.
    /// </summary>
    protected override void UpdateGeometry()
    {
      if(Points == null)
      {
        Mesh.Positions = null;
        return;
      }

      var n = Points.Count;
      if(n > 0)
      {
        if(Mesh.TriangleIndices.Count != n * 3)
        {
          Mesh.TriangleIndices = builder.CreateIndices(n);
        }

        Mesh.Positions = builder.CreatePositions(Points, Thickness, DepthOffset);
      }
      else
      {
        Mesh.Positions = null;
      }
    }

    /// <summary>
    /// Updates the transforms.
    /// </summary>
    /// <returns>
    /// True if the transform is updated.
    /// </returns>
    protected override bool UpdateTransforms() => builder.UpdateTransforms();
  }
}