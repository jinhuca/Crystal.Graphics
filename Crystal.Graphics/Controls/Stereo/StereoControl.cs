using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace Crystal.Graphics
{
  /// <summary>
  /// Base class for controls that use stereo cameras
  /// </summary>
  [ContentProperty("Content")]
  public class StereoControl : ContentControl
  {
    // todo: keyboard shortcut 'x' to change cross/parallel viewing
    /// <summary>
    /// Identifies the <see cref="Camera"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
      nameof(Camera), typeof(PerspectiveCamera), typeof(StereoControl), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="CameraRotationMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraRotationModeProperty = DependencyProperty.Register(
      nameof(CameraRotationMode), typeof(CameraRotationMode), typeof(StereoControl), new UIPropertyMetadata(CameraRotationMode.Turntable));

    /// <summary>
    /// Identifies the <see cref="CopyDirectionVector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CopyDirectionVectorProperty = DependencyProperty.Register(
      nameof(CopyDirectionVector), typeof(bool), typeof(StereoControl), new UIPropertyMetadata(true, StereoViewChanged));

    /// <summary>
    /// Identifies the <see cref="CopyUpVector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CopyUpVectorProperty = DependencyProperty.Register(
      nameof(CopyUpVector), typeof(bool), typeof(StereoControl), new UIPropertyMetadata(false, StereoViewChanged));

    /// <summary>
    /// Identifies the <see cref="CrossViewing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CrossViewingProperty = DependencyProperty.Register(
      nameof(CrossViewing), typeof(bool), typeof(StereoControl), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="StereoBase"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StereoBaseProperty = DependencyProperty.Register(
      nameof(StereoBase), typeof(double), typeof(StereoControl), new UIPropertyMetadata(0.12, StereoViewChanged));

    /// <summary>
    /// Initializes static members of the <see cref="StereoControl"/> class.
    /// </summary>
    static StereoControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StereoControl), new FrameworkPropertyMetadata(typeof(StereoControl)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StereoControl"/> class.
    /// </summary>
    public StereoControl()
    {
      Camera = CameraHelper.CreateDefaultCamera();
      Camera.Changed += CameraChanged;
      Children = new ObservableCollection<Visual3D>();
    }

    /*        void StereoControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.X)
        {
            CrossViewing = !CrossViewing;
        }
    }
    */
    /// <summary>
    /// Gets or sets the camera.
    /// </summary>
    /// <value>The camera.</value>
    public PerspectiveCamera Camera
    {
      get => (PerspectiveCamera)GetValue(CameraProperty);
      set => SetValue(CameraProperty, value);
    }

    /// <summary>
    /// Gets or sets the camera rotation mode.
    /// </summary>
    /// <value>The camera rotation mode.</value>
    public CameraRotationMode CameraRotationMode
    {
      get => (CameraRotationMode)GetValue(CameraRotationModeProperty);
      set => SetValue(CameraRotationModeProperty, value);
    }

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>The children.</value>
    public ObservableCollection<Visual3D> Children { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether [copy direction vector].
    /// </summary>
    /// <value><c>true</c> if [copy direction vector]; otherwise, <c>false</c>.</value>
    public bool CopyDirectionVector
    {
      get => (bool)GetValue(CopyDirectionVectorProperty);
      set => SetValue(CopyDirectionVectorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [copy up vector].
    /// </summary>
    /// <value><c>true</c> if [copy up vector]; otherwise, <c>false</c>.</value>
    public bool CopyUpVector
    {
      get => (bool)GetValue(CopyUpVectorProperty);
      set => SetValue(CopyUpVectorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the cameras are set up for cross viewing.
    /// </summary>
    /// <value><c>true</c> if [cross viewing]; otherwise, <c>false</c>.</value>
    public bool CrossViewing
    {
      get => (bool)GetValue(CrossViewingProperty);
      set => SetValue(CrossViewingProperty, value);
    }

    /// <summary>
    /// Gets or sets the left camera.
    /// </summary>
    /// <value>The left camera.</value>
    public PerspectiveCamera LeftCamera { get; set; }

    /// <summary>
    /// Gets or sets the left viewport.
    /// </summary>
    /// <value>The left viewport.</value>
    public Viewport3D LeftViewport { get; set; }

    /// <summary>
    /// Gets or sets the right camera.
    /// </summary>
    /// <value>The right camera.</value>
    public PerspectiveCamera RightCamera { get; set; }

    /// <summary>
    /// Gets or sets the right viewport.
    /// </summary>
    /// <value>The right viewport.</value>
    public Viewport3D RightViewport { get; set; }

    /// <summary>
    /// Gets or sets the stereo base.
    /// </summary>
    /// <value>The stereo base.</value>
    public double StereoBase
    {
      get => (double)GetValue(StereoBaseProperty);
      set => SetValue(StereoBaseProperty, value);
    }

    /// <summary>
    /// Binds the viewports.
    /// </summary>
    /// <param name="left">
    /// The left.
    /// </param>
    /// <param name="right">
    /// The right.
    /// </param>
    public void BindViewports(Viewport3D left, Viewport3D right) => BindViewports(left, right, true, true);

    /// <summary>
    /// Binds the viewports.
    /// </summary>
    /// <param name="left">
    /// The left.
    /// </param>
    /// <param name="right">
    /// The right.
    /// </param>
    /// <param name="createLights">
    /// if set to <c>true</c> [create lights].
    /// </param>
    /// <param name="createCamera">
    /// if set to <c>true</c> [create camera].
    /// </param>
    public void BindViewports(Viewport3D left, Viewport3D right, bool createLights, bool createCamera)
    {
      LeftViewport = left;
      RightViewport = right;

      Children.CollectionChanged += ChildrenCollectionChanged;

      if(createLights)
      {
        Children.Add(new DefaultLights());
      }

      if(createCamera)
      {
        if(LeftViewport.Camera == null)
        {
          LeftViewport.Camera = CameraHelper.CreateDefaultCamera();
        }
        else
        {
          CameraHelper.Reset(LeftViewport.Camera as PerspectiveCamera);
        }

        if(RightViewport != null && RightViewport.Camera == null)
        {
          RightViewport.Camera = new PerspectiveCamera();
        }
      }

      LeftCamera = LeftViewport.Camera as PerspectiveCamera;
      if(RightViewport != null)
      {
        RightCamera = RightViewport.Camera as PerspectiveCamera;
      }

      UpdateCameras();
    }

    /// <summary>
    /// Clears the children collection.
    /// </summary>
    public void Clear()
    {
      Children.Clear();
      SynchronizeStereoModel();
    }

    /// <summary>
    /// Exports the views to kerkythea.
    /// </summary>
    /// <param name="leftFileName">
    /// Name of the left file.
    /// </param>
    /// <param name="rightFileName">
    /// Name of the right file.
    /// </param>
    public void ExportKerkythea(string leftFileName, string rightFileName)
    {
      var scb = Background as SolidColorBrush;

      var leftExporter = new KerkytheaExporter();
      if(scb != null)
      {
        leftExporter.BackgroundColor = scb.Color;
      }

      leftExporter.Reflections = true;
      leftExporter.Shadows = true;
      leftExporter.SoftShadows = true;
      leftExporter.Width = (int)LeftViewport.ActualWidth;
      leftExporter.Height = (int)LeftViewport.ActualHeight;
      using(var stream = File.Create(leftFileName))
      {
        leftExporter.Export(LeftViewport, stream);
      }

      var rightExporter = new KerkytheaExporter();
      if(scb != null)
      {
        rightExporter.BackgroundColor = scb.Color;
      }

      rightExporter.Reflections = true;
      rightExporter.Shadows = true;
      rightExporter.SoftShadows = true;
      rightExporter.Width = (int)RightViewport.ActualWidth;
      rightExporter.Height = (int)RightViewport.ActualHeight;
      using(var stream = File.Create(rightFileName))
      {
        rightExporter.Export(RightViewport, stream);
      }
    }

    /// <summary>
    /// Synchronizes the stereo model.
    /// </summary>
    public void SynchronizeStereoModel()
    {
      LeftViewport.Children.Clear();
      if(RightViewport != null)
      {
        RightViewport.Children.Clear();
      }

      foreach(var v in Children)
      {
        LeftViewport.Children.Add(v);
        if(RightViewport != null)
        {
          Visual3D clone = StereoHelper.CreateClone(v);
          if(clone != null)
          {
            RightViewport.Children.Add(clone);
          }
        }
      }
    }

    /// <summary>
    /// Updates the cameras.
    /// </summary>
    public void UpdateCameras()
    {
      StereoHelper.UpdateStereoCameras(Camera, LeftCamera, RightCamera, StereoBase, CrossViewing, CopyUpVector, CopyDirectionVector);
    }

    /// <summary>
    /// The stereo view changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void StereoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var v = (StereoControl)d;
      v.UpdateCameras();
    }

    /// <summary>
    /// Handle the camera changed event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void CameraChanged(object sender, EventArgs e)
    {
      UpdateCameras();
    }

    /// <summary>
    /// Handle changes in the children collection.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void ChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // todo: update left and right collections here
    }

  }
}