namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a color axis for a numeric value range.
  /// </summary>
  public class RangeColorAxis : ColorAxis
  {
    /// <summary>
    /// Identifies the <see cref="FormatProvider"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FormatProviderProperty = DependencyProperty.Register(
      nameof(FormatProvider), typeof(IFormatProvider), typeof(RangeColorAxis), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="FormatString"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(
      nameof(FormatString), typeof(string), typeof(RangeColorAxis), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Maximum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      nameof(Maximum), typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(100.0));

    /// <summary>
    /// Identifies the <see cref="MaximumTextureCoordinate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumTextureCoordinateProperty = DependencyProperty.Register(
      nameof(MaximumTextureCoordinate), typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="Minimum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      nameof(Minimum), typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="MinimumTextureCoordinate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumTextureCoordinateProperty = DependencyProperty.Register(
      nameof(MinimumTextureCoordinate), typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="Step"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
      nameof(Step), typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(10.0));

    /// <summary>
    /// Gets or sets the format provider.
    /// </summary>
    /// <value>The format provider.</value>
    public IFormatProvider FormatProvider
    {
      get => (IFormatProvider)GetValue(FormatProviderProperty);
      set => SetValue(FormatProviderProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public string FormatString
    {
      get => (string)GetValue(FormatStringProperty);
      set => SetValue(FormatStringProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>The maximum.</value>
    public double Maximum
    {
      get => (double)GetValue(MaximumProperty);
      set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum texture coordinate.
    /// </summary>
    /// <value>The maximum texture coordinate.</value>
    public double MaximumTextureCoordinate
    {
      get => (double)GetValue(MaximumTextureCoordinateProperty);
      set => SetValue(MaximumTextureCoordinateProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>The minimum.</value>
    public double Minimum
    {
      get => (double)GetValue(MinimumProperty);
      set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum texture coordinate.
    /// </summary>
    /// <value>The minimum texture coordinate.</value>
    public double MinimumTextureCoordinate
    {
      get => (double)GetValue(MinimumTextureCoordinateProperty);
      set => SetValue(MinimumTextureCoordinateProperty, value);
    }

    /// <summary>
    /// Gets or sets the step.
    /// </summary>
    /// <value>The step.</value>
    public double Step
    {
      get => (double)GetValue(StepProperty);
      set => SetValue(StepProperty, value);
    }

    /// <summary>
    /// Updates the visuals.
    /// </summary>
    protected override void AddVisuals()
    {
      if(Maximum <= Minimum || Step <= 0 || ColorScheme == null)
      {
        return;
      }

      base.AddVisuals();

      var minY = ColorArea.Bottom - (MinimumTextureCoordinate * ColorArea.Height);
      var maxY = ColorArea.Bottom - (MaximumTextureCoordinate * ColorArea.Height);
      double Transform(double v) => minY + ((v - Minimum) / (Maximum - Minimum) * (maxY - minY));

      var p = double.MinValue;
      var yMax = Transform(Maximum);
      foreach(var v in GetTickValues())
      {
        var text = v.ToString(FormatString, FormatProvider);
        var tb = new TextBlock(new Run(text)) { Foreground = Foreground };
        tb.Measure(new Size(ActualWidth, ActualHeight));
        var y = Transform(v);
        Point p0, p1, p2;
        switch(Position)
        {
          case ColorAxisPosition.Right:
            p0 = new Point(ColorArea.Right, y);
            p1 = new Point(ColorArea.Left - TickLength, y);
            p2 = new Point(ColorArea.Left - TickLength - TextMargin - tb.DesiredSize.Width, y - (tb.DesiredSize.Height / 2));
            break;
          default:
            p0 = new Point(ColorArea.Left, y);
            p1 = new Point(ColorArea.Right + TickLength, y);
            p2 = new Point(ColorArea.Right + TickLength + TextMargin, y - (tb.DesiredSize.Height / 2));
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

        var h = tb.DesiredSize.Height * 0.7;
        if(v < Maximum && Math.Abs(y - yMax) < h)
        {
          continue;
        }

        if(Math.Abs(y - p) < h)
        {
          continue;
        }

        Canvas.SetLeft(tb, p2.X);
        Canvas.SetTop(tb, p2.Y);
        Canvas.Children.Add(tb);
        p = y;
      }
    }

    /// <summary>
    /// Gets the tick labels.
    /// </summary>
    /// <returns>
    /// The labels.
    /// </returns>
    protected override IEnumerable<string> GetTickLabels() => GetTickValues().Select(v => v.ToString(FormatString, FormatProvider));

    /// <summary>
    /// Gets the tick values.
    /// </summary>
    /// <returns>The tick values</returns>
    private IEnumerable<double> GetTickValues()
    {
      yield return Minimum;
      var x = Math.Floor(Minimum / Step) * Step;
      while(x < Maximum)
      {
        if(x > Minimum)
        {
          yield return x;
        }

        x += Step;
      }

      yield return Maximum;
    }
  }
}