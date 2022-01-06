namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that contains a collection of text billboards.
  /// </summary>
  [ContentProperty("Items")]
  public class BillboardTextGroupVisual3D : RenderingModelVisual3D, IBoundsIgnoredVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Background"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        "Background", typeof(Brush), typeof(BillboardTextGroupVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        "BorderBrush",
        typeof(Brush),
        typeof(BillboardTextGroupVisual3D),
        new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty =
        DependencyProperty.Register(
            "BorderThickness",
            typeof(Thickness),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(new Thickness(1), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
        "FontFamily",
        typeof(FontFamily),
        typeof(BillboardTextGroupVisual3D),
        new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        "FontSize", typeof(double), typeof(BillboardTextGroupVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        "FontWeight",
        typeof(FontWeight),
        typeof(BillboardTextGroupVisual3D),
        new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        "Foreground",
        typeof(Brush),
        typeof(BillboardTextGroupVisual3D),
        new UIPropertyMetadata(Brushes.Black, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="HeightFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightFactorProperty = DependencyProperty.Register(
        "HeightFactor", typeof(double), typeof(BillboardTextGroupVisual3D), new PropertyMetadata(1.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Items"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        "Items",
        typeof(IList<BillboardTextItem>),
        typeof(BillboardTextGroupVisual3D),
        new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        "Padding",
        typeof(Thickness),
        typeof(BillboardTextGroupVisual3D),
        new UIPropertyMetadata(new Thickness(0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Offset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register("Offset", typeof(Vector), typeof(BillboardTextGroupVisual3D), new PropertyMetadata(new Vector(0, 0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="PinWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PinWidthProperty =
        DependencyProperty.Register("PinWidth", typeof(double), typeof(BillboardTextGroupVisual3D), new PropertyMetadata(4d));

    /// <summary>
    /// Identifies the <see cref="PinBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PinBrushProperty =
        DependencyProperty.Register("PinBrush", typeof(Brush), typeof(BillboardTextGroupVisual3D), new PropertyMetadata(Brushes.Black, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="IsEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register("IsEnabled", typeof(bool), typeof(BillboardTextGroupVisual3D), new PropertyMetadata(true));

    /// <summary>
    /// The geometry builder.
    /// </summary>
    private readonly BillboardGeometryBuilder builder;

    /// <summary>
    /// The billboard meshes.
    /// </summary>
    private readonly Dictionary<MeshGeometry3D, IList<Billboard>> meshes = new();

    /// <summary>
    /// The pin meshes
    /// </summary>
    private readonly Dictionary<MeshGeometry3D, IList<Billboard>> pinMeshes = new();

    /// <summary>
    /// The is rendering flag.
    /// </summary>
    private bool isRendering;

    /// <summary>
    /// Initializes a new instance of the <see cref="BillboardTextGroupVisual3D" /> class.
    /// </summary>
    public BillboardTextGroupVisual3D()
    {
      builder = new BillboardGeometryBuilder(this);
    }

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
    /// Gets or sets a value indicating whether updating of this object is enabled.
    /// </summary>
    /// <value><c>true</c> if this object is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled
    {
      get => (bool)GetValue(IsEnabledProperty);

      set => SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the pin brush.
    /// </summary>
    /// <value>The pin brush.</value>
    public Brush PinBrush
    {
      get => (Brush)GetValue(PinBrushProperty);

      set => SetValue(PinBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the 'pin'.
    /// </summary>
    /// <value>The width of the pin.</value>
    /// <remarks>
    /// You must set the Offset property for the pin to show up.
    /// </remarks>
    public double PinWidth
    {
      get => (double)GetValue(PinWidthProperty);

      set => SetValue(PinWidthProperty, value);
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
    /// Gets or sets a value indicating whether this instance is being rendered.
    /// When the visual is removed from the visual tree, this property should be set to false.
    /// </summary>
    public bool IsRendering
    {
      get => isRendering;

      set
      {
        if(value != isRendering)
        {
          isRendering = value;
          if(isRendering)
          {
            SubscribeToRenderingEvent();
          }
          else
          {
            UnsubscribeRenderingEvent();
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    /// <value>The items.</value>
    public IList<BillboardTextItem> Items
    {
      get => (IList<BillboardTextItem>)GetValue(ItemsProperty);

      set => SetValue(ItemsProperty, value);
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
    /// Gets or sets the offset of the billboard text (in screen coordinates).
    /// </summary>
    /// <value>The offset.</value>
    public Vector Offset
    {
      get => (Vector)GetValue(OffsetProperty);

      set => SetValue(OffsetProperty, value);
    }

    /// <summary>
    /// Handles the CompositionTarget.Rendering event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The <see cref="System.Windows.Media.RenderingEventArgs" /> instance containing the event data.</param>
    protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs eventArgs)
    {
      if(isRendering && IsEnabled)
      {
        if(!this.IsAttachedToViewport3D())
        {
          return;
        }

        if(UpdateTransforms())
        {
          UpdateGeometry();
        }
      }
    }

    /// <summary>
    /// Called when the parent of the 3-D visual object is changed.
    /// </summary>
    /// <param name="oldParent">
    /// A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the
    ///     <see
    ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
    /// object. If the
    ///     <see
    ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
    /// object did not have a previous parent, the value of the parameter is null.
    /// </param>
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);
      var parent = VisualTreeHelper.GetParent(this);
      IsRendering = parent != null;
    }

    /// <summary>
    /// Updates the geometry.
    /// </summary>
    protected void UpdateGeometry()
    {
      foreach(var m in meshes)
      {
        m.Key.Positions = builder.GetPositions(m.Value, Offset);
      }

      foreach(var m in pinMeshes)
      {
        m.Key.Positions = builder.GetPinPositions(m.Value, Offset, PinWidth);
      }
    }

    /// <summary>
    /// Updates the transforms.
    /// </summary>
    /// <returns>
    /// True if the transform is updated.
    /// </returns>
    protected bool UpdateTransforms()
    {
      return builder.UpdateTransforms();
    }

    /// <summary>
    /// The visual appearance changed.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BillboardTextGroupVisual3D)d).VisualChanged();
    }

    /// <summary>
    /// Creates the element for the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A FrameworkElement.</returns>
    private FrameworkElement CreateElement(string text)
    {
      var textBlock = new TextBlock(new Run(text))
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

      if(BorderBrush != null)
      {
        return new Border
        {
          BorderBrush = BorderBrush,
          BorderThickness = BorderThickness,
          Child = textBlock
        };
      }

      return textBlock;
    }

    /// <summary>
    /// Updates the visual appearance (texture and geometry).
    /// </summary>
    private void VisualChanged()
    {
      meshes.Clear();
      pinMeshes.Clear();

      if(Items == null)
      {
        Content = null;
        return;
      }

      var pinMaterial = new DiffuseMaterial(PinBrush);

      var items = Items.Where(i => !string.IsNullOrEmpty(i.Text)).ToList();
      var group = new Model3DGroup();
      while(items.Count > 0)
      {
        var material = TextGroupVisual3D.CreateTextMaterial(
            items, CreateElement, Background, out var elementMap, out var elementPositions);
        material.Freeze();

        var billboards = new List<Billboard>();
        var addedChildren = new List<BillboardTextItem>();
        var textureCoordinates = new PointCollection();
        foreach(var item in items)
        {
          var element = elementMap[item.Text];
          var r = elementPositions[element];
          if(r.Bottom > 1)
          {
            break;
          }

          billboards.Add(
              new Billboard(
                  item.Position,
                  element.ActualWidth,
                  element.ActualHeight,
                  item.HorizontalAlignment,
                  item.VerticalAlignment,
                  item.DepthOffset,
                  item.WorldDepthOffset));
          textureCoordinates.Add(new Point(r.Left, r.Bottom));
          textureCoordinates.Add(new Point(r.Right, r.Bottom));
          textureCoordinates.Add(new Point(r.Right, r.Top));
          textureCoordinates.Add(new Point(r.Left, r.Top));

          addedChildren.Add(item);
        }

        var triangleIndices = BillboardGeometryBuilder.CreateIndices(billboards.Count);
        triangleIndices.Freeze();

        var g = new MeshGeometry3D
        {
          TriangleIndices = triangleIndices,
          TextureCoordinates = textureCoordinates,
          Positions = builder.GetPositions(billboards, Offset)
        };
        group.Children.Add(new GeometryModel3D(g, material));
        meshes.Add(g, billboards);

        if(Offset.Length > 0)
        {
          var pinGeometry = new MeshGeometry3D
          {
            TriangleIndices = triangleIndices,
            Positions = builder.GetPinPositions(billboards, Offset, PinWidth)
          };
          group.Children.Add(new GeometryModel3D(pinGeometry, pinMaterial));
          pinMeshes.Add(pinGeometry, billboards);
        }

        foreach(var c in addedChildren)
        {
          items.Remove(c);
        }
      }

      Content = group;
    }
  }
}