namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a color axis for categories.
  /// </summary>
  public class CategorizedColorAxis : ColorAxis
  {
    /// <summary>
    /// Identifies the <see cref="Categories"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register(
      nameof(Categories),
      typeof(IList<string>),
      typeof(CategorizedColorAxis),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, PropertyChanged));

    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    /// <value>The categories.</value>
    public IList<string> Categories
    {
      get => (IList<string>)GetValue(CategoriesProperty);
      set => SetValue(CategoriesProperty, value);
    }

    /// <summary>
    /// Updates the visuals.
    /// </summary>
    protected override void AddVisuals()
    {
      if(Categories == null || Categories.Count == 0 || ColorScheme == null)
      {
        return;
      }

      base.AddVisuals();

      for(int i = 0; i < Categories.Count; i++)
      {
        var text = Categories[i];
        var tb = new TextBlock(new Run(text)) { Foreground = Foreground };
        tb.Measure(new Size(ActualWidth, ActualHeight));

        double y = ColorArea.Top + (((double)i / Categories.Count) * ColorArea.Height);
        double y1 = ColorArea.Top + (((i + 0.5) / Categories.Count) * ColorArea.Height);
        double y2 = ColorArea.Top + (((i + 1.0) / Categories.Count) * ColorArea.Height);

        Point p0, p1, p2, p3, p4;
        switch(Position)
        {
          case ColorAxisPosition.Right:
            p0 = new Point(ColorArea.Right, y);
            p1 = new Point(ColorArea.Left - TickLength, y);
            p2 = new Point(ColorArea.Left - TickLength - TextMargin - tb.DesiredSize.Width, y1 - (tb.DesiredSize.Height / 2));
            p3 = new Point(ColorArea.Right, y2);
            p4 = new Point(ColorArea.Left - TickLength, y2);
            break;
          default:
            p0 = new Point(ColorArea.Left, y);
            p1 = new Point(ColorArea.Right + TickLength, y);
            p2 = new Point(ColorArea.Right + TickLength + TextMargin, y1 - (tb.DesiredSize.Height / 2));
            p3 = new Point(ColorArea.Left, y2);
            p4 = new Point(ColorArea.Right + TickLength, y2);
            break;
        }

        var l = new System.Windows.Shapes.Line
        {
          X1 = p0.X,
          X2 = p1.X,
          Y1 = p0.Y,
          Y2 = p1.Y,
          Stroke = Foreground,
          StrokeThickness = 1,
          SnapsToDevicePixels = true
        };

        Canvas.Children.Add(l);
        if(i == Categories.Count - 1)
        {
          var l2 = new System.Windows.Shapes.Line
          {
            X1 = p3.X,
            X2 = p4.X,
            Y1 = p3.Y,
            Y2 = p4.Y,
            Stroke = BorderBrush,
            StrokeThickness = 1,
            SnapsToDevicePixels = true
          };
          Canvas.Children.Add(l2);
        }

        Canvas.SetLeft(tb, p2.X);
        Canvas.SetTop(tb, p2.Y);
        Canvas.Children.Add(tb);
      }
    }

    /// <summary>
    /// Gets the tick labels.
    /// </summary>
    /// <returns>
    /// The labels.
    /// </returns>
    protected override IEnumerable<string> GetTickLabels()
    {
      if(Categories == null)
      {
        return new[] { string.Empty };
      }

      return Categories;
    }
  }
}