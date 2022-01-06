namespace Crystal.Graphics
{
  /// <summary>
  /// Defines the type of material.
  /// </summary>
  public enum MaterialType
  {
    /// <summary>
    /// A diffuse material.
    /// </summary>
    Diffuse,

    /// <summary>
    /// An emissive material.
    /// </summary>
    Emissive
  }

  /// <summary>
  /// A visual element that contains a text billboard.
  /// </summary>
  public class BillboardTextVisual3D : BillboardVisual3D, IBoundsIgnoredVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Background"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
      nameof(Background), typeof(Brush), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
      nameof(BorderBrush), typeof(Brush), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
      nameof(BorderThickness), typeof(Thickness), typeof(BillboardTextVisual3D), new UIPropertyMetadata(new Thickness(1), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
      nameof(FontFamily), typeof(FontFamily), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
      nameof(FontSize), typeof(double), typeof(BillboardTextVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
      nameof(FontWeight), typeof(FontWeight), typeof(BillboardTextVisual3D), new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
      nameof(Foreground), typeof(Brush), typeof(BillboardTextVisual3D), new UIPropertyMetadata(Brushes.Black, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="HeightFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightFactorProperty = DependencyProperty.Register(
      nameof(HeightFactor), typeof(double), typeof(BillboardTextVisual3D), new PropertyMetadata(1.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
      nameof(Padding), typeof(Thickness), typeof(BillboardTextVisual3D), new UIPropertyMetadata(new Thickness(0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      nameof(Text), typeof(string), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="MaterialType"/> dependency property.
    /// </summary>        
    public static readonly DependencyProperty MaterialTypeProperty = DependencyProperty.Register(
      nameof(MaterialType), typeof(MaterialType), typeof(BillboardTextVisual3D), new PropertyMetadata(MaterialType.Diffuse, VisualChanged));


    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>The background.</value>
    public Brush Background
    {
      get => (Brush)GetValue(BackgroundProperty);
      set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the border brush.
    /// </summary>
    /// <value>The border brush.</value>
    public Brush BorderBrush
    {
      get => (Brush)GetValue(BorderBrushProperty);
      set => SetValue(BorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the border thickness.
    /// </summary>
    /// <value>The border thickness.</value>
    public Thickness BorderThickness
    {
      get => (Thickness)GetValue(BorderThicknessProperty);
      set => SetValue(BorderThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    /// <value>The font family.</value>
    public FontFamily FontFamily
    {
      get => (FontFamily)GetValue(FontFamilyProperty);
      set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the font.
    /// </summary>
    /// <value>The size of the font.</value>
    public double FontSize
    {
      get => (double)GetValue(FontSizeProperty);
      set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the font weight.
    /// </summary>
    /// <value>The font weight.</value>
    public FontWeight FontWeight
    {
      get => (FontWeight)GetValue(FontWeightProperty);
      set => SetValue(FontWeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush.
    /// </summary>
    /// <value>The foreground.</value>
    public Brush Foreground
    {
      get => (Brush)GetValue(ForegroundProperty);
      set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the height factor.
    /// </summary>
    /// <value>
    /// The height factor.
    /// </value>
    public double HeightFactor
    {
      get => (double)GetValue(HeightFactorProperty);
      set => SetValue(HeightFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of the material.
    /// </summary>
    /// <value>The type of the material.</value>
    public MaterialType MaterialType
    {
      get => (MaterialType)GetValue(MaterialTypeProperty);
      set => SetValue(MaterialTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>The padding.</value>
    public Thickness Padding
    {
      get => (Thickness)GetValue(PaddingProperty);
      set => SetValue(PaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get => (string)GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The visual appearance changed.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BillboardTextVisual3D)d).VisualChanged();
    }

    /// <summary>
    /// Updates the text block when the visual appearance changed.
    /// </summary>
    private void VisualChanged()
    {
      if(string.IsNullOrEmpty(Text))
      {
        Material = null;
        return;
      }

      var textBlock = new TextBlock(new Run(Text))
      {
        Foreground = Foreground,
        Background = Background,
        FontWeight = FontWeight,
        Padding = Padding
      };

      if(FontFamily != null)
      {
        textBlock.FontFamily = FontFamily;
      }

      if(FontSize > 0)
      {
        textBlock.FontSize = FontSize;
      }

      var element = BorderBrush != null
        ? (FrameworkElement)new Border { BorderBrush = BorderBrush, BorderThickness = BorderThickness, Child = textBlock }
        : textBlock;

      element.Measure(new Size(1000, 1000));
      element.Arrange(new Rect(element.DesiredSize));

      var rtb = new RenderTargetBitmap((int)element.ActualWidth + 1, (int)element.ActualHeight + 1, 96, 96, PixelFormats.Pbgra32);
      rtb.Render(element);

      var brush = new ImageBrush(rtb);

      Material = MaterialType switch
      {
        MaterialType.Diffuse => new DiffuseMaterial(brush),
        MaterialType.Emissive => new EmissiveMaterial(brush),
        _ => Material
      };

      Width = element.ActualWidth;
      Height = element.ActualHeight * HeightFactor;
    }
  }
}