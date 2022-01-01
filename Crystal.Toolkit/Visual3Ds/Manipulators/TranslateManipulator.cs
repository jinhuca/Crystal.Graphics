using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a visual element that contains a manipulator that can translate along an axis.
  /// </summary>
  public class TranslateManipulator : Manipulator
  {
    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
        "Diameter", typeof(double), typeof(TranslateManipulator), new UIPropertyMetadata(0.2, UpdateGeometry));

    /// <summary>
    /// Identifies the <see cref="Direction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
        "Direction",
        typeof(Vector3D),
        typeof(TranslateManipulator),
        new UIPropertyMetadata(UpdateGeometry));

    /// <summary>
    /// Identifies the <see cref="Length"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        "Length", typeof(double), typeof(TranslateManipulator), new UIPropertyMetadata(2.0, UpdateGeometry));

    /// <summary>
    /// The last point.
    /// </summary>
    private Point3D lastPoint;

    /// <summary>
    /// Gets or sets the diameter of the manipulator arrow.
    /// </summary>
    /// <value> The diameter. </value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);

      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction of the translation.
    /// </summary>
    /// <value> The direction. </value>
    public Vector3D Direction
    {
      get => (Vector3D)GetValue(DirectionProperty);

      set => SetValue(DirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the length of the manipulator arrow.
    /// </summary>
    /// <value> The length. </value>
    public double Length
    {
      get => (double)GetValue(LengthProperty);

      set => SetValue(LengthProperty, value);
    }

    /// <summary>
    /// Updates the geometry.
    /// </summary>
    protected override void UpdateGeometry()
    {
      var mb = new MeshBuilder(false, false);
      var p0 = new Point3D(0, 0, 0);
      var d = Direction;
      d.Normalize();
      var p1 = p0 + (d * Length);
      mb.AddArrow(p0, p1, Diameter);
      Model.Geometry = mb.ToMesh();
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      var direction = ToWorld(Direction);

      var up = Vector3D.CrossProduct(Camera.LookDirection, direction);
      var hitPlaneOrigin = ToWorld(Position);
      HitPlaneNormal = Vector3D.CrossProduct(up, direction);
      var p = e.GetPosition(ParentViewport);

      var np = GetNearestPoint(p, hitPlaneOrigin, HitPlaneNormal);
      if(np == null)
      {
        return;
      }

      var lp = ToLocal(np.Value);

      lastPoint = lp;
      CaptureMouse();
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if(IsMouseCaptured)
      {
        var hitPlaneOrigin = ToWorld(Position);
        var p = e.GetPosition(ParentViewport);
        var nearestPoint = GetNearestPoint(p, hitPlaneOrigin, HitPlaneNormal);
        if(nearestPoint == null)
        {
          return;
        }

        var delta = ToLocal(nearestPoint.Value) - lastPoint;
        Value += Vector3D.DotProduct(delta, Direction);

        if(TargetTransform != null)
        {
          var translateTransform = new TranslateTransform3D(delta);
          TargetTransform = Transform3DHelper.CombineTransform(translateTransform, TargetTransform);
        }
        else
        {
          Position += delta;
        }

        nearestPoint = GetNearestPoint(p, hitPlaneOrigin, HitPlaneNormal);
        if(nearestPoint != null)
        {
          lastPoint = ToLocal(nearestPoint.Value);
        }
      }
    }

    /// <summary>
    /// Gets the nearest point on the translation axis.
    /// </summary>
    /// <param name="position">
    /// The position (in screen coordinates).
    /// </param>
    /// <param name="hitPlaneOrigin">
    /// The hit plane origin (world coordinate system).
    /// </param>
    /// <param name="hitPlaneNormal">
    /// The hit plane normal (world coordinate system).
    /// </param>
    /// <returns>
    /// The nearest point (world coordinates) or null if no point could be found.
    /// </returns>
    private Point3D? GetNearestPoint(Point position, Point3D hitPlaneOrigin, Vector3D hitPlaneNormal)
    {
      var hpp = GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
      if(hpp == null)
      {
        return null;
      }

      var ray = new Ray3D(ToWorld(Position), ToWorld(Direction));
      return ray.GetNearest(hpp.Value);
    }
  }
}