namespace Crystal.Graphics
{
  /// <summary>
  /// Specifies the sorting method for the SortingVisual3D.
  /// </summary>
  public enum SortingMethod
  {
    /// <summary>
    /// Sort on the distance from camera to bounding box center.
    /// </summary>
    BoundingBoxCenter,

    /// <summary>
    /// Sort on the minimum distance from camera to bounding box corners.
    /// </summary>
    BoundingBoxCorners,

    /// <summary>
    /// Sort on the minimum distance from camera to bounding sphere surface.
    /// </summary>
    BoundingSphereSurface
  }

  /// <summary>
  /// A visual element that sorts the children by distance from camera.
  /// </summary>
  /// <remarks>
  /// The children are sorted by the distance to the camera position. This will not always work when you have overlapping objects.
  /// </remarks>
  public class SortingVisual3D : RenderingModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="CheckForOpaqueVisuals"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CheckForOpaqueVisualsProperty =
        DependencyProperty.Register(
            "CheckForOpaqueVisuals", typeof(bool), typeof(SortingVisual3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsSorting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSortingProperty = DependencyProperty.Register(
        "IsSorting", typeof(bool), typeof(SortingVisual3D), new UIPropertyMetadata(false, IsSortingChanged));

    /// <summary>
    /// Identifies the <see cref="Method"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MethodProperty = DependencyProperty.Register(
        "Method",
        typeof(SortingMethod),
        typeof(SortingVisual3D),
        new UIPropertyMetadata(SortingMethod.BoundingBoxCorners));

    /// <summary>
    /// Identifies the <see cref="SortingFrequency"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SortingFrequencyProperty =
        DependencyProperty.Register(
            "SortingFrequency", typeof(double), typeof(SortingVisual3D), new UIPropertyMetadata(60.0));

    /// <summary>
    /// The start tick.
    /// </summary>
    private long startTick;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingVisual3D" /> class.
    /// </summary>
    public SortingVisual3D()
    {
      IsSorting = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to check if there are opaque child visuals.
    /// </summary>
    public bool CheckForOpaqueVisuals
    {
      get => (bool)GetValue(CheckForOpaqueVisualsProperty);

      set => SetValue(CheckForOpaqueVisualsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is being sorted. When the visual is removed from the Viewport3D, this property should be set to false to unsubscribe the rendering event.
    /// </summary>
    public bool IsSorting
    {
      get => (bool)GetValue(IsSortingProperty);

      set => SetValue(IsSortingProperty, value);
    }

    /// <summary>
    /// Gets or sets the sorting method.
    /// </summary>
    /// <value> The method. </value>
    public SortingMethod Method
    {
      get => (SortingMethod)GetValue(MethodProperty);

      set => SetValue(MethodProperty, value);
    }

    /// <summary>
    /// Gets or sets the sorting frequency (Hz).
    /// </summary>
    /// <value> The sorting frequency. </value>
    public double SortingFrequency
    {
      get => (double)GetValue(SortingFrequencyProperty);

      set => SetValue(SortingFrequencyProperty, value);
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
      if(startTick == 0)
      {
        startTick = e.RenderingTime.Ticks;
      }

      var time = 100e-9 * (e.RenderingTime.Ticks - startTick);
      if(IsSorting && time >= 1.0 / SortingFrequency)
      {
        startTick = e.RenderingTime.Ticks;
        SortChildren();
      }
    }

    /// <summary>
    /// The is sorting changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void IsSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SortingVisual3D)d).OnIsSortingChanged();
    }

    /// <summary>
    /// Gets the distance from the camera for the specified visual.
    /// </summary>
    /// <param name="c">
    /// The visual.
    /// </param>
    /// <param name="cameraPos">
    /// The camera position.
    /// </param>
    /// <param name="transform">
    /// The total transform of the visual.
    /// </param>
    /// <returns>
    /// The camera distance.
    /// </returns>
    private double GetCameraDistance(Visual3D c, Point3D cameraPos, Transform3D transform)
    {
      var bounds = c.FindBounds(transform);
      switch(Method)
      {
        case SortingMethod.BoundingBoxCenter:
          var mid = new Point3D(
              bounds.X + bounds.SizeX * 0.5, bounds.Y + bounds.SizeY * 0.5, bounds.Z + bounds.SizeZ * 0.5);
          return (mid - cameraPos).LengthSquared;
        case SortingMethod.BoundingBoxCorners:
          var d = double.MaxValue;
          d = Math.Min(d, cameraPos.DistanceTo(new Point3D(bounds.X, bounds.Y, bounds.Z)));
          d = Math.Min(d, cameraPos.DistanceTo(new Point3D(bounds.X + bounds.SizeX, bounds.Y, bounds.Z)));
          d = Math.Min(
              d, cameraPos.DistanceTo(new Point3D(bounds.X + bounds.SizeX, bounds.Y + bounds.SizeY, bounds.Z)));
          d = Math.Min(d, cameraPos.DistanceTo(new Point3D(bounds.X, bounds.Y + bounds.SizeY, bounds.Z)));
          d = Math.Min(d, cameraPos.DistanceTo(new Point3D(bounds.X, bounds.Y, bounds.Z + bounds.SizeZ)));
          d = Math.Min(
              d, cameraPos.DistanceTo(new Point3D(bounds.X + bounds.SizeX, bounds.Y, bounds.Z + bounds.SizeZ)));
          d = Math.Min(
              d,
              cameraPos.DistanceTo(
                  new Point3D(bounds.X + bounds.SizeX, bounds.Y + bounds.SizeY, bounds.Z + bounds.SizeZ)));
          d = Math.Min(
              d, cameraPos.DistanceTo(new Point3D(bounds.X, bounds.Y + bounds.SizeY, bounds.Z + bounds.SizeZ)));
          return d;
        default:
          var boundingSphere = BoundingSphere.CreateFromRect3D(bounds);
          return boundingSphere.DistanceFrom(cameraPos);
      }
    }

    /// <summary>
    /// Determines if the specified visual is transparent.
    /// </summary>
    /// <param name="visual">
    /// The visual.
    /// </param>
    /// <returns>
    /// True if the visual is transparent.
    /// </returns>
    private bool IsVisualTransparent(Visual3D visual)
    {
      return ElementSortingHelper.IsTransparent(visual);
    }

    /// <summary>
    /// The on is sorting changed.
    /// </summary>
    private void OnIsSortingChanged()
    {
      if(IsSorting)
      {
        startTick = 0;
        SubscribeToRenderingEvent();
      }
      else
      {
        UnsubscribeRenderingEvent();
      }
    }

    /// <summary>
    /// The sort children.
    /// </summary>
    private void SortChildren()
    {
      var vp = this.GetViewport3D();
      if(vp == null)
      {
        return;
      }

      if(vp.Camera is not ProjectionCamera cam)
      {
        return;
      }

      var cameraPos = cam.Position;
      var transform = new MatrixTransform3D(this.GetTransform());

      IList<Visual3D> transparentChildren = new List<Visual3D>();
      IList<Visual3D> opaqueChildren = new List<Visual3D>();
      if(CheckForOpaqueVisuals)
      {
        foreach(var child in Children)
        {
          if(IsVisualTransparent(child))
          {
            transparentChildren.Add(child);
          }
          else
          {
            opaqueChildren.Add(child);
          }
        }
      }
      else
      {
        transparentChildren = Children;
      }

      // sort the children by distance from camera (note that OrderBy is a stable sort algorithm)
      var sortedTransparentChildren =
          transparentChildren.OrderBy(item => -GetCameraDistance(item, cameraPos, transform)).ToList();

      // Now that opaqueChildren and sortedTransparentChildren describe our desired ordering, we need sort the current children in the new order. 
      // To optimize the efficiency of this procedure we want to change the children list as little as possible.
      // Unfortunatally the Visual3DCollection does not have a swap method and we always need to remove an item before we can add it again as
      // temporary duplicates result in exceptions in the visual tree.
      // Due to this set of considerations we use selection sort to sort the current Children. (if we could swap without removal, cycle sort might be a small improvement)

      for(var desiredIndex = 0; desiredIndex < opaqueChildren.Count; desiredIndex++)
      {
        var currentChild = opaqueChildren[desiredIndex];
        var currentIndex = Children.IndexOf(currentChild);
        //Insert in the proper spot if not contained:
        if(currentIndex == -1)
        {
          Children.Insert(desiredIndex, currentChild);
          continue;
        }
        //Do nothing if it is in the correct spot;
        //The order of the opaque children does not matter as long as they are before the transparent children:
        if(currentIndex < opaqueChildren.Count)
          continue;

        //remove from old spot and insert to the new correct spot:
        Children.RemoveAt(currentIndex);
        Children.Insert(desiredIndex, currentChild);
      }

      for(var desiredIndex = opaqueChildren.Count; desiredIndex < opaqueChildren.Count + sortedTransparentChildren.Count; desiredIndex++)
      {
        var currentChild = sortedTransparentChildren[desiredIndex - opaqueChildren.Count];
        var currentIndex = Children.IndexOf(currentChild);
        //Insert in the proper spot if not contained:
        if(currentIndex == -1)
        {
          Children.Insert(desiredIndex, currentChild);
          continue;
        }
        //Do nothing if it is in the correct spot:
        if(currentIndex == desiredIndex)
          continue;

        //remove from old spot and insert to the new correct spot:
        Children.RemoveAt(currentIndex);
        Children.Insert(desiredIndex, currentChild);
      }
    }

  }
}