namespace Crystal.Graphics
{
  /// <summary>
  /// Builds a mesh geometry for a collection of points.
  /// </summary>
  public class PointGeometryBuilder : ScreenGeometryBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PointGeometryBuilder"/> class.
    /// </summary>
    /// <param name="visual">
    /// The visual.
    /// </param>
    public PointGeometryBuilder(Visual3D visual) : base(visual)
    {
    }

    /// <summary>
    /// Creates the triangle indices.
    /// </summary>
    /// <param name="n">
    /// The number of points.
    /// </param>
    /// <returns>
    /// The triangle indices.
    /// </returns>
    public Int32Collection CreateIndices(int n)
    {
      var indices = new Int32Collection(n * 6);

      for(var i = 0; i < n; i++)
      {
        indices.Add(i * 4 + 2);
        indices.Add(i * 4 + 1);
        indices.Add(i * 4 + 0);

        indices.Add(i * 4 + 2);
        indices.Add(i * 4 + 3);
        indices.Add(i * 4 + 1);
      }

      indices.Freeze();
      return indices;
    }

    /// <summary>
    /// Creates the positions for the specified points.
    /// </summary>
    /// <param name="points">
    /// The points.
    /// </param>
    /// <param name="size">
    /// The size of the points.
    /// </param>
    /// <param name="depthOffset">
    /// The depth offset. A positive number (e.g. 0.0001) moves the point towards the camera.
    /// </param>
    /// <returns>
    /// The positions collection.
    /// </returns>
    public Point3DCollection CreatePositions(IList<Point3D>? points, double size = 1.0, double depthOffset = 0.0)
    {
      var halfSize = size / 2.0;
      var numPoints = points.Count;

      var outline = new[] {
        new Vector(-halfSize, halfSize), new Vector(-halfSize, -halfSize), new Vector(halfSize, halfSize),
        new Vector(halfSize, -halfSize)
      };

      var positions = new Point3DCollection(numPoints * 4);
      for(var i = 0; i < numPoints; i++)
      {
        var screenPoint = (Point4D)points[i] * visualToScreen;
        var spx = screenPoint.X;
        var spy = screenPoint.Y;
        var spz = screenPoint.Z;
        var spw = screenPoint.W;

        if(!depthOffset.Equals(0))
        {
          spz -= depthOffset * spw;
        }

        var p0 = new Point4D(spx, spy, spz, spw) * screenToVisual;
        var pwinverse = 1 / p0.W;

        foreach(var v in outline)
        {
          var p = new Point4D(spx + v.X * spw, spy + v.Y * spw, spz, spw) * screenToVisual;
          positions.Add(new Point3D(p.X * pwinverse, p.Y * pwinverse, p.Z * pwinverse));
        }
      }

      positions.Freeze();
      return positions;
    }
  }
}