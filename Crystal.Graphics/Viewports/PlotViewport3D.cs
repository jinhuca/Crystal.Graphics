namespace Crystal.Graphics
{
  public abstract class PlotViewport3D : CrystalViewport3D
  {
    #region Bounding box with axes indications

    public double BoundingBoxThickness
    {
      get => (double)GetValue(BoundingBoxThicknessProperty);
      set => SetValue(BoundingBoxThicknessProperty, value);
    }

    public static readonly DependencyProperty BoundingBoxThicknessProperty = DependencyProperty.Register(
      nameof(BoundingBoxThickness), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.005d, ModelChanged));

    public Material BoundingBoxMaterial
    {
      get => (Material)GetValue(BoundingBoxMaterialProperty);
      set => SetValue(BoundingBoxMaterialProperty, value);
    }

    public static readonly DependencyProperty BoundingBoxMaterialProperty = DependencyProperty.Register(
      nameof(BoundingBoxMaterial), typeof(Material), typeof(PointPlotViewport3D), new UIPropertyMetadata(Materials.Black, ModelChanged));

    #endregion Bounding box with axes indications

    #region Point Dependency Properties
    
    public new double FontSize
    {
      get => (double)GetValue(FontSizeProperty);
      set => SetValue(FontSizeProperty, value);
    }

    public new static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
      nameof(FontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(.1d, ModelChanged));

    public double SphereSize
    {
      get => (double)GetValue(SphereSizeProperty);
      set => SetValue(SphereSizeProperty, value);
    }

    public static readonly DependencyProperty SphereSizeProperty = DependencyProperty.Register(
      nameof(SphereSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(.15d, ModelChanged));

    #endregion Point Dependency Properties

    #region Axes Intervals

    public double IntervalX
    {
      get => (double)GetValue(IntervalXNewProperty);
      set => SetValue(IntervalXNewProperty, value);
    }

    public static readonly DependencyProperty IntervalXNewProperty = DependencyProperty.Register(
      nameof(IntervalX), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double IntervalY
    {
      get => (double)GetValue(IntervalYProperty);
      set => SetValue(IntervalYProperty, value);
    }

    public static readonly DependencyProperty IntervalYProperty = DependencyProperty.Register(
      nameof(IntervalY), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(1.0d, ModelChanged));

    public double IntervalZ
    {
      get => (double)GetValue(IntervalZProperty);
      set => SetValue(IntervalZProperty, value);
    }

    public static readonly DependencyProperty IntervalZProperty = DependencyProperty.Register(
      nameof(IntervalZ), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(1.0d, ModelChanged));

    #endregion Axes Intervals

    #region X-Axis Title Dependency Properties

    public string XAxisTitleContent
    {
      get => (string)GetValue(XAxisTitleContentProperty);
      set => SetValue(XAxisTitleContentProperty, value);
    }

    public static readonly DependencyProperty XAxisTitleContentProperty = DependencyProperty.Register(
      nameof(XAxisTitleContent), typeof(string), typeof(PointPlotViewport3D), new UIPropertyMetadata("X-axis", ModelChanged));

    public double XAxisTitleFontSize
    {
      get => (double)GetValue(XAxisTitleFontSizeProperty);
      set => SetValue(XAxisTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty XAxisTitleFontSizeProperty = DependencyProperty.Register(
      nameof(XAxisTitleFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    public Brush XAxisTitleBrush
    {
      get => (Brush)GetValue(XAxisTitleBrushProperty);
      set => SetValue(XAxisTitleBrushProperty, value);
    }

    public static readonly DependencyProperty XAxisTitleBrushProperty = DependencyProperty.Register(
      nameof(XAxisTitleBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Black, ModelChanged));

    #endregion X-Axis Title Dependency Properties

    #region X-Axis Label Dependency Properties

    public Brush XAxisLabelBrush
    {
      get => (Brush)GetValue(XAxisLabelBrushProperty);
      set => SetValue(XAxisLabelBrushProperty, value);
    }

    public static readonly DependencyProperty XAxisLabelBrushProperty = DependencyProperty.Register(
      nameof(XAxisLabelBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Blue, ModelChanged));

    public double XAxisLabelFontSize
    {
      get => (double)GetValue(XAxisLabelFontSizeProperty);
      set => SetValue(XAxisLabelFontSizeProperty, value);
    }

    public static readonly DependencyProperty XAxisLabelFontSizeProperty = DependencyProperty.Register(
      nameof(XAxisLabelFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    #endregion X-Axis Label Dependency Properties

    #region Y-Axis Title Dependency Properties

    public string YAxisTitleContent
    {
      get => (string)GetValue(YAxisTitleContentProperty);
      set => SetValue(YAxisTitleContentProperty, value);
    }

    public static readonly DependencyProperty YAxisTitleContentProperty = DependencyProperty.Register(
      nameof(YAxisTitleContent), typeof(string), typeof(PointPlotViewport3D), new UIPropertyMetadata("Y-Axis", ModelChanged));

    public double YAxisTitleFontSize
    {
      get => (double)GetValue(YAxisTitleFontSizeProperty);
      set => SetValue(YAxisTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty YAxisTitleFontSizeProperty = DependencyProperty.Register(
      nameof(YAxisTitleFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    public Brush YAxisTitleBrush
    {
      get => (Brush)GetValue(YAxisTitleBrushProperty);
      set => SetValue(YAxisTitleBrushProperty, value);
    }

    public static readonly DependencyProperty YAxisTitleBrushProperty = DependencyProperty.Register(
      nameof(YAxisTitleBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Black, ModelChanged));

    #endregion Y-Axis Title Dependency Properties

    #region Y-Axis Label Dependency Properties

    public Brush YAxisLabelBrush
    {
      get => (Brush)GetValue(YAxisLabelBrushProperty);
      set => SetValue(YAxisLabelBrushProperty, value);
    }

    public static readonly DependencyProperty YAxisLabelBrushProperty = DependencyProperty.Register(
      nameof(YAxisLabelBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Blue, ModelChanged));

    public double YAxisLabelFontSize
    {
      get => (double)GetValue(YAxisLabelFontSizeProperty);
      set => SetValue(YAxisLabelFontSizeProperty, value);
    }

    public static readonly DependencyProperty YAxisLabelFontSizeProperty = DependencyProperty.Register(
      nameof(YAxisLabelFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    #endregion Y-Axis Label Dependency Properties

    #region Z-Axis Title Dependency Properties

    public string ZAxisTitleContent
    {
      get => (string)GetValue(ZAxisTitleContentProperty);
      set => SetValue(ZAxisTitleContentProperty, value);
    }

    public static readonly DependencyProperty ZAxisTitleContentProperty = DependencyProperty.Register(
      nameof(ZAxisTitleContent), typeof(string), typeof(PointPlotViewport3D), new UIPropertyMetadata("Z-axis", ModelChanged));

    public double ZAxisTitleFontSize
    {
      get => (double)GetValue(ZAxisTitleFontSizeProperty);
      set => SetValue(ZAxisTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty ZAxisTitleFontSizeProperty = DependencyProperty.Register(
      nameof(ZAxisTitleFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    public Brush ZAxisTitleBrush
    {
      get => (Brush)GetValue(ZAxisTitleBrushProperty);
      set => SetValue(ZAxisTitleBrushProperty, value);
    }

    public static readonly DependencyProperty ZAxisTitleBrushProperty = DependencyProperty.Register(
      nameof(ZAxisTitleBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Black, ModelChanged));

    #endregion Z-Axis Title Dependency Properties

    #region Z-Axis Label Dependency Properties

    public Brush ZAxisLabelBrush
    {
      get => (Brush)GetValue(ZAxisLabelBrushProperty);
      set => SetValue(ZAxisLabelBrushProperty, value);
    }

    public static readonly DependencyProperty ZAxisLabelBrushProperty = DependencyProperty.Register(
      nameof(ZAxisLabelBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(Brushes.Blue, ModelChanged));

    public double ZAxisLabelFontSize
    {
      get => (double)GetValue(ZAxisLabelFontSizeProperty);
      set => SetValue(ZAxisLabelFontSizeProperty, value);
    }

    public static readonly DependencyProperty ZAxisLabelFontSizeProperty = DependencyProperty.Register(
      nameof(ZAxisLabelFontSize), typeof(double), typeof(PointPlotViewport3D), new UIPropertyMetadata(0.1d, ModelChanged));

    #endregion Z-Axis Label Dependency Properties
    
    public bool ShowGridLine
    {
      get => (bool)GetValue(ShowGridLineProperty);
      set => SetValue(ShowGridLineProperty, value);
    }

    public static readonly DependencyProperty ShowGridLineProperty = DependencyProperty.Register(
      nameof(ShowGridLine), typeof(bool), typeof(PlotViewport3D), new PropertyMetadata(false, ModelChanged));

    public static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }
  }
}
