using System.Windows.Shapes;

namespace Crystal.Graphics
{
  /// <summary>
  /// The base class for color axes.
  /// </summary>
  [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
  public abstract class ColorAxis : Control
  {
    /// <summary>
    /// Identifies the <see cref="BarWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register(
      nameof(BarWidth), typeof(double), typeof(ColorAxis), new UIPropertyMetadata(20.0));

    /// <summary>
    /// Identifies the <see cref="ColorScheme"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorSchemeProperty = DependencyProperty.Register(
      nameof(ColorScheme), typeof(Brush), typeof(ColorAxis), new UIPropertyMetadata(null, PropertyChanged));

    /// <summary>
    /// Identifies the <see cref="FlipColorScheme"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FlipColorSchemeProperty = DependencyProperty.Register(
      nameof(FlipColorScheme), typeof(bool), typeof(ColorAxis), new UIPropertyMetadata(false, PropertyChanged));

    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
      nameof(Position), typeof(ColorAxisPosition), typeof(ColorAxis), new UIPropertyMetadata(ColorAxisPosition.Left));

    /// <summary>
    /// Identifies the <see cref="TextMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(
      nameof(TextMargin), typeof(double), typeof(ColorAxis), new UIPropertyMetadata(2.0));

    /// <summary>
    /// Identifies the <see cref="TickLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TickLengthProperty = DependencyProperty.Register(
      nameof(TickLength), typeof(double), typeof(ColorAxis), new UIPropertyMetadata(3.0));

    /// <summary>
    /// Initializes static members of the <see cref="ColorAxis" /> class.
    /// </summary>
    static ColorAxis()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorAxis), new FrameworkPropertyMetadata(typeof(ColorAxis)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorAxis" /> class.
    /// </summary>
    protected ColorAxis()
    {
      SizeChanged += (_, _) => UpdateVisuals();
      Loaded += (_, _) => UpdateVisuals();
    }

    /// <summary>
    /// Gets or sets the width of the color bar rectangle.
    /// </summary>
    /// <value>The width.</value>
    public double BarWidth
    {
      get => (double)GetValue(BarWidthProperty);
      set => SetValue(BarWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the color scheme.
    /// </summary>
    /// <value>The color scheme.</value>
    public Brush ColorScheme
    {
      get => (Brush)GetValue(ColorSchemeProperty);
      set => SetValue(ColorSchemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color scheme direction, if true inverts the color normal color brush direction.
    /// </summary>
    /// <value>A boolean indicating inverted color direction when true.</value>
    public bool FlipColorScheme
    {
      get => (bool)GetValue(FlipColorSchemeProperty);
      set => SetValue(FlipColorSchemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public ColorAxisPosition Position
    {
      get => (ColorAxisPosition)GetValue(PositionProperty);
      set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the text margin.
    /// </summary>
    /// <value>The text margin.</value>
    public double TextMargin
    {
      get => (double)GetValue(TextMarginProperty);
      set => SetValue(TextMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the length of the tick.
    /// </summary>
    /// <value>The length of the tick.</value>
    public double TickLength
    {
      get => (double)GetValue(TickLengthProperty);
      set => SetValue(TickLengthProperty, value);
    }

    /// <summary>
    /// Gets the canvas.
    /// </summary>
    protected Canvas? Canvas { get; private set; }

    /// <summary>
    /// Gets the color rectangle area.
    /// </summary>
    protected Rect ColorArea { get; private set; }

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call
    ///  <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      Canvas = (Canvas)GetTemplateChild("PART_Canvas");
    }

    /// <summary>
    /// Handles changes in properties.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ColorAxis)d).UpdateVisuals();
    }

    /// <summary>
    /// Adds the visuals.
    /// </summary>
    protected virtual void AddVisuals()
    {
      switch(Position)
      {
        case ColorAxisPosition.Left:
          ColorArea = new Rect(
              Padding.Left,
              Padding.Top,
              BarWidth,
              Math.Max(0, ActualHeight - Padding.Bottom - Padding.Top));
          break;
        case ColorAxisPosition.Right:
          ColorArea = new Rect(
              Math.Max(0, ActualWidth - Padding.Right - BarWidth),
              Padding.Top,
              BarWidth,
              Math.Max(0, ActualHeight - Padding.Bottom - Padding.Top));
          break;
      }

      var r = new Rectangle
      {
        Fill = ColorScheme,
        Width = ColorArea.Width,
        Height = ColorArea.Height
      };

      if(FlipColorScheme)
      {
        r.LayoutTransform = new RotateTransform(180);
      }

      Canvas.SetLeft(r, ColorArea.Left);
      Canvas.SetTop(r, ColorArea.Top);

      Canvas.Children.Add(r);

      Canvas.Children.Add(
        new Line
        {
          Stroke = Foreground,
          StrokeThickness = 1,
          SnapsToDevicePixels = true,
          X1 = ColorArea.Left,
          Y1 = ColorArea.Top,
          X2 = ColorArea.Left,
          Y2 = ColorArea.Bottom
        });
      Canvas.Children.Add(
        new Line
        {
          Stroke = Foreground,
          StrokeThickness = 1,
          SnapsToDevicePixels = true,
          X1 = ColorArea.Right,
          Y1 = ColorArea.Top,
          X2 = ColorArea.Right,
          Y2 = ColorArea.Bottom
        });
    }

    /// <summary>
    /// Gets the tick labels.
    /// </summary>
    /// <returns>The labels.</returns>
    protected abstract IEnumerable<string> GetTickLabels();

    /// <summary>
    /// Measures the child elements of a <see cref="T:System.Windows.Controls.Canvas"/> in anticipation of arranging them during the
    ///     <see cref="M:System.Windows.Controls.Canvas.ArrangeOverride(System.Windows.Size)"/>
    /// pass.
    /// </summary>
    /// <param name="constraint">
    /// An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.
    /// </param>
    /// <returns>
    /// A <see cref="T:System.Windows.Size"/> that represents the size that is required to arrange child content.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
      var size = base.MeasureOverride(constraint);

      var maxWidth = GetTickLabels().Max(
        c =>
        {
          var tb = new TextBlock(new Run(c));
          tb.Measure(constraint);
          return tb.DesiredSize.Width;
        });
      size.Width = maxWidth + BarWidth + TickLength + Padding.Left + Padding.Right + TextMargin;
      return size;
    }

    /// <summary>
    /// Updates the visuals.
    /// </summary>
    protected void UpdateVisuals()
    {
      if(Canvas == null)
      {
        return;
      }

      Canvas.Children.Clear();
      AddVisuals();
    }
  }
}