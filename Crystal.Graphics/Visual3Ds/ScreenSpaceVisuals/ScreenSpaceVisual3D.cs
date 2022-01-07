namespace Crystal.Graphics
{
  /// <summary>
  /// An abstract base class for visuals that use screen space dimensions when rendering.
  /// </summary>
  public abstract class ScreenSpaceVisual3D : RenderingModelVisual3D
  {
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Color Color
    {
      get => (Color)GetValue(ColorProperty);
      set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Color"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
      nameof(Color), typeof(Color), typeof(ScreenSpaceVisual3D), new UIPropertyMetadata(Colors.Black, ColorChanged));

    /// <summary>
    /// Gets or sets the depth offset.
    /// A small positive number (0.0001) will move the visual slightly in front of other objects.
    /// </summary>
    /// <value>
    /// The depth offset.
    /// </value>
    public double DepthOffset
    {
      get => (double)GetValue(DepthOffsetProperty);
      set => SetValue(DepthOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="DepthOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DepthOffsetProperty = DependencyProperty.Register(
      nameof(DepthOffset), typeof(double), typeof(ScreenSpaceVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the points collection.
    /// </summary>
    /// <value>
    /// The points collection.
    /// </value>
    public Point3DCollection? Points
    {
      get => (Point3DCollection)GetValue(PointsProperty);
      set => SetValue(PointsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Points"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
      nameof(Points), typeof(Point3DCollection), typeof(ScreenSpaceVisual3D), 
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, PointsChanged));

    /// <summary>
    /// The is rendering flag.
    /// </summary>
    private bool isRendering;

    /// <summary>
    /// The listening to collection
    /// </summary>
    private Point3DCollection collectionBeingListenedTo;

    /// <summary>
    /// Initializes a new instance of the <see cref = "ScreenSpaceVisual3D" /> class.
    /// </summary>
    protected ScreenSpaceVisual3D()
    {
      Mesh = new MeshGeometry3D();
      Model = new GeometryModel3D { Geometry = Mesh };
      Content = Model;
      Points = new Point3DCollection();
      ColorChanged();
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
    /// Gets or sets the clipping object.
    /// </summary>
    protected CohenSutherlandClipping Clipping { get; set; }

    /// <summary>
    /// Gets or sets the mesh.
    /// </summary>
    protected MeshGeometry3D Mesh { get; set; }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    protected GeometryModel3D Model { get; set; }

    /// <summary>
    /// Called when geometry properties have changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    protected static void GeometryChanged(object sender, DependencyPropertyChangedEventArgs e) => ((ScreenSpaceVisual3D)sender).UpdateGeometry();

    /// <summary>
    /// Called when points have changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private static void PointsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var screenSpaceVisual3D = (ScreenSpaceVisual3D)sender;
      screenSpaceVisual3D.UpdateGeometry();

      if(screenSpaceVisual3D.collectionBeingListenedTo is { IsFrozen: false } && screenSpaceVisual3D != null)
      {
        screenSpaceVisual3D.collectionBeingListenedTo.Changed -= screenSpaceVisual3D.HandlePointsChanged;
      }

      var pc = e.NewValue as Point3DCollection;
      if(pc is { IsFrozen: false })
      {
        screenSpaceVisual3D.collectionBeingListenedTo = pc;

        // TODO: use a weak event manager
        pc.Changed += screenSpaceVisual3D.HandlePointsChanged;
      }
      else
      {
        screenSpaceVisual3D.collectionBeingListenedTo = pc!;
      }
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
          UpdateClipping();
          UpdateGeometry();
        }
      }
    }

    /// <summary>
    /// Called when the parent of the 3-D visual object is changed.
    /// </summary>
    /// <param name="oldParent">
    /// A value of type <see cref="T:System.Windows.DependencyObject"/> that represents the previous parent of the <see cref="T:System.Windows.Media.Media3D.Visual3D"/> object. If the <see cref="T:System.Windows.Media.Media3D.Visual3D"/> object did not have a previous parent, the value of the parameter is null.
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
    protected abstract void UpdateGeometry();

    /// <summary>
    /// Updates the transforms.
    /// </summary>
    /// <returns>
    /// True if the transform is updated.
    /// </returns>
    protected abstract bool UpdateTransforms();

    /// <summary>
    /// Changes the material when the color changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private static void ColorChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      ((ScreenSpaceVisual3D)sender).ColorChanged();
    }

    /// <summary>
    /// Handles changes in the <see cref="Points" /> collection.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    private void HandlePointsChanged(object sender, EventArgs e)
    {
      UpdateGeometry();
    }

    /// <summary>
    /// Changes the material when the color changed.
    /// </summary>
    private void ColorChanged()
    {
      var mg = new MaterialGroup();
      mg.Children.Add(new DiffuseMaterial(Brushes.Black));
      mg.Children.Add(new EmissiveMaterial(new SolidColorBrush(Color)));
      mg.Freeze();
      Model.Material = mg;
      Model.BackMaterial = mg;
    }

    /// <summary>
    /// Updates the clipping object.
    /// </summary>
    private void UpdateClipping()
    {
      var vp = this.GetViewport3D();
      if(vp == null)
      {
        return;
      }

      Clipping = new CohenSutherlandClipping(10, vp.ActualWidth - 20, 10, vp.ActualHeight - 20);
    }
  }
}