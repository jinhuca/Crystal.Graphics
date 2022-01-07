using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Handles rotation.
  /// </summary>
  internal class RotateHandler : MouseGestureHandler
  {
    /// <summary>
    /// The change look at.
    /// </summary>
    private readonly bool changeLookAt;

    /// <summary>
    /// The x rotation axis.
    /// </summary>
    private Vector3D rotationAxisX;

    /// <summary>
    /// The y rotation axis.
    /// </summary>
    private Vector3D rotationAxisY;

    /// <summary>
    /// The rotation point.
    /// </summary>
    private Point rotationPoint;

    /// <summary>
    /// The 3D rotation point.
    /// </summary>
    private Point3D rotationPoint3D;

    /// <summary>
    /// Initializes a new instance of the <see cref="RotateHandler"/> class.
    /// </summary>
    /// <param name="controller">
    /// The controller.
    /// </param>
    /// <param name="changeLookAt">
    /// The change look at.
    /// </param>
    public RotateHandler(CameraController controller, bool changeLookAt = false)
      : base(controller)
    {
      this.changeLookAt = changeLookAt;
    }

    /// <summary>
    /// Occurs when the manipulation is completed.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Completed(ManipulationEventArgs e)
    {
      base.Completed(e);
      Controller.HideTargetAdorner();
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Delta(ManipulationEventArgs e)
    {
      base.Delta(e);
      Rotate(LastPoint, e.CurrentPosition, rotationPoint3D);
      LastPoint = e.CurrentPosition;
    }

    /// <summary>
    /// Change the "look-at" point.
    /// </summary>
    /// <param name="target">
    /// The target.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void LookAt(Point3D target, double animationTime)
    {
      if(!Controller.IsPanEnabled)
      {
        return;
      }
      Camera.LookAt(target, animationTime);
      Controller.OnLookAtChanged();
    }

    /// <summary>
    /// Rotate the camera around the specified point.
    /// </summary>
    /// <param name="p0">
    /// The p 0.
    /// </param>
    /// <param name="p1">
    /// The p 1.
    /// </param>
    /// <param name="rotateAround">
    /// The rotate around.
    /// </param>
    public void Rotate(Point p0, Point p1, Point3D rotateAround)
    {
      if(!Controller.IsRotationEnabled)
      {
        return;
      }

      switch(Controller.CameraRotationMode)
      {
        case CameraRotationMode.Trackball:
          RotateTrackball(p0, p1, rotateAround);
          break;
        case CameraRotationMode.Turntable:
          RotateTurntable(p1 - p0, rotateAround);
          break;
        case CameraRotationMode.Turnball:
          RotateTurnball(p0, p1, rotateAround);
          break;
      }

      if(Math.Abs(Controller.CameraUpDirection.Length - 1) > 1e-8)
      {
        Controller.CameraUpDirection.Normalize();
      }
    }

    /// <summary>
    /// The rotate.
    /// </summary>
    /// <param name="delta">
    /// The delta.
    /// </param>
    public void Rotate(Vector delta)
    {
      var p0 = LastPoint;
      var p1 = p0 + delta;
      if(MouseDownPoint3D != null)
      {
        Rotate(p0, p1, MouseDownPoint3D.Value);
      }

      LastPoint = p0;
    }

    /// <summary>
    /// Rotate around three axes.
    /// </summary>
    /// <param name="p1">
    /// The previous mouse position.
    /// </param>
    /// <param name="p2">
    /// The current mouse position.
    /// </param>
    /// <param name="rotateAround">
    /// The point to rotate around.
    /// </param>
    public void RotateTurnball(Point p1, Point p2, Point3D rotateAround)
    {
      InitTurnballRotationAxes(p1);

      var delta = p2 - p1;

      var relativeTarget = rotateAround - CameraTarget;
      var relativePosition = rotateAround - CameraPosition;

      double d = -1;
      if(CameraMode != CameraMode.Inspect)
      {
        d = 0.2;
      }

      d *= RotationSensitivity;

      var q1 = new Quaternion(rotationAxisX, d * delta.X);
      var q2 = new Quaternion(rotationAxisY, d * delta.Y);
      var q = q1 * q2;

      var m = new Matrix3D();
      m.Rotate(q);

      var newLookDir = m.Transform(CameraLookDirection);
      var newUpDirection = m.Transform(CameraUpDirection);

      var newRelativeTarget = m.Transform(relativeTarget);
      var newRelativePosition = m.Transform(relativePosition);

      var newRightVector = Vector3D.CrossProduct(newLookDir, newUpDirection);
      newRightVector.Normalize();
      var modUpDir = Vector3D.CrossProduct(newRightVector, newLookDir);
      modUpDir.Normalize();
      if((newUpDirection - modUpDir).Length > 1e-8)
      {
        newUpDirection = modUpDir;
      }

      var newTarget = rotateAround - newRelativeTarget;
      var newPosition = rotateAround - newRelativePosition;
      var newLookDirection = newTarget - newPosition;

      CameraLookDirection = newLookDirection;
      if(CameraMode == CameraMode.Inspect)
      {
        CameraPosition = newPosition;
      }
      CameraUpDirection = newUpDirection;
    }

    /// <summary>
    /// Rotate camera using 'Turntable' rotation.
    /// </summary>
    /// <param name="delta">
    /// The relative change in position.
    /// </param>
    /// <param name="rotateAround">
    /// The point to rotate around.
    /// </param>
    public void RotateTurntable(Vector delta, Point3D rotateAround)
    {
      var relativeTarget = rotateAround - CameraTarget;
      var relativePosition = rotateAround - CameraPosition;

      var up = ModelUpDirection;
      var dir = CameraLookDirection;
      dir.Normalize();

      var right = Vector3D.CrossProduct(dir, CameraUpDirection);
      right.Normalize();

      var d = -0.5;
      if(CameraMode != CameraMode.Inspect)
      {
        d *= -0.2;
      }

      d *= RotationSensitivity;

      var q1 = new Quaternion(up, d * delta.X);
      var q2 = new Quaternion(right, d * delta.Y);
      var q = q1 * q2;

      var m = new Matrix3D();
      m.Rotate(q);

      var newUpDirection = m.Transform(CameraUpDirection);

      var newRelativeTarget = m.Transform(relativeTarget);
      var newRelativePosition = m.Transform(relativePosition);

      var newTarget = rotateAround - newRelativeTarget;
      var newPosition = rotateAround - newRelativePosition;

      CameraLookDirection = newTarget - newPosition;
      if(CameraMode == CameraMode.Inspect)
      {
        CameraPosition = newPosition;
      }

      CameraUpDirection = newUpDirection;
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Started(ManipulationEventArgs e)
    {
      base.Started(e);

      rotationPoint = new Point(
          Controller.Viewport.ActualWidth / 2, Controller.Viewport.ActualHeight / 2);
      rotationPoint3D = Controller.CameraTarget;

      switch(Controller.CameraMode)
      {
        case CameraMode.WalkAround:
          rotationPoint = MouseDownPoint;
          rotationPoint3D = Controller.CameraPosition;
          break;
        default:
          if(Controller.FixedRotationPointEnabled)
          {
            rotationPoint = Viewport.Point3DtoPoint2D(Controller.FixedRotationPoint);
            rotationPoint3D = Controller.FixedRotationPoint;
          }
          else if(changeLookAt && MouseDownNearestPoint3D != null)
          {
            LookAt(MouseDownNearestPoint3D.Value, 0);
            rotationPoint3D = Controller.CameraTarget;
          }
          else if(Controller.RotateAroundMouseDownPoint && MouseDownNearestPoint3D != null)
          {
            rotationPoint = MouseDownPoint;
            rotationPoint3D = MouseDownNearestPoint3D.Value;
          }

          break;
      }

      if(Controller.CameraMode == CameraMode.Inspect)
      {
        Controller.ShowTargetAdorner(Controller.ZoomAroundMouseDownPoint ? MouseDownNearestPoint2D : MouseDownPoint);
      }

      switch(Controller.CameraRotationMode)
      {
        case CameraRotationMode.Trackball:
          break;
        case CameraRotationMode.Turntable:
          break;
        case CameraRotationMode.Turnball:
          InitTurnballRotationAxes(e.CurrentPosition);
          break;
      }

      Controller.StopSpin();
    }

    /// <summary>
    /// The can execute.
    /// </summary>
    /// <returns>
    /// True if the execution can continue.
    /// </returns>
    protected override bool CanExecute()
    {
      if(changeLookAt)
      {
        return Controller.CameraMode != CameraMode.FixedPosition && Controller.IsPanEnabled;
      }

      return Controller.IsRotationEnabled;
    }

    /// <summary>
    /// Gets the cursor.
    /// </summary>
    /// <returns>
    /// A cursor.
    /// </returns>
    protected override Cursor GetCursor()
    {
      return Controller.RotateCursor;
    }

    /// <summary>
    /// Called when inertia is starting.
    /// </summary>
    /// <param name="elapsedTime">
    /// The elapsed time.
    /// </param>
    protected override void OnInertiaStarting(int elapsedTime)
    {
      var delta = LastPoint - MouseDownPoint;
      // Debug.WriteLine("SpinInertiaStarting: " + elapsedTime + "ms " + delta.Length + "px");
      Controller.StartSpin(4 * delta * ((double)Controller.SpinReleaseTime / elapsedTime), MouseDownPoint, rotationPoint3D);
    }

    /// <summary>
    /// Projects a screen position to the trackball unit sphere.
    /// </summary>
    /// <param name="point">
    /// The screen position.
    /// </param>
    /// <param name="w">
    /// The width of the viewport.
    /// </param>
    /// <param name="h">
    /// The height of the viewport.
    /// </param>
    /// <returns>
    /// A trackball coordinate.
    /// </returns>
    private static Vector3D ProjectToTrackball(Point point, double w, double h)
    {
      // Use the diagonal for scaling, making sure that the whole client area is inside the trackball
      var r = Math.Sqrt(w * w + h * h) / 2;
      var x = (point.X - w / 2) / r;
      var y = (h / 2 - point.Y) / r;
      var z2 = 1 - x * x - y * y;
      var z = z2 > 0 ? Math.Sqrt(z2) : 0;
      return new Vector3D(x, y, z);
    }

    /// <summary>
    /// The init turnball rotation axes.
    /// </summary>
    /// <param name="p1">
    /// The p 1.
    /// </param>
    private void InitTurnballRotationAxes(Point p1)
    {
      var fx = p1.X / ViewportWidth;
      var fy = p1.Y / ViewportHeight;
      var up = CameraUpDirection;
      var dir = CameraLookDirection;
      dir.Normalize();

      var right = Vector3D.CrossProduct(dir, CameraUpDirection);
      right.Normalize();

      rotationAxisX = up;
      rotationAxisY = right;
      if(fy > 0.8 || fy < 0.2)
      {
        // delta.Y = 0;
      }

      if(fx > 0.8)
      {
        // delta.X = 0;
        rotationAxisY = dir;
      }

      if(fx < 0.2)
      {
        // delta.X = 0;
        rotationAxisY = -dir;
      }
    }

    /// <summary>
    /// Rotates around the camera up and right axes.
    /// </summary>
    /// <param name="p1">
    /// The previous mouse position.
    /// </param>
    /// <param name="p2">
    /// The current mouse position.
    /// </param>
    /// <param name="rotateAround">
    /// The point to rotate around.
    /// </param>
    private void RotateAroundUpAndRight(Point p1, Point p2, Point3D rotateAround)
    {
      var dp = p2 - p1;

      // Rotate around the camera up direction
      var delta1 = new Quaternion(CameraUpDirection, -dp.X * RotationSensitivity);

      // Rotate around the camera right direction
      var delta2 = new Quaternion(
          Vector3D.CrossProduct(CameraUpDirection, CameraLookDirection), dp.Y * RotationSensitivity);

      var delta = delta1 * delta2;
      var rotate = new RotateTransform3D(new QuaternionRotation3D(delta));
      var relativeTarget = rotateAround - CameraTarget;
      var relativePosition = rotateAround - CameraPosition;
      var newRelativeTarget = rotate.Transform(relativeTarget);
      var newRelativePosition = rotate.Transform(relativePosition);
      var newUpDirection = rotate.Transform(CameraUpDirection);
      var newTarget = rotateAround - newRelativeTarget;
      var newPosition = rotateAround - newRelativePosition;

      CameraLookDirection = newTarget - newPosition;
      if(CameraMode == CameraMode.Inspect)
      {
        CameraPosition = newPosition;
      }
      CameraUpDirection = newUpDirection;
    }

    /// <summary>
    /// The rotate trackball.
    /// </summary>
    /// <param name="p1">
    /// The previous mouse position.
    /// </param>
    /// <param name="p2">
    /// The current mouse position.
    /// </param>
    /// <param name="rotateAround">
    /// The point to rotate around.
    /// </param>
    private void RotateTrackball(Point p1, Point p2, Point3D rotateAround)
    {
      // http://viewport3d.com/trackball.htm
      // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
      var v1 = ProjectToTrackball(p1, ViewportWidth, ViewportHeight);
      var v2 = ProjectToTrackball(p2, ViewportWidth, ViewportHeight);

      // transform the trackball coordinates to view space
      var viewZ = CameraLookDirection;
      var viewX = Vector3D.CrossProduct(CameraUpDirection, viewZ);
      var viewY = Vector3D.CrossProduct(viewX, viewZ);
      viewX.Normalize();
      viewY.Normalize();
      viewZ.Normalize();
      var u1 = viewZ * v1.Z + viewX * v1.X + viewY * v1.Y;
      var u2 = viewZ * v2.Z + viewX * v2.X + viewY * v2.Y;

      // Could also use the Camera ViewMatrix
      // var vm = Viewport3DHelper.GetViewMatrix(this.ActualCamera);
      // vm.Invert();
      // var ct = new MatrixTransform3D(vm);
      // var u1 = ct.Transform(v1);
      // var u2 = ct.Transform(v2);

      // Find the rotation axis and angle
      var axis = Vector3D.CrossProduct(u1, u2);
      if(axis.LengthSquared < 1e-8)
      {
        return;
      }

      var angle = Vector3D.AngleBetween(u1, u2);

      // Create the transform
      var delta = new Quaternion(axis, -angle * RotationSensitivity * 5);
      var rotate = new RotateTransform3D(new QuaternionRotation3D(delta));

      // Find vectors relative to the rotate-around point
      var relativeTarget = rotateAround - CameraTarget;
      var relativePosition = rotateAround - CameraPosition;

      // Rotate the relative vectors
      var newRelativeTarget = rotate.Transform(relativeTarget);
      var newRelativePosition = rotate.Transform(relativePosition);
      var newUpDirection = rotate.Transform(CameraUpDirection);

      // Find new camera position
      var newTarget = rotateAround - newRelativeTarget;
      var newPosition = rotateAround - newRelativePosition;

      CameraLookDirection = newTarget - newPosition;
      if(CameraMode == CameraMode.Inspect)
      {
        CameraPosition = newPosition;
      }
      CameraUpDirection = newUpDirection;
    }
  }
}