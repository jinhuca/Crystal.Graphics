namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows text.
  /// </summary>
  /// <remarks>
  /// Set the Text property last to avoid multiple updates.
  /// </remarks>
  public class TextVisual3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Background"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        "Background", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        "BorderBrush", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty =
        DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(TextVisual3D), new UIPropertyMetadata(new Thickness(1), VisualChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the text should be flipped (mirrored horizontally).
    /// </summary>
    /// <remarks>
    /// This may be useful when using a mirror transform on the text visual.
    /// </remarks>
    /// <value>
    ///   <c>true</c> if text is flipped; otherwise, <c>false</c>.
    /// </value>
    public bool IsFlipped
    {
      get => (bool)GetValue(IsFlippedProperty);
      set => SetValue(IsFlippedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsFlipped"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsFlippedProperty =
        DependencyProperty.Register("IsFlipped", typeof(bool), typeof(TextVisual3D), new PropertyMetadata(false, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
        "FontFamily", typeof(FontFamily), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        "FontSize", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        "FontWeight", typeof(FontWeight), typeof(TextVisual3D), new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        "Foreground", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(Brushes.Black, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
        "Height", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(11.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="HorizontalAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.Register(
            "HorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(TextVisual3D),
            new UIPropertyMetadata(HorizontalAlignment.Center, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="IsDoubleSided"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsDoubleSidedProperty = DependencyProperty.Register(
        "IsDoubleSided", typeof(bool), typeof(TextVisual3D), new UIPropertyMetadata(true, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        "Padding", typeof(Thickness), typeof(TextVisual3D), new UIPropertyMetadata(new Thickness(0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        "Position",
        typeof(Point3D),
        typeof(TextVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="TextDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextDirectionProperty = DependencyProperty.Register(
        "TextDirection",
        typeof(Vector3D),
        typeof(TextVisual3D),
        new UIPropertyMetadata(new Vector3D(1, 0, 0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="UpDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
        "UpDirection",
        typeof(Vector3D),
        typeof(TextVisual3D),
        new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="VerticalAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.Register(
            "VerticalAlignment",
            typeof(VerticalAlignment),
            typeof(TextVisual3D),
            new UIPropertyMetadata(VerticalAlignment.Center, VisualChanged));

    /// <summary>
    /// Gets or sets the background brush.
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
    /// Gets or sets the size of the font (if not set, the Height property is used.
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
    /// Gets or sets the foreground (text) brush.
    /// </summary>
    /// <value>The foreground brush.</value>
    public Brush Foreground
    {
      get => (Brush)GetValue(ForegroundProperty);

      set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the text.
    /// </summary>
    /// <value>The text height.</value>
    public double Height
    {
      get => (double)GetValue(HeightProperty);

      set => SetValue(HeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    /// <value>The horizontal alignment.</value>
    public HorizontalAlignment HorizontalAlignment
    {
      get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
      set => SetValue(HorizontalAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this text visual is double sided.
    /// </summary>
    /// <value><c>true</c> if this instance is double sided; otherwise, <c>false</c>.</value>
    public bool IsDoubleSided
    {
      get => (bool)GetValue(IsDoubleSidedProperty);
      set => SetValue(IsDoubleSidedProperty, value);
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
    /// Gets or sets the position of the text.
    /// </summary>
    /// <value>The position.</value>
    public Point3D Position
    {
      get => (Point3D)GetValue(PositionProperty);

      set => SetValue(PositionProperty, value);
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
    /// Gets or sets the text direction.
    /// </summary>
    /// <value>The direction.</value>
    public Vector3D TextDirection
    {
      get => (Vector3D)GetValue(TextDirectionProperty);

      set => SetValue(TextDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the up direction of the text.
    /// </summary>
    /// <value>The up direction.</value>
    public Vector3D UpDirection
    {
      get => (Vector3D)GetValue(UpDirectionProperty);

      set => SetValue(UpDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    /// <value>The vertical alignment.</value>
    public VerticalAlignment VerticalAlignment
    {
      get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
      set => SetValue(VerticalAlignmentProperty, value);
    }

    /// <summary>
    /// The visual changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TextVisual3D)d).VisualChanged();
    }

    /// <summary>
    /// Called when the visual changed.
    /// </summary>
    private void VisualChanged()
    {
      if(string.IsNullOrEmpty(Text))
      {
        Content = null;
        return;
      }

      // First we need a textblock containing the text of our label
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
                        ? (FrameworkElement)
                          new Border
                          {
                            BorderBrush = BorderBrush,
                            BorderThickness = BorderThickness,
                            Child = textBlock
                          }
                        : textBlock;

      element.Measure(new Size(1000, 1000));
      element.Arrange(new Rect(element.DesiredSize));

      Material material;
      if(FontSize > 0)
      {
        var rtb = new RenderTargetBitmap(
            (int)element.ActualWidth + 1, (int)element.ActualHeight + 1, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(element);
        rtb.Freeze();
        material = new DiffuseMaterial(new ImageBrush(rtb));
      }
      else
      {
        material = new DiffuseMaterial { Brush = new VisualBrush(element) };
      }

      var width = element.ActualWidth / element.ActualHeight * Height;

      var position = Position;
      var textDirection = TextDirection;
      var updirection = UpDirection;
      var height = Height;

      // Set horizontal alignment factor
      var xa = -0.5;
      if(HorizontalAlignment == HorizontalAlignment.Left)
      {
        xa = 0;
      }
      if(HorizontalAlignment == HorizontalAlignment.Right)
      {
        xa = -1;
      }

      // Set vertical alignment factor
      var ya = -0.5;
      if(VerticalAlignment == VerticalAlignment.Top)
      {
        ya = -1;
      }
      if(VerticalAlignment == VerticalAlignment.Bottom)
      {
        ya = 0;
      }

      // Since the parameter coming in was the center of the label,
      // we need to find the four corners
      // p0 is the lower left corner
      // p1 is the upper left
      // p2 is the lower right
      // p3 is the upper right
      var p0 = position + (xa * width) * textDirection + (ya * height) * updirection;
      var p1 = p0 + updirection * height;
      var p2 = p0 + textDirection * width;
      var p3 = p0 + updirection * height + textDirection * width;

      // Now build the geometry for the sign.  It's just a
      // rectangle made of two triangles, on each side.
      var mg = new MeshGeometry3D { Positions = new Point3DCollection { p0, p1, p2, p3 } };

      var isDoubleSided = IsDoubleSided;
      if(isDoubleSided)
      {
        mg.Positions.Add(p0); // 4
        mg.Positions.Add(p1); // 5
        mg.Positions.Add(p2); // 6
        mg.Positions.Add(p3); // 7
      }

      mg.TriangleIndices.Add(0);
      mg.TriangleIndices.Add(3);
      mg.TriangleIndices.Add(1);
      mg.TriangleIndices.Add(0);
      mg.TriangleIndices.Add(2);
      mg.TriangleIndices.Add(3);

      if(isDoubleSided)
      {
        mg.TriangleIndices.Add(4);
        mg.TriangleIndices.Add(5);
        mg.TriangleIndices.Add(7);
        mg.TriangleIndices.Add(4);
        mg.TriangleIndices.Add(7);
        mg.TriangleIndices.Add(6);
      }

      double u0 = IsFlipped ? 1 : 0;
      double u1 = IsFlipped ? 0 : 1;

      // These texture coordinates basically stretch the
      // TextBox brush to cover the full side of the label.
      mg.TextureCoordinates.Add(new Point(u0, 1));
      mg.TextureCoordinates.Add(new Point(u0, 0));
      mg.TextureCoordinates.Add(new Point(u1, 1));
      mg.TextureCoordinates.Add(new Point(u1, 0));

      if(isDoubleSided)
      {
        mg.TextureCoordinates.Add(new Point(u1, 1));
        mg.TextureCoordinates.Add(new Point(u1, 0));
        mg.TextureCoordinates.Add(new Point(u0, 1));
        mg.TextureCoordinates.Add(new Point(u0, 0));
      }

      Content = new GeometryModel3D(mg, material);
    }

    // http://www.ericsink.com/wpf3d/4_Text.html
  }
}