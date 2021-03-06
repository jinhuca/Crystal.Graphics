using System.Windows.Media.Effects;

namespace Crystal.Graphics
{
  /// <summary>
  /// Provides an interlaced blending effect.
  /// </summary>
  /// <remarks>
  /// Usage:
  /// 1. Add the effect to the LEFT EYE UIElement.
  /// 2. Set RightInput to a VisualBrush of the RIGHT EYE UIElement.
  /// See the InterlacedView3D for an example.
  /// </remarks>
  public class InterlacedEffect : ShaderEffect
  {
    /// <summary>
    /// Identifies the <see cref="LeftInput"/> dependency property.
    /// </summary>
    /// <remarks>
    /// Brush-valued properties turn into sampler-property in the shader.
    /// This helper sets "ImplicitInput" as the default, meaning the default
    /// sampler is whatever the rendering of the element it's being applied to is.
    /// </remarks>
    public static readonly DependencyProperty LeftInputProperty = RegisterPixelShaderSamplerProperty(
        "LeftInput", typeof(InterlacedEffect), 0);

    /// <summary>
    /// Identifies the <see cref="EvenLeft"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EvenLeftProperty = DependencyProperty.Register(
        "EvenLeft",
        typeof(bool),
        typeof(InterlacedEffect),
        new UIPropertyMetadata(true, EvenLeftChanged));


    /// <summary>
    /// Identifies the <see cref="RightInput"/> dependency property.
    /// </summary>
    /// <remarks>
    /// Brush-valued properties turn into sampler-property in the shader.
    /// This helper sets "ImplicitInput" as the default, meaning the default
    /// sampler is whatever the rendering of the element it's being applied to is.
    /// </remarks>
    public static readonly DependencyProperty RightInputProperty = RegisterPixelShaderSamplerProperty(
        "RightInput", typeof(InterlacedEffect), 1);

    /// <summary>
    /// The effect file.
    /// </summary>
    private const string EffectFile = "ShaderEffects/InterlacedEffect.ps";

    /// <summary>
    /// This property is mapped to the offset variable within the pixel shader.
    /// </summary>
    private static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
        "Offset",
        typeof(float),
        typeof(InterlacedEffect),
        new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(1)));

    /// <summary>
    /// The shader.
    /// </summary>
    private static readonly PixelShader Shader = new();

    /// <summary>
    /// This property is mapped to the method variable within the pixel shader.
    /// </summary>
    private static readonly DependencyProperty ShaderMethodProperty = DependencyProperty.Register(
        "ShaderMethod",
        typeof(float),
        typeof(InterlacedEffect),
        new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(0)));

    /// <summary>
    /// Initializes static members of the <see cref="InterlacedEffect"/> class.
    /// </summary>
    static InterlacedEffect()
    {
      var a = typeof(InterlacedEffect).Assembly;
      var assemblyShortName = a.ToString().Split(',')[0];
      var uriString = "pack://application:,,,/" + assemblyShortName + ";component/" + EffectFile;
      Shader.UriSource = new Uri(uriString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "InterlacedEffect" /> class.
    /// </summary>
    public InterlacedEffect()
    {
      PixelShader = Shader;

      // Update each DependencyProperty that's registered with a shader register.  This
      // is needed to ensure the shader gets sent the proper default value.
      UpdateShaderValue(EvenLeftProperty);
      UpdateShaderValue(LeftInputProperty);
      UpdateShaderValue(RightInputProperty);
    }

    /// <summary>
    /// Gets or sets the left input brush.
    /// </summary>
    /// <value>The left input.</value>
    public Brush LeftInput
    {
      get => (Brush)GetValue(LeftInputProperty);

      set => SetValue(LeftInputProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether even lines should be for the left or right view.
    /// </summary>
    /// <value><c>True</c> if even lines should show the left view, <c>false</c> otherwise.</value>
    public bool EvenLeft
    {
      get => (bool)GetValue(EvenLeftProperty);

      set => SetValue(EvenLeftProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal offset.
    /// </summary>
    public float Offset
    {
      get => (float)GetValue(OffsetProperty);

      set => SetValue(OffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the right input brush.
    /// </summary>
    /// <value>The right input.</value>
    public Brush RightInput
    {
      get => (Brush)GetValue(RightInputProperty);

      set => SetValue(RightInputProperty, value);
    }

    /// <summary>
    /// Gets or sets the shader method.
    /// </summary>
    private float ShaderMethod
    {
      set => SetValue(ShaderMethodProperty, value);
    }

    /// <summary>
    /// The anaglyph method changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void EvenLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var ef = (InterlacedEffect)d;
      ef.ShaderMethod = ef.EvenLeft ? 0 : 1;
    }
  }
}