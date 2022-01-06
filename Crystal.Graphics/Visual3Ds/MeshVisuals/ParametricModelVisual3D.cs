namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a base class for parametric surfaces evaluated on a rectangular mesh.
  /// </summary>
  /// <remarks>
  /// Override the Evaluate method to define the points.
  /// </remarks>
  public abstract class ParametricModelVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the mesh size in u-direction.
    /// </summary>
    /// <value>The mesh size U.</value>
    public int MeshSizeU
    {
      get => (int)GetValue(MeshSizeUProperty);
      set => SetValue(MeshSizeUProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MeshSizeU"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MeshSizeUProperty = DependencyProperty.Register(
      nameof(MeshSizeU), typeof(int), typeof(ParametricModelVisual3D), new UIPropertyMetadata(120, GeometryChanged));

    /// <summary>
    /// Gets or sets the mesh size in v-direction.
    /// </summary>
    /// <value>The mesh size V.</value>
    public int MeshSizeV
    {
      get => (int)GetValue(MeshSizeVProperty);
      set => SetValue(MeshSizeVProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MeshSizeV"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MeshSizeVProperty = DependencyProperty.Register(
      nameof(MeshSizeV), typeof(int), typeof(ParametricModelVisual3D), new UIPropertyMetadata(120, GeometryChanged));

    /// <summary>
    /// Evaluates the surface at the specified u,v parameters.
    /// </summary>
    /// <param name="u">
    /// The u parameter.
    /// </param>
    /// <param name="v">
    /// The v parameter.
    /// </param>
    /// <param name="textureCoord">
    /// The texture coordinates.
    /// </param>
    /// <returns>
    /// The evaluated <see cref="Point3D"/>.
    /// </returns>
    protected abstract Point3D Evaluate(double u, double v, out Point textureCoord);

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var mesh = new MeshGeometry3D();

      var n = MeshSizeU;
      var m = MeshSizeV;
      var p = new Point3D[m * n];
      var tc = new Point[m * n];

      // todo: use MeshBuilder

      // todo: parallel execution...
      // Parallel.For(0, n, (i) =>
      for(var i = 0; i < n; i++)
      {
        var u = 1.0 * i / (n - 1);

        for(var j = 0; j < m; j++)
        {
          var v = 1.0 * j / (m - 1);
          var ij = (i * m) + j;
          p[ij] = Evaluate(u, v, out tc[ij]);
        }
      }

      // );
      var idx = 0;
      for(var i = 0; i < n; i++)
      {
        for(var j = 0; j < m; j++)
        {
          mesh.Positions.Add(p[idx]);
          mesh.TextureCoordinates.Add(tc[idx]);
          idx++;
        }
      }

      for(var i = 0; i + 1 < n; i++)
      {
        for(var j = 0; j + 1 < m; j++)
        {
          var x0 = i * m;
          var x1 = (i + 1) * m;
          var y0 = j;
          var y1 = j + 1;
          AddTriangle(mesh, x0 + y0, x0 + y1, x1 + y0);
          AddTriangle(mesh, x1 + y0, x0 + y1, x1 + y1);
        }
      }

      return mesh;
    }

    /// <summary>
    /// The add triangle.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    /// <param name="i1">
    /// The i 1.
    /// </param>
    /// <param name="i2">
    /// The i 2.
    /// </param>
    /// <param name="i3">
    /// The i 3.
    /// </param>
    private static void AddTriangle(MeshGeometry3D? mesh, int i1, int i2, int i3)
    {
      if (mesh != null)
      {
        var p1 = mesh.Positions[i1];
        if(!IsDefined(p1))
        {
          return;
        }
      }

      if (mesh != null)
      {
        var p2 = mesh.Positions[i2];
        if(!IsDefined(p2))
        {
          return;
        }
      }

      if (mesh != null)
      {
        var p3 = mesh.Positions[i3];
        if(!IsDefined(p3))
        {
          return;
        }
      }

      mesh?.TriangleIndices.Add(i1);
      mesh?.TriangleIndices.Add(i2);
      mesh?.TriangleIndices.Add(i3);
    }

    /// <summary>
    /// Determines whether the specified point is defined.
    /// </summary>
    /// <param name="point">
    /// The point.
    /// </param>
    /// <returns>
    /// The is defined.
    /// </returns>
    private static bool IsDefined(Point3D point)
    {
      return !double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z);
    }
  }
}