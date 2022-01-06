namespace Crystal.Graphics
{
  /// <summary>
  /// A stereoscopic Viewport3D control.
  /// </summary>
  public partial class StereoView3D
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StereoView3D"/> class.
    /// </summary>
    public StereoView3D()
    {
      InitializeComponent();
      BindViewports(LeftView, RightView);
    }

  }
}