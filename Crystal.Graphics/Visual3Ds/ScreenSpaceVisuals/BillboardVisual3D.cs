namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that contains a billboard (a quadrilateral that always faces camera). The size of the billboard is defined in screen space.
  /// </summary>
  public class BillboardVisual3D : RenderingModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="DepthOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DepthOffsetProperty = DependencyProperty.Register(
        "DepthOffset", typeof(double), typeof(BillboardVisual3D), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
        "Height", typeof(double), typeof(BillboardVisual3D), new UIPropertyMetadata(10.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="HorizontalAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.Register(
            "HorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(BillboardVisual3D),
            new UIPropertyMetadata(HorizontalAlignment.Center));

    /// <summary>
    /// Identifies the <see cref="Material"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register(
        "Material",
        typeof(Material),
        typeof(BillboardVisual3D),
        new UIPropertyMetadata(Materials.Red, MaterialChanged));

    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        "Position",
        typeof(Point3D),
        typeof(BillboardVisual3D),
        new UIPropertyMetadata(new Point3D(), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="VerticalAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.Register(
            "VerticalAlignment",
            typeof(VerticalAlignment),
            typeof(BillboardVisual3D),
            new UIPropertyMetadata(VerticalAlignment.Center));

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
        "Width", typeof(double), typeof(BillboardVisual3D), new UIPropertyMetadata(10.0, GeometryChanged));

    /// <summary>
    /// The builder.
    /// </summary>
    private readonly BillboardGeometryBuilder builder;

    /// <summary>
    /// The is rendering flag.
    /// </summary>
    private bool isRendering;

    /// <summary>
    /// Initializes a new instance of the <see cref="BillboardVisual3D" /> class.
    /// </summary>
    public BillboardVisual3D()
    {
      builder = new BillboardGeometryBuilder(this);
      Mesh = new MeshGeometry3D
      {
        TriangleIndices = BillboardGeometryBuilder.CreateIndices(1),
        TextureCoordinates =
                              new PointCollection
                                  {
                                            new(0, 1),
                                            new(1, 1),
                                            new(1, 0),
                                            new(0, 0)
                                  }
      };

      Model = new GeometryModel3D { Geometry = Mesh };
      Content = Model;
      OnMaterialChanged();
      OnGeometryChanged();
    }

    /// <summary>
    /// Gets or sets the depth offset.
    /// </summary>
    /// <value>The depth offset.</value>
    public double DepthOffset
    {
      get => (double)GetValue(DepthOffsetProperty);

      set => SetValue(DepthOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
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
    /// Gets or sets the material.
    /// </summary>
    /// <value>The material.</value>
    public Material? Material
    {
      get => (Material)GetValue(MaterialProperty);

      set => SetValue(MaterialProperty, value);
    }

    /// <summary>
    /// Gets or sets the position (center) of the billboard.
    /// </summary>
    /// <value>The position.</value>
    public Point3D Position
    {
      get => (Point3D)GetValue(PositionProperty);

      set => SetValue(PositionProperty, value);
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
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public double Width
    {
      get => (double)GetValue(WidthProperty);

      set => SetValue(WidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the mesh.
    /// </summary>
    protected MeshGeometry3D Mesh { get; set; }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    protected GeometryModel3D Model { get; set; }

    /// <summary>
    /// The on material changed.
    /// </summary>
    public void OnMaterialChanged()
    {
      Model.Material = Material;
    }

    /// <summary>
    /// The geometry changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BillboardVisual3D)d).OnGeometryChanged();
    }

    /// <summary>
    /// The composition target_ rendering.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
    {
      if(isRendering)
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
      var bb = new Billboard(
          Position,
          Width,
          Height,
          HorizontalAlignment,
          VerticalAlignment,
          DepthOffset);

      Mesh.Positions = builder.GetPositions(new[] { bb }, new Vector());
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
    /// The color changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BillboardVisual3D)d).OnMaterialChanged();
    }

    /// <summary>
    /// Called when geometry properties have changed.
    /// </summary>
    private void OnGeometryChanged()
    {
      UpdateGeometry();
    }
  }
}