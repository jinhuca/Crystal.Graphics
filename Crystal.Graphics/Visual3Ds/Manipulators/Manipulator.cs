using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  ///   Provides an abstract base class for manipulators.
  /// </summary>
  public abstract class Manipulator : UIElement3D
  {
    /// <summary>
    /// Identifies the <see cref="Color"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        "Color", typeof(Color), typeof(Manipulator), new UIPropertyMetadata((s, _) => ((Manipulator)s).ColorChanged()));

    /// <summary>
    /// Identifies the <see cref="Offset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
        "Offset",
        typeof(Vector3D),
        typeof(Manipulator),
        new FrameworkPropertyMetadata(
            new Vector3D(0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((Manipulator)s).PositionChanged(e)));

    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        "Position",
        typeof(Point3D),
        typeof(Manipulator),
        new FrameworkPropertyMetadata(
            new Point3D(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((Manipulator)s).PositionChanged(e)));

    /// <summary>
    /// Identifies the <see cref="TargetTransform"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetTransformProperty =
        DependencyProperty.Register(
            "TargetTransform",
            typeof(Transform3D),
            typeof(Manipulator),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        "Value",
        typeof(double),
        typeof(Manipulator),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((Manipulator)s).ValueChanged(e)));

    /// <summary>
    /// Identifies the <see cref="Material"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaterialProperty =
        DependencyProperty.Register("Material", typeof(Material), typeof(Manipulator), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="BackMaterial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackMaterialProperty =
        DependencyProperty.Register("BackMaterial", typeof(Material), typeof(Manipulator), new PropertyMetadata(null));

    /// <summary>
    ///   Initializes a new instance of the <see cref="Manipulator" /> class.
    /// </summary>
    protected Manipulator()
    {
      Model = new GeometryModel3D();
      BindingOperations.SetBinding(Model, GeometryModel3D.MaterialProperty, new Binding("Material") { Source = this });
      BindingOperations.SetBinding(Model, GeometryModel3D.BackMaterialProperty, new Binding("BackMaterial") { Source = this });
      Visual3DModel = Model;
    }

    /// <summary>
    ///   Gets or sets the color of the manipulator.
    /// </summary>
    /// <value> The color. </value>
    public Color Color
    {
      get => (Color)GetValue(ColorProperty);

      set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the material of the manipulator.
    /// </summary>
    public Material Material
    {
      get => (Material)GetValue(MaterialProperty);
      set => SetValue(MaterialProperty, value);
    }

    /// <summary>
    /// Gets or sets the back material of the manipulator.
    /// </summary>
    public Material BackMaterial
    {
      get => (Material)GetValue(BackMaterialProperty);
      set => SetValue(BackMaterialProperty, value);
    }

    /// <summary>
    ///   Gets or sets the offset of the visual (this vector is added to the Position point).
    /// </summary>
    /// <value> The offset. </value>
    public Vector3D Offset
    {
      get => (Vector3D)GetValue(OffsetProperty);

      set => SetValue(OffsetProperty, value);
    }

    /// <summary>
    ///   Gets or sets the position of the manipulator.
    /// </summary>
    /// <value> The position. </value>
    public Point3D Position
    {
      get => (Point3D)GetValue(PositionProperty);

      set => SetValue(PositionProperty, value);
    }

    /// <summary>
    ///   Gets or sets the target transform.
    /// </summary>
    public Transform3D TargetTransform
    {
      get => (Transform3D)GetValue(TargetTransformProperty);

      set => SetValue(TargetTransformProperty, value);
    }

    /// <summary>
    ///   Gets or sets the manipulator value.
    /// </summary>
    /// <value> The value. </value>
    public double Value
    {
      get => (double)GetValue(ValueProperty);

      set => SetValue(ValueProperty, value);
    }

    /// <summary>
    ///   Gets or sets the camera.
    /// </summary>
    protected ProjectionCamera Camera { get; set; }

    /// <summary>
    ///   Gets or sets the hit plane normal.
    /// </summary>
    protected Vector3D HitPlaneNormal { get; set; }

    /// <summary>
    ///   Gets or sets the model.
    /// </summary>
    protected GeometryModel3D Model { get; set; }

    /// <summary>
    ///   Gets or sets the parent viewport.
    /// </summary>
    protected Viewport3D ParentViewport { get; set; }

    /// <summary>
    /// Binds this manipulator to a given Visual3D.
    /// </summary>
    /// <param name="source">
    /// Source Visual3D which receives the manipulator transforms.
    /// </param>
    public virtual void Bind(ModelVisual3D source)
    {
      BindingOperations.SetBinding(this, TargetTransformProperty, new Binding("Transform") { Source = source });
      BindingOperations.SetBinding(this, TransformProperty, new Binding("Transform") { Source = source });
    }

    /// <summary>
    ///   Releases the binding of this manipulator.
    /// </summary>
    public virtual void UnBind()
    {
      BindingOperations.ClearBinding(this, TargetTransformProperty);
      BindingOperations.ClearBinding(this, TransformProperty);
    }

    /// <summary>
    /// Called when a property related to the geometry is changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    protected static void UpdateGeometry(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Manipulator)d).UpdateGeometry();
    }

    /// <summary>
    /// Projects the point on the hit plane.
    /// </summary>
    /// <param name="p">
    /// The p.
    /// </param>
    /// <param name="hitPlaneOrigin">
    /// The hit Plane Origin.
    /// </param>
    /// <param name="hitPlaneNormal">
    /// The hit plane normal (world coordinate system).
    /// </param>
    /// <returns>
    /// The point in world coordinates.
    /// </returns>
    protected virtual Point3D? GetHitPlanePoint(Point p, Point3D hitPlaneOrigin, Vector3D hitPlaneNormal)
    {
      return ParentViewport.UnProject(p, hitPlaneOrigin, hitPlaneNormal);
    }

    /// <summary>
    /// Updates the geometry.
    /// </summary>
    protected abstract void UpdateGeometry();

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      ParentViewport = this.GetViewport3D();
      Camera = ParentViewport.Camera as ProjectionCamera;
      var projectionCamera = Camera;
      if(projectionCamera != null)
      {
        HitPlaneNormal = projectionCamera.LookDirection;
      }

      CaptureMouse();
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the mouse button was released.</param>
    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      ReleaseMouseCapture();
    }

    /// <summary>
    /// Handles changes in the Position property.
    /// </summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void PositionChanged(DependencyPropertyChangedEventArgs e)
    {
      Transform = new TranslateTransform3D(
          Position.X + Offset.X, Position.Y + Offset.Y, Position.Z + Offset.Z);
    }

    /// <summary>
    /// Handles changes in the Value property.
    /// </summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void ValueChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    /// <summary>
    /// Transforms from world to local coordinates.
    /// </summary>
    /// <param name="worldPoint">
    /// The point (world coordinates).
    /// </param>
    /// <returns>
    /// Transformed vector (local coordinates).
    /// </returns>
    protected Point3D ToLocal(Point3D worldPoint)
    {
      var mat = this.GetTransform();
      mat.Invert();
      var t = new MatrixTransform3D(mat);
      return t.Transform(worldPoint);
    }

    /// <summary>
    /// Transforms from local to world coordinates.
    /// </summary>
    /// <param name="point">
    /// The point (local coordinates).
    /// </param>
    /// <returns>
    /// Transformed point (world coordinates).
    /// </returns>
    protected Point3D ToWorld(Point3D point)
    {
      var mat = this.GetTransform();
      var t = new MatrixTransform3D(mat);
      return t.Transform(point);
    }

    /// <summary>
    /// Transforms from local to world coordinates.
    /// </summary>
    /// <param name="vector">
    /// The vector (local coordinates).
    /// </param>
    /// <returns>
    /// Transformed vector (world coordinates).
    /// </returns>
    protected Vector3D ToWorld(Vector3D vector)
    {
      var mat = this.GetTransform();
      var t = new MatrixTransform3D(mat);
      return t.Transform(vector);
    }

    /// <summary>
    ///   Handles changes in the Color property (this will override the materials).
    /// </summary>
    private void ColorChanged()
    {
      Material = MaterialHelper.CreateMaterial(Color);
      BackMaterial = Material;
    }
  }
}