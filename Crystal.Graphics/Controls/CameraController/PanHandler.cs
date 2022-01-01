using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Handles panning.
  /// </summary>
  internal class PanHandler : MouseGestureHandler
  {
    /// <summary>
    /// The 3D pan origin.
    /// </summary>
    private Point3D panPoint3D;

    /// <summary>
    /// Initializes a new instance of the <see cref="PanHandler"/> class.
    /// </summary>
    /// <param name="controller">
    /// The controller.
    /// </param>
    public PanHandler(CameraController controller)
        : base(controller)
    {
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Delta(ManipulationEventArgs e)
    {
      base.Delta(e);
      var thisPoint3D = UnProject(e.CurrentPosition, panPoint3D, Controller.CameraLookDirection);

      if(LastPoint3D == null || thisPoint3D == null)
      {
        return;
      }

      Vector3D delta3D = LastPoint3D.Value - thisPoint3D.Value;
      Pan(delta3D);

      LastPoint = e.CurrentPosition;
      LastPoint3D = UnProject(e.CurrentPosition, panPoint3D, Controller.CameraLookDirection);
    }

    /// <summary>
    /// Pans the camera by the specified 3D vector (world coordinates).
    /// </summary>
    /// <param name="delta">
    /// The panning vector.
    /// </param>
    public void Pan(Vector3D delta)
    {
      if(!Controller.IsPanEnabled)
      {
        return;
      }

      if(CameraMode == CameraMode.FixedPosition)
      {
        return;
      }

      CameraPosition += delta;
    }

    /// <summary>
    /// Pans the camera by the specified 2D vector (screen coordinates).
    /// </summary>
    /// <param name="delta">
    /// The delta.
    /// </param>
    public void Pan(Vector delta)
    {
      var mousePoint = LastPoint + delta;

      var thisPoint3D = UnProject(mousePoint, panPoint3D, Controller.CameraLookDirection);

      if(LastPoint3D == null || thisPoint3D == null)
      {
        return;
      }

      Vector3D delta3D = LastPoint3D.Value - thisPoint3D.Value;
      Pan(delta3D);

      LastPoint3D = UnProject(mousePoint, panPoint3D, Controller.CameraLookDirection);

      LastPoint = mousePoint;
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Started(ManipulationEventArgs e)
    {
      base.Started(e);
      panPoint3D = Controller.CameraTarget;
      if(MouseDownNearestPoint3D != null)
      {
        panPoint3D = MouseDownNearestPoint3D.Value;
      }

      LastPoint3D = UnProject(MouseDownPoint, panPoint3D, Controller.CameraLookDirection);
    }

    /// <summary>
    /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
    /// </summary>
    /// <returns>
    /// True if the execution can continue.
    /// </returns>
    protected override bool CanExecute()
    {
      return Controller.IsPanEnabled && Controller.CameraMode != CameraMode.FixedPosition;
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>
    /// A cursor.
    /// </returns>
    protected override Cursor GetCursor()
    {
      return Controller.PanCursor;
    }

    /// <summary>
    /// Called when inertia is starting.
    /// </summary>
    /// <param name="elapsedTime">
    /// The elapsed time (milliseconds).
    /// </param>
    protected override void OnInertiaStarting(int elapsedTime)
    {
      var speed = (LastPoint - MouseDownPoint) * (40.0 / elapsedTime);
      Controller.AddPanForce(speed.X, speed.Y);
    }

  }
}