﻿namespace Crystal.Graphics
{
  /// <summary>
  /// An adorner showing a rectangle with a crosshair in the middle. This is shown when zooming a rectangle.
  /// </summary>
  public class RectangleAdorner : Adorner
  {
    /// <summary>
    /// The cross hair size.
    /// </summary>
    private readonly double crossHairSize;

    /// <summary>
    /// The pen.
    /// </summary>
    private readonly Pen pen;

    /// <summary>
    /// The pen 2.
    /// </summary>
    private readonly Pen pen2;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleAdorner"/> class.
    /// </summary>
    /// <param name="adornedElement">
    /// The adorned element.
    /// </param>
    /// <param name="rectangle">
    /// The rectangle.
    /// </param>
    /// <param name="color1">
    /// The color1.
    /// </param>
    /// <param name="color2">
    /// The color2.
    /// </param>
    /// <param name="thickness1">
    /// The thickness1.
    /// </param>
    /// <param name="thickness2">
    /// The thickness2.
    /// </param>
    /// <param name="crossHairSize">
    /// Size of the cross hair.
    /// </param>
    public RectangleAdorner(
        UIElement adornedElement,
        Rect rectangle,
        Color color1,
        Color color2,
        double thickness1 = 1.0,
        double thickness2 = 1.0,
        double crossHairSize = 10)
        : this(adornedElement, rectangle, color1, color2, thickness1, thickness2, crossHairSize, DashStyles.Dash)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleAdorner"/> class.
    /// </summary>
    /// <param name="adornedElement">
    /// The adorned element.
    /// </param>
    /// <param name="rectangle">
    /// The rectangle.
    /// </param>
    /// <param name="color1">
    /// The color1.
    /// </param>
    /// <param name="color2">
    /// The color2.
    /// </param>
    /// <param name="thickness1">
    /// The thickness1.
    /// </param>
    /// <param name="thickness2">
    /// The thickness2.
    /// </param>
    /// <param name="crossHairSize">
    /// Size of the cross hair.
    /// </param>
    /// <param name="dashStyle2">
    /// The dash style2.
    /// </param>
    public RectangleAdorner(
        UIElement adornedElement,
        Rect rectangle,
        Color color1,
        Color color2,
        double thickness1,
        double thickness2,
        double crossHairSize,
        DashStyle dashStyle2)
        : base(adornedElement)
    {
      if(adornedElement == null)
      {
        throw new ArgumentNullException(nameof(adornedElement));
      }

      Rectangle = rectangle;

      // http://www.wpftutorial.net/DrawOnPhysicalDevicePixels.html
      var ps = PresentationSource.FromVisual(adornedElement);
      if(ps == null)
      {
        return;
      }

      var ct = ps.CompositionTarget;
      if(ct == null)
      {
        return;
      }

      var m = ct.TransformToDevice;
      var dpiFactor = 1 / m.M11;

      pen = new Pen(new SolidColorBrush(color1), thickness1 * dpiFactor);
      pen2 = new Pen(new SolidColorBrush(color2), thickness2 * dpiFactor)
      {
        DashStyle = dashStyle2
      };
      this.crossHairSize = crossHairSize;
    }

    /// <summary>
    /// Gets or sets Rectangle.
    /// </summary>
    public Rect Rectangle { get; set; }

    /// <summary>
    /// Called when rendering.
    /// </summary>
    /// <param name="dc">
    /// The dc.
    /// </param>
    protected override void OnRender(DrawingContext dc)
    {
      var halfPenWidth = pen.Thickness / 2;

      var mx = (Rectangle.Left + Rectangle.Right) / 2;
      var my = (Rectangle.Top + Rectangle.Bottom) / 2;
      mx = (int)mx + halfPenWidth;
      my = (int)my + halfPenWidth;

      var rect = new Rect(
          (int)Rectangle.Left + halfPenWidth,
          (int)Rectangle.Top + halfPenWidth,
          (int)Rectangle.Width,
          (int)Rectangle.Height);

      // Create a guidelines set
      /*GuidelineSet guidelines = new GuidelineSet();
      guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
      guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
      guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
      guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);
      guidelines.GuidelinesX.Add(mx + halfPenWidth);
      guidelines.GuidelinesY.Add(my + halfPenWidth);

      dc.PushGuidelineSet(guidelines);*/
      dc.DrawRectangle(null, pen, rect);
      dc.DrawRectangle(null, pen2, rect);

      if(crossHairSize > 0)
      {
        dc.DrawLine(pen, new Point(mx, my - crossHairSize), new Point(mx, my + crossHairSize));
        dc.DrawLine(pen, new Point(mx - crossHairSize, my), new Point(mx + crossHairSize, my));
        dc.DrawLine(pen2, new Point(mx, my - crossHairSize), new Point(mx, my + crossHairSize));
        dc.DrawLine(pen2, new Point(mx - crossHairSize, my), new Point(mx + crossHairSize, my));
      }

      // dc.Pop();
    }
  }
}
