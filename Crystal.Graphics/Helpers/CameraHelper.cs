namespace Crystal.Graphics
{
  /// <summary>
  /// Provides extension methods for <see cref="Camera"/> derived classes.
  /// </summary>
  public static class CameraHelper
  {
    /// <summary>
    /// Animates the camera position and directions.
    /// </summary>
    /// <param name="camera">
    /// The camera to animate.
    /// </param>
    /// <param name="newPosition">
    /// The position to animate to.
    /// </param>
    /// <param name="newDirection">
    /// The direction to animate to.
    /// </param>
    /// <param name="newUpDirection">
    /// The up direction to animate to.
    /// </param>
    /// <param name="animationTime">
    /// Animation time in milliseconds.
    /// </param>
    public static void AnimateTo(this ProjectionCamera? camera, Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime)
    {
      var fromPosition = camera.Position;
      var fromDirection = camera.LookDirection;
      var fromUpDirection = camera.UpDirection;

      camera.Position = newPosition;
      camera.LookDirection = newDirection;
      camera.UpDirection = newUpDirection;

      if(animationTime > 0)
      {
        var a1 = new Point3DAnimation(fromPosition, newPosition, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
          AccelerationRatio = 0.3,
          DecelerationRatio = 0.5,
          FillBehavior = FillBehavior.Stop
        };
        a1.Completed += (_, _) => camera.BeginAnimation(ProjectionCamera.PositionProperty, null);
        camera.BeginAnimation(ProjectionCamera.PositionProperty, a1);

        var a2 = new Vector3DAnimation(fromDirection, newDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
          AccelerationRatio = 0.3,
          DecelerationRatio = 0.5,
          FillBehavior = FillBehavior.Stop
        };
        a2.Completed += (_, _) => camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, null);
        camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, a2);

        var a3 = new Vector3DAnimation(fromUpDirection, newUpDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
          AccelerationRatio = 0.3,
          DecelerationRatio = 0.5,
          FillBehavior = FillBehavior.Stop
        };
        a3.Completed += (_, _) => camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, null);
        camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, a3);
      }
    }

    /// <summary>
    /// Animates the orthographic width.
    /// </summary>
    /// <param name="camera">
    /// An orthographic camera.
    /// </param>
    /// <param name="newWidth">
    /// The width to animate to.
    /// </param>
    /// <param name="animationTime">
    /// Animation time in milliseconds
    /// </param>
    public static void AnimateWidth(this OrthographicCamera camera, double newWidth, double animationTime)
    {
      var fromWidth = camera.Width;
      camera.Width = newWidth;
      if(animationTime > 0)
      {
        var a1 = new DoubleAnimation(fromWidth, newWidth, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
          AccelerationRatio = 0.3,
          DecelerationRatio = 0.5,
          FillBehavior = FillBehavior.Stop
        };
        camera.BeginAnimation(OrthographicCamera.WidthProperty, a1);
      }
    }

    /// <summary>
    /// Changes the direction of a camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="newLookDirection">
    /// The new look direction.
    /// </param>
    /// <param name="newUpDirection">
    /// The new up direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void ChangeDirection(this ProjectionCamera? camera, Vector3D newLookDirection, Vector3D newUpDirection, double animationTime)
    {
      var target = camera.Position + camera.LookDirection;
      var length = camera.LookDirection.Length;
      newLookDirection.Normalize();
      LookAt(camera, target, newLookDirection * length, newUpDirection, animationTime);
    }

    /// <summary>
    /// Copies the specified camera, converts field of view/width if necessary.
    /// </summary>
    /// <param name="source">The source camera.</param>
    /// <param name="dest">The destination camera.</param>
    /// <param name="copyNearFarPlaneDistances">Copy near and far plane distances if set to <c>true</c>.</param>
    public static void Copy(this ProjectionCamera? source, ProjectionCamera? dest, bool copyNearFarPlaneDistances = true)
    {
      if(source == null || dest == null)
      {
        return;
      }

      dest.LookDirection = source.LookDirection;
      dest.Position = source.Position;
      dest.UpDirection = source.UpDirection;

      if(copyNearFarPlaneDistances)
      {
        dest.NearPlaneDistance = source.NearPlaneDistance;
        dest.FarPlaneDistance = source.FarPlaneDistance;
      }

      var psrc = source as PerspectiveCamera;
      var osrc = source as OrthographicCamera;
      var odest = dest as OrthographicCamera;
      if(dest is PerspectiveCamera pdest)
      {
        double fov = 45;
        if(psrc != null)
        {
          fov = psrc.FieldOfView;
        }

        if(osrc != null)
        {
          var dist = source.LookDirection.Length;
          fov = Math.Atan(osrc.Width / 2 / dist) * 180 / Math.PI * 2;
        }

        pdest.FieldOfView = fov;
      }

      if(odest != null)
      {
        double width = 100;
        if(psrc != null)
        {
          var dist = source.LookDirection.Length;
          width = Math.Tan(psrc.FieldOfView / 180 * Math.PI / 2) * dist * 2;
        }

        if(osrc != null)
        {
          width = osrc.Width;
        }

        odest.Width = width;
      }
    }

    /// <summary>
    /// Copy the direction of the source <see cref="Camera"/>. Used for the CoordinateSystem view.
    /// </summary>
    /// <param name="source">
    /// The source camera.
    /// </param>
    /// <param name="dest">
    /// The destination camera.
    /// </param>
    /// <param name="distance">
    /// New length of the LookDirection vector.
    /// </param>
    public static void CopyDirectionOnly(this ProjectionCamera? source, ProjectionCamera? dest, double distance)
    {
      if(source == null || dest == null)
      {
        return;
      }

      var dir = source.LookDirection;
      dir.Normalize();
      dir *= distance;

      dest.LookDirection = dir;
      dest.Position = new Point3D(-dest.LookDirection.X, -dest.LookDirection.Y, -dest.LookDirection.Z);
      dest.UpDirection = source.UpDirection;
    }

    /// <summary>
    /// Creates a default perspective camera.
    /// </summary>
    /// <returns>A perspective camera.</returns>
    public static PerspectiveCamera CreateDefaultCamera()
    {
      var camera = new PerspectiveCamera();
      Reset(camera);
      return camera;
    }

    /// <summary>
    /// Gets an information string about the specified camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <returns>
    /// The get info.
    /// </returns>
    public static string GetInfo(this Camera? camera)
    {
      var matrixCamera = camera as MatrixCamera;
      var perspectiveCamera = camera as PerspectiveCamera;
      var projectionCamera = camera as ProjectionCamera;
      var orthographicCamera = camera as OrthographicCamera;
      var sb = new StringBuilder();
      sb.AppendLine(camera.GetType().Name);
      if(projectionCamera != null)
      {
        sb.AppendLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "LookDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                projectionCamera.LookDirection.X,
                projectionCamera.LookDirection.Y,
                projectionCamera.LookDirection.Z));
        sb.AppendLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "UpDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                projectionCamera.UpDirection.X,
                projectionCamera.UpDirection.Y,
                projectionCamera.UpDirection.Z));
        sb.AppendLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "Position:\t\t{0:0.000},{1:0.000},{2:0.000}",
                projectionCamera.Position.X,
                projectionCamera.Position.Y,
                projectionCamera.Position.Z));
        var target = projectionCamera.Position + projectionCamera.LookDirection;
        sb.AppendLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "Target:\t\t{0:0.000},{1:0.000},{2:0.000}",
                target.X,
                target.Y,
                target.Z));
        sb.AppendLine(
            string.Format(
                CultureInfo.InvariantCulture, "NearPlaneDist:\t{0}", projectionCamera.NearPlaneDistance));
        sb.AppendLine(
            string.Format(CultureInfo.InvariantCulture, "FarPlaneDist:\t{0}", projectionCamera.FarPlaneDistance));
      }

      if(perspectiveCamera != null)
      {
        sb.AppendLine(
            string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", perspectiveCamera.FieldOfView));
      }

      if(orthographicCamera != null)
      {
        sb.AppendLine(
            string.Format(CultureInfo.InvariantCulture, "Width:\t{0:0.###}", orthographicCamera.Width));
      }

      if(matrixCamera != null)
      {
        sb.AppendLine("ProjectionMatrix:");
        sb.AppendLine(matrixCamera.ProjectionMatrix.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("ViewMatrix:");
        sb.AppendLine(matrixCamera.ViewMatrix.ToString(CultureInfo.InvariantCulture));
      }

      return sb.ToString().Trim();
    }

    /// <summary>
    /// Set the camera target point without changing the look direction.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void LookAt(this ProjectionCamera? camera, Point3D target, double animationTime)
    {
      LookAt(camera, target, camera.LookDirection, animationTime);
    }

    /// <summary>
    /// Set the camera target point and look direction
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    /// <param name="newLookDirection">
    /// The new look direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void LookAt(this ProjectionCamera? camera, Point3D target, Vector3D newLookDirection, double animationTime)
    {
      LookAt(camera, target, newLookDirection, camera.UpDirection, animationTime);
    }

    /// <summary>
    /// Set the camera target point and directions
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    /// <param name="newLookDirection">
    /// The new look direction.
    /// </param>
    /// <param name="newUpDirection">
    /// The new up direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void LookAt(this ProjectionCamera? camera, Point3D target, Vector3D newLookDirection, Vector3D newUpDirection, double animationTime)
    {
      var newPosition = target - newLookDirection;

      if(camera is PerspectiveCamera perspectiveCamera)
      {
        AnimateTo(perspectiveCamera, newPosition, newLookDirection, newUpDirection, animationTime);
        return;
      }

      if(camera is OrthographicCamera orthographicCamera)
      {
        AnimateTo(orthographicCamera, newPosition, newLookDirection, newUpDirection, animationTime);
      }
    }

    /// <summary>
    /// Set the camera target point and camera distance.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="target">
    /// The target point.
    /// </param>
    /// <param name="distance">
    /// The distance to the camera.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void LookAt(this ProjectionCamera? camera, Point3D target, double distance, double animationTime)
    {
      var d = camera.LookDirection;
      d.Normalize();
      LookAt(camera, target, d * distance, animationTime);
    }

    /// <summary>
    /// Resets the specified camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    public static void Reset(this Camera? camera)
    {
      if(camera is PerspectiveCamera pcamera)
      {
        Reset(pcamera);
      }

      if(camera is OrthographicCamera ocamera)
      {
        Reset(ocamera);
      }
    }

    /// <summary>
    /// Resets the specified perspective camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    public static void Reset(this PerspectiveCamera? camera)
    {
      if(camera == null)
      {
        return;
      }

      camera.Position = new Point3D(2, 16, 20);
      camera.LookDirection = new Vector3D(-2, -16, -20);
      camera.UpDirection = new Vector3D(0, 0, 1);
      camera.FieldOfView = 45;
      camera.NearPlaneDistance = 0.1;
      camera.FarPlaneDistance = double.PositiveInfinity;
    }

    /// <summary>
    /// Resets the specified orthographic camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    public static void Reset(this OrthographicCamera? camera)
    {
      if(camera == null)
      {
        return;
      }

      camera.Position = new Point3D(2, 16, 20);
      camera.LookDirection = new Vector3D(-2, -16, -20);
      camera.UpDirection = new Vector3D(0, 0, 1);
      camera.Width = 40;
      camera.NearPlaneDistance = 0.1;
      camera.FarPlaneDistance = double.PositiveInfinity;
    }

    /// <summary>
    /// Obtains the view transform matrix for a camera. (see page 327)
    /// </summary>
    /// <param name="camera">
    /// Camera to obtain the ViewMatrix for
    /// </param>
    /// <returns>
    /// A Matrix3D object with the camera view transform matrix, or a Matrix3D with all zeros if the "camera" is null.
    /// </returns>
    public static Matrix3D GetViewMatrix(this Camera camera)
    {
      if(camera == null)
      {
        throw new ArgumentNullException(nameof(camera));
      }

      if(camera is MatrixCamera matrixCamera)
      {
        return matrixCamera.ViewMatrix;
      }

      if(camera is ProjectionCamera projectionCamera)
      {
        var zaxis = -projectionCamera.LookDirection;
        zaxis.Normalize();

        var xaxis = Vector3D.CrossProduct(projectionCamera.UpDirection, zaxis);
        xaxis.Normalize();

        var yaxis = Vector3D.CrossProduct(zaxis, xaxis);
        var pos = (Vector3D)projectionCamera.Position;

        return new Matrix3D(
            xaxis.X,
            yaxis.X,
            zaxis.X,
            0,
            xaxis.Y,
            yaxis.Y,
            zaxis.Y,
            0,
            xaxis.Z,
            yaxis.Z,
            zaxis.Z,
            0,
            -Vector3D.DotProduct(xaxis, pos),
            -Vector3D.DotProduct(yaxis, pos),
            -Vector3D.DotProduct(zaxis, pos),
            1);
      }

      throw new CrystalException("Unknown camera type.");
    }

    /// <summary>
    /// Gets the projection matrix for the specified camera.
    /// </summary>
    /// <param name="camera">The camera.</param>
    /// <param name="aspectRatio">The aspect ratio.</param>
    /// <returns>The projection matrix.</returns>
    public static Matrix3D GetProjectionMatrix(this Camera camera, double aspectRatio)
    {
      if(camera == null)
      {
        throw new ArgumentNullException(nameof(camera));
      }

      if(camera is PerspectiveCamera perspectiveCamera)
      {
        // The angle-to-radian formula is a little off because only
        // half the angle enters the calculation.
        var xscale = 1 / Math.Tan(Math.PI * perspectiveCamera.FieldOfView / 360);
        var yscale = xscale * aspectRatio;
        var znear = perspectiveCamera.NearPlaneDistance;
        var zfar = perspectiveCamera.FarPlaneDistance;
        var zscale = double.IsPositiveInfinity(zfar) ? -1 : (zfar / (znear - zfar));
        var zoffset = znear * zscale;

        return new Matrix3D(xscale, 0, 0, 0, 0, yscale, 0, 0, 0, 0, zscale, -1, 0, 0, zoffset, 0);
      }

      if(camera is OrthographicCamera orthographicCamera)
      {
        var xscale = 2.0 / orthographicCamera.Width;
        var yscale = xscale * aspectRatio;
        var znear = orthographicCamera.NearPlaneDistance;
        var zfar = orthographicCamera.FarPlaneDistance;

        if(double.IsPositiveInfinity(zfar))
        {
          zfar = znear * 1e5;
        }

        var dzinv = 1.0 / (znear - zfar);

        var m = new Matrix3D(xscale, 0, 0, 0, 0, yscale, 0, 0, 0, 0, dzinv, 0, 0, 0, znear * dzinv, 1);
        return m;
      }

      if(camera is MatrixCamera matrixCamera)
      {
        return matrixCamera.ProjectionMatrix;
      }

      throw new CrystalException("Unknown camera type.");
    }

    /// <summary>
    /// Gets the combined view and projection transform.
    /// </summary>
    /// <param name="camera">The camera.</param>
    /// <param name="aspectRatio">The aspect ratio.</param>
    /// <returns>The total view and projection transform.</returns>
    public static Matrix3D GetTotalTransform(this Camera camera, double aspectRatio)
    {
      var m = Matrix3D.Identity;

      if(camera == null)
      {
        throw new ArgumentNullException(nameof(camera));
      }

      if(camera.Transform != null)
      {
        var cameraTransform = camera.Transform.Value;

        if(!cameraTransform.HasInverse)
        {
          throw new CrystalException("Camera transform has no inverse.");
        }

        cameraTransform.Invert();
        m.Append(cameraTransform);
      }

      m.Append(GetViewMatrix(camera));
      m.Append(GetProjectionMatrix(camera, aspectRatio));
      return m;
    }

    /// <summary>
    /// Gets the inverse camera transform.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="aspectRatio">
    /// The aspect ratio.
    /// </param>
    /// <returns>
    /// The inverse transform.
    /// </returns>
    public static Matrix3D GetInverseTransform(this Camera camera, double aspectRatio)
    {
      var m = GetTotalTransform(camera, aspectRatio);

      if(!m.HasInverse)
      {
        throw new CrystalException("Camera transform has no inverse.");
      }

      m.Invert();
      return m;
    }

    /// <summary>
    /// Fits the current scene in the current view.
    /// </summary>
    /// <param name="camera">The actual camera.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="animationTime">The animation time.</param>
    public static void FitView(
        this ProjectionCamera? camera,
        Viewport3D viewport,
        double animationTime = 0)
    {
      if(camera is PerspectiveCamera perspectiveCamera)
      {
        FitView(camera, viewport, perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, animationTime);
        return;
      }

      if(camera is OrthographicCamera orthoCamera)
      {
        FitView(camera, viewport, orthoCamera.LookDirection, orthoCamera.UpDirection, animationTime);
      }
    }

    /// <summary>
    /// Fits the current scene in the current view.
    /// </summary>
    /// <param name="camera">The actual camera.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="lookDirection">The look direction.</param>
    /// <param name="upDirection">The up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public static void FitView(this ProjectionCamera? camera, Viewport3D viewport, Vector3D lookDirection, Vector3D upDirection, double animationTime = 0)
    {
      var bounds = viewport.Children.FindBounds();
      var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

      if(bounds.IsEmpty || diagonal.LengthSquared < double.Epsilon)
      {
        return;
      }

      FitView(camera, viewport, bounds, lookDirection, upDirection, animationTime);
    }

    /// <summary>
    /// Zooms to fit the extents of the specified viewport.
    /// </summary>
    /// <param name="camera">
    /// The actual camera.
    /// </param>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void ZoomExtents(this ProjectionCamera? camera, Viewport3D viewport, double animationTime = 0)
    {
      var bounds = viewport.Children.FindBounds();
      var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

      if(bounds.IsEmpty || diagonal.LengthSquared < double.Epsilon)
      {
        return;
      }

      ZoomExtents(camera, viewport, bounds, animationTime);
    }

    /// <summary>
    /// Zooms to fit the specified bounding rectangle.
    /// </summary>
    /// <param name="camera">
    /// The camera to change.
    /// </param>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="bounds">
    /// The bounding rectangle.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void ZoomExtents(
        this ProjectionCamera? camera,
        Viewport3D viewport,
        Rect3D bounds,
        double animationTime = 0)
    {
      if(camera is PerspectiveCamera perspectiveCamera)
      {
        FitView(camera, viewport, bounds, perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, animationTime);
        return;
      }

      if(camera is OrthographicCamera orthoCamera)
      {
        FitView(camera, viewport, bounds, orthoCamera.LookDirection, orthoCamera.UpDirection, animationTime);
      }
    }

    /// <summary>
    /// Fits the specified bounding rectangle in the current view.
    /// </summary>
    /// <param name="camera">The camera to change.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="bounds">The bounding rectangle.</param>
    /// <param name="lookDirection">The look direction.</param>
    /// <param name="upDirection">The up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public static void FitView(this ProjectionCamera? camera, Viewport3D viewport, Rect3D bounds, Vector3D lookDirection, Vector3D upDirection, double animationTime = 0)
    {
      var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
      var center = bounds.Location + (diagonal * 0.5);
      var radius = diagonal.Length * 0.5;
      FitView(camera, viewport, center, radius, lookDirection, upDirection, animationTime);
    }

    /// <summary>
    /// Zooms to fit the specified sphere.
    /// </summary>
    /// <param name="camera">
    /// The camera to change.
    /// </param>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="center">
    /// The center of the sphere.
    /// </param>
    /// <param name="radius">
    /// The radius of the sphere.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public static void ZoomExtents(
        ProjectionCamera? camera,
        Viewport3D viewport,
        Point3D center,
        double radius,
        double animationTime = 0)
    {
      if(camera is PerspectiveCamera perspectiveCamera)
      {
        FitView(camera, viewport, center, radius, perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, animationTime);
        return;
      }

      if(camera is OrthographicCamera orthoCamera)
      {
        FitView(camera, viewport, center, radius, orthoCamera.LookDirection, orthoCamera.UpDirection, animationTime);
      }
    }

    /// <summary>
    /// Fits the specified bounding sphere to the view.
    /// </summary>
    /// <param name="camera">The camera to change.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="center">The center of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="lookDirection">The look direction.</param>
    /// <param name="upDirection">The up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public static void FitView(
        ProjectionCamera? camera,
        Viewport3D viewport,
        Point3D center,
        double radius,
        Vector3D lookDirection,
        Vector3D upDirection,
        double animationTime = 0)
    {
      if(camera is PerspectiveCamera perspectiveCamera)
      {
        var pcam = perspectiveCamera;
        var disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
        var vfov = pcam.FieldOfView;
        if(viewport.ActualWidth > 0 && viewport.ActualHeight > 0)
        {
          vfov *= viewport.ActualHeight / viewport.ActualWidth;
        }

        var distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);
        var dist = Math.Max(disth, distv);
        var dir = lookDirection;
        dir.Normalize();
        LookAt(perspectiveCamera, center, dir * dist, upDirection, animationTime);
        return;
      }

      if(camera is OrthographicCamera orthographicCamera)
      {
        var dir = lookDirection;
        dir.Normalize();
        LookAt(orthographicCamera, center, dir, upDirection, animationTime);
        var newWidth = radius * 2;

        if(viewport.ActualWidth > viewport.ActualHeight)
        {
          newWidth = radius * 2 * viewport.ActualWidth / viewport.ActualHeight;
        }

        AnimateWidth(orthographicCamera, newWidth, animationTime);
      }
    }

    /// <summary>
    /// Zooms the camera to the specified rectangle.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="zoomRectangle">
    /// The zoom rectangle.
    /// </param>
    public static void ZoomToRectangle(this ProjectionCamera? camera, Viewport3D viewport, Rect zoomRectangle)
    {
      var topLeftRay = viewport.Point2DtoRay3D(zoomRectangle.TopLeft);
      var topRightRay = viewport.Point2DtoRay3D(zoomRectangle.TopRight);
      var centerRay = viewport.Point2DtoRay3D(new Point((zoomRectangle.Left + zoomRectangle.Right) * 0.5, (zoomRectangle.Top + zoomRectangle.Bottom) * 0.5));

      if(topLeftRay == null || topRightRay == null || centerRay == null)
      {
        // could not invert camera matrix
        return;
      }

      var u = topLeftRay.Direction;
      var v = topRightRay.Direction;
      var w = centerRay.Direction;
      u.Normalize();
      v.Normalize();
      w.Normalize();
      if(camera is PerspectiveCamera perspectiveCamera)
      {
        var distance = camera.LookDirection.Length;

        // option 1: change distance
        var newDistance = distance * zoomRectangle.Width / viewport.ActualWidth;
        var newLookDirection = newDistance * w;
        var newPosition = perspectiveCamera.Position + ((distance - newDistance) * w);
        var newTarget = newPosition + newLookDirection;
        LookAt(camera, newTarget, newLookDirection, 200);

        // option 2: change fov
        //    double newFieldOfView = Math.Acos(Vector3D.DotProduct(u, v));
        //    var newTarget = camera.Position + distance * w;
        //    pcamera.FieldOfView = newFieldOfView * 180 / Math.PI;
        //    LookAt(camera, newTarget, distance * w, 0);
      }

      if(camera is OrthographicCamera orthographicCamera)
      {
        orthographicCamera.Width *= zoomRectangle.Width / viewport.ActualWidth;
        var oldTarget = camera.Position + camera.LookDirection;
        var distance = camera.LookDirection.Length;
        if(centerRay.PlaneIntersection(oldTarget, w, out var newTarget))
        {
          orthographicCamera.LookDirection = w * distance;
          orthographicCamera.Position = newTarget - orthographicCamera.LookDirection;
        }
      }
    }
  }
}