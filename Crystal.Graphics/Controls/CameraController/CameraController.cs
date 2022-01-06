using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a control that manipulates the camera by mouse and keyboard gestures.
  /// </summary>
  public class CameraController : Grid
  {
    /// <summary>
    /// Identifies the <see cref="CameraMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
      nameof(CameraMode), typeof(CameraMode), typeof(CameraController), new UIPropertyMetadata(CameraMode.Inspect));

    /// <summary>
    /// Identifies the <see cref="Camera"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
      nameof(Camera), typeof(ProjectionCamera), typeof(CameraController), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CameraChanged));

    /// <summary>
    /// Identifies the <see cref="CameraRotationMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraRotationModeProperty = DependencyProperty.Register(
      nameof(CameraRotationMode), typeof(CameraRotationMode), typeof(CameraController), new UIPropertyMetadata(CameraRotationMode.Turntable));

    /// <summary>
    /// Identifies the <see cref="ChangeFieldOfViewCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChangeFieldOfViewCursorProperty = DependencyProperty.Register(
      nameof(ChangeFieldOfViewCursor), typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.ScrollNS));

    /// <summary>
    /// Identifies the <see cref="DefaultCamera"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
      nameof(DefaultCamera), typeof(ProjectionCamera), typeof(CameraController), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Enabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
      nameof(Enabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="InertiaFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InertiaFactorProperty = DependencyProperty.Register(
      nameof(InertiaFactor), typeof(double), typeof(CameraController), new UIPropertyMetadata(0.9));

    /// <summary>
    /// Identifies the <see cref="InfiniteSpin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
      nameof(InfiniteSpin), typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsChangeFieldOfViewEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty = DependencyProperty.Register(
      nameof(IsChangeFieldOfViewEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsInertiaEnabledProperty = DependencyProperty.Register(
      nameof(IsInertiaEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsMoveEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
      nameof(IsMoveEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsPanEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
      nameof(IsPanEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsRotationEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsRotationEnabledProperty = DependencyProperty.Register(
      nameof(IsRotationEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsTouchZoomEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTouchZoomEnabledProperty = DependencyProperty.Register(
      nameof(IsTouchZoomEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsZoomEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
      nameof(IsZoomEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="LeftRightPanSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftRightPanSensitivityProperty = DependencyProperty.Register(
      nameof(LeftRightPanSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="LeftRightRotationSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftRightRotationSensitivityProperty = DependencyProperty.Register(
      nameof(LeftRightRotationSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// The look at (target) point changed event
    /// </summary>
    public static readonly RoutedEvent LookAtChangedEvent = EventManager.RegisterRoutedEvent(
      nameof(LookAtChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CameraController));

    /// <summary>
    /// Identifies the <see cref="MaximumFieldOfView"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumFieldOfViewProperty = DependencyProperty.Register(
      nameof(MaximumFieldOfView), typeof(double), typeof(CameraController), new UIPropertyMetadata(160.0));

    /// <summary>
    /// Identifies the <see cref="MinimumFieldOfView"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumFieldOfViewProperty = DependencyProperty.Register(
      nameof(MinimumFieldOfView), typeof(double), typeof(CameraController), new UIPropertyMetadata(5.0));

    /// <summary>
    /// Identifies the <see cref="ModelUpDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModelUpDirectionProperty = DependencyProperty.Register(
      nameof(ModelUpDirection), typeof(Vector3D), typeof(CameraController), new UIPropertyMetadata(new Vector3D(0, 0, 1)));

    /// <summary>
    /// Identifies the <see cref="MoveSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MoveSensitivityProperty = DependencyProperty.Register(
      nameof(MoveSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="PageUpDownZoomSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PageUpDownZoomSensitivityProperty = DependencyProperty.Register(
      nameof(PageUpDownZoomSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="PanCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
      nameof(PanCursor), typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.Hand));

    /// <summary>
    /// Identifies the <see cref="RotateAroundMouseDownPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotateAroundMouseDownPointProperty = DependencyProperty.Register(
      nameof(RotateAroundMouseDownPoint), typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="FixedRotationPointEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FixedRotationPointEnabledProperty = DependencyProperty.Register(
      nameof(FixedRotationPointEnabled), typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="FixedRotationPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FixedRotationPointProperty = DependencyProperty.Register(
      nameof(FixedRotationPoint), typeof(Point3D), typeof(CameraController), new UIPropertyMetadata(default(Point3D)));

    /// <summary>
    /// Identifies the <see cref="RotateCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
      nameof(RotateCursor), typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.SizeAll));

    /// <summary>
    /// Identifies the <see cref="RotationSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationSensitivityProperty = DependencyProperty.Register(
      nameof(RotationSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="ShowCameraTarget"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCameraTargetProperty = DependencyProperty.Register(
      nameof(ShowCameraTarget), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="SpinReleaseTime"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinReleaseTimeProperty = DependencyProperty.Register(
      nameof(SpinReleaseTime), typeof(int), typeof(CameraController), new UIPropertyMetadata(200));

    /// <summary>
    /// Identifies the <see cref="UpDownPanSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpDownPanSensitivityProperty = DependencyProperty.Register(
      nameof(UpDownPanSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="UpDownRotationSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpDownRotationSensitivityProperty = DependencyProperty.Register(
      nameof(UpDownRotationSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="Viewport"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
      nameof(Viewport), typeof(Viewport3D), typeof(CameraController), new PropertyMetadata(null, ViewportChanged));

    /// <summary>
    /// Identifies the <see cref="ZoomAroundMouseDownPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomAroundMouseDownPointProperty = DependencyProperty.Register(
      nameof(ZoomAroundMouseDownPoint), typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="SnapMouseDownPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SnapMouseDownPointProperty = DependencyProperty.Register(
      nameof(SnapMouseDownPoint), typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="ZoomCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
      nameof(ZoomCursor), typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.SizeNS));

    /// <summary>
    /// Identifies the <see cref="ZoomRectangleCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomRectangleCursorProperty = DependencyProperty.Register(
      nameof(ZoomRectangleCursor), typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.ScrollSE));

    /// <summary>
    /// Identifies the <see cref="ZoomSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomSensitivityProperty = DependencyProperty.Register(
      nameof(ZoomSensitivity), typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

    /// <summary>
    /// The zoomed by rectangle event
    /// </summary>
    public static readonly RoutedEvent ZoomedByRectangleEvent = EventManager.RegisterRoutedEvent(
      nameof(ZoomedByRectangle), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CameraController));

    /// <summary>
    /// The camera history stack.
    /// </summary>
    /// <remarks>
    /// Implemented as a linkedlist since we want to remove items at the bottom of the stack.
    /// </remarks>
    private readonly LinkedList<CameraSetting> cameraHistory = new();

    /// <summary>
    /// The rendering event listener.
    /// </summary>
    private readonly RenderingEventListener renderingEventListener;

    /// <summary>
    /// The stacked cursors - used for restoring to original cursor
    /// </summary>
    private readonly Stack<Cursor> cursorStack = new();

    /// <summary>
    /// The change field of view event handler.
    /// </summary>
    private ZoomHandler changeFieldOfViewHandler;

    /// <summary>
    /// The change look at event handler.
    /// </summary>
    private RotateHandler changeLookAtHandler;

    /// <summary>
    /// The is spinning flag.
    /// </summary>
    private bool isSpinning;

    /// <summary>
    /// The last tick.
    /// </summary>
    private long lastTick;

    /// <summary>
    /// The move speed.
    /// </summary>
    private Vector3D moveSpeed;

    /// <summary>
    /// The pan event handler.
    /// </summary>
    private PanHandler panHandler;

    /// <summary>
    /// The pan speed.
    /// </summary>
    private Vector3D panSpeed;

    /// <summary>
    /// The rectangle adorner.
    /// </summary>
    private RectangleAdorner rectangleAdorner;

    /// <summary>
    /// The rotation event handler.
    /// </summary>
    private RotateHandler rotateHandler;

    /// <summary>
    /// The 3D rotation point.
    /// </summary>
    private Point3D rotationPoint3D;

    /// <summary>
    /// The rotation position.
    /// </summary>
    private Point rotationPosition;

    /// <summary>
    /// The rotation speed.
    /// </summary>
    private Vector rotationSpeed;

    /// <summary>
    /// The 3D point to spin around.
    /// </summary>
    private Point3D spinningPoint3D;

    /// <summary>
    /// The spinning position.
    /// </summary>
    private Point spinningPosition;

    /// <summary>
    /// The spinning speed.
    /// </summary>
    private Vector spinningSpeed;

    /// <summary>
    /// The target adorner.
    /// </summary>
    private Adorner targetAdorner;

    /// <summary>
    /// The touch point in the last touch delta event
    /// </summary>
    private Point touchPreviousPoint;

    /// <summary>
    /// The number of touch manipulators (fingers) in the last touch delta event
    /// </summary>
    private int manipulatorCount;

    /// <summary>
    /// The zoom event handler.
    /// </summary>
    private ZoomHandler zoomHandler;

    /// <summary>
    /// The point to zoom around.
    /// </summary>
    private Point3D zoomPoint3D;

    /// <summary>
    /// The zoom rectangle event handler.
    /// </summary>
    private ZoomRectangleHandler zoomRectangleHandler;

    /// <summary>
    /// The zoom speed.
    /// </summary>
    private double zoomSpeed;

    /// <summary>
    /// Initializes static members of the <see cref="CameraController" /> class.
    /// </summary>
    static CameraController()
    {
      BackgroundProperty.OverrideMetadata(typeof(CameraController), new FrameworkPropertyMetadata(Brushes.Transparent));
      FocusVisualStyleProperty.OverrideMetadata(typeof(CameraController), new FrameworkPropertyMetadata(null));
      BackViewCommand = new RoutedCommand();
      BottomViewCommand = new RoutedCommand();
      ChangeFieldOfViewCommand = new RoutedCommand();
      ChangeLookAtCommand = new RoutedCommand();
      FrontViewCommand = new RoutedCommand();
      LeftViewCommand = new RoutedCommand();
      PanCommand = new RoutedCommand();
      ResetCameraCommand = new RoutedCommand();
      RightViewCommand = new RoutedCommand();
      RotateCommand = new RoutedCommand();
      TopViewCommand = new RoutedCommand();
      ZoomCommand = new RoutedCommand();
      ZoomExtentsCommand = new RoutedCommand();
      ZoomRectangleCommand = new RoutedCommand();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraController" /> class.
    /// </summary>
    public CameraController()
    {
      Loaded += CameraControllerLoaded;
      Unloaded += CameraControllerUnloaded;

      // Must be focusable to received key events
      Focusable = true;
      FocusVisualStyle = null;

      IsManipulationEnabled = true;
      RotataAroundClosestVertexComplexity = 5000;

      InitializeBindings();
      renderingEventListener = new RenderingEventListener(OnCompositionTargetRendering);
    }

    /// <summary>
    /// Occurs when the look at/target point changed.
    /// </summary>
    public event RoutedEventHandler LookAtChanged
    {
      add => AddHandler(LookAtChangedEvent, value);

      remove => RemoveHandler(LookAtChangedEvent, value);
    }

    /// <summary>
    /// Occurs when the view is zoomed by rectangle.
    /// </summary>
    public event RoutedEventHandler ZoomedByRectangle
    {
      add => AddHandler(ZoomedByRectangleEvent, value);

      remove => RemoveHandler(ZoomedByRectangleEvent, value);
    }

    /// <summary>
    /// Gets the back view command.
    /// </summary>
    public static RoutedCommand BackViewCommand { get; }

    /// <summary>
    /// Gets the bottom view command.
    /// </summary>
    public static RoutedCommand BottomViewCommand { get; }

    /// <summary>
    /// Gets the change field of view command.
    /// </summary>
    public static RoutedCommand ChangeFieldOfViewCommand { get; }

    /// <summary>
    /// Gets the change look at command.
    /// </summary>
    public static RoutedCommand ChangeLookAtCommand { get; }

    /// <summary>
    /// Gets the front view command.
    /// </summary>
    public static RoutedCommand FrontViewCommand { get; }

    /// <summary>
    /// Gets the left view command.
    /// </summary>
    public static RoutedCommand LeftViewCommand { get; }

    /// <summary>
    /// Gets the pan command.
    /// </summary>
    public static RoutedCommand PanCommand { get; }

    /// <summary>
    /// Gets the reset camera command.
    /// </summary>
    public static RoutedCommand ResetCameraCommand { get; }

    /// <summary>
    /// Gets the right view command.
    /// </summary>
    public static RoutedCommand RightViewCommand { get; }

    /// <summary>
    /// Gets the rotate command.
    /// </summary>
    public static RoutedCommand RotateCommand { get; }

    /// <summary>
    /// Gets the top view command.
    /// </summary>
    public static RoutedCommand TopViewCommand { get; }

    /// <summary>
    /// Gets the zoom command.
    /// </summary>
    public static RoutedCommand ZoomCommand { get; }

    /// <summary>
    /// Gets the zoom extents command.
    /// </summary>
    public static RoutedCommand ZoomExtentsCommand { get; }

    /// <summary>
    /// Gets the zoom rectangle command.
    /// </summary>
    public static RoutedCommand ZoomRectangleCommand { get; }

    /// <summary>
    /// Gets ActualCamera.
    /// </summary>
    public ProjectionCamera? ActualCamera => Camera ?? Viewport.Camera as ProjectionCamera;

    /// <summary>
    /// Gets or sets Camera.
    /// </summary>
    public ProjectionCamera? Camera
    {
      get => (ProjectionCamera)GetValue(CameraProperty);
      set => SetValue(CameraProperty, value);
    }

    /// <summary>
    /// Gets or sets CameraLookDirection.
    /// </summary>
    public Vector3D CameraLookDirection
    {
      get => ActualCamera!.LookDirection;
      set => ActualCamera!.LookDirection = value;
    }

    /// <summary>
    /// Gets or sets CameraMode.
    /// </summary>
    public CameraMode CameraMode
    {
      get => (CameraMode)GetValue(CameraModeProperty);
      set => SetValue(CameraModeProperty, value);
    }

    /// <summary>
    /// Gets or sets CameraPosition.
    /// </summary>
    public Point3D CameraPosition
    {
      get => ActualCamera!.Position;
      set => ActualCamera!.Position = value;
    }

    /// <summary>
    /// Gets or sets CameraRotationMode.
    /// </summary>
    public CameraRotationMode CameraRotationMode
    {
      get => (CameraRotationMode)GetValue(CameraRotationModeProperty);
      set => SetValue(CameraRotationModeProperty, value);
    }

    /// <summary>
    /// Gets or sets CameraTarget.
    /// </summary>
    public Point3D CameraTarget
    {
      get => CameraPosition + CameraLookDirection;
      set => CameraLookDirection = value - CameraPosition;
    }

    /// <summary>
    /// Gets or sets CameraUpDirection.
    /// </summary>
    public Vector3D CameraUpDirection
    {
      get => ActualCamera!.UpDirection;
      set => ActualCamera!.UpDirection = value;
    }

    /// <summary>
    /// Gets or sets the change field of view cursor.
    /// </summary>
    /// <value> The change field of view cursor. </value>
    public Cursor ChangeFieldOfViewCursor
    {
      get => (Cursor)GetValue(ChangeFieldOfViewCursorProperty);
      set => SetValue(ChangeFieldOfViewCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the default camera (used when resetting the view).
    /// </summary>
    /// <value> The default camera. </value>
    public ProjectionCamera DefaultCamera
    {
      get => (ProjectionCamera)GetValue(DefaultCameraProperty);
      set => SetValue(DefaultCameraProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Enabled.
    /// </summary>
    public bool Enabled
    {
      get => (bool)GetValue(EnabledProperty);
      set => SetValue(EnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets InertiaFactor.
    /// </summary>
    public double InertiaFactor
    {
      get => (double)GetValue(InertiaFactorProperty);
      set => SetValue(InertiaFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether InfiniteSpin.
    /// </summary>
    public bool InfiniteSpin
    {
      get => (bool)GetValue(InfiniteSpinProperty);
      set => SetValue(InfiniteSpinProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether IsActive.
    /// </summary>
    public bool IsActive => Enabled && Viewport != null && ActualCamera != null;

    /// <summary>
    /// Gets or sets a value indicating whether field of view can be changed.
    /// </summary>
    public bool IsChangeFieldOfViewEnabled
    {
      get => (bool)GetValue(IsChangeFieldOfViewEnabledProperty);
      set => SetValue(IsChangeFieldOfViewEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether inertia is enabled for the camera manipulations.
    /// </summary>
    /// <value><c>true</c> if inertia is enabled; otherwise, <c>false</c>.</value>
    public bool IsInertiaEnabled
    {
      get => (bool)GetValue(IsInertiaEnabledProperty);
      set => SetValue(IsInertiaEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether move is enabled.
    /// </summary>
    /// <value> <c>true</c> if move is enabled; otherwise, <c>false</c> . </value>
    public bool IsMoveEnabled
    {
      get => (bool)GetValue(IsMoveEnabledProperty);
      set => SetValue(IsMoveEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether pan is enabled.
    /// </summary>
    public bool IsPanEnabled
    {
      get => (bool)GetValue(IsPanEnabledProperty);
      set => SetValue(IsPanEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsRotationEnabled.
    /// </summary>
    public bool IsRotationEnabled
    {
      get => (bool)GetValue(IsRotationEnabledProperty);
      set => SetValue(IsRotationEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether touch zoom (pinch gesture) is enabled.
    /// </summary>
    /// <value> <c>true</c> if touch zoom is enabled; otherwise, <c>false</c> . </value>
    public bool IsTouchZoomEnabled
    {
      get => (bool)GetValue(IsTouchZoomEnabledProperty);
      set => SetValue(IsTouchZoomEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsZoomEnabled.
    /// </summary>
    public bool IsZoomEnabled
    {
      get => (bool)GetValue(IsZoomEnabledProperty);
      set => SetValue(IsZoomEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for pan by the left and right keys.
    /// </summary>
    /// <value> The pan sensitivity. </value>
    /// <remarks>
    /// Use -1 to invert the pan direction.
    /// </remarks>
    public double LeftRightPanSensitivity
    {
      get => (double)GetValue(LeftRightPanSensitivityProperty);
      set => SetValue(LeftRightPanSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for rotation by the left and right keys.
    /// </summary>
    /// <value> The rotation sensitivity. </value>
    /// <remarks>
    /// Use -1 to invert the rotation direction.
    /// </remarks>
    public double LeftRightRotationSensitivity
    {
      get => (double)GetValue(LeftRightRotationSensitivityProperty);
      set => SetValue(LeftRightRotationSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum field of view.
    /// </summary>
    /// <value> The maximum field of view. </value>
    public double MaximumFieldOfView
    {
      get => (double)GetValue(MaximumFieldOfViewProperty);
      set => SetValue(MaximumFieldOfViewProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum field of view.
    /// </summary>
    /// <value> The minimum field of view. </value>
    public double MinimumFieldOfView
    {
      get => (double)GetValue(MinimumFieldOfViewProperty);
      set => SetValue(MinimumFieldOfViewProperty, value);
    }

    /// <summary>
    /// Gets or sets the model up direction.
    /// </summary>
    public Vector3D ModelUpDirection
    {
      get => (Vector3D)GetValue(ModelUpDirectionProperty);
      set => SetValue(ModelUpDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the move sensitivity.
    /// </summary>
    /// <value> The move sensitivity. </value>
    public double MoveSensitivity
    {
      get => (double)GetValue(MoveSensitivityProperty);
      set => SetValue(MoveSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for zoom by the page up and page down keys.
    /// </summary>
    /// <value> The zoom sensitivity. </value>
    /// <remarks>
    /// Use -1 to invert the zoom direction.
    /// </remarks>
    public double PageUpDownZoomSensitivity
    {
      get => (double)GetValue(PageUpDownZoomSensitivityProperty);
      set => SetValue(PageUpDownZoomSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the pan cursor.
    /// </summary>
    /// <value> The pan cursor. </value>
    public Cursor PanCursor
    {
      get => (Cursor)GetValue(PanCursorProperty);
      set => SetValue(PanCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to rotate around the mouse down point.
    /// </summary>
    /// <value> <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
    public bool RotateAroundMouseDownPoint
    {
      get => (bool)GetValue(RotateAroundMouseDownPointProperty);
      set => SetValue(RotateAroundMouseDownPointProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to rotate around a fixed point.
    /// </summary>
    /// <value> <c>true</c> if rotation around a fixed point is enabled; otherwise, <c>false</c> . </value>
    public bool FixedRotationPointEnabled
    {
      get => (bool)GetValue(FixedRotationPointEnabledProperty);
      set => SetValue(FixedRotationPointEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating the center of rotation.
    /// </summary>
    /// <value> <c>true</c> if rotation around a fixed point is enabled; otherwise, <c>false</c> . </value>
    public Point3D FixedRotationPoint
    {
      get => (Point3D)GetValue(FixedRotationPointProperty);
      set => SetValue(FixedRotationPointProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotate cursor.
    /// </summary>
    /// <value> The rotate cursor. </value>
    public Cursor RotateCursor
    {
      get => (Cursor)GetValue(RotateCursorProperty);
      set => SetValue(RotateCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotation sensitivity (degrees/pixel).
    /// </summary>
    /// <value> The rotation sensitivity. </value>
    public double RotationSensitivity
    {
      get => (double)GetValue(RotationSensitivityProperty);
      set => SetValue(RotationSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show a target adorner when manipulating the camera.
    /// </summary>
    public bool ShowCameraTarget
    {
      get => (bool)GetValue(ShowCameraTargetProperty);
      set => SetValue(ShowCameraTargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the max duration of mouse drag to activate spin.
    /// </summary>
    /// <remarks>
    /// If the time between mouse down and mouse up is less than this value, spin is activated.
    /// </remarks>
    public int SpinReleaseTime
    {
      get => (int)GetValue(SpinReleaseTimeProperty);
      set => SetValue(SpinReleaseTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for pan by the up and down keys.
    /// </summary>
    /// <value> The pan sensitivity. </value>
    /// <remarks>
    /// Use -1 to invert the pan direction.
    /// </remarks>
    public double UpDownPanSensitivity
    {
      get => (double)GetValue(UpDownPanSensitivityProperty);
      set => SetValue(UpDownPanSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for rotation by the up and down keys.
    /// </summary>
    /// <value> The rotation sensitivity. </value>
    /// <remarks>
    /// Use -1 to invert the rotation direction.
    /// </remarks>
    public double UpDownRotationSensitivity
    {
      get => (double)GetValue(UpDownRotationSensitivityProperty);
      set => SetValue(UpDownRotationSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets Viewport.
    /// </summary>
    public Viewport3D Viewport
    {
      get => (Viewport3D)GetValue(ViewportProperty);
      set => SetValue(ViewportProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to zoom around mouse down point.
    /// </summary>
    /// <value> <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> . </value>
    public bool ZoomAroundMouseDownPoint
    {
      get => (bool)GetValue(ZoomAroundMouseDownPointProperty);
      set => SetValue(ZoomAroundMouseDownPointProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to snap the mouse down point to a model.
    /// </summary>
    /// <value> <c>true</c> if snapping the mouse down point is enabled; otherwise, <c>false</c> . </value>
    public bool SnapMouseDownPoint
    {
      get => (bool)GetValue(SnapMouseDownPointProperty);
      set => SetValue(SnapMouseDownPointProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom cursor.
    /// </summary>
    /// <value> The zoom cursor. </value>
    public Cursor ZoomCursor
    {
      get => (Cursor)GetValue(ZoomCursorProperty);
      set => SetValue(ZoomCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom rectangle cursor.
    /// </summary>
    /// <value> The zoom rectangle cursor. </value>
    public Cursor ZoomRectangleCursor
    {
      get => (Cursor)GetValue(ZoomRectangleCursorProperty);
      set => SetValue(ZoomRectangleCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets ZoomSensitivity.
    /// </summary>
    public double ZoomSensitivity
    {
      get => (double)GetValue(ZoomSensitivityProperty);
      set => SetValue(ZoomSensitivityProperty, value);
    }


    /// <summary>
    /// Efficiency option, lower values decrease computation time for camera interaction when
    /// RotateAroundMouseDownPoint or ZoomAroundMouseDownPoint is set to true in inspect mode.
    /// Note: Will mostly save on computation time once the bounds are already calculated and cashed within the MeshGeometry3D.
    /// </summary>
    public int RotataAroundClosestVertexComplexity { get; set; }

    /// <summary>
    /// Gets a value indicating whether IsOrthographicCamera.
    /// </summary>
    protected bool IsOrthographicCamera => ActualCamera is OrthographicCamera;

    /// <summary>
    /// Gets a value indicating whether IsPerspectiveCamera.
    /// </summary>
    protected bool IsPerspectiveCamera => ActualCamera is PerspectiveCamera;

    /// <summary>
    /// Gets OrthographicCamera.
    /// </summary>
    protected OrthographicCamera? OrthographicCamera => ActualCamera as OrthographicCamera;

    /// <summary>
    /// Gets PerspectiveCamera.
    /// </summary>
    protected PerspectiveCamera? PerspectiveCamera => ActualCamera as PerspectiveCamera;
    
    /// <summary>
    /// Gets or sets a value indicating whether [limit FPS].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [limit FPS]; otherwise, <c>false</c>.
    /// </value>
    public bool LimitFPS { set; get; } = true;

    #region Private Variables
    private TimeSpan prevTime;
    #endregion
    
    /// <summary>
    /// Adds the specified move force.
    /// </summary>
    /// <param name="dx">
    /// The delta x.
    /// </param>
    /// <param name="dy">
    /// The delta y.
    /// </param>
    /// <param name="dz">
    /// The delta z.
    /// </param>
    public void AddMoveForce(double dx, double dy, double dz)
    {
      AddMoveForce(new Vector3D(dx, dy, dz));
    }

    /// <summary>
    /// Adds the specified move force.
    /// </summary>
    /// <param name="delta">
    /// The delta.
    /// </param>
    public void AddMoveForce(Vector3D delta)
    {
      if(!IsMoveEnabled)
      {
        return;
      }

      PushCameraSetting();
      moveSpeed += delta * 40;
    }

    /// <summary>
    /// Adds the specified pan force.
    /// </summary>
    /// <param name="dx">
    /// The delta x.
    /// </param>
    /// <param name="dy">
    /// The delta y.
    /// </param>
    public void AddPanForce(double dx, double dy)
    {
      AddPanForce(FindPanVector(dx, dy));
    }

    /// <summary>
    /// The add pan force.
    /// </summary>
    /// <param name="pan">
    /// The pan.
    /// </param>
    public void AddPanForce(Vector3D pan)
    {
      if(!IsPanEnabled)
      {
        return;
      }

      PushCameraSetting();
      if(IsInertiaEnabled)
      {
        panSpeed += pan * 40;
      }
      else
      {
        panHandler.Pan(pan);
      }
    }

    /// <summary>
    /// The add rotate force.
    /// </summary>
    /// <param name="dx">
    /// The delta x.
    /// </param>
    /// <param name="dy">
    /// The delta y.
    /// </param>
    public void AddRotateForce(double dx, double dy)
    {
      if(!IsRotationEnabled)
      {
        return;
      }

      PushCameraSetting();
      if(IsInertiaEnabled)
      {
        rotationPoint3D = CameraTarget;
        rotationPosition = new Point(ActualWidth / 2, ActualHeight / 2);
        rotationSpeed.X += dx * 40;
        rotationSpeed.Y += dy * 40;
      }
      else
      {
        rotationPosition = new Point(ActualWidth / 2, ActualHeight / 2);
        rotateHandler.Rotate(rotationPosition, rotationPosition + new Vector(dx, dy), CameraTarget);
      }
    }

    /// <summary>
    /// Adds the zoom force.
    /// </summary>
    /// <param name="delta">
    /// The delta.
    /// </param>
    public void AddZoomForce(double delta)
    {
      AddZoomForce(delta, CameraTarget);
    }

    /// <summary>
    /// Adds the zoom force.
    /// </summary>
    /// <param name="delta">
    /// The delta.
    /// </param>
    /// <param name="zoomOrigin">
    /// The zoom origin.
    /// </param>
    public void AddZoomForce(double delta, Point3D zoomOrigin)
    {
      if(!IsZoomEnabled)
      {
        return;
      }

      PushCameraSetting();

      if(IsInertiaEnabled)
      {
        zoomPoint3D = zoomOrigin;
        zoomSpeed += delta * 8;
      }
      else
      {
        zoomHandler.Zoom(delta, zoomOrigin);
      }
    }

    /// <summary>
    /// Changes the direction of the camera.
    /// </summary>
    /// <param name="lookDir">
    /// The look direction.
    /// </param>
    /// <param name="upDir">
    /// The up direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void ChangeDirection(Vector3D lookDir, Vector3D upDir, double animationTime = 500)
    {
      if(!IsRotationEnabled)
      {
        return;
      }

      StopAnimations();
      PushCameraSetting();
      ActualCamera.ChangeDirection(lookDir, upDir, animationTime);
    }

    /// <summary>
    /// Changes the direction of the camera.
    /// </summary>
    /// <param name="lookDir">
    /// The look direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void ChangeDirection(Vector3D lookDir, double animationTime = 500)
    {
      if(!IsRotationEnabled)
      {
        return;
      }

      StopAnimations();
      PushCameraSetting();
      ActualCamera.ChangeDirection(lookDir, ActualCamera!.UpDirection, animationTime);
    }

    /// <summary>
    /// Hides the rectangle.
    /// </summary>
    public void HideRectangle()
    {
      var myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
      if(myAdornerLayer == null) { return; }
      if(rectangleAdorner != null)
      {
        myAdornerLayer.Remove(rectangleAdorner);
      }
      rectangleAdorner = null!;
      Viewport.InvalidateVisual();
    }

    /// <summary>
    /// Hides the target adorner.
    /// </summary>
    public void HideTargetAdorner()
    {
      var myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
      if(myAdornerLayer == null) { return; }
      if(targetAdorner != null)
      {
        myAdornerLayer.Remove(targetAdorner);
      }

      targetAdorner = null!;

      // the adorner sometimes leaves some 'dust', so refresh the viewport
      RefreshViewport();
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
    [Obsolete]
    public void LookAt(Point3D target, double animationTime)
    {
      if(!IsPanEnabled)
      {
        return;
      }

      PushCameraSetting();
      Camera.LookAt(target, animationTime);
    }

    /// <summary>
    /// Push the current camera settings on an internal stack.
    /// </summary>
    public void PushCameraSetting()
    {
      cameraHistory.AddLast(new CameraSetting(ActualCamera));
      if(cameraHistory.Count > 100)
      {
        cameraHistory.RemoveFirst();
      }
    }

    /// <summary>
    /// Resets the camera.
    /// </summary>
    public void ResetCamera()
    {
      if(!IsZoomEnabled || !IsRotationEnabled || !IsPanEnabled)
      {
        return;
      }

      PushCameraSetting();
      if(DefaultCamera != null)
      {
        DefaultCamera.Copy(ActualCamera);
      }
      else
      {
        ActualCamera.Reset();
        ActualCamera.ZoomExtents(Viewport);
      }
    }

    /// <summary>
    /// Resets the camera up direction.
    /// </summary>
    public void ResetCameraUpDirection()
    {
      CameraUpDirection = ModelUpDirection;
    }

    /// <summary>
    /// Restores the most recent camera setting from the internal stack.
    /// </summary>
    /// <returns> The restore camera setting. </returns>
    public bool RestoreCameraSetting()
    {
      switch (cameraHistory.Count)
      {
        case > 0:
        {
          var cs = cameraHistory.Last?.Value;
          cameraHistory.RemoveLast();
          cs?.UpdateCamera(ActualCamera);
          return true;
        }
        default:
          return false;
      }
    }

    /// <summary>
    /// Shows the rectangle.
    /// </summary>
    /// <param name="rect">
    /// The rectangle.
    /// </param>
    /// <param name="color1">
    /// The color 1.
    /// </param>
    /// <param name="color2">
    /// The color 2.
    /// </param>
    public void ShowRectangle(Rect rect, Color color1, Color color2)
    {
      if(rectangleAdorner != null)
      {
        return;
      }

      var myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
      if(myAdornerLayer == null) { return; }
      rectangleAdorner = new RectangleAdorner(Viewport, rect, color1, color2, 3, 1, 10, DashStyles.Solid);
      myAdornerLayer.Add(rectangleAdorner);
    }

    /// <summary>
    /// Shows the target adorner.
    /// </summary>
    /// <param name="position">
    /// The position.
    /// </param>
    public void ShowTargetAdorner(Point position)
    {
      if(!ShowCameraTarget)
      {
        return;
      }

      if(targetAdorner != null)
      {
        return;
      }

      var myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
      if(myAdornerLayer == null) { return; }
      targetAdorner = new TargetSymbolAdorner(Viewport, position);
      myAdornerLayer.Add(targetAdorner);
    }

    /// <summary>
    /// Starts the spin.
    /// </summary>
    /// <param name="speed">
    /// The speed.
    /// </param>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <param name="aroundPoint">
    /// The spin around point.
    /// </param>
    public void StartSpin(Vector speed, Point position, Point3D aroundPoint)
    {
      spinningSpeed = speed;
      spinningPosition = position;
      spinningPoint3D = aroundPoint;
      isSpinning = true;
    }

    /// <summary>
    /// Stops the spin.
    /// </summary>
    public void StopSpin()
    {
      isSpinning = false;
    }

    /// <summary>
    /// Updates the rectangle.
    /// </summary>
    /// <param name="rect">
    /// The rectangle.
    /// </param>
    public void UpdateRectangle(Rect rect)
    {
      if(rectangleAdorner == null)
      {
        return;
      }

      rectangleAdorner.Rectangle = rect;
      rectangleAdorner.InvalidateVisual();
    }

    /// <summary>
    /// Zooms by the specified delta value.
    /// </summary>
    /// <param name="delta">
    /// The delta value.
    /// </param>
    public void Zoom(double delta)
    {
      zoomHandler.Zoom(delta);
    }

    /// <summary>
    /// Zooms to the extents of the model.
    /// </summary>
    /// <param name="animationTime">
    /// The animation time (milliseconds).
    /// </param>
    public void ZoomExtents(double animationTime = 200)
    {
      if(!IsZoomEnabled)
      {
        return;
      }

      PushCameraSetting();
      ActualCamera.ZoomExtents(Viewport, animationTime);
    }

    /// <summary>
    /// Restores the cursor from the cursor stack.
    /// </summary>
    public void RestoreCursor()
    {
      Cursor = cursorStack.Pop();
    }

    /// <summary>
    /// Sets the cursor and pushes the current cursor to the cursor stack.
    /// </summary>
    /// <param name="cursor">The cursor.</param>
    /// <remarks>Use <see cref="RestoreCursor" /> to restore the cursor again.</remarks>
    public void SetCursor(Cursor cursor)
    {
      cursorStack.Push(Cursor);
      Cursor = cursor;
    }

    /// <summary>
    /// Raises the LookAtChanged event.
    /// </summary>
    protected internal virtual void OnLookAtChanged()
    {
      var args = new RoutedEventArgs(LookAtChangedEvent);
      RaiseEvent(args);
    }

    /// <summary>
    /// Raises the ZoomedByRectangle event.
    /// </summary>
    protected internal virtual void OnZoomedByRectangle()
    {
      var args = new RoutedEventArgs(ZoomedByRectangleEvent);
      RaiseEvent(args);
    }

    /// <summary>
    /// Called when the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/> event occurs.
    /// </summary>
    /// <param name="e">
    /// The data for the event.
    /// </param>
    protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
    {
      base.OnManipulationCompleted(e);
      var p = e.ManipulationOrigin + e.TotalManipulation.Translation;

      if(manipulatorCount == 1)
      {
        rotateHandler.Completed(new ManipulationEventArgs(p));
      }

      if(manipulatorCount == 2)
      {
        panHandler.Completed(new ManipulationEventArgs(p));
        zoomHandler.Completed(new ManipulationEventArgs(p));
      }

      e.Handled = true;
    }

    /// <summary>
    /// Called when the <see cref="E:System.Windows.UIElement.ManipulationDelta"/> event occurs.
    /// </summary>
    /// <param name="e">
    /// The data for the event.
    /// </param>
    protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
    {
      base.OnManipulationDelta(e);

      // number of manipulators (fingers)
      var n = e.Manipulators.Count();
      var position = touchPreviousPoint + e.DeltaManipulation.Translation;
      touchPreviousPoint = position;

      // http://msdn.microsoft.com/en-us/library/system.windows.uielement.manipulationdelta.aspx

      //// System.Diagnostics.Debug.WriteLine("OnManipulationDelta: T={0}, S={1}, R={2}, O={3}", e.DeltaManipulation.Translation, e.DeltaManipulation.Scale, e.DeltaManipulation.Rotation, e.ManipulationOrigin);
      //// System.Diagnostics.Debug.WriteLine(n + " Delta:" + e.DeltaManipulation.Translation + " Origin:" + e.ManipulationOrigin + " pos:" + position);

      if(manipulatorCount != n)
      {
        // the number of manipulators has changed
        if(manipulatorCount == 1)
        {
          rotateHandler.Completed(new ManipulationEventArgs(position));
        }

        if(manipulatorCount == 2)
        {
          panHandler.Completed(new ManipulationEventArgs(position));
          zoomHandler.Completed(new ManipulationEventArgs(position));
        }

        if(n == 2)
        {
          panHandler.Started(new ManipulationEventArgs(position));
          zoomHandler.Started(new ManipulationEventArgs(e.ManipulationOrigin));
        }
        else
        {
          rotateHandler.Started(new ManipulationEventArgs(position));
        }

        // skip this event, the origin may have changed
        manipulatorCount = n;
        e.Handled = true;
        return;
      }

      if(n == 1)
      {
        // one finger rotates
        rotateHandler.Delta(new ManipulationEventArgs(position));
      }

      if(n == 2)
      {
        // two fingers pans
        panHandler.Delta(new ManipulationEventArgs(position));
      }

      if(IsTouchZoomEnabled && n == 2)
      {
        var zoomAroundPoint = zoomHandler.UnProject(
            e.ManipulationOrigin, zoomHandler.Origin, CameraLookDirection);
        if(zoomAroundPoint != null)
        {
          zoomHandler.Zoom(1 - (e.DeltaManipulation.Scale.Length / Math.Sqrt(2)), zoomAroundPoint.Value);
        }
      }

      e.Handled = true;
    }

    /// <summary>
    /// Called when the <see cref="E:System.Windows.UIElement.ManipulationStarted"/> event occurs.
    /// </summary>
    /// <param name="e">
    /// The data for the event.
    /// </param>
    protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
    {
      base.OnManipulationStarted(e);
      Focus();
      touchPreviousPoint = e.ManipulationOrigin;
      manipulatorCount = 0;

      e.Handled = true;
    }

    /// <summary>
    /// Invoked when an unhandled MouseDown attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.
    /// </param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      Focus();
      if(e.ChangedButton == MouseButton.XButton1)
      {
        RestoreCameraSetting();
      }
    }

    /// <summary>
    /// Invoked when an unhandled StylusSystemGesture attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.Windows.Input.StylusSystemGestureEventArgs"/> that contains the event data.
    /// </param>
    protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
    {
      base.OnStylusSystemGesture(e);

      // Debug.WriteLine("OnStylusSystemGesture: " + e.SystemGesture);
      if(e.SystemGesture == SystemGesture.HoldEnter)
      {
        var p = e.GetPosition(this);
        changeLookAtHandler.Started(new ManipulationEventArgs(p));
        changeLookAtHandler.Completed(new ManipulationEventArgs(p));
        e.Handled = true;
      }

      if(e.SystemGesture == SystemGesture.TwoFingerTap)
      {
        ZoomExtents();
        e.Handled = true;
      }
    }

    /// <summary>
    /// The camera changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void CameraChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CameraController)d).OnCameraChanged();
    }

    /// <summary>
    /// The viewport changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void ViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CameraController)d).OnViewportChanged();
    }

    /// <summary>
    /// The back view event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void BackViewHandler(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeDirection(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
    }

    /// <summary>
    /// The bottom view event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void BottomViewHandler(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeDirection(new Vector3D(0, 0, 1), new Vector3D(0, -1, 0));
    }

    /// <summary>
    /// The camera controller_ loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void CameraControllerLoaded(object sender, RoutedEventArgs e)
    {
      SubscribeEvents();
    }

    /// <summary>
    /// Called when the CameraController is unloaded.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void CameraControllerUnloaded(object sender, RoutedEventArgs e)
    {
      UnSubscribeEvents();
    }

    /// <summary>
    /// Clamps the specified value between the limits.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="min">
    /// The min.
    /// </param>
    /// <param name="max">
    /// The max.
    /// </param>
    /// <returns>
    /// The clamp.
    /// </returns>
    private double Clamp(double value, double min, double max)
    {
      if(value < min)
      {
        return min;
      }

      if(value > max)
      {
        return max;
      }

      return value;
    }

    /// <summary>
    /// Finds the pan vector.
    /// </summary>
    /// <param name="dx">
    /// The delta x.
    /// </param>
    /// <param name="dy">
    /// The delta y.
    /// </param>
    /// <returns>
    /// The <see cref="Vector3D"/> .
    /// </returns>
    private Vector3D FindPanVector(double dx, double dy)
    {
      var axis1 = Vector3D.CrossProduct(CameraLookDirection, CameraUpDirection);
      var axis2 = Vector3D.CrossProduct(axis1, CameraLookDirection);
      axis1.Normalize();
      axis2.Normalize();
      var l = CameraLookDirection.Length;
      var f = l * 0.001;
      var move = (-axis1 * f * dx) + (axis2 * f * dy);

      // this should be dependent on distance to target?
      return move;
    }

    /// <summary>
    /// The front view event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void FrontViewHandler(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeDirection(new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1));
    }

    /// <summary>
    /// Initializes the input bindings.
    /// </summary>
    private void InitializeBindings()
    {
      changeLookAtHandler = new RotateHandler(this, true);
      rotateHandler = new RotateHandler(this);
      zoomRectangleHandler = new ZoomRectangleHandler(this);
      zoomHandler = new ZoomHandler(this);
      panHandler = new PanHandler(this);
      changeFieldOfViewHandler = new ZoomHandler(this, true);

      CommandBindings.Add(new CommandBinding(ZoomRectangleCommand, zoomRectangleHandler.Execute));
      CommandBindings.Add(new CommandBinding(ZoomExtentsCommand, ZoomExtentsHandler));
      CommandBindings.Add(new CommandBinding(RotateCommand, rotateHandler.Execute));
      CommandBindings.Add(new CommandBinding(ZoomCommand, zoomHandler.Execute));
      CommandBindings.Add(new CommandBinding(PanCommand, panHandler.Execute));
      CommandBindings.Add(new CommandBinding(ResetCameraCommand, ResetCameraHandler));
      CommandBindings.Add(new CommandBinding(ChangeLookAtCommand, changeLookAtHandler.Execute));
      CommandBindings.Add(new CommandBinding(ChangeFieldOfViewCommand, changeFieldOfViewHandler.Execute));
      CommandBindings.Add(new CommandBinding(TopViewCommand, TopViewHandler));
      CommandBindings.Add(new CommandBinding(BottomViewCommand, BottomViewHandler));
      CommandBindings.Add(new CommandBinding(LeftViewCommand, LeftViewHandler));
      CommandBindings.Add(new CommandBinding(RightViewCommand, RightViewHandler));
      CommandBindings.Add(new CommandBinding(FrontViewCommand, FrontViewHandler));
      CommandBindings.Add(new CommandBinding(BackViewCommand, BackViewHandler));
    }

    /// <summary>
    /// The left view event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void LeftViewHandler(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeDirection(new Vector3D(0, -1, 0), new Vector3D(0, 0, 1));
    }

    /// <summary>
    /// The on camera changed.
    /// </summary>
    private void OnCameraChanged()
    {
      cameraHistory.Clear();
      PushCameraSetting();
    }

    /// <summary>
    /// The rendering event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
    {
      if(LimitFPS && prevTime == e.RenderingTime)
      {
        return;
      }
      prevTime = e.RenderingTime;
      var ticks = e.RenderingTime.Ticks;
      var time = 100e-9 * (ticks - lastTick);

      if(lastTick != 0)
      {
        OnTimeStep(time);
      }

      lastTick = ticks;
    }

    /// <summary>
    /// Called when a key is pressed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.
    /// </param>
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      OnKeyDown(e);
      var shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
      var control = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
      var f = control ? 0.25 : 1;

      if(!shift)
      {
        switch(e.Key)
        {
          case Key.Left:
            AddRotateForce(-1 * f * LeftRightRotationSensitivity, 0);
            e.Handled = true;
            break;
          case Key.Right:
            AddRotateForce(1 * f * LeftRightRotationSensitivity, 0);
            e.Handled = true;
            break;
          case Key.Up:
            AddRotateForce(0, -1 * f * UpDownRotationSensitivity);
            e.Handled = true;
            break;
          case Key.Down:
            AddRotateForce(0, 1 * f * UpDownRotationSensitivity);
            e.Handled = true;
            break;
        }
      }
      else
      {
        switch(e.Key)
        {
          case Key.Left:
            AddPanForce(-5 * f * LeftRightPanSensitivity, 0);
            e.Handled = true;
            break;
          case Key.Right:
            AddPanForce(5 * f * LeftRightPanSensitivity, 0);
            e.Handled = true;
            break;
          case Key.Up:
            AddPanForce(0, -5 * f * UpDownPanSensitivity);
            e.Handled = true;
            break;
          case Key.Down:
            AddPanForce(0, 5 * f * UpDownPanSensitivity);
            e.Handled = true;
            break;
        }
      }

      switch(e.Key)
      {
        case Key.PageUp:
          AddZoomForce(-0.1 * f * PageUpDownZoomSensitivity);
          e.Handled = true;
          break;
        case Key.PageDown:
          AddZoomForce(0.1 * f * PageUpDownZoomSensitivity);
          e.Handled = true;
          break;
        case Key.Back:
          if(RestoreCameraSetting())
          {
            e.Handled = true;
          }

          break;
      }

      switch(e.Key)
      {
        case Key.W:
          AddMoveForce(0, 0, 0.1 * f * MoveSensitivity);
          break;
        case Key.A:
          AddMoveForce(-0.1 * f * LeftRightPanSensitivity, 0, 0);
          break;
        case Key.S:
          AddMoveForce(0, 0, -0.1 * f * MoveSensitivity);
          break;
        case Key.D:
          AddMoveForce(0.1 * f * LeftRightPanSensitivity, 0, 0);
          break;
        case Key.Z:
          AddMoveForce(0, -0.1 * f * LeftRightPanSensitivity, 0);
          break;
        case Key.Q:
          AddMoveForce(0, 0.1 * f * LeftRightPanSensitivity, 0);
          break;
      }
    }

    /// <summary>
    /// Called when the mouse wheel is moved.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.
    /// </param>
    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if(!IsZoomEnabled)
      {
        return;
      }

      if(ZoomAroundMouseDownPoint)
      {
        var point = e.GetPosition(this);

        var nearestPoint = new Closest3DPointHitTester(Viewport, RotataAroundClosestVertexComplexity)
            .CalculateMouseDownNearestPoint(point, SnapMouseDownPoint).MouseDownNearestPoint3D;
        if(nearestPoint.HasValue)
        {
          AddZoomForce(-e.Delta * 0.001, nearestPoint.Value);
          e.Handled = true;
          return;
        }
      }

      AddZoomForce(-e.Delta * 0.001);
      e.Handled = true;
    }

    /// <summary>
    /// The on time step.
    /// </summary>
    /// <param name="time">
    /// The time.
    /// </param>
    private void OnTimeStep(double time)
    {
      // should be independent of time
      var factor = IsInertiaEnabled ? Math.Pow(InertiaFactor, time / 0.012) : 0;
      factor = Clamp(factor, 0.2, 1);

      if(isSpinning && spinningSpeed.LengthSquared > 0)
      {
        rotateHandler.Rotate(
            spinningPosition, spinningPosition + (spinningSpeed * time), spinningPoint3D);

        if(!InfiniteSpin)
        {
          spinningSpeed *= factor;
        }
      }

      if(rotationSpeed.LengthSquared > 0.1)
      {
        rotateHandler.Rotate(
            rotationPosition, rotationPosition + (rotationSpeed * time), rotationPoint3D);
        rotationSpeed *= factor;
      }

      if(Math.Abs(panSpeed.LengthSquared) > 0.0001)
      {
        panHandler.Pan(panSpeed * time);
        panSpeed *= factor;
      }

      if(Math.Abs(moveSpeed.LengthSquared) > 0.0001)
      {
        zoomHandler.MoveCameraPosition(moveSpeed * time);
        moveSpeed *= factor;
      }

      if(Math.Abs(zoomSpeed) > 0.1)
      {
        zoomHandler.Zoom(zoomSpeed * time, zoomPoint3D);
        zoomSpeed *= factor;
      }
    }

    /// <summary>
    /// The on viewport changed.
    /// </summary>
    private void OnViewportChanged()
    {
    }

    /// <summary>
    /// The refresh viewport.
    /// </summary>
    private void RefreshViewport()
    {
      // todo: this is a hack, should be improved

      // var mg = new ModelVisual3D { Content = new AmbientLight(Colors.White) };
      // Viewport.Children.Add(mg);
      // Viewport.Children.Remove(mg);
      var c = Viewport.Camera;
      Viewport.Camera = null;
      Viewport.Camera = c;

      // var w = Viewport.Width;
      // Viewport.Width = w-1;
      // Viewport.Width = w;

      // Viewport.InvalidateVisual();
    }

    /// <summary>
    /// The reset camera event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void ResetCameraHandler(object sender, ExecutedRoutedEventArgs e)
    {
      if(IsPanEnabled && IsZoomEnabled && CameraMode != CameraMode.FixedPosition)
      {
        StopAnimations();
        ResetCamera();
      }
    }

    /// <summary>
    /// The right view event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void RightViewHandler(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeDirection(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));
    }

    /// <summary>
    /// The stop animations.
    /// </summary>
    private void StopAnimations()
    {
      rotationSpeed = new Vector();
      panSpeed = new Vector3D();
      zoomSpeed = 0;
    }

    /// <summary>
    /// The subscribe events.
    /// </summary>
    private void SubscribeEvents()
    {
      MouseWheel += OnMouseWheel;
      KeyDown += OnKeyDown;
      RenderingEventManager.AddListener(renderingEventListener);
    }

    /// <summary>
    /// The top view event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void TopViewHandler(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeDirection(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0));
    }

    /// <summary>
    /// The un subscribe events.
    /// </summary>
    private void UnSubscribeEvents()
    {
      MouseWheel -= OnMouseWheel;
      KeyDown -= OnKeyDown;
      RenderingEventManager.RemoveListener(renderingEventListener);
    }

    /// <summary>
    /// The Zoom extents event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void ZoomExtentsHandler(object sender, ExecutedRoutedEventArgs e)
    {
      StopAnimations();
      ZoomExtents();
    }
  }
}
