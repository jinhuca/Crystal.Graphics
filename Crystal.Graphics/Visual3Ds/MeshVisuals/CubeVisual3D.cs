namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that displays a cube.
  /// </summary>
  public class CubeVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the center of the cube.
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
      nameof(Center), typeof(Point3D), typeof(CubeVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

    /// <summary>
    /// Gets or sets the length of the cube sides.
    /// </summary>
    /// <value>The length of the sides.</value>
    public double SideLength
    {
      get => (double)GetValue(SideLengthProperty);
      set => SetValue(SideLengthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SideLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SideLengthProperty = DependencyProperty.Register(
      nameof(SideLength), typeof(double), typeof(CubeVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>The mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var b = new MeshBuilder(false);
      b.AddCubeFace(Center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), SideLength, SideLength, SideLength);
      b.AddCubeFace(Center, new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), SideLength, SideLength, SideLength);
      b.AddCubeFace(Center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), SideLength, SideLength, SideLength);
      b.AddCubeFace(Center, new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), SideLength, SideLength, SideLength);
      b.AddCubeFace(Center, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), SideLength, SideLength, SideLength);
      b.AddCubeFace(Center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), SideLength, SideLength, SideLength);
      return b.ToMesh();
    }
  }
}