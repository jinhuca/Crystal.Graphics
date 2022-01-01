using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// A control that contains a <see cref="Viewport3D" /> and a <see cref="CameraController" />.
  /// </summary>
  [ContentProperty("Children")]
  [TemplatePart(Name = "PART_CameraController", Type = typeof(CameraController))]
  [TemplatePart(Name = "PART_ViewportGrid", Type = typeof(Grid))]
  [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
  [TemplatePart(Name = "PART_CoordinateView", Type = typeof(Viewport3D))]
  [TemplatePart(Name = "PART_ViewCubeViewport", Type = typeof(Viewport3D))]
  [Localizability(LocalizationCategory.NeverLocalize)]
  public class CrystalViewport3D : ItemsControl, ICrystalViewport3D
  {
    /// <summary>
    /// Identifies the <see cref="BackViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackViewGestureProperty = DependencyProperty.Register(
      nameof(BackViewGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.B, ModifierKeys.Control)));

    /// <summary>
    /// Identifies the <see cref="BottomViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BottomViewGestureProperty = DependencyProperty.Register(
      nameof(BottomViewGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.D, ModifierKeys.Control)));

    /// <summary>
    /// The camera changed event.
    /// </summary>
    public static readonly RoutedEvent CameraChangedEvent = EventManager.RegisterRoutedEvent(
      nameof(CameraChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CrystalViewport3D));

    /// <summary>
    /// Identifies the <see cref="CameraInertiaFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraInertiaFactorProperty = DependencyProperty.Register(
      nameof(CameraInertiaFactor), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(0.93));

    /// <summary>
    /// Identifies the <see cref="CameraInfo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraInfoProperty = DependencyProperty.Register(
      nameof(CameraInfo), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="CameraMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
      nameof(CameraMode), typeof(CameraMode), typeof(CrystalViewport3D), new UIPropertyMetadata(CameraMode.Inspect));

    /// <summary>
    /// Identifies the <see cref="CameraRotationMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CameraRotationModeProperty = DependencyProperty.Register(
      nameof(CameraRotationMode), typeof(CameraRotationMode), typeof(CrystalViewport3D),
      new UIPropertyMetadata(CameraRotationMode.Turntable, (s, e) => ((CrystalViewport3D)s).OnCameraRotationModeChanged()));

    /// <summary>
    /// Identifies the <see cref="ChangeFieldOfViewCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChangeFieldOfViewCursorProperty = DependencyProperty.Register(
      nameof(ChangeFieldOfViewCursor), typeof(Cursor), typeof(CrystalViewport3D), new UIPropertyMetadata(Cursors.ScrollNS));

    /// <summary>
    /// Identifies the <see cref="ChangeFieldOfViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChangeFieldOfViewGestureProperty = DependencyProperty.Register(
      nameof(ChangeFieldOfViewGesture), typeof(MouseGesture), typeof(CrystalViewport3D),
      new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt)));

    /// <summary>
    /// Identifies the <see cref="ChangeLookAtGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChangeLookAtGestureProperty = DependencyProperty.Register(
      nameof(ChangeLookAtGesture), typeof(MouseGesture), typeof(CrystalViewport3D),
      new UIPropertyMetadata(new MouseGesture(MouseAction.RightDoubleClick)));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemHeightProperty = DependencyProperty.Register(
      nameof(CoordinateSystemHeight), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(80.0));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemHorizontalPosition"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemHorizontalPositionProperty = DependencyProperty.Register(
      nameof(CoordinateSystemHorizontalPosition), typeof(HorizontalAlignment), typeof(CrystalViewport3D),
      new UIPropertyMetadata(HorizontalAlignment.Left));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemLabelForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelForegroundProperty = DependencyProperty.Register(
      nameof(CoordinateSystemLabelForeground), typeof(Brush), typeof(CrystalViewport3D), new PropertyMetadata(Brushes.Black));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemLabelX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelXProperty = DependencyProperty.Register(
      nameof(CoordinateSystemLabelX), typeof(string), typeof(CrystalViewport3D), new PropertyMetadata("X"));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemLabelY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelYProperty = DependencyProperty.Register(
      nameof(CoordinateSystemLabelY), typeof(string), typeof(CrystalViewport3D), new PropertyMetadata("Y"));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemLabelZ"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelZProperty = DependencyProperty.Register(
      nameof(CoordinateSystemLabelZ), typeof(string), typeof(CrystalViewport3D), new PropertyMetadata("Z"));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemVerticalPosition"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemVerticalPositionProperty = DependencyProperty.Register(
      nameof(CoordinateSystemVerticalPosition), typeof(VerticalAlignment), typeof(CrystalViewport3D),
      new UIPropertyMetadata(VerticalAlignment.Bottom));

    /// <summary>
    /// Identifies the <see cref="CoordinateSystemWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemWidthProperty = DependencyProperty.Register(
      nameof(CoordinateSystemWidth), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(80.0));

    /// <summary>
    /// Identifies the CurrentPosition dependency property.
    /// </summary>
    public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
      nameof(CurrentPosition), typeof(Point3D), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(new Point3D(0, 0, 0)));

    /// <summary>
    /// Identifies the EnableCurrentPosition dependency property.
    /// </summary>
    public static readonly DependencyProperty EnableCurrentPositionProperty = DependencyProperty.Register(
      nameof(EnableCurrentPosition), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="CalculateCursorPosition"/> dependency property.
    /// It enables (true) or disables (false) the calculation of the cursor position in the 3D Viewport
    /// </summary>
    public static readonly DependencyProperty CalculateCursorPositionProperty = DependencyProperty.Register(
      nameof(CalculateCursorPosition), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="CursorPosition"/> dependency property.
    /// </summary>
    /// <remarks>
    /// The return value equals ConstructionPlanePosition or CursorModelSnapPosition if CursorSnapToModels is not null.
    /// </remarks>
    public static readonly DependencyProperty CursorPositionProperty = DependencyProperty.Register(
      nameof(CursorPosition), typeof(Point3D?), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="CursorOnElementPosition"/> dependency property.
    /// </summary>
    /// <remarks>
    /// This property returns the position of the nearest model.
    /// </remarks>
    public static readonly DependencyProperty CursorOnElementPositionProperty = DependencyProperty.Register(
      nameof(CursorOnElementPosition), typeof(Point3D?), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="CursorOnConstructionPlanePosition"/> dependency property.
    /// </summary>
    /// <remarks>
    /// This property returns the point on the cursor plane..
    /// </remarks>
    public static readonly DependencyProperty CursorOnConstructionPlanePositionProperty = DependencyProperty.Register(
      nameof(CursorOnConstructionPlanePosition), typeof(Point3D?), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="ConstructionPlane"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ConstructionPlaneProperty = DependencyProperty.Register(
      nameof(ConstructionPlane), typeof(Plane3D), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 1)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="CursorRay"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CursorRayProperty = DependencyProperty.Register(
      nameof(CursorRay), typeof(Ray3D), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(new Ray3D(new Point3D(0, 0, 0), new Vector3D(0, 0, -1)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="DebugInfo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DebugInfoProperty = DependencyProperty.Register(
      nameof(DebugInfo), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="DefaultCamera"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
      nameof(DefaultCamera), typeof(ProjectionCamera), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="FieldOfViewText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FieldOfViewTextProperty = DependencyProperty.Register(
      nameof(FieldOfViewText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="FrameRate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
      nameof(FrameRate), typeof(int), typeof(CrystalViewport3D));

    /// <summary>
    /// Identifies the <see cref="FrameRateText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FrameRateTextProperty = DependencyProperty.Register(
      nameof(FrameRateText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="FrontViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FrontViewGestureProperty = DependencyProperty.Register(
      nameof(FrontViewGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));

    /// <summary>
    /// Identifies the <see cref="InfiniteSpin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
      nameof(InfiniteSpin), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="InfoBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register(
      nameof(InfoBackground), typeof(Brush), typeof(CrystalViewport3D), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))));

    /// <summary>
    /// Identifies the <see cref="InfoForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register(
      nameof(InfoForeground), typeof(Brush), typeof(CrystalViewport3D), new UIPropertyMetadata(Brushes.Black));

    /// <summary>
    /// Identifies the <see cref="IsChangeFieldOfViewEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty = DependencyProperty.Register(
      nameof(IsChangeFieldOfViewEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsHeadLightEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsHeadlightEnabledProperty = DependencyProperty.Register(
      nameof(IsHeadLightEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false, (s, e) => ((CrystalViewport3D)s).OnHeadlightChanged()));

    /// <summary>
    /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsInertiaEnabledProperty = DependencyProperty.Register(
      nameof(IsInertiaEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsPanEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
      nameof(IsPanEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsMoveEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
      nameof(IsMoveEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsTopBottomViewOrientedToFrontBack"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTopBottomViewOrientedToFrontBackProperty = DependencyProperty.Register(
      nameof(IsTopBottomViewOrientedToFrontBack), typeof(bool), typeof(CrystalViewport3D), new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref=" IsViewCubeEdgeClicksEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsViewCubeEdgeClicksEnabledProperty = DependencyProperty.Register(
      nameof(IsViewCubeEdgeClicksEnabled), typeof(bool), typeof(CrystalViewport3D), new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsRotationEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsRotationEnabledProperty = DependencyProperty.Register(
      nameof(IsRotationEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsTouchZoomEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTouchZoomEnabledProperty = DependencyProperty.Register(
      nameof(IsTouchZoomEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsZoomEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
      nameof(IsZoomEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="LeftRightPanSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftRightPanSensitivityProperty = DependencyProperty.Register(
      nameof(LeftRightPanSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="LeftRightRotationSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftRightRotationSensitivityProperty = DependencyProperty.Register(
      nameof(LeftRightRotationSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="LeftViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftViewGestureProperty = DependencyProperty.Register(
      nameof(LeftViewGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.L, ModifierKeys.Control)));

    /// <summary>
    /// The look at (target) point changed event
    /// </summary>
    public static readonly RoutedEvent LookAtChangedEvent = EventManager.RegisterRoutedEvent(
      nameof(LookAtChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CrystalViewport3D));

    /// <summary>
    /// Identifies the <see cref="MaximumFieldOfView"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumFieldOfViewProperty = DependencyProperty.Register(
      nameof(MaximumFieldOfView), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(140.0));

    /// <summary>
    /// Identifies the <see cref="MinimumFieldOfView"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumFieldOfViewProperty = DependencyProperty.Register(
      nameof(MinimumFieldOfView), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(5.0));

    /// <summary>
    /// Identifies the <see cref="ModelUpDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModelUpDirectionProperty = DependencyProperty.Register(
      nameof(ModelUpDirection), typeof(Vector3D), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(new Vector3D(0, 0, 1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="Orthographic"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrthographicProperty = DependencyProperty.Register(
      nameof(Orthographic), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false, (s, e) => ((CrystalViewport3D)s).OnOrthographicChanged()));

    /// <summary>
    /// Identifies the <see cref="OrthographicToggleGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrthographicToggleGestureProperty = DependencyProperty.Register(
      nameof(OrthographicToggleGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift)));

    /// <summary>
    /// Identifies the <see cref="PageUpDownZoomSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PageUpDownZoomSensitivityProperty = DependencyProperty.Register(
      nameof(PageUpDownZoomSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="PanCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
      nameof(PanCursor), typeof(Cursor), typeof(CrystalViewport3D), new UIPropertyMetadata(Cursors.Hand));

    /// <summary>
    /// Identifies the <see cref="PanGesture2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PanGesture2Property = DependencyProperty.Register(
      nameof(PanGesture2), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new MouseGesture(MouseAction.MiddleClick)));

    /// <summary>
    /// Identifies the <see cref="PanGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PanGestureProperty = DependencyProperty.Register(
      nameof(PanGesture), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Shift)));

    /// <summary>
    /// Identifies the <see cref="ResetCameraGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ResetCameraGestureProperty = DependencyProperty.Register(
      nameof(ResetCameraGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new MouseGesture(MouseAction.MiddleDoubleClick)));

    /// <summary>
    /// Identifies the <see cref="ResetCameraKeyGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ResetCameraKeyGestureProperty = DependencyProperty.Register(
      nameof(ResetCameraKeyGesture), typeof(KeyGesture), typeof(CrystalViewport3D), new FrameworkPropertyMetadata(new KeyGesture(Key.Home), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifies the <see cref="RightViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RightViewGestureProperty = DependencyProperty.Register(
      nameof(RightViewGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.R, ModifierKeys.Control)));

    /// <summary>
    /// Identifies the <see cref="RotateAroundMouseDownPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotateAroundMouseDownPointProperty = DependencyProperty.Register(
      nameof(RotateAroundMouseDownPoint), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="FixedRotationPointEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FixedRotationPointEnabledProperty = DependencyProperty.Register(
      nameof(FixedRotationPointEnabled), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="FixedRotationPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FixedRotationPointProperty = DependencyProperty.Register(
      nameof(FixedRotationPoint), typeof(Point3D), typeof(CrystalViewport3D), new UIPropertyMetadata(default(Point3D)));

    /// <summary>
    /// Identifies the <see cref="RotateCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
      nameof(RotateCursor), typeof(Cursor), typeof(CrystalViewport3D), new UIPropertyMetadata(Cursors.SizeAll));

    /// <summary>
    /// Identifies the <see cref="RotateGesture2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotateGesture2Property = DependencyProperty.Register(
      nameof(RotateGesture2), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="RotateGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotateGestureProperty = DependencyProperty.Register(
      nameof(RotateGesture), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick)));

    /// <summary>
    /// Identifies the <see cref="RotationSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationSensitivityProperty = DependencyProperty.Register(
      nameof(RotationSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="ShowCameraInfo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCameraInfoProperty = DependencyProperty.Register(
      nameof(ShowCameraInfo), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false, (s, e) => ((CrystalViewport3D)s).UpdateCameraInfo()));

    /// <summary>
    /// Identifies the <see cref="ShowCameraTarget"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCameraTargetProperty = DependencyProperty.Register(
      nameof(ShowCameraTarget), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="ShowCoordinateSystem"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCoordinateSystemProperty = DependencyProperty.Register(
      nameof(ShowCoordinateSystem), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="ShowFieldOfView"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowFieldOfViewProperty = DependencyProperty.Register(
      nameof(ShowFieldOfView), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false, (s, e) => ((CrystalViewport3D)s).UpdateFieldOfViewInfo()));

    /// <summary>
    /// Identifies the <see cref="ShowFrameRate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowFrameRateProperty = DependencyProperty.Register(
      nameof(ShowFrameRate), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false, (s, e) => ((CrystalViewport3D)s).OnShowFrameRateChanged()));

    /// <summary>
    /// Identifies the <see cref="ShowTriangleCountInfo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowTriangleCountInfoProperty = DependencyProperty.Register(
      nameof(ShowTriangleCountInfo), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false, (s, e) => ((CrystalViewport3D)s).OnShowTriangleCountInfoChanged()));

    /// <summary>
    /// Identifies the <see cref="ShowViewCube"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowViewCubeProperty = DependencyProperty.Register(
      nameof(ShowViewCube), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="Status"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
      nameof(Status), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SubTitle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
      nameof(SubTitle), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SubTitleSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SubTitleSizeProperty = DependencyProperty.Register(
      nameof(SubTitleSize), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(12.0));

    /// <summary>
    /// Identifies the <see cref="TextBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(
      nameof(TextBrush), typeof(Brush), typeof(CrystalViewport3D), new UIPropertyMetadata(Brushes.Black));

    /// <summary>
    /// Identifies the <see cref="TitleBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register(
      nameof(TitleBackground), typeof(Brush), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="TitleFontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleFontFamilyProperty = DependencyProperty.Register(
      nameof(TitleFontFamily), typeof(FontFamily), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
      nameof(Title), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="TitleSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register(
      nameof(TitleSize), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(12.0));

    /// <summary>
    /// Identifies the <see cref="TopViewGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TopViewGestureProperty = DependencyProperty.Register(
      nameof(TopViewGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.U, ModifierKeys.Control)));

    /// <summary>
    /// Identifies the <see cref="TriangleCountInfo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TriangleCountInfoProperty = DependencyProperty.Register(
      nameof(TriangleCountInfo), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="UpDownPanSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpDownPanSensitivityProperty = DependencyProperty.Register(
      nameof(UpDownPanSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="UpDownRotationSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpDownRotationSensitivityProperty = DependencyProperty.Register(
      nameof(UpDownRotationSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="ViewCubeBackText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeBackTextProperty = DependencyProperty.Register(
      nameof(ViewCubeBackText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata("B"));

    /// <summary>
    /// Identifies the <see cref="ViewCubeBottomText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeBottomTextProperty = DependencyProperty.Register(
      nameof(ViewCubeBottomText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata("D"));

    /// <summary>
    /// Identifies the <see cref="ViewCubeFrontText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeFrontTextProperty = DependencyProperty.Register(
      nameof(ViewCubeFrontText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata("F"));

    /// <summary>
    /// Identifies the <see cref="ViewCubeHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeHeightProperty = DependencyProperty.Register(
      nameof(ViewCubeHeight), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(80.0));

    /// <summary>
    /// Identifies the <see cref="ViewCubeHorizontalPosition"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeHorizontalPositionProperty = DependencyProperty.Register(
      nameof(ViewCubeHorizontalPosition), typeof(HorizontalAlignment), typeof(CrystalViewport3D), new UIPropertyMetadata(HorizontalAlignment.Right));

    /// <summary>
    /// Identifies the <see cref="ViewCubeLeftText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeLeftTextProperty = DependencyProperty.Register(
      nameof(ViewCubeLeftText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata("L"));

    /// <summary>
    /// Identifies the <see cref="ViewCubeOpacity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeOpacityProperty = DependencyProperty.Register(
      nameof(ViewCubeOpacity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(0.5));

    /// <summary>
    /// Identifies the <see cref="ViewCubeRightText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeRightTextProperty = DependencyProperty.Register(
      nameof(ViewCubeRightText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata("R"));

    /// <summary>
    /// Identifies the <see cref="ViewCubeTopText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeTopTextProperty = DependencyProperty.Register(
      nameof(ViewCubeTopText), typeof(string), typeof(CrystalViewport3D), new UIPropertyMetadata("U"));

    /// <summary>
    /// Identifies the <see cref="ViewCubeVerticalPosition"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeVerticalPositionProperty = DependencyProperty.Register(
      nameof(ViewCubeVerticalPosition), typeof(VerticalAlignment), typeof(CrystalViewport3D), new UIPropertyMetadata(VerticalAlignment.Bottom));

    /// <summary>
    /// Identifies the <see cref="ViewCubeWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeWidthProperty = DependencyProperty.Register(
      nameof(ViewCubeWidth), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(80.0));

    /// <summary>
    /// Identifies the <see cref="ZoomAroundMouseDownPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomAroundMouseDownPointProperty = DependencyProperty.Register(
      nameof(ZoomAroundMouseDownPoint), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="SnapMouseDownPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SnapMouseDownPointProperty = DependencyProperty.Register(
      nameof(SnapMouseDownPoint), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="ZoomCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
      nameof(ZoomCursor), typeof(Cursor), typeof(CrystalViewport3D), new UIPropertyMetadata(Cursors.SizeNS));

    /// <summary>
    /// Identifies the <see cref="ZoomExtentsGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomExtentsGestureProperty = DependencyProperty.Register(
      nameof(ZoomExtentsGesture), typeof(InputGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Shift)));

    /// <summary>
    /// Identifies the <see cref="ZoomExtentsWhenLoaded"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomExtentsWhenLoadedProperty = DependencyProperty.Register(
      nameof(ZoomExtentsWhenLoaded), typeof(bool), typeof(CrystalViewport3D), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="ZoomGesture2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomGesture2Property = DependencyProperty.Register(
      nameof(ZoomGesture2), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="ZoomGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomGestureProperty = DependencyProperty.Register(
      nameof(ZoomGesture), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Control)));

    /// <summary>
    /// Identifies the <see cref="ZoomRectangleCursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomRectangleCursorProperty = DependencyProperty.Register(
      nameof(ZoomRectangleCursor), typeof(Cursor), typeof(CrystalViewport3D), new UIPropertyMetadata(Cursors.ScrollSE));

    /// <summary>
    /// Identifies the <see cref="ZoomRectangleGesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomRectangleGestureProperty = DependencyProperty.Register(
      nameof(ZoomRectangleGesture), typeof(MouseGesture), typeof(CrystalViewport3D), new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Shift)));

    /// <summary>
    /// Identifies the <see cref="ZoomSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomSensitivityProperty = DependencyProperty.Register(
      nameof(ZoomSensitivity), typeof(double), typeof(CrystalViewport3D), new UIPropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="ZoomedByRectangle"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ZoomedByRectangleEvent = EventManager.RegisterRoutedEvent(
      nameof(ZoomedByRectangle), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CrystalViewport3D));

    /// <summary>
    /// The limit FPS property
    /// </summary>
    public static readonly DependencyProperty LimitFPSProperty = DependencyProperty.Register(
      nameof(LimitFPS), typeof(bool), typeof(CrystalViewport3D), new PropertyMetadata(true, (d, e) => { ((CrystalViewport3D)d).limitFPS = (bool)e.NewValue; }));

    /// <summary>
    /// The adorner layer name.
    /// </summary>
    private const string PartAdornerLayer = "PART_AdornerLayer";

    /// <summary>
    /// The viewport grid name.
    /// </summary>
    private const string PartViewportGrid = "PART_ViewportGrid";

    /// <summary>
    /// The camera controller name.
    /// </summary>
    private const string PartCameraController = "PART_CameraController";

    /// <summary>
    /// The coordinate view name.
    /// </summary>
    private const string PartCoordinateView = "PART_CoordinateView";

    /// <summary>
    /// The view cube name.
    /// </summary>
    private const string PartViewCube = "PART_ViewCube";

    /// <summary>
    /// The view cube viewport name.
    /// </summary>
    private const string PartViewCubeViewport = "PART_ViewCubeViewport";

    /// <summary>
    /// The frame rate stopwatch.
    /// </summary>
    private readonly Stopwatch fpsWatch = new Stopwatch();

    /// <summary>
    /// The headlight.
    /// </summary>
    private readonly DirectionalLight headLight = new DirectionalLight { Color = Colors.White };

    /// <summary>
    /// The lights.
    /// </summary>
    private readonly Model3DGroup lights;

    /// <summary>
    /// The orthographic camera.
    /// </summary>
    private readonly OrthographicCamera orthographicCamera;

    /// <summary>
    /// The perspective camera.
    /// </summary>
    private readonly PerspectiveCamera perspectiveCamera;

    /// <summary>
    /// The rendering event listener.
    /// </summary>
    private readonly RenderingEventListener renderingEventListener;

    /// <summary>
    /// The viewport.
    /// </summary>
    private readonly Viewport3D viewport;

    /// <summary>
    /// The adorner layer.
    /// </summary>
    private AdornerDecorator adornerLayer;

    /// <summary>
    /// The camera controller.
    /// </summary>
    private CameraController cameraController;

    /// <summary>
    /// The coordinate system lights.
    /// </summary>
    private Model3DGroup coordinateSystemLights;

    /// <summary>
    /// The coordinate view.
    /// </summary>
    private Viewport3D coordinateView;

    /// <summary>
    /// The current camera.
    /// </summary>
    private Camera currentCamera;

    /// <summary>
    /// The frame counter.
    /// </summary>
    private int frameCounter;

    /// <summary>
    /// The "control has been loaded before" flag.
    /// </summary>
    private bool hasBeenLoadedBefore;

    /// <summary>
    /// The frame counter for info field updates.
    /// </summary>
    private int infoFrameCounter;

    /// <summary>
    /// The is subscribed to rendering event.
    /// </summary>
    private bool isSubscribedToRenderingEvent;

    /// <summary>
    /// The view cube.
    /// </summary>
    private ViewCubeVisual3D viewCube;

    /// <summary>
    /// The view cube lights.
    /// </summary>
    private Model3DGroup viewCubeLights;

    /// <summary>
    /// The view cube view.
    /// </summary>
    private Viewport3D viewCubeViewport;

    /// <summary>
    /// Initializes static members of the <see cref="CrystalViewport3D"/> class.
    /// </summary>
    static CrystalViewport3D()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CrystalViewport3D), new FrameworkPropertyMetadata(typeof(CrystalViewport3D)));
      ClipToBoundsProperty.OverrideMetadata(typeof(CrystalViewport3D), new FrameworkPropertyMetadata(true));
      OrthographicToggleCommand = new RoutedCommand();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrystalViewport3D"/> class.
    /// </summary>
    public CrystalViewport3D()
    {
      // The Viewport3D must be created here since the Children collection is attached directly
      viewport = new Viewport3D();

      // viewport.SetBinding(UIElement.IsHitTestVisibleProperty, new Binding("IsViewportHitTestVisible") { Source = this });
      // viewport.SetBinding(UIElement.ClipToBoundsProperty, new Binding("ClipToBounds") { Source = this });

      // headlight
      lights = new Model3DGroup();
      viewport.Children.Add(new ModelVisual3D { Content = lights });

      perspectiveCamera = new PerspectiveCamera();
      orthographicCamera = new OrthographicCamera();
      perspectiveCamera.Reset();
      orthographicCamera.Reset();

      Camera = Orthographic ? (ProjectionCamera)orthographicCamera : perspectiveCamera;

      // http://blogs.msdn.com/wpfsdk/archive/2007/01/15/maximizing-wpf-3d-performance-on-tier-2-hardware.aspx
      // RenderOptions.EdgeMode?

      // start a watch for FPS calculations
      fpsWatch.Start();

      Loaded += OnControlLoaded;
      Unloaded += OnControlUnloaded;

      CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyHandler));
      CommandBindings.Add(new CommandBinding(OrthographicToggleCommand, OrthographicToggle));
      renderingEventListener = new RenderingEventListener(CompositionTargetRendering);
    }

    /// <summary>
    /// Event when a property has been changed
    /// </summary>
    public event RoutedEventHandler CameraChanged
    {
      add
      {
        AddHandler(CameraChangedEvent, value);
      }

      remove
      {
        RemoveHandler(CameraChangedEvent, value);
      }
    }

    /// <summary>
    /// Occurs when the look at/target point changed.
    /// </summary>
    public event RoutedEventHandler LookAtChanged
    {
      add
      {
        AddHandler(LookAtChangedEvent, value);
      }

      remove
      {
        RemoveHandler(LookAtChangedEvent, value);
      }
    }

    /// <summary>
    /// Occurs when the view is zoomed by rectangle.
    /// </summary>
    public event RoutedEventHandler ZoomedByRectangle
    {
      add
      {
        AddHandler(ZoomedByRectangleEvent, value);
      }

      remove
      {
        RemoveHandler(ZoomedByRectangleEvent, value);
      }
    }

    /// <summary>
    /// Gets the command that toggles between orthographic and perspective camera.
    /// </summary>
    public static RoutedCommand OrthographicToggleCommand { get; private set; }

    /// <summary>
    /// Gets or sets the back view gesture.
    /// </summary>
    /// <value>
    /// The back view gesture.
    /// </value>
    public InputGesture BackViewGesture
    {
      get => (InputGesture)GetValue(BackViewGestureProperty);
      set => SetValue(BackViewGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the bottom view gesture.
    /// </summary>
    /// <value>
    /// The bottom view gesture.
    /// </value>
    public InputGesture BottomViewGesture
    {
      get => (InputGesture)GetValue(BottomViewGestureProperty);
      set => SetValue(BottomViewGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the camera.
    /// </summary>
    /// <value>
    /// The camera.
    /// </value>
    public ProjectionCamera Camera
    {
      get => (ProjectionCamera)Viewport.Camera;
      set
      {
        if(currentCamera != null)
        {
          currentCamera.Changed -= CameraPropertyChanged;
        }
        Viewport.Camera = value;
        currentCamera = Viewport.Camera;
        currentCamera.Changed += CameraPropertyChanged;
      }
    }

    /// <summary>
    /// Gets the camera controller.
    /// </summary>
    public CameraController CameraController => cameraController;

    /// <summary>
    /// Gets or sets the camera inertia factor.
    /// </summary>
    /// <value>
    /// The camera inertia factor.
    /// </value>
    public double CameraInertiaFactor
    {
      get => (double)GetValue(CameraInertiaFactorProperty);
      set => SetValue(CameraInertiaFactorProperty, value);
    }

    /// <summary>
    /// Gets the camera info.
    /// </summary>
    /// <value>
    /// The camera info.
    /// </value>
    public string CameraInfo
    {
      get => (string)GetValue(CameraInfoProperty);
      private set => SetValue(CameraInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="CameraMode" />
    /// </summary>
    public CameraMode CameraMode
    {
      get => (CameraMode)GetValue(CameraModeProperty);
      set => SetValue(CameraModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the camera rotation mode.
    /// </summary>
    public CameraRotationMode CameraRotationMode
    {
      get => (CameraRotationMode)GetValue(CameraRotationModeProperty);
      set => SetValue(CameraRotationModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the cursor used when changing field of view.
    /// </summary>
    /// <value>
    /// A <see cref="Cursor"/>.
    /// </value>
    public Cursor ChangeFieldOfViewCursor
    {
      get => (Cursor)GetValue(ChangeFieldOfViewCursorProperty);
      set => SetValue(ChangeFieldOfViewCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the change field of view gesture.
    /// </summary>
    /// <value>
    /// The change field of view gesture.
    /// </value>
    public MouseGesture ChangeFieldOfViewGesture
    {
      get => (MouseGesture)GetValue(ChangeFieldOfViewGestureProperty);
      set => SetValue(ChangeFieldOfViewGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the change look-at gesture.
    /// </summary>
    /// <value>
    /// The change look-at gesture.
    /// </value>
    public MouseGesture ChangeLookAtGesture
    {
      get => (MouseGesture)GetValue(ChangeLookAtGestureProperty);
      set => SetValue(ChangeLookAtGestureProperty, value);
    }

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>
    /// The children.
    /// </value>
    public Visual3DCollection Children => viewport.Children;

    /// <summary>
    /// Gets or sets the height of the coordinate system viewport.
    /// </summary>
    /// <value>
    /// The height of the coordinate system viewport.
    /// </value>
    public double CoordinateSystemHeight
    {
      get => (double)GetValue(CoordinateSystemHeightProperty);
      set => SetValue(CoordinateSystemHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal position of the coordinate system viewport.
    /// </summary>
    /// <value>
    /// The horizontal position.
    /// </value>
    public HorizontalAlignment CoordinateSystemHorizontalPosition
    {
      get => (HorizontalAlignment)GetValue(CoordinateSystemHorizontalPositionProperty);
      set => SetValue(CoordinateSystemHorizontalPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the coordinate system label.
    /// </summary>
    /// <value>
    /// The color of the coordinate system label.
    /// </value>
    public Brush CoordinateSystemLabelForeground
    {
      get => (Brush)GetValue(CoordinateSystemLabelForegroundProperty);
      set => SetValue(CoordinateSystemLabelForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the coordinate system X label.
    /// </summary>
    /// <value>
    /// The coordinate system X label.
    /// </value>
    public string CoordinateSystemLabelX
    {
      get => (string)GetValue(CoordinateSystemLabelXProperty);
      set => SetValue(CoordinateSystemLabelXProperty, value);
    }

    /// <summary>
    /// Gets or sets the coordinate system Y label.
    /// </summary>
    /// <value>
    /// The coordinate system Y label.
    /// </value>
    public string CoordinateSystemLabelY
    {
      get => (string)GetValue(CoordinateSystemLabelYProperty);
      set => SetValue(CoordinateSystemLabelYProperty, value);
    }

    /// <summary>
    /// Gets or sets the coordinate system Z label.
    /// </summary>
    /// <value>
    /// The coordinate system Z label.
    /// </value>
    public string CoordinateSystemLabelZ
    {
      get => (string)GetValue(CoordinateSystemLabelZProperty);
      set => SetValue(CoordinateSystemLabelZProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical position of the coordinate system viewport.
    /// </summary>
    /// <value>
    /// The vertical position.
    /// </value>
    public VerticalAlignment CoordinateSystemVerticalPosition
    {
      get => (VerticalAlignment)GetValue(CoordinateSystemVerticalPositionProperty);
      set => SetValue(CoordinateSystemVerticalPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the coordinate system viewport.
    /// </summary>
    /// <value>
    /// The width of the coordinate system viewport.
    /// </value>
    public double CoordinateSystemWidth
    {
      get => (double)GetValue(CoordinateSystemWidthProperty);
      set => SetValue(CoordinateSystemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the current position.
    /// </summary>
    /// <value>
    /// The current position.
    /// </value>
    /// <remarks>
    /// The <see cref="CalculateCursorPosition" /> property must be set to true to enable updating of this property.
    /// </remarks>
    [Obsolete("CurrentPosition is now obsolete, please use CursorPosition instead", false)]
    public Point3D CurrentPosition
    {
      get => (Point3D)GetValue(CurrentPositionProperty);
      set => SetValue(CurrentPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the debug info text.
    /// </summary>
    /// <value>
    /// The debug info text.
    /// </value>
    public string DebugInfo
    {
      get => (string)GetValue(DebugInfoProperty);
      set => SetValue(DebugInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets the default camera.
    /// </summary>
    /// <value>
    /// The default camera.
    /// </value>
    public ProjectionCamera DefaultCamera
    {
      get => (ProjectionCamera)GetValue(DefaultCameraProperty);
      set => SetValue(DefaultCameraProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether calculation of the <see cref="CurrentPosition" /> property is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if calculation is enabled; otherwise, <c>false</c> .
    /// </value>
    [Obsolete("EnableCurrentPosition is now obsolete, please use CalculateCursorPosition instead", false)]
    public bool EnableCurrentPosition
    {
      get => CalculateCursorPosition;
      set => CalculateCursorPosition = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether calculation of the <see cref="CursorPosition" /> properties is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if calculation is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool CalculateCursorPosition
    {
      get => (bool)GetValue(CalculateCursorPositionProperty);
      set => SetValue(CalculateCursorPositionProperty, value);
    }

    /// <summary>
    /// Gets the current cursor position.
    /// </summary>
    /// <value>
    /// The current cursor position.
    /// </value>
    /// <remarks>
    /// The <see cref="CalculateCursorPosition" /> property must be set to true to enable updating of this property.
    /// </remarks>
    public Point3D? CursorPosition
    {
      get => (Point3D?)GetValue(CursorPositionProperty);
      private set => SetValue(CursorPositionProperty, value);
    }

    /// <summary>
    /// Gets the current cursor position on the cursor plane.
    /// </summary>
    /// <value>
    /// The cursor plane position.
    /// </value>
    /// <remarks>
    /// The <see cref="CalculateCursorPosition" /> property must be set to true to enable updating of this property.
    /// </remarks>
    public Point3D? CursorOnConstructionPlanePosition
    {
      get => (Point3D?)GetValue(CursorOnConstructionPlanePositionProperty);
      private set => SetValue(CursorOnConstructionPlanePositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the plane that defines the <see cref="CursorOnConstructionPlanePosition" />.
    /// </summary>
    /// <value>
    /// The plane used to calculate the <see cref="CursorOnConstructionPlanePosition" />.
    /// </value>
    public Plane3D ConstructionPlane
    {
      get => (Plane3D)GetValue(ConstructionPlaneProperty);
      set => SetValue(ConstructionPlaneProperty, value);
    }

    /// <summary>
    /// Gets the cursor ray.
    /// </summary>
    /// <value>
    /// The cursor ray.
    /// </value>
    public Ray3D CursorRay
    {
      get => (Ray3D)GetValue(CursorRayProperty);
      private set => SetValue(CursorRayProperty, value);
    }

    /// <summary>
    /// Gets the current cursor position on the nearest model. If the model is not hit, the position is <c>null</c>.
    /// </summary>
    /// <value>
    /// The position of the model intersection.
    /// </value>
    /// <remarks>
    /// The <see cref="CalculateCursorPosition" /> property must be set to <c>true</c> to enable updating of this property.
    /// </remarks>
    public Point3D? CursorOnElementPosition
    {
      get => (Point3D?)GetValue(CursorOnElementPositionProperty);
      private set => SetValue(CursorOnElementPositionProperty, value);
    }

    /// <summary>
    /// Gets the field of view text.
    /// </summary>
    /// <value>
    /// The field of view text.
    /// </value>
    public string FieldOfViewText
    {
      get => (string)GetValue(FieldOfViewTextProperty);
      private set => SetValue(FieldOfViewTextProperty, value);
    }

    /// <summary>
    /// Gets the frame rate.
    /// </summary>
    /// <value>
    /// The frame rate.
    /// </value>
    public int FrameRate
    {
      get => (int)GetValue(FrameRateProperty);
      private set => SetValue(FrameRateProperty, value);
    }

    /// <summary>
    /// Gets the frame rate text.
    /// </summary>
    /// <value>
    /// The frame rate text.
    /// </value>
    public string FrameRateText
    {
      get => (string)GetValue(FrameRateTextProperty);
      private set => SetValue(FrameRateTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the front view gesture.
    /// </summary>
    /// <value>
    /// The front view gesture.
    /// </value>
    public InputGesture FrontViewGesture
    {
      get => (InputGesture)GetValue(FrontViewGestureProperty);
      set => SetValue(FrontViewGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable infinite spin.
    /// </summary>
    /// <value>
    /// <c>true</c> if infinite spin is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool InfiniteSpin
    {
      get => (bool)GetValue(InfiniteSpinProperty);
      set => SetValue(InfiniteSpinProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for the CameraInfo and TriangleCount fields.
    /// </summary>
    /// <value>
    /// The info background.
    /// </value>
    public Brush InfoBackground
    {
      get => (Brush)GetValue(InfoBackgroundProperty);
      set => SetValue(InfoBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for informational text.
    /// </summary>
    /// <value>
    /// The foreground brush.
    /// </value>
    public Brush InfoForeground
    {
      get => (Brush)GetValue(InfoForegroundProperty);
      set => SetValue(InfoForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether change of field-of-view is enabled.
    /// </summary>
    public bool IsChangeFieldOfViewEnabled
    {
      get => (bool)GetValue(IsChangeFieldOfViewEnabledProperty);
      set => SetValue(IsChangeFieldOfViewEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the head light is enabled.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the head light is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool IsHeadLightEnabled
    {
      get => (bool)GetValue(IsHeadlightEnabledProperty);
      set => SetValue(IsHeadlightEnabledProperty, value);
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
    /// Gets or sets a value indicating whether pan is enabled.
    /// </summary>
    public bool IsPanEnabled
    {
      get => (bool)GetValue(IsPanEnabledProperty);
      set => SetValue(IsPanEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether move (by AWSD keys) is enabled.
    /// </summary>
    public bool IsMoveEnabled
    {
      get => (bool)GetValue(IsMoveEnabledProperty);
      set => SetValue(IsMoveEnabledProperty, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the top and bottom views are oriented to front and back.
    /// </summary>
    public bool IsTopBottomViewOrientedToFrontBack
    {
      get => (bool)GetValue(IsTopBottomViewOrientedToFrontBackProperty);
      set => SetValue(IsTopBottomViewOrientedToFrontBackProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube edge clickable.
    /// </summary>
    public bool IsViewCubeEdgeClicksEnabled
    {
      get => (bool)GetValue(IsViewCubeEdgeClicksEnabledProperty);
      set => SetValue(IsViewCubeEdgeClicksEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether rotation is enabled.
    /// </summary>
    public bool IsRotationEnabled
    {
      get => (bool)GetValue(IsRotationEnabledProperty);
      set => SetValue(IsRotationEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether touch zoom (pinch gesture) is enabled.
    /// </summary>
    /// <value>
    ///     <c>true</c> if touch zoom is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool IsTouchZoomEnabled
    {
      get => (bool)GetValue(IsTouchZoomEnabledProperty);
      set => SetValue(IsTouchZoomEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether zoom is enabled.
    /// </summary>
    public bool IsZoomEnabled
    {
      get => (bool)GetValue(IsZoomEnabledProperty);
      set => SetValue(IsZoomEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for pan by the left and right keys.
    /// </summary>
    /// <value>
    /// The pan sensitivity.
    /// </value>
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
    /// <value>
    /// The rotation sensitivity.
    /// </value>
    /// <remarks>
    /// Use -1 to invert the rotation direction.
    /// </remarks>
    public double LeftRightRotationSensitivity
    {
      get => (double)GetValue(LeftRightRotationSensitivityProperty);
      set => SetValue(LeftRightRotationSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the left view gesture.
    /// </summary>
    /// <value>
    /// The left view gesture.
    /// </value>
    public InputGesture LeftViewGesture
    {
      get => (InputGesture)GetValue(LeftViewGestureProperty);
      set => SetValue(LeftViewGestureProperty, value);
    }

    /// <summary>
    /// Gets the lights.
    /// </summary>
    /// <value>
    /// The lights.
    /// </value>
    public Model3DGroup Lights => lights;

    /// <summary>
    /// Gets or sets the maximum field of view.
    /// </summary>
    /// <value>
    /// The maximum field of view.
    /// </value>
    public double MaximumFieldOfView
    {
      get => (double)GetValue(MaximumFieldOfViewProperty);
      set => SetValue(MaximumFieldOfViewProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum field of view.
    /// </summary>
    /// <value>
    /// The minimum field of view.
    /// </value>
    public double MinimumFieldOfView
    {
      get => (double)GetValue(MinimumFieldOfViewProperty);
      set => SetValue(MinimumFieldOfViewProperty, value);
    }

    /// <summary>
    /// Gets or sets the up direction of the model. This is used by the view cube.
    /// </summary>
    /// <value>
    /// The model up direction.
    /// </value>
    public Vector3D ModelUpDirection
    {
      get => (Vector3D)GetValue(ModelUpDirectionProperty);
      set => SetValue(ModelUpDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="CrystalViewport3D" /> should use an orthographic camera.
    /// </summary>
    /// <value>
    ///     <c>true</c> if an orthographic camera should be used; otherwise, <c>false</c> .
    /// </value>
    public bool Orthographic
    {
      get => (bool)GetValue(OrthographicProperty);
      set => SetValue(OrthographicProperty, value);
    }

    /// <summary>
    /// Gets or sets the orthographic toggle gesture.
    /// </summary>
    /// <value>
    /// The orthographic toggle gesture.
    /// </value>
    public InputGesture OrthographicToggleGesture
    {
      get => (InputGesture)GetValue(OrthographicToggleGestureProperty);
      set => SetValue(OrthographicToggleGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for zoom by the page up and page down keys.
    /// </summary>
    /// <value>
    /// The zoom sensitivity.
    /// </value>
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
    /// <value>
    /// The pan cursor.
    /// </value>
    public Cursor PanCursor
    {
      get => (Cursor)GetValue(PanCursorProperty);
      set => SetValue(PanCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the pan gesture.
    /// </summary>
    /// <value>
    /// The pan gesture.
    /// </value>
    public MouseGesture PanGesture
    {
      get => (MouseGesture)GetValue(PanGestureProperty);
      set => SetValue(PanGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the alternative pan gesture.
    /// </summary>
    /// <value>
    /// The alternative pan gesture.
    /// </value>
    public MouseGesture PanGesture2
    {
      get => (MouseGesture)GetValue(PanGesture2Property);
      set => SetValue(PanGesture2Property, value);
    }

    /// <summary>
    /// Gets or sets the reset camera gesture.
    /// </summary>
    public InputGesture ResetCameraGesture
    {
      get => (InputGesture)GetValue(ResetCameraGestureProperty);
      set => SetValue(ResetCameraGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the reset camera key gesture.
    /// </summary>
    /// <value>
    /// The reset camera key gesture.
    /// </value>
    public KeyGesture ResetCameraKeyGesture
    {
      get => (KeyGesture)GetValue(ResetCameraKeyGestureProperty);
      set => SetValue(ResetCameraKeyGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the right view gesture.
    /// </summary>
    /// <value>
    /// The right view gesture.
    /// </value>
    public InputGesture RightViewGesture
    {
      get => (InputGesture)GetValue(RightViewGestureProperty);
      set => SetValue(RightViewGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to rotate around the mouse down point.
    /// </summary>
    /// <value>
    ///     <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> .
    /// </value>
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
    /// Gets or sets the rotation cursor.
    /// </summary>
    /// <value>
    /// The rotation cursor.
    /// </value>
    public Cursor RotateCursor
    {
      get => (Cursor)GetValue(RotateCursorProperty);
      set => SetValue(RotateCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotation gesture.
    /// </summary>
    /// <value>
    /// The rotation gesture.
    /// </value>
    public MouseGesture RotateGesture
    {
      get => (MouseGesture)GetValue(RotateGestureProperty);
      set => SetValue(RotateGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the alternative rotation gesture.
    /// </summary>
    /// <value>
    /// The alternative rotation gesture.
    /// </value>
    public MouseGesture RotateGesture2
    {
      get => (MouseGesture)GetValue(RotateGesture2Property);
      set => SetValue(RotateGesture2Property, value);
    }

    /// <summary>
    /// Gets or sets the rotation sensitivity.
    /// </summary>
    /// <value>
    /// The rotation sensitivity.
    /// </value>
    public double RotationSensitivity
    {
      get => (double)GetValue(RotationSensitivityProperty);
      set => SetValue(RotationSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show camera info.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the camera info should be shown; otherwise, <c>false</c> .
    /// </value>
    public bool ShowCameraInfo
    {
      get => (bool)GetValue(ShowCameraInfoProperty);
      set => SetValue(ShowCameraInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the camera target adorner.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the camera target adorner should be shown; otherwise, <c>false</c> .
    /// </value>
    public bool ShowCameraTarget
    {
      get => (bool)GetValue(ShowCameraTargetProperty);
      set => SetValue(ShowCameraTargetProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the coordinate system.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the coordinate system should be shown; otherwise, <c>false</c> .
    /// </value>
    public bool ShowCoordinateSystem
    {
      get => (bool)GetValue(ShowCoordinateSystemProperty);
      set => SetValue(ShowCoordinateSystemProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the current field of view.
    /// </summary>
    /// <value>
    ///     <c>true</c> if field of view should be shown; otherwise, <c>false</c> .
    /// </value>
    public bool ShowFieldOfView
    {
      get => (bool)GetValue(ShowFieldOfViewProperty);
      set => SetValue(ShowFieldOfViewProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the frame rate.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the frame rate should be shown; otherwise, <c>false</c> .
    /// </value>
    public bool ShowFrameRate
    {
      get => (bool)GetValue(ShowFrameRateProperty);
      set => SetValue(ShowFrameRateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the total number of triangles in the scene.
    /// </summary>
    public bool ShowTriangleCountInfo
    {
      get => (bool)GetValue(ShowTriangleCountInfoProperty);
      set => SetValue(ShowTriangleCountInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the view cube.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the view cube should be shown; otherwise, <c>false</c> .
    /// </value>
    public bool ShowViewCube
    {
      get => (bool)GetValue(ShowViewCubeProperty);
      set => SetValue(ShowViewCubeProperty, value);
    }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public string Status
    {
      get => (string)GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the sub title.
    /// </summary>
    /// <value>
    /// The sub title.
    /// </value>
    public string SubTitle
    {
      get => (string)GetValue(SubTitleProperty);
      set => SetValue(SubTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the sub title.
    /// </summary>
    /// <value>
    /// The size of the sub title.
    /// </value>
    public double SubTitleSize
    {
      get => (double)GetValue(SubTitleSizeProperty);
      set => SetValue(SubTitleSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the text brush.
    /// </summary>
    /// <value>
    /// The text brush.
    /// </value>
    public Brush TextBrush
    {
      get => (Brush)GetValue(TextBrushProperty);
      set => SetValue(TextBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title
    {
      get => (string)GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the title background brush.
    /// </summary>
    /// <value>
    /// The title background.
    /// </value>
    public Brush TitleBackground
    {
      get => (Brush)GetValue(TitleBackgroundProperty);
      set => SetValue(TitleBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the title font family.
    /// </summary>
    /// <value>
    /// The title font family.
    /// </value>
    public FontFamily TitleFontFamily
    {
      get => (FontFamily)GetValue(TitleFontFamilyProperty);
      set => SetValue(TitleFontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the title.
    /// </summary>
    /// <value>
    /// The size of the title.
    /// </value>
    public double TitleSize
    {
      get => (double)GetValue(TitleSizeProperty);
      set => SetValue(TitleSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the top view gesture.
    /// </summary>
    /// <value>
    /// The top view gesture.
    /// </value>
    public InputGesture TopViewGesture
    {
      get => (InputGesture)GetValue(TopViewGestureProperty);
      set => SetValue(TopViewGestureProperty, value);
    }

    /// <summary>
    /// Gets information about the triangle count.
    /// </summary>
    public string TriangleCountInfo
    {
      get => (string)GetValue(TriangleCountInfoProperty);
      private set => SetValue(TriangleCountInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets the sensitivity for pan by the up and down keys.
    /// </summary>
    /// <value>
    /// The pan sensitivity.
    /// </value>
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
    /// <value>
    /// The rotation sensitivity.
    /// </value>
    /// <remarks>
    /// Use -1 to invert the rotation direction.
    /// </remarks>
    public double UpDownRotationSensitivity
    {
      get => (double)GetValue(UpDownRotationSensitivityProperty);
      set => SetValue(UpDownRotationSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube back text.
    /// </summary>
    /// <value>
    /// The view cube back text.
    /// </value>
    public string ViewCubeBackText
    {
      get => (string)GetValue(ViewCubeBackTextProperty);
      set => SetValue(ViewCubeBackTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube bottom text.
    /// </summary>
    /// <value>
    /// The view cube bottom text.
    /// </value>
    public string ViewCubeBottomText
    {
      get => (string)GetValue(ViewCubeBottomTextProperty);
      set => SetValue(ViewCubeBottomTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube front text.
    /// </summary>
    /// <value>
    /// The view cube front text.
    /// </value>
    public string ViewCubeFrontText
    {
      get => (string)GetValue(ViewCubeFrontTextProperty);
      set => SetValue(ViewCubeFrontTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the view cube viewport.
    /// </summary>
    /// <value>
    /// The height of the view cube viewport.
    /// </value>
    public double ViewCubeHeight
    {
      get => (double)GetValue(ViewCubeHeightProperty);
      set => SetValue(ViewCubeHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal position of the view cube viewport.
    /// </summary>
    /// <value>
    /// The horizontal position.
    /// </value>
    public HorizontalAlignment ViewCubeHorizontalPosition
    {
      get => (HorizontalAlignment)GetValue(ViewCubeHorizontalPositionProperty);
      set => SetValue(ViewCubeHorizontalPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube left text.
    /// </summary>
    /// <value>
    /// The view cube left text.
    /// </value>
    public string ViewCubeLeftText
    {
      get => (string)GetValue(ViewCubeLeftTextProperty);
      set => SetValue(ViewCubeLeftTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the opacity of the view cube when inactive.
    /// </summary>
    public double ViewCubeOpacity
    {
      get => (double)GetValue(ViewCubeOpacityProperty);
      set => SetValue(ViewCubeOpacityProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube right text.
    /// </summary>
    /// <value>
    /// The view cube right text.
    /// </value>
    public string ViewCubeRightText
    {
      get => (string)GetValue(ViewCubeRightTextProperty);
      set => SetValue(ViewCubeRightTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the view cube top text.
    /// </summary>
    /// <value>
    /// The view cube top text.
    /// </value>
    public string ViewCubeTopText
    {
      get => (string)GetValue(ViewCubeTopTextProperty);
      set => SetValue(ViewCubeTopTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical position of view cube viewport.
    /// </summary>
    /// <value>
    /// The vertical position.
    /// </value>
    public VerticalAlignment ViewCubeVerticalPosition
    {
      get => (VerticalAlignment)GetValue(ViewCubeVerticalPositionProperty);
      set => SetValue(ViewCubeVerticalPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the view cube viewport.
    /// </summary>
    /// <value>
    /// The width of the view cube viewport.
    /// </value>
    public double ViewCubeWidth
    {
      get => (double)GetValue(ViewCubeWidthProperty);
      set => SetValue(ViewCubeWidthProperty, value);
    }

    /// <summary>
    /// Gets the viewport.
    /// </summary>
    /// <value>
    /// The viewport.
    /// </value>
    public Viewport3D Viewport => viewport;

    /// <summary>
    /// Gets or sets a value indicating whether to zoom around the mouse down point.
    /// </summary>
    /// <value>
    ///     <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> .
    /// </value>
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
    /// <value>
    /// The zoom cursor.
    /// </value>
    public Cursor ZoomCursor
    {
      get => (Cursor)GetValue(ZoomCursorProperty);
      set => SetValue(ZoomCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom extents gesture.
    /// </summary>
    public InputGesture ZoomExtentsGesture
    {
      get => (InputGesture)GetValue(ZoomExtentsGestureProperty);
      set => SetValue(ZoomExtentsGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to Zoom extents when the control has loaded.
    /// </summary>
    public bool ZoomExtentsWhenLoaded
    {
      get => (bool)GetValue(ZoomExtentsWhenLoadedProperty);
      set => SetValue(ZoomExtentsWhenLoadedProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom gesture.
    /// </summary>
    /// <value>
    /// The zoom gesture.
    /// </value>
    public MouseGesture ZoomGesture
    {
      get => (MouseGesture)GetValue(ZoomGestureProperty);
      set => SetValue(ZoomGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the alternative zoom gesture.
    /// </summary>
    /// <value>
    /// The alternative zoom gesture.
    /// </value>
    public MouseGesture ZoomGesture2
    {
      get => (MouseGesture)GetValue(ZoomGesture2Property);
      set => SetValue(ZoomGesture2Property, value);
    }

    /// <summary>
    /// Gets or sets the zoom rectangle cursor.
    /// </summary>
    /// <value>
    /// The zoom rectangle cursor.
    /// </value>
    public Cursor ZoomRectangleCursor
    {
      get => (Cursor)GetValue(ZoomRectangleCursorProperty);
      set => SetValue(ZoomRectangleCursorProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom rectangle gesture.
    /// </summary>
    /// <value>
    /// The zoom rectangle gesture.
    /// </value>
    public MouseGesture ZoomRectangleGesture
    {
      get => (MouseGesture)GetValue(ZoomRectangleGestureProperty);
      set => SetValue(ZoomRectangleGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom sensitivity.
    /// </summary>
    /// <value>
    /// The zoom sensitivity.
    /// </value>
    public double ZoomSensitivity
    {
      get => (double)GetValue(ZoomSensitivityProperty);
      set => SetValue(ZoomSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [limit FPS].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [limit FPS]; otherwise, <c>false</c>.
    /// </value>
    public bool LimitFPS
    {
      get => (bool)GetValue(LimitFPSProperty);
      set => SetValue(LimitFPSProperty, value);
    }

    #region Private Variables
    private bool limitFPS = true;
    private TimeSpan prevTime;
    #endregion
    /// <summary>
    /// Changes the camera direction.
    /// </summary>
    /// <param name="newDirection">
    /// The new direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void ChangeCameraDirection(Vector3D newDirection, double animationTime = 0)
    {
      if(cameraController != null)
      {
        cameraController.ChangeDirection(newDirection, animationTime);
      }
    }

    /// <summary>
    /// Copies the view to the clipboard.
    /// </summary>
    public void Copy() => Viewport.Copy(Viewport.ActualWidth * 2, Viewport.ActualHeight * 2, Brushes.White, 2);

    /// <summary>
    /// Copies the view to the clipboard as <c>xaml</c>.
    /// </summary>
    public void CopyXaml() => Clipboard.SetText(XamlHelper.GetXaml(Viewport.Children));

    /// <summary>
    /// Exports the view to the specified file.
    /// </summary>
    /// <remarks>
    /// Exporters.Filter contains all supported export file types.
    /// </remarks>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    public void Export(string fileName) => Viewport.Export(fileName, Background);

    /// <summary>
    /// Exports the view to a stereo image with the specified file name.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="stereoBase">The stereo base.</param>
    public void ExportStereo(string fileName, double stereoBase) => Viewport.ExportStereo(fileName, stereoBase, Background);

    /// <summary>
    /// Finds the nearest object.
    /// </summary>
    /// <param name="pt">
    /// The 3D position.
    /// </param>
    /// <param name="pos">
    /// The 2D position.
    /// </param>
    /// <param name="normal">
    /// The normal at the hit point.
    /// </param>
    /// <param name="obj">
    /// The object that was hit.
    /// </param>
    /// <returns>
    /// <c>True</c> if an object was hit.
    /// </returns>
    public bool FindNearest(Point pt, out Point3D pos, out Vector3D normal, out DependencyObject obj) => Viewport.FindNearest(pt, out pos, out normal, out obj);

    /// <summary>
    /// Finds the nearest point.
    /// </summary>
    /// <param name="pt">
    /// The point.
    /// </param>
    /// <returns>
    /// A point.
    /// </returns>
    public Point3D? FindNearestPoint(Point pt) => Viewport.FindNearestPoint(pt);

    /// <summary>
    /// Finds the <see cref="Visual3D" /> that is nearest the camera ray through the specified point.
    /// </summary>
    /// <param name="pt">
    /// The point.
    /// </param>
    /// <returns>
    /// The nearest <see cref="Visual3D" /> or <c>null</c> if no visual was hit.
    /// </returns>
    public Visual3D FindNearestVisual(Point pt) => Viewport.FindNearestVisual(pt);

    /// <summary>
    /// Changes the camera to look at the specified point.
    /// </summary>
    /// <param name="target">
    /// The point.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void LookAt(Point3D target, double animationTime = 0) => Camera.LookAt(target, animationTime);

    /// <summary>
    /// Changes the camera to look at the specified point.
    /// </summary>
    /// <param name="target">
    /// The target point.
    /// </param>
    /// <param name="distance">
    /// The distance.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void LookAt(Point3D target, double distance, double animationTime) => Camera.LookAt(target, distance, animationTime);

    /// <summary>
    /// Changes the camera to look at the specified point.
    /// </summary>
    /// <param name="target">The target point.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public void LookAt(Point3D target, Vector3D direction, double animationTime) => Camera.LookAt(target, direction, animationTime);

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call
    /// <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />
    /// .
    /// </summary>
    /// <exception cref="CrystalException">
    /// A part is missing from the template.
    /// </exception>
    public override void OnApplyTemplate()
    {
      if(adornerLayer == null)
      {
        adornerLayer = Template.FindName(PartAdornerLayer, this) as AdornerDecorator;
      }

      var grid = Template.FindName(PartViewportGrid, this) as Grid;
      if(grid == null)
      {
        throw new CrystalException("{0} is missing from the template.", PartViewportGrid);
      }

      grid.Children.Add(viewport);

      if(adornerLayer == null)
      {
        throw new CrystalException("{0} is missing from the template.", PartAdornerLayer);
      }

      if(cameraController == null)
      {
        cameraController = Template.FindName(PartCameraController, this) as CameraController;
        if(cameraController != null)
        {
          cameraController.Viewport = Viewport;
          cameraController.LimitFPS = limitFPS;
          cameraController.LookAtChanged += (s, e) => OnLookAtChanged();
          cameraController.ZoomedByRectangle += (s, e) => OnZoomedByRectangle();
        }
      }

      if(cameraController == null)
      {
        throw new CrystalException("{0} is missing from the template.", PartCameraController);
      }

      if(coordinateView == null)
      {
        coordinateView = Template.FindName(PartCoordinateView, this) as Viewport3D;

        coordinateSystemLights = new Model3DGroup();

        // coordinateSystemLights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
        // coordinateSystemLights.Children.Add(new AmbientLight(Colors.DarkGray));
        coordinateSystemLights.Children.Add(new AmbientLight(Colors.LightGray));

        if(coordinateView != null)
        {
          coordinateView.Camera = new PerspectiveCamera();
          coordinateView.Children.Add(new ModelVisual3D { Content = coordinateSystemLights });
        }
      }

      if(coordinateView == null)
      {
        throw new CrystalException("{0} is missing from the template.", PartCoordinateView);
      }

      if(viewCubeViewport == null)
      {
        viewCubeViewport = Template.FindName(PartViewCubeViewport, this) as Viewport3D;

        viewCubeLights = new Model3DGroup();
        viewCubeLights.Children.Add(new AmbientLight(Colors.White));
        if(viewCubeViewport != null)
        {
          viewCubeViewport.Camera = new PerspectiveCamera();
          viewCubeViewport.Children.Add(new ModelVisual3D { Content = viewCubeLights });
          viewCubeViewport.MouseEnter += ViewCubeViewportMouseEnter;
          viewCubeViewport.MouseLeave += ViewCubeViewportMouseLeave;
        }

        viewCube = Template.FindName(PartViewCube, this) as ViewCubeVisual3D;
        if(viewCube != null)
        {
          viewCube.Viewport = Viewport;
        }
      }

      // update the coordinateview camera
      OnCameraChanged();

      // add the default headlight
      OnHeadlightChanged();
      base.OnApplyTemplate();
    }

    /// <summary>
    /// Resets the camera.
    /// </summary>
    public void ResetCamera()
    {
      if(cameraController != null)
      {
        cameraController.ResetCamera();
      }
    }

    /// <summary>
    /// Sets the camera position and orientation.
    /// </summary>
    /// <param name="newPosition">
    /// The new camera position.
    /// </param>
    /// <param name="newDirection">
    /// The new camera look direction.
    /// </param>
    /// <param name="newUpDirection">
    /// The new camera up direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void SetView(Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime = 0)
      => Camera.AnimateTo(newPosition, newDirection, newUpDirection, animationTime);

    /// <summary>
    /// Sets the camera orientation and adjusts the camera position to fit the model into the view.
    /// </summary>
    /// <param name="newDirection">The new camera look direction.</param>
    /// <param name="newUpDirection">The new camera up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public void FitView(Vector3D newDirection, Vector3D newUpDirection, double animationTime = 0)
      => Camera.FitView(Viewport, newDirection, newUpDirection, animationTime);

    /// <summary>
    /// Zooms to the extents of the screen.
    /// </summary>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void ZoomExtents(double animationTime = 0)
    {
      if(cameraController != null)
      {
        cameraController.ZoomExtents(animationTime);
      }
    }

    /// <summary>
    /// Zooms to the extents of the specified bounding box.
    /// </summary>
    /// <param name="bounds">
    /// The bounding box.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void ZoomExtents(Rect3D bounds, double animationTime = 0) => Camera.ZoomExtents(Viewport, bounds, animationTime);

    /// <summary>
    /// Raises the LookAtChanged event.
    /// </summary>
    protected internal virtual void OnLookAtChanged() => RaiseEvent(new RoutedEventArgs(LookAtChangedEvent));

    /// <summary>
    /// Raises the ZoomedByRectangle event.
    /// </summary>
    protected internal virtual void OnZoomedByRectangle() => RaiseEvent(new RoutedEventArgs(ZoomedByRectangleEvent));

    /// <summary>
    /// Handles camera changes.
    /// </summary>
    protected virtual void OnCameraChanged()
    {
      // update the camera of the coordinate system
      if(coordinateView != null)
      {
        Camera.CopyDirectionOnly(coordinateView.Camera as PerspectiveCamera, 30);
      }

      // update the camera of the view cube
      if(viewCubeViewport != null)
      {
        Camera.CopyDirectionOnly(viewCubeViewport.Camera as PerspectiveCamera, 20);
      }

      // update the headlight and coordinate system light
      if(Camera != null)
      {
        if(headLight != null)
        {
          headLight.Direction = Camera.LookDirection;
        }

        if(coordinateSystemLights != null)
        {
          var cshl = coordinateSystemLights.Children[0] as DirectionalLight;
          if(cshl != null)
          {
            cshl.Direction = Camera.LookDirection;
          }
        }
      }

      if(ShowFieldOfView)
      {
        UpdateFieldOfViewInfo();
      }

      if(ShowCameraInfo)
      {
        UpdateCameraInfo();
      }
    }

    /// <summary>
    /// Handles changes to the <see cref="IsHeadLightEnabled" /> property.
    /// </summary>
    protected void OnHeadlightChanged()
    {
      if(lights == null)
      {
        return;
      }

      if(IsHeadLightEnabled && !lights.Children.Contains(headLight))
      {
        lights.Children.Add(headLight);
      }

      if(!IsHeadLightEnabled && lights.Children.Contains(headLight))
      {
        lights.Children.Remove(headLight);
      }
    }

    /// <summary>
    /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items" /> property changes.
    /// </summary>
    /// <param name="e">Information about the change.</param>
    /// <exception cref="System.NotImplementedException">
    /// Move operation not implemented.
    /// or
    /// Replace operation not implemented.
    /// </exception>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      switch(e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          AddItems(e.NewItems);
          break;

        case NotifyCollectionChangedAction.Move:
          throw new NotImplementedException("Move operation not implemented.");
        case NotifyCollectionChangedAction.Remove:
          RemoveItems(e.OldItems);
          break;

        case NotifyCollectionChangedAction.Replace:
          throw new NotImplementedException("Replace operation not implemented.");
        case NotifyCollectionChangedAction.Reset:
          Children.Clear();
          break;
      }
    }

    /// <summary>
    /// Called when the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property changes.
    /// </summary>
    /// <param name="oldValue">
    /// Old value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property.
    /// </param>
    /// <param name="newValue">
    /// New value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property.
    /// </param>
    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      RemoveItems(oldValue);
      AddItems(newValue);
    }

    /// <summary>
    /// Invoked when an unhandled MouseMove attached event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.
    /// </param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if(CalculateCursorPosition)
      {
        var pt = e.GetPosition(this);
        UpdateCursorPosition(pt);
      }
    }

    /// <summary>
    /// Raises the camera changed event.
    /// </summary>
    protected virtual void RaiseCameraChangedEvent() => RaiseEvent(new RoutedEventArgs(CameraChangedEvent));

    /// <summary>
    /// Updates the cursor position.
    /// </summary>
    /// <param name="pt">The position of the cursor (in viewport coordinates).</param>
    private void UpdateCursorPosition(Point pt)
    {
      CursorOnElementPosition = FindNearestPoint(pt);
      CursorPosition = Viewport.UnProject(pt);

      // Calculate the cursor ray
      Point3D cursorNearPlanePoint;
      Point3D cursorFarPlanePoint;
      var ok = Viewport.Point2DtoPoint3D(pt, out cursorNearPlanePoint, out cursorFarPlanePoint);
      if(ok)
      {
        var ray = new Ray3D(cursorFarPlanePoint, cursorNearPlanePoint);
        CursorRay = ray;
      }
      else
      {
        CursorOnConstructionPlanePosition = null;
        CursorRay = null;
      }

      // Calculate the intersection between the construction plane and the cursor ray
      if(CursorRay != null)
      {
        CursorOnConstructionPlanePosition = ConstructionPlane.LineIntersection(
            CursorRay.Origin,
            CursorRay.Origin + CursorRay.Direction);
      }
      else
      {
        CursorOnConstructionPlanePosition = null;
      }

      // TODO: remove this code when the CurrentPosition property is removed
#pragma warning disable 618
      if(CursorOnElementPosition.HasValue)
      {
        CurrentPosition = CursorOnElementPosition.Value;
      }
      else
      {
        if(CursorPosition.HasValue)
        {
          CurrentPosition = CursorPosition.Value;
        }
      }
#pragma warning restore 618
    }

    /// <summary>
    /// Adds the specified items.
    /// </summary>
    /// <param name="newValue">The items to add.</param>
    private void AddItems(IEnumerable newValue)
    {
      if(newValue != null)
      {
        foreach(var element in newValue)
        {
          var visual = element as Visual3D;
          if(visual != null)
          {
            Children.Add(visual);
          }
        }
      }
    }

    /// <summary>
    /// Handles the Changed event of the current camera.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void CameraPropertyChanged(object sender, EventArgs e)
    {
      // Raise notification
      RaiseCameraChangedEvent();

      // Update the CoordinateView camera and the headlight direction
      OnCameraChanged();
    }

    /// <summary>
    /// Handles the Rendering event of the CompositionTarget control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void CompositionTargetRendering(object sender, RenderingEventArgs e)
    {
      if(limitFPS && prevTime == e.RenderingTime)
      {
        return;
      }
      prevTime = e.RenderingTime;
      frameCounter++;
      if(ShowFrameRate && fpsWatch.ElapsedMilliseconds > 500)
      {
        FrameRate = (int)(frameCounter / (0.001 * fpsWatch.ElapsedMilliseconds));
        FrameRateText = FrameRate + " FPS";
        frameCounter = 0;
        fpsWatch.Reset();
        fpsWatch.Start();
      }

      // update the info fields every 100 frames
      // (it would be better to update only when the visual model of the Viewport3D changes)
      infoFrameCounter++;
      if(ShowTriangleCountInfo && infoFrameCounter > 100)
      {
        var count = viewport.GetTotalNumberOfTriangles();
        TriangleCountInfo = string.Format("Triangles: {0}", count);
        infoFrameCounter = 0;
      }
    }

    /// <summary>
    /// Handles the <see cref="ApplicationCommands.Copy" /> command.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
    private void CopyHandler(object sender, ExecutedRoutedEventArgs e)
    {
      // var vm = Viewport3DHelper.GetViewMatrix(Camera);
      // double ar = ActualWidth / ActualHeight;
      // var pm = Viewport3DHelper.GetProjectionMatrix(Camera, ar);
      // double w = 2 / pm.M11;
      // pm.OffsetX = -1
      // ;
      // pm.M11 *= 2;
      // pm.M22 *= 2;
      // var mc = new MatrixCamera(vm, pm);
      // viewport.Camera = mc;
      Copy();
    }

    /// <summary>
    /// Handles changes to the camera rotation mode.
    /// </summary>
    private void OnCameraRotationModeChanged()
    {
      if(CameraRotationMode != CameraRotationMode.Trackball && cameraController != null)
      {
        cameraController.ResetCameraUpDirection();
      }
    }

    /// <summary>
    /// Handles the Loaded event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void OnControlLoaded(object sender, RoutedEventArgs e)
    {
      OnItemsSourceChanged(ItemsSource, ItemsSource);

      if(!hasBeenLoadedBefore)
      {
        if(DefaultCamera != null)
        {
          DefaultCamera.Copy(perspectiveCamera);
          DefaultCamera.Copy(orthographicCamera);
        }

        hasBeenLoadedBefore = true;
      }

      UpdateRenderingEventSubscription();
      if(ZoomExtentsWhenLoaded)
      {
        ZoomExtents();
      }
    }

    /// <summary>
    /// Handles the Unloaded event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void OnControlUnloaded(object sender, RoutedEventArgs e) => UnsubscribeRenderingEvent();

    /// <summary>
    /// Handles changes to the <see cref="Orthographic" /> property.
    /// </summary>
    private void OnOrthographicChanged()
    {
      var oldCamera = Camera;
      if(Orthographic)
      {
        Camera = orthographicCamera;
      }
      else
      {
        Camera = perspectiveCamera;
      }

      oldCamera.Copy(Camera, false);
    }

    /// <summary>
    /// Handles changes to the <see cref="ShowFrameRate" /> property.
    /// </summary>
    private void OnShowFrameRateChanged() => UpdateRenderingEventSubscription();

    /// <summary>
    /// Handles changes to the <see cref="ShowTriangleCountInfo" /> property.
    /// </summary>
    private void OnShowTriangleCountInfoChanged() => UpdateRenderingEventSubscription();

    /// <summary>
    /// Handles the <see cref="OrthographicToggleCommand" />.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
    private void OrthographicToggle(object sender, ExecutedRoutedEventArgs e) => Orthographic = !Orthographic;

    /// <summary>
    /// Removes the specified items.
    /// </summary>
    /// <param name="oldValue">
    /// The items to remove.
    /// </param>
    private void RemoveItems(IEnumerable oldValue)
    {
      if(oldValue != null)
      {
        foreach(var element in oldValue)
        {
          var visual = element as Visual3D;
          if(visual != null)
          {
            Children.Remove(visual);
          }
        }
      }
    }

    /// <summary>
    /// Subscribes to the rendering event.
    /// </summary>
    private void SubscribeToRenderingEvent()
    {
      if(!isSubscribedToRenderingEvent)
      {
        RenderingEventManager.AddListener(renderingEventListener);
        isSubscribedToRenderingEvent = true;
      }
    }

    /// <summary>
    /// Unsubscribes the rendering event.
    /// </summary>
    private void UnsubscribeRenderingEvent()
    {
      if(isSubscribedToRenderingEvent)
      {
        RenderingEventManager.RemoveListener(renderingEventListener);
        isSubscribedToRenderingEvent = false;
      }
    }

    /// <summary>
    /// Updates the camera info.
    /// </summary>
    private void UpdateCameraInfo() => CameraInfo = Camera.GetInfo();

    /// <summary>
    /// Updates the field of view info.
    /// </summary>
    private void UpdateFieldOfViewInfo()
    {
      var pc = Camera as PerspectiveCamera;
      FieldOfViewText = pc != null ? string.Format("FoV ∠ {0:0}°", pc.FieldOfView) : null;
    }

    /// <summary>
    /// Updates the rendering event subscription.
    /// </summary>
    private void UpdateRenderingEventSubscription()
    {
      if(ShowFrameRate || ShowTriangleCountInfo)
      {
        SubscribeToRenderingEvent();
      }
      else
      {
        UnsubscribeRenderingEvent();
      }
    }

    /// <summary>
    /// Handles the mouse enter events on the view cube.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void ViewCubeViewportMouseEnter(object sender, MouseEventArgs e) => viewCubeViewport.AnimateOpacity(1.0, 200);

    /// <summary>
    /// Handles the mouse leave events on the view cube.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void ViewCubeViewportMouseLeave(object sender, MouseEventArgs e) => viewCubeViewport.AnimateOpacity(ViewCubeOpacity, 200);
  }
}
