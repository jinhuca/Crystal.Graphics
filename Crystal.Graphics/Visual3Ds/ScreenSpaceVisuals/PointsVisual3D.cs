namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that contains a set of points. The size of the points is defined in screen space.
  /// </summary>
  public class PointsVisual3D : ScreenSpaceVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Size"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        "Size", typeof(double), typeof(PointsVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// The builder.
    /// </summary>
    private readonly PointGeometryBuilder builder;

    /// <summary>
    /// Initializes a new instance of the <see cref = "PointsVisual3D" /> class.
    /// </summary>
    public PointsVisual3D()
    {
      builder = new PointGeometryBuilder(this);
    }

    /// <summary>
    /// Gets or sets the size of the points.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public double Size
    {
      get => (double)GetValue(SizeProperty);

      set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Updates the geometry.
    /// </summary>
    protected override void UpdateGeometry()
    {
      Mesh.Positions = null;
      if(Points == null)
      {
        return;
      }

      var n = Points.Count;
      if(n > 0)
      {
        if(Mesh.TriangleIndices.Count != n * 6)
        {
          Mesh.TriangleIndices = builder.CreateIndices(n);
        }

        Mesh.Positions = builder.CreatePositions(Points, Size, DepthOffset);
      }
    }

    /// <summary>
    /// Updates the transforms.
    /// </summary>
    /// <returns>
    /// True if the transform is updated.
    /// </returns>
    protected override bool UpdateTransforms()
    {
      return builder.UpdateTransforms();
    }

  }
}