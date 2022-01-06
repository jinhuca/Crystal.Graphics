using System.Diagnostics;
using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// An abstract base class for the mouse gesture handlers.
  /// </summary>
  internal abstract class MouseGestureHandler
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseGestureHandler"/> class.
    /// </summary>
    /// <param name="controller">
    /// The controller.
    /// </param>
    protected MouseGestureHandler(CameraController controller)
    {
      Controller = controller;
      ManipulationWatch = new Stopwatch();
    }

    /// <summary>
    /// Gets the origin.
    /// </summary>
    public Point3D Origin
    {
      get
      {
        if(MouseDownNearestPoint3D != null)
        {
          return MouseDownNearestPoint3D.Value;
        }

        if(MouseDownPoint3D != null)
        {
          return MouseDownPoint3D.Value;
        }

        return new Point3D();
      }
    }

    /// <summary>
    /// Gets the camera.
    /// </summary>
    /// <value>The camera.</value>
    protected ProjectionCamera? Camera => Viewport.Camera as ProjectionCamera;

    /// <summary>
    /// Gets or sets the camera look direction.
    /// </summary>
    /// <value>The camera look direction.</value>
    protected Vector3D CameraLookDirection
    {
      get => Controller.CameraLookDirection;

      set => Controller.CameraLookDirection = value;
    }

    /// <summary>
    /// Gets the camera mode.
    /// </summary>
    /// <value>The camera mode.</value>
    protected CameraMode CameraMode => Controller.CameraMode;

    /// <summary>
    /// Gets or sets the camera position.
    /// </summary>
    /// <value>The camera position.</value>
    protected Point3D CameraPosition
    {
      get => Controller.CameraPosition;

      set => Controller.CameraPosition = value;
    }

    /// <summary>
    /// Gets the camera target.
    /// </summary>
    /// <value>The camera target.</value>
    protected Point3D CameraTarget => CameraPosition + CameraLookDirection;

    /// <summary>
    /// Gets or sets the camera up direction.
    /// </summary>
    /// <value>The camera up direction.</value>
    protected Vector3D CameraUpDirection
    {
      get => Controller.CameraUpDirection;

      set => Controller.CameraUpDirection = value;
    }

    /// <summary>
    /// Gets or sets the controller.
    /// </summary>
    protected CameraController Controller { get; set; }

    /// <summary>
    /// Gets or sets the last point (2D screen coordinates).
    /// </summary>
    protected Point LastPoint { get; set; }

    /// <summary>
    /// Gets or sets the last point (3D world coordinates).
    /// </summary>
    protected Point3D? LastPoint3D { get; set; }

    /// <summary>
    /// Gets or sets the manipulation watch.
    /// </summary>
    protected Stopwatch ManipulationWatch { get; set; }

    /// <summary>
    /// Gets the model up direction.
    /// </summary>
    /// <value>The model up direction.</value>
    protected Vector3D ModelUpDirection => Controller.ModelUpDirection;

    /// <summary>
    /// Gets or sets the mouse down point at the nearest hit element (3D world coordinates).
    /// </summary>
    protected Point3D? MouseDownNearestPoint3D { get; set; }

    /// <summary>
    /// Gets or sets the mouse down point (2D screen coordinates).
    /// </summary>
    protected Point MouseDownNearestPoint2D { get; set; }

    /// <summary>
    /// Gets or sets the mouse down point (2D screen coordinates).
    /// </summary>
    protected Point MouseDownPoint { get; set; }

    /// <summary>
    /// Gets or sets the mouse down point (3D world coordinates).
    /// </summary>
    protected Point3D? MouseDownPoint3D { get; set; }

    /// <summary>
    /// Gets the rotation sensitivity.
    /// </summary>
    /// <value>The rotation sensitivity.</value>
    protected double RotationSensitivity => Controller.RotationSensitivity;

    /// <summary>
    /// Gets the viewport.
    /// </summary>
    /// <value>The viewport.</value>
    protected Viewport3D Viewport => Controller.Viewport;

    /// <summary>
    /// Gets the height of the viewport.
    /// </summary>
    /// <value>The height of the viewport.</value>
    protected double ViewportHeight => Controller.ActualHeight;

    /// <summary>
    /// Gets the width of the viewport.
    /// </summary>
    /// <value>The width of the viewport.</value>
    protected double ViewportWidth => Controller.ActualWidth;

    /// <summary>
    /// Gets the zoom sensitivity.
    /// </summary>
    /// <value>The zoom sensitivity.</value>
    protected double ZoomSensitivity => Controller.ZoomSensitivity;

    /// <summary>
    /// Occurs when the manipulation is completed.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    public virtual void Completed(ManipulationEventArgs e)
    {
      var elapsed = ManipulationWatch.ElapsedMilliseconds;
      if(elapsed > 0 && elapsed < Controller.SpinReleaseTime)
      {
        OnInertiaStarting((int)ManipulationWatch.ElapsedMilliseconds);
      }
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    public virtual void Delta(ManipulationEventArgs e)
    {
    }

    /// <summary>
    /// Starts the mouse gesture handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    public void Execute(object sender, ExecutedRoutedEventArgs e)
    {
      if(!CanExecute())
      {
        return;
      }

      Controller.PushCameraSetting();
      OnMouseDown(sender, null);
      Controller.Focus();
      Controller.CaptureMouse();
      Controller.PushCameraSetting();

      e.Handled = true;
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    public virtual void Started(ManipulationEventArgs e)
    {
      SetMouseDownPoint(e.CurrentPosition);
      LastPoint = MouseDownPoint;
      LastPoint3D = MouseDownPoint3D;
      ManipulationWatch.Restart();
    }

    /// <summary>
    /// Un projects a point from the screen (2D) to a point on plane (3D)
    /// </summary>
    /// <param name="p">
    /// The 2D point.
    /// </param>
    /// <param name="position">
    /// A point on the plane .
    /// </param>
    /// <param name="normal">
    /// The plane normal.
    /// </param>
    /// <returns>
    /// A 3D point.
    /// </returns>
    public Point3D? UnProject(Point p, Point3D position, Vector3D normal)
    {
      var ray = GetRay(p);
      if(ray == null)
      {
        return null;
      }

      return ray.PlaneIntersection(position, normal, out var i) ? i : null;
    }

    /// <summary>
    /// Un projects a point from the screen (2D) to a point on the plane trough the camera target point.
    /// </summary>
    /// <param name="p">
    /// The 2D point.
    /// </param>
    /// <returns>
    /// A 3D point.
    /// </returns>
    public Point3D? UnProject(Point p)
    {
      return UnProject(p, CameraTarget, CameraLookDirection);
    }

    /// <summary>
    /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
    /// </summary>
    /// <returns>
    /// True if the execution can continue.
    /// </returns>
    protected virtual bool CanExecute()
    {
      return true;
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>
    /// A cursor.
    /// </returns>
    protected abstract Cursor GetCursor();

    /// <summary>
    /// Get the ray into the view volume given by the position in 2D (screen coordinates)
    /// </summary>
    /// <param name="position">
    /// A 2D point.
    /// </param>
    /// <returns>
    /// A ray
    /// </returns>
    protected Ray3D? GetRay(Point position)
    {
      var ok = Viewport.Point2DtoPoint3D(position, out var point1, out var point2);
      return !ok ? null : new Ray3D { Origin = point1, Direction = point2 - point1 };
    }

    /// <summary>
    /// Called when inertia is starting.
    /// </summary>
    /// <param name="elapsedTime">
    /// The elapsed time (milliseconds).
    /// </param>
    protected virtual void OnInertiaStarting(int elapsedTime)
    {
    }

    /// <summary>
    /// Called when the mouse button is pressed down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnMouseDown(object sender, MouseButtonEventArgs? e)
    {
      Controller.MouseUp += OnMouseUp;
      Controller.MouseMove += OnMouseMove;
      Controller.SetCursor(GetCursor());

      Started(new ManipulationEventArgs(Mouse.GetPosition(Controller)));
    }

    /// <summary>
    /// Handles the mouse move events.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected virtual void OnMouseMove(object sender, MouseEventArgs e)
    {
      Delta(new ManipulationEventArgs(Mouse.GetPosition(Controller)));
    }

    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      // TODO: this will handle all mouse button release events - not only the one assigned to this command...
      // May need to refactor and not use input bindings to support only releasing for the relevant mouse button...
      Controller.MouseMove -= OnMouseMove;
      Controller.MouseUp -= OnMouseUp;
      Controller.ReleaseMouseCapture();
      Controller.RestoreCursor();
      Completed(new ManipulationEventArgs(Mouse.GetPosition(Controller)));
    }

    /// <summary>
    /// Calculate the screen position of a 3D point.
    /// </summary>
    /// <param name="p">
    /// The 3D point.
    /// </param>
    /// <returns>
    /// The 2D point.
    /// </returns>
    protected Point Project(Point3D p)
    {
      return Viewport.Point3DtoPoint2D(p);
    }

    /// <summary>
    /// The set mouse down point.
    /// </summary>
    /// <param name="position">
    /// The position.
    /// </param>
    private void SetMouseDownPoint(Point position)
    {
      MouseDownPoint = position;
      MouseDownPoint3D = UnProject(MouseDownPoint);
      var nearestPoint = new Closest3DPointHitTester(Controller.Viewport, Controller.RotataAroundClosestVertexComplexity)
          .CalculateMouseDownNearestPoint(position, Controller.SnapMouseDownPoint);
      MouseDownNearestPoint2D = nearestPoint.MouseDownNearestPoint2D;
      MouseDownNearestPoint3D = nearestPoint.MouseDownNearestPoint3D;


    }
  }
}