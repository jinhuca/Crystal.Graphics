namespace Crystal.Graphics
{
  /// <summary>
  /// Provides line clipping against a rectangle.
  /// </summary>
  public class CohenSutherlandClipping
  {
    // http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland
    /// <summary>
    /// The bottom.
    /// </summary>
    private const int BOTTOM = 4; // 0100

    /// <summary>
    /// The inside.
    /// </summary>
    private const int INSIDE = 0; // 0000

    /// <summary>
    /// The left.
    /// </summary>
    private const int LEFT = 1; // 0001

    /// <summary>
    /// The right.
    /// </summary>
    private const int RIGHT = 2; // 0010

    /// <summary>
    /// The top.
    /// </summary>
    private const int TOP = 8; // 1000

    /// <summary>
    /// The xmax.
    /// </summary>
    private readonly double xmax;

    /// <summary>
    /// The xmin.
    /// </summary>
    private readonly double xmin;

    /// <summary>
    /// The ymax.
    /// </summary>
    private readonly double ymax;

    /// <summary>
    /// The ymin.
    /// </summary>
    private readonly double ymin;

    /// <summary>
    /// Initializes a new instance of the <see cref="CohenSutherlandClipping"/> class.
    /// </summary>
    /// <param name="xmin">
    /// The xmin.
    /// </param>
    /// <param name="xmax">
    /// The xmax.
    /// </param>
    /// <param name="ymin">
    /// The ymin.
    /// </param>
    /// <param name="ymax">
    /// The ymax.
    /// </param>
    public CohenSutherlandClipping(double xmin, double xmax, double ymin, double ymax)
    {
      this.xmin = xmin;
      this.ymin = ymin;
      this.xmax = xmax;
      this.ymax = ymax;
    }

    // Compute the bit code for a point (x, y) using the clip rectangle
    // bounded diagonally by (xmin, ymin), and (xmax, ymax)
    /// <summary>
    /// Cohen–Sutherland clipping algorithm clips a line against a rectangle.
    /// </summary>
    /// <param name="x0">
    /// </param>
    /// <param name="y0">
    /// </param>
    /// <param name="x1">
    /// </param>
    /// <param name="y1">
    /// </param>
    /// <returns>
    /// true if the line is inside
    /// </returns>
    public bool ClipLine(ref double x0, ref double y0, ref double x1, ref double y1)
    {
      // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
      var outcode0 = ComputeOutCode(x0, y0);
      var outcode1 = ComputeOutCode(x1, y1);
      var accept = false;

      while(true)
      {
        if((outcode0 | outcode1) == 0)
        {
          // logical or is 0. Trivially accept and get out of loop
          accept = true;
          break;
        }

        if((outcode0 & outcode1) != 0)
        {
          // logical and is not 0. Trivially reject and get out of loop
          break;
        }

        // failed both tests, so calculate the line segment to clip
        // from an outside point to an intersection with clip edge
        double x = 0, y = 0;

        // At least one endpoint is outside the clip rectangle; pick it.
        var outcodeOut = outcode0 != 0 ? outcode0 : outcode1;

        // Now find the intersection point;
        // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
        if((outcodeOut & TOP) != 0)
        {
          // point is above the clip rectangle
          x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
          y = ymax;
        }
        else if((outcodeOut & BOTTOM) != 0)
        {
          // point is below the clip rectangle
          x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
          y = ymin;
        }
        else if((outcodeOut & RIGHT) != 0)
        {
          // point is to the right of clip rectangle
          y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
          x = xmax;
        }
        else if((outcodeOut & LEFT) != 0)
        {
          // point is to the left of clip rectangle
          y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
          x = xmin;
        }

        // Now we move outside point to intersection point to clip
        // and get ready for next pass.
        if(outcodeOut == outcode0)
        {
          x0 = x;
          y0 = y;
          outcode0 = ComputeOutCode(x0, y0);
        }
        else
        {
          x1 = x;
          y1 = y;
          outcode1 = ComputeOutCode(x1, y1);
        }
      }

      return accept;
    }

    /// <summary>
    /// The is inside.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The is inside.
    /// </returns>
    public bool IsInside(double x, double y)
    {
      return ComputeOutCode(x, y) == INSIDE;
    }

    /// <summary>
    /// The compute out code.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The compute out code.
    /// </returns>
    private int ComputeOutCode(double x, double y)
    {
      var code = INSIDE; // initialized as being inside of clip window

      if(x < xmin)
      {
        // to the left of clip window
        code |= LEFT;
      }
      else if(x > xmax)
      {
        // to the right of clip window
        code |= RIGHT;
      }

      if(y < ymin)
      {
        // below the clip window
        code |= BOTTOM;
      }
      else if(y > ymax)
      {
        // above the clip window
        code |= TOP;
      }

      return code;
    }

  }
}