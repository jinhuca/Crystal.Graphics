namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a camera state.
  /// </summary>
  public class CameraSetting
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CameraSetting"/> class.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    public CameraSetting(ProjectionCamera camera)
    {
      Position = camera.Position;
      LookDirection = camera.LookDirection;
      UpDirection = camera.UpDirection;
      NearPlaneDistance = camera.NearPlaneDistance;
      FarPlaneDistance = camera.FarPlaneDistance;
      var pcamera = camera as PerspectiveCamera;
      if(pcamera != null)
      {
        FieldOfView = pcamera.FieldOfView;
      }

      var ocamera = camera as OrthographicCamera;
      if(ocamera != null)
      {
        Width = ocamera.Width;
      }
    }

    /// <summary>
    /// Gets or sets the far plane distance.
    /// </summary>
    public double FarPlaneDistance { get; set; }

    /// <summary>
    /// Gets or sets the field of view.
    /// </summary>
    public double FieldOfView { get; set; }

    /// <summary>
    /// Gets or sets the look direction.
    /// </summary>
    public Vector3D LookDirection { get; set; }

    /// <summary>
    /// Gets or sets the near plane distance.
    /// </summary>
    public double NearPlaneDistance { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Point3D Position { get; set; }

    /// <summary>
    /// Gets or sets the up direction.
    /// </summary>
    public Vector3D UpDirection { get; set; }

    /// <summary>
    /// Gets or sets the width of an orthographic camera.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Updates the camera to this state.
    /// </summary>
    /// <param name="camera">
    /// The camera to update.
    /// </param>
    public void UpdateCamera(ProjectionCamera camera)
    {
      camera.Position = Position;
      camera.LookDirection = LookDirection;
      camera.UpDirection = UpDirection;
      camera.NearPlaneDistance = NearPlaneDistance;
      camera.FarPlaneDistance = FarPlaneDistance;
      var pcamera = camera as PerspectiveCamera;
      if(pcamera != null)
      {
        pcamera.FieldOfView = FieldOfView;
      }

      var ocamera = camera as OrthographicCamera;
      if(ocamera != null)
      {
        ocamera.Width = Width;
      }
    }
  }
}