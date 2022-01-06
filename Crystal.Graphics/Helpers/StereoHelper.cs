namespace Crystal.Graphics
{
  /// <summary>
  /// Contains helper methods for stereoscopic views.
  /// </summary>
  public static class StereoHelper
  {
    /// <summary>
    /// Calculate the stereo base using the full Bercovitz formula
    /// </summary>
    /// <param name="L">
    /// Largest distance from the camera lens
    /// </param>
    /// <param name="N">
    /// Nearest distance from the camera lens
    /// </param>
    /// <param name="screenWidth">
    /// Width of screen
    /// </param>
    /// <param name="depthRatio">
    /// depth ratio 1/30
    /// </param>
    /// <param name="hfov">
    /// Horizontal field of view
    /// </param>
    /// <returns>
    /// The stereo base
    /// </returns>
    public static double CalculateStereoBase(double L, double N, double screenWidth, double depthRatio, double hfov)
    {
      var formatHoriz = screenWidth;
      var F = FindFocalLength(hfov, formatHoriz);
      var P = depthRatio * formatHoriz;
      return CalculateStereoBase(P, L, N, F);
    }

    /// <summary>
    /// Calculate the stereo base using the full Bercovitz formula
    /// B = P(LN/(L-N)) (1/F - (L+N)/2LN)
    /// http://nzphoto.tripod.com/stereo/3dtake/fbercowitz.htm
    /// </summary>
    /// <param name="P">
    /// Parallax aimed for, in mm on the film
    /// </param>
    /// <param name="L">
    /// Largest distance from the camera lens, mm
    /// </param>
    /// <param name="N">
    /// Nearest distance from the camera lens, mm
    /// </param>
    /// <param name="F">
    /// Focal length of the lens, mm
    /// </param>
    /// <returns>
    /// The stereo base
    /// </returns>
    public static double CalculateStereoBase(double P, double L, double N, double F)
    {
      return P * (L * N / (L - N)) * (1 / F - (L + N) / (2 * L * N));
    }

    /// <summary>
    /// Create a clone of a Visual3D
    /// </summary>
    /// <param name="v">
    /// a Visual3D
    /// </param>
    /// <returns>
    /// the clone
    /// </returns>
    public static Visual3D? CreateClone(Visual3D v)
    {
      switch (v)
      {
        case ModelUIElement3D element3D:
        {
          var m = element3D;
          if(m.Model != null)
          {
            /*if (m.Model.CanFreeze)
              m.Model.Freeze();
          if (m.Model.IsFrozen)*/
            {
              var clonedModel = m.Model.Clone();
              var clonedElement = new ModelUIElement3D
              {
                Transform = m.Transform,
                Model = clonedModel
              };
              return clonedElement;
            }
          }

          break;
        }
        case ModelVisual3D visual3D:
        {
          var m = visual3D;
          var clone = new ModelVisual3D
          {
            Transform = m.Transform
          };
          if(m.Content is { CanFreeze: true })
          {
            m.Content.Freeze();
            var clonedModel = m.Content.Clone();
            clone.Content = clonedModel;
          }

          switch (m.Children.Count)
          {
            case > 0:
            {
              foreach(var child in m.Children)
              {
                var clonedChild = CreateClone(child);
                clone.Children.Add(clonedChild);
              }

              break;
            }
          }

          return clone;
        }
      }

      return null;
    }

    /// <summary>
    /// Find the focal length given the field of view and the format
    /// http://en.wikipedia.org/wiki/Angle_of_view
    /// </summary>
    /// <param name="fov">
    /// field of view (degrees)
    /// </param>
    /// <param name="format">
    /// e.g. 36mm
    /// </param>
    /// <returns>
    /// The focal length in the same unit as the format
    /// </returns>
    public static double FindFocalLength(double fov, double format)
    {
      return format / 2 / Math.Tan(fov / 2 * Math.PI / 180);
    }

    /// <summary>
    /// Updates the left and right camera based on a center camera.
    /// </summary>
    /// <param name="centerCamera">Center camera (input)</param>
    /// <param name="leftCamera">Left camera (is updated)</param>
    /// <param name="rightCamera">Right camera (is updated)</param>
    /// <param name="stereoBase">Stereo base</param>
    /// <param name="crossViewing">true for cross-viewing, false for parallel-viewing (default is <c>false</c>)</param>
    /// <param name="sameUpDirection">use the same UpDirection for both cameras (default is <c>true</c>)</param>
    /// <param name="sameDirection">use the same LookDirection for both cameras (default is <c>true</c>)</param>
    public static void UpdateStereoCameras(
        PerspectiveCamera? centerCamera,
        PerspectiveCamera? leftCamera,
        PerspectiveCamera? rightCamera,
        double stereoBase,
        bool crossViewing = false,
        bool sameUpDirection = true,
        bool sameDirection = true)
    {
      if(centerCamera == null || leftCamera == null || rightCamera == null)
      {
        return;
      }

      if(crossViewing)
      {
        stereoBase *= -1;
      }

      var lookAt = centerCamera.Position + centerCamera.LookDirection;
      var right = Vector3D.CrossProduct(centerCamera.LookDirection, centerCamera.UpDirection);
      right.Normalize();

      leftCamera.Position = centerCamera.Position - (right * stereoBase / 2);
      rightCamera.Position = centerCamera.Position + (right * stereoBase / 2);

      if(sameDirection)
      {
        leftCamera.LookDirection = centerCamera.LookDirection;
        rightCamera.LookDirection = centerCamera.LookDirection;
      }
      else
      {
        // both cameras looking towards the lookAt point
        leftCamera.LookDirection = lookAt - leftCamera.Position;
        rightCamera.LookDirection = lookAt - rightCamera.Position;
      }

      // TODO: not sure what is best here?
      if(sameUpDirection)
      {
        leftCamera.UpDirection = centerCamera.UpDirection;
        rightCamera.UpDirection = centerCamera.UpDirection;
      }
      else
      {
        leftCamera.UpDirection = Vector3D.CrossProduct(right, leftCamera.LookDirection);
        rightCamera.UpDirection = Vector3D.CrossProduct(right, rightCamera.LookDirection);
      }

      leftCamera.FieldOfView = centerCamera.FieldOfView;
      leftCamera.NearPlaneDistance = centerCamera.NearPlaneDistance;
      leftCamera.FarPlaneDistance = centerCamera.FarPlaneDistance;

      rightCamera.FieldOfView = centerCamera.FieldOfView;
      rightCamera.NearPlaneDistance = centerCamera.NearPlaneDistance;
      rightCamera.FarPlaneDistance = centerCamera.FarPlaneDistance;
    }
  }
}