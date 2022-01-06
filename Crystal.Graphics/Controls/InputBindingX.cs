using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Input binding supporting binding the Gesture.
  /// </summary>
  public class InputBindingX : InputBinding
  {
    /// <summary>
    /// Gets or sets the gesture.
    /// </summary>
    /// <value>The gesture.</value>
    public InputGesture Gesture
    {
      get => (InputGesture)GetValue(GestureProperty);
      set => SetValue(GestureProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Gesture"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GestureProperty =
        DependencyProperty.Register("Gesture", typeof(InputGesture), typeof(InputBindingX), new UIPropertyMetadata(null, GestureChanged));

    /// <summary>
    /// Gesture the changed.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void GestureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InputBindingX)d).OnGestureChanged();
    }

    /// <summary>
    /// Called when [gesture changed].
    /// </summary>
    protected virtual void OnGestureChanged()
    {
      base.Gesture = Gesture;
    }
  }
}