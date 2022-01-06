namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a base class for lights that operates in camera space.
  /// </summary>
  /// <typeparam name="T">The light type.</typeparam>
  public abstract class GenericHeadLight<T> : LightSetup where T : Light, new()
  {
    /// <summary>
    /// Identifies the <see cref="Brightness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register("Brightness", typeof(double), typeof(GenericHeadLight<T>), new PropertyMetadata(1d, Update));

    /// <summary>
    /// Identifies the <see cref="Color"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color), typeof(GenericHeadLight<T>), new PropertyMetadata(Colors.White, Update));

    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(Point3D), typeof(GenericHeadLight<T>), new PropertyMetadata(new Point3D(0, 0, 3), Update));

    /// <summary>
    /// The light
    /// </summary>
    private T light;

    /// <summary>
    /// The camera
    /// </summary>
    private Camera camera;

    /// <summary>
    /// Gets or sets the brightness of the headlight. If set, this property overrides the <see cref="Color" /> property.
    /// </summary>
    /// <value>The brightness.</value>
    public double Brightness
    {
      get => (double)GetValue(BrightnessProperty);
      set => SetValue(BrightnessProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the headlight. This property is used if <see cref="Brightness" /> is set to <c>NaN</c>.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
      get => (Color)GetValue(ColorProperty);
      set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the position of the headlight (in camera space).
    /// </summary>
    /// <value>The position.</value>
    public Point3D Position
    {
      get => (Point3D)GetValue(PositionProperty);
      set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Called when the parent of the 3-D visual object is changed.
    /// </summary>
    /// <param name="oldParent">A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the <see cref="T:System.Windows.Media.Media3D.Visual3D" /> object. If the <see cref="T:System.Windows.Media.Media3D.Visual3D" /> object did not have a previous parent, the value of the parameter is null.</param>
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);
      var viewport = this.GetViewport3D();
      if(camera != null)
      {
        camera.Changed -= CameraChanged;
      }

      camera = viewport?.Camera;
      if(camera != null)
      {
        camera.Changed += CameraChanged;
      }

      Update();
    }

    /// <summary>
    /// Adds the lights to the element.
    /// </summary>
    /// <param name="lightGroup">The light group.</param>
    protected override void AddLights(Model3DGroup lightGroup)
    {
      light = new T();
      lightGroup.Children.Add(light);
    }

    /// <summary>
    /// Updates the light.
    /// </summary>
    /// <param name="d">The sender.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((GenericHeadLight<T>)d).Update();
    }

    /// <summary>
    /// Handles changes to the camera.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CameraChanged(object sender, EventArgs e)
    {
      Update();
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    private void Update()
    {
      if(!double.IsNaN(Brightness))
      {
        var a = (byte)(Brightness * 255);
        light.Color = Color.FromArgb(255, a, a, a);
      }
      else
      {
        light.Color = Color;
      }

      if(camera is ProjectionCamera projectionCamera)
      {
        var y = projectionCamera.LookDirection;
        var x = Vector3D.CrossProduct(projectionCamera.LookDirection, projectionCamera.UpDirection);
        x.Normalize();
        y.Normalize();
        var z = Vector3D.CrossProduct(x, y);
        var lightPosition = projectionCamera.Position + (Position.X * x) + (Position.Y * y) + (Position.Z * z);
        var target = projectionCamera.Position + projectionCamera.LookDirection;
        var lightDirection = target - lightPosition;
        lightDirection.Normalize();

        if(light is SpotLight spotLight)
        {
          spotLight.Position = lightPosition;
          spotLight.Direction = lightDirection;
        }

        if(light is DirectionalLight directionalLight)
        {
          directionalLight.Direction = lightDirection;
        }
      }
    }
  }
}