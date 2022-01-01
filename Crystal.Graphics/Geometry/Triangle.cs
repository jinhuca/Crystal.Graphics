namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a triangle in two-dimensional space.
  /// </summary>
  public class Triangle
  {
    /// <summary>
    /// The first point of the triangle.
    /// </summary>
    private readonly Point p1;

    /// <summary>
    /// The second point of the triangle.
    /// </summary>
    private readonly Point p2;

    /// <summary>
    /// The third point of the triangle.
    /// </summary>
    private readonly Point p3;

    /// <summary>
    /// Initializes a new instance of the <see cref="Triangle"/> class.
    /// </summary>
    /// <param name="a">The first point of the triangle.</param>
    /// <param name="b">The second point of the triangle.</param>
    /// <param name="c">The third point of the triangle.</param>
    public Triangle(Point a, Point b, Point c)
    {
      p1 = a;
      p2 = b;
      p3 = c;
    }

    /// <summary>
    /// Gets the first point of the triangle.
    /// </summary>
    /// <value>The point.</value>
    public Point P1 => p1;

    /// <summary>
    /// Gets the second point of the triangle.
    /// </summary>
    /// <value>The point.</value>
    public Point P2 => p2;

    /// <summary>
    /// Gets the third point of the triangle.
    /// </summary>
    /// <value>The point.</value>
    public Point P3 => p3;

    /// <summary>
    /// Checks whether the specified rectangle is completely inside the current triangle.
    /// </summary>
    /// <param name="rect">The rectangle</param>
    /// <returns>
    /// <c>true</c> if the specified rectangle is inside the current triangle; otherwise <c>false</c>.
    /// </returns>
    public bool IsCompletelyInside(Rect rect)
    {
      return rect.Contains(p2) && rect.Contains(p3) && rect.Contains(P3);
    }

    /// <summary>
    /// Checks whether the specified rectangle is completely inside the current triangle.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <returns>
    /// <c>true</c> if the specified rectangle is inside the current triangle; otherwise <c>false</c>.
    /// </returns>
    public bool IsRectCompletelyInside(Rect rect)
    {
      return IsPointInside(rect.TopLeft) && IsPointInside(rect.TopRight)
             && IsPointInside(rect.BottomLeft) && IsPointInside(rect.BottomRight);
    }

    /// <summary>
    /// Checks whether the specified point is inside the triangle. 
    /// </summary>
    /// <param name="p">The point to be checked.</param>
    /// <returns>
    /// <c>true</c> if the specified point is inside the current triangle; otherwise <c>false</c>.
    /// </returns>
    public bool IsPointInside(Point p)
    {
      // http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
      var s = (p1.Y * p3.X) - (p1.X * p3.Y) + ((p3.Y - p1.Y) * p.X) + ((p1.X - p3.X) * p.Y);
      var t = (p1.X * p2.Y) - (p1.Y * p2.X) + ((p1.Y - p2.Y) * p.X) + ((p2.X - p1.X) * p.Y);

      if((s < 0) != (t < 0))
      {
        return false;
      }

      var a = (-p2.Y * p3.X) + (p1.Y * (p3.X - p2.X)) + (p1.X * (p2.Y - p3.Y)) + (p2.X * p3.Y);
      if(a < 0.0)
      {
        s = -s;
        t = -t;
        a = -a;
      }

      return s > 0 && t > 0 && (s + t) < a;
    }

    /// <summary>
    /// Indicates whether the specified rectangle intersects with the current triangle.
    /// </summary>
    /// <param name="rect">The rectangle to check.</param>
    /// <returns>
    /// <c>true</c> if the specified rectangle intersects with the current triangle; otherwise <c>false</c>.
    /// </returns>
    public bool IntersectsWith(Rect rect)
    {
      return LineSegment.AreLineSegmentsIntersecting(p1, p2, rect.BottomLeft, rect.BottomRight)
             || LineSegment.AreLineSegmentsIntersecting(p1, p2, rect.BottomLeft, rect.TopLeft)
             || LineSegment.AreLineSegmentsIntersecting(p1, p2, rect.TopLeft, rect.TopRight)
             || LineSegment.AreLineSegmentsIntersecting(p1, p2, rect.TopRight, rect.BottomRight)
             || LineSegment.AreLineSegmentsIntersecting(p2, p3, rect.BottomLeft, rect.BottomRight)
             || LineSegment.AreLineSegmentsIntersecting(p2, p3, rect.BottomLeft, rect.TopLeft)
             || LineSegment.AreLineSegmentsIntersecting(p2, p3, rect.TopLeft, rect.TopRight)
             || LineSegment.AreLineSegmentsIntersecting(p2, p3, rect.TopRight, rect.BottomRight)
             || LineSegment.AreLineSegmentsIntersecting(p3, p1, rect.BottomLeft, rect.BottomRight)
             || LineSegment.AreLineSegmentsIntersecting(p3, p1, rect.BottomLeft, rect.TopLeft)
             || LineSegment.AreLineSegmentsIntersecting(p3, p1, rect.TopLeft, rect.TopRight)
             || LineSegment.AreLineSegmentsIntersecting(p3, p1, rect.TopRight, rect.BottomRight);
    }
  }
}