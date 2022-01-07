namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a 3D polygon.
  /// </summary>
  public class Polygon3D
  {
    /// <summary>
    /// Initializes a new instance of the <see cref = "Polygon3D" /> class.
    /// </summary>
    public Polygon3D()
    {
      Points = new List<Point3D>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Polygon3D"/> class.
    /// </summary>
    /// <param name="pts">
    /// The PTS.
    /// </param>
    public Polygon3D(IList<Point3D> pts)
    {
      Points = pts;
    }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    /// <value>The points.</value>
    public IList<Point3D> Points { get; set; }

    //// http://en.wikipedia.org/wiki/Polygon_triangulation
    //// http://en.wikipedia.org/wiki/Monotone_polygon
    //// http://www.codeproject.com/KB/recipes/hgrd.aspx LGPL
    //// http://www.springerlink.com/content/g805787811vr1v9v/

    /// <summary>
    /// Flattens this polygon.
    /// </summary>
    /// <returns>
    /// The 2D polygon.
    /// </returns>
    public Polygon Flatten()
    {
      // http://forums.xna.com/forums/p/16529/86802.aspx
      // http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
      var up = GetNormal();
      up.Normalize();
      var right = Vector3D.CrossProduct(

                up, Math.Abs(up.X) > Math.Abs(up.Z) ? new Vector3D(0, 0, 1) : new Vector3D(1, 0, 0));
      var backward = Vector3D.CrossProduct(
                right, up);
      var m = new Matrix3D(backward.X, right.X, up.X, 0, backward.Y, right.Y, up.Y, 0, backward.Z, right.Z, up.Z, 0, 0, 0, 0, 1);

      // make first point origin
      var offs = m.Transform(Points[0]);
      m.OffsetX = -offs.X;
      m.OffsetY = -offs.Y;

      var polygon = new Polygon { Points = new PointCollection(Points.Count) };
      foreach(var p in Points)
      {
        var pp = m.Transform(p);
        polygon.Points.Add(new Point(pp.X, pp.Y));
      }

      return polygon;
    }

    /// <summary>
    /// Gets the normal of the polygon.
    /// </summary>
    /// <returns>
    /// The normal.
    /// </returns>
    public Vector3D GetNormal()
    {
      if(Points.Count < 3)
      {
        throw new InvalidOperationException("At least three points required in the polygon to find a normal.");
      }

      var v1 = Points[1] - Points[0];
      for(var i = 2; i < Points.Count; i++)
      {
        var n = Vector3D.CrossProduct(v1, Points[i] - Points[0]);

        if(n.LengthSquared > 1e-10)
        {
          n.Normalize();
          return n;
        }
      }

      var result = Vector3D.CrossProduct(v1, Points[2] - Points[0]);
      result.Normalize();
      return result;
    }

    /// <summary>
    /// Determines whether this polygon is planar.
    /// </summary>
    /// <returns>
    /// The is planar.
    /// </returns>
    public bool IsPlanar()
    {
      var v1 = Points[1] - Points[0];
      var normal = new Vector3D();
      for(var i = 2; i < Points.Count; i++)
      {
        var n = Vector3D.CrossProduct(v1, Points[i] - Points[0]);
        n.Normalize();
        if(i == 2)
        {
          normal = n;
        }
        else if(Math.Abs(Vector3D.DotProduct(n, normal) - 1) > 1e-8)
        {
          return false;
        }
      }

      return true;
    }
  }
}