using System.Diagnostics;

namespace Crystal.Graphics
{
  /// <summary>
  /// A stereoscopic wiggle control.
  /// </summary>
  public partial class WiggleView3D : StereoControl
  {
    /// <summary>
    /// Identifies the <see cref="WiggleRate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WiggleRateProperty = DependencyProperty.Register(
      nameof(WiggleRate), typeof(double), typeof(WiggleView3D), new UIPropertyMetadata(5.0, WiggleRateChanged));

    /// <summary>
    /// The timer.
    /// </summary>
    private readonly DispatcherTimer timer = new DispatcherTimer();

    /// <summary>
    /// The watch.
    /// </summary>
    private readonly Stopwatch watch = new Stopwatch();

    /// <summary>
    /// Initializes a new instance of the <see cref = "WiggleView3D" /> class.
    /// </summary>
    public WiggleView3D()
    {
      InitializeComponent();

      RightCamera = new PerspectiveCamera();
      BindViewports(View1, null, true, true);

      Loaded += ControlLoaded;
      Unloaded += ControlUnloaded;

      UpdateTimer();
      watch.Start();
      renderingEventListener = new RenderingEventListener(OnCompositionTargetRendering);
    }

    private readonly RenderingEventListener renderingEventListener;

    private void ControlUnloaded(object sender, RoutedEventArgs e) => RenderingEventManager.RemoveListener(renderingEventListener);

    private void ControlLoaded(object sender, RoutedEventArgs e) => RenderingEventManager.AddListener(renderingEventListener);

    /// <summary>
    /// Wiggles per second
    /// </summary>
    public double WiggleRate
    {
      get => (double)GetValue(WiggleRateProperty);
      set => SetValue(WiggleRateProperty, value);
    }

    /// <summary>
    /// The wiggle rate changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void WiggleRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WiggleView3D)d).UpdateTimer();

    /// <summary>
    /// The composition target_ rendering.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void OnCompositionTargetRendering(object sender, EventArgs e)
    {
      if(watch.ElapsedMilliseconds > 1000 / WiggleRate)
      {
        watch.Reset();
        watch.Start();
        Wiggle();
      }
    }

    /// <summary>
    /// The update timer.
    /// </summary>
    private void UpdateTimer()
    {
      timer.Interval = TimeSpan.FromSeconds(1.0 / WiggleRate);
    }

    /// <summary>
    /// Toggle between left and right camera.
    /// </summary>
    private void Wiggle()
    {
      View1.Camera = View1.Camera == LeftCamera ? RightCamera : LeftCamera;
    }
  }
}