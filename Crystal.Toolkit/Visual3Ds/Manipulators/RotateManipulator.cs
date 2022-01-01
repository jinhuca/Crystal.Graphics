using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a visual element containing a manipulator that can rotate around an axis.
  /// </summary>
  public class RotateManipulator : Manipulator
  {
    /// <summary>
    /// Identifies the <see cref="Axis"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AxisProperty = DependencyProperty.Register(
        "Axis",
        typeof(Vector3D),
        typeof(RotateManipulator),
        new UIPropertyMetadata(new Vector3D(0, 0, 1), UpdateGeometry));

    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
        "Diameter", typeof(double), typeof(RotateManipulator), new UIPropertyMetadata(3.0, UpdateGeometry));

    /// <summary>
    /// Identifies the <see cref="InnerDiameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InnerDiameterProperty = DependencyProperty.Register(
        "InnerDiameter", typeof(double), typeof(RotateManipulator), new UIPropertyMetadata(2.5, UpdateGeometry));

    /// <summary>
    /// Identifies the <see cref="Length"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        "Length", typeof(double), typeof(RotateManipulator), new UIPropertyMetadata(0.1, UpdateGeometry));

    /// <summary>
    /// Identifies the <see cref="Pivot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PivotProperty = DependencyProperty.Register(
        "Pivot", typeof(Point3D), typeof(Manipulator), new PropertyMetadata(new Point3D()));

    /// <summary>
    /// The last point.
    /// </summary>
    private Point3D lastPoint;

    /// <summary>
    /// Gets or sets the rotation axis.
    /// </summary>
    /// <value>The axis.</value>
    public Vector3D Axis
    {
      get => (Vector3D)GetValue(AxisProperty);

      set => SetValue(AxisProperty, value);
    }

    /// <summary>
    /// Gets or sets the outer diameter of the manipulator.
    /// </summary>
    /// <value>The outer diameter.</value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);

      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner diameter of the manipulator.
    /// </summary>
    /// <value>The inner diameter.</value>
    public double InnerDiameter
    {
      get => (double)GetValue(InnerDiameterProperty);

      set => SetValue(InnerDiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the length (thickness) of the manipulator.
    /// </summary>
    /// <value>The length.</value>
    public double Length
    {
      get => (double)GetValue(LengthProperty);

      set => SetValue(LengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the pivot point of the manipulator.
    /// </summary>
    /// <value> The position. </value>
    public Point3D Pivot
    {
      get => (Point3D)GetValue(PivotProperty);

      set => SetValue(PivotProperty, value);
    }

    /// <summary>
    /// Updates the geometry.
    /// </summary>
    protected override void UpdateGeometry()
    {
      var mb = new MeshBuilder(false, false);
      var p0 = new Point3D(0, 0, 0);
      var d = Axis;
      d.Normalize();
      var p1 = p0 - (d * Length * 0.5);
      var p2 = p0 + (d * Length * 0.5);
      mb.AddPipe(p1, p2, InnerDiameter, Diameter, 60);
      Model.Geometry = mb.ToMesh();
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      var hitPlaneOrigin = ToWorld(Position);
      var hitPlaneNormal = ToWorld(Axis);
      var p = e.GetPosition(ParentViewport);

      var hitPoint = GetHitPlanePoint(p, hitPlaneOrigin, hitPlaneNormal);
      if(hitPoint != null)
      {
        lastPoint = ToLocal(hitPoint.Value);
      }
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
        var hitPlaneNormal = ToWorld(Axis);

        var position = e.GetPosition(ParentViewport);
        var hitPoint = GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
        if(hitPoint == null)
        {
          return;
        }

        var currentPoint = ToLocal(hitPoint.Value);

        var v = lastPoint - Position;
        var u = currentPoint - Position;
        v.Normalize();
        u.Normalize();

        var currentAxis = Vector3D.CrossProduct(u, v);
        double sign = -Vector3D.DotProduct(Axis, currentAxis);
        double theta = Math.Sign(sign) * Math.Asin(currentAxis.Length) / Math.PI * 180;
        Value += theta;

        if(TargetTransform != null)
        {
          var rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(Axis, theta), Pivot);
          TargetTransform = Transform3DHelper.CombineTransform(rotateTransform, TargetTransform);
        }

        hitPoint = GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
        if(hitPoint != null)
        {
          lastPoint = ToLocal(hitPoint.Value);
        }
      }
    }
  }
}