namespace Crystal.Graphics
{
  /// <summary>
  /// Contains a set of predefined gradient brushes.
  /// </summary>
  /// <example>
  /// Usage in XAML:
  ///  <code>
  /// Fill="{x:Static GradientBrushes.Hue}"
  ///  </code>
  /// </example>
  public static class GradientBrushes
  {
    /// <summary>
    /// A blue-white-red gradient brush.
    /// </summary>
    public static LinearGradientBrush BlueWhiteRed = BrushHelper.CreateGradientBrush(Colors.Blue, Colors.White, Colors.Red);

    /// <summary>
    /// A hue gradient brush.
    /// </summary>
    public static LinearGradientBrush Hue = BrushHelper.CreateHsvBrush();

    /// <summary>
    /// A hue gradient brush with 12 stripes.
    /// </summary>
    public static LinearGradientBrush HueStripes = BrushHelper.CreateSteppedGradientBrush(Hue, 12);

    /// <summary>
    /// A rainbow gradient brush.
    /// </summary>
    public static LinearGradientBrush Rainbow = BrushHelper.CreateRainbowBrush();

    /// <summary>
    /// A rainbow brush with 12 stripes.
    /// </summary>
    public static LinearGradientBrush RainbowStripes = BrushHelper.CreateSteppedGradientBrush(Rainbow, 12);

  }
}