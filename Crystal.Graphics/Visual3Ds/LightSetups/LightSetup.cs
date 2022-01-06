namespace Crystal.Graphics
{
  /// <summary>
  /// An abstract base class for light models.
  /// </summary>
  public abstract class LightSetup : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="ShowLights"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowLightsProperty = DependencyProperty.Register(
        "ShowLights", typeof(bool), typeof(LightSetup), new UIPropertyMetadata(false, ShowLightsChanged));

    /// <summary>
    /// The light group.
    /// </summary>
    private readonly Model3DGroup lightGroup = new();

    /// <summary>
    /// The lights visual.
    /// </summary>
    private readonly ModelVisual3D lightsVisual = new();

    /// <summary>
    /// Initializes a new instance of the <see cref = "LightSetup" /> class.
    /// </summary>
    protected LightSetup()
    {
      Content = lightGroup;
      Children.Add(lightsVisual);
      OnSetupChanged();
      OnShowLightsChanged();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show light visuals.
    /// </summary>
    public bool ShowLights
    {
      get => (bool)GetValue(ShowLightsProperty);

      set => SetValue(ShowLightsProperty, value);
    }

    /// <summary>
    /// The setup changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void SetupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((LightSetup)d).OnSetupChanged();
    }

    /// <summary>
    /// Adds the lights to the element.
    /// </summary>
    /// <param name="lightGroup">
    /// The light group.
    /// </param>
    protected abstract void AddLights(Model3DGroup lightGroup);

    /// <summary>
    /// Handles changes to the light setup.
    /// </summary>
    protected void OnSetupChanged()
    {
      lightGroup.Children.Clear();
      AddLights(lightGroup);
    }

    /// <summary>
    /// Called when show lights is changed.
    /// </summary>
    protected void OnShowLightsChanged()
    {
      lightsVisual.Children.Clear();
      if(ShowLights)
      {
        foreach(var light in lightGroup.Children)
        {
          if(light is PointLight pl)
          {
            var sphere = new SphereVisual3D();
            sphere.BeginEdit();
            sphere.Center = pl.Position;
            sphere.Radius = 1.0;
            sphere.Fill = new SolidColorBrush(pl.Color);
            sphere.EndEdit();
            lightsVisual.Children.Add(sphere);
          }

          if(light is DirectionalLight dl)
          {
            var dir = dl.Direction;
            dir.Normalize();

            var target = new Point3D(0, 0, 0);
            var source = target - (dir * 20);
            var p2 = source + (dir * 10);

            var sphere = new SphereVisual3D();
            sphere.BeginEdit();
            sphere.Center = source;
            sphere.Radius = 1.0;
            sphere.Fill = new SolidColorBrush(dl.Color);
            sphere.EndEdit();
            lightsVisual.Children.Add(sphere);

            var arrow = new ArrowVisual3D();
            arrow.BeginEdit();
            arrow.Point1 = source;
            arrow.Point2 = p2;
            arrow.Diameter = 0.5;
            arrow.Fill = new SolidColorBrush(dl.Color);
            arrow.EndEdit();
            lightsVisual.Children.Add(arrow);
          }

          if(light is AmbientLight al)
          {
            var pos = new Point3D(0, 0, 20);
            lightsVisual.Children.Add(
                new CubeVisual3D { Center = pos, SideLength = 1.0, Fill = new SolidColorBrush(al.Color) });
          }
        }
      }
    }

    /// <summary>
    /// The show lights changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void ShowLightsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((LightSetup)d).OnShowLightsChanged();
    }

  }
}