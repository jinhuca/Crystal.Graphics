﻿using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Markup extension for key and mouse gestures.
  /// </summary>
  public class KeyGestureExtension : MarkupExtension
  {
    // http://www.bryanewert.net/journal/2010/6/8/how-to-use-markupextension-for-a-string-resource-that-define.html

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyGestureExtension"/> class.
    /// </summary>
    /// <param name="gesture">The gesture.</param>
    public KeyGestureExtension(string gesture)
    {
      var kgc = new KeyGestureConverter();
      this.gesture = kgc.ConvertFromString(gesture) as KeyGesture;
    }

    private readonly KeyGesture gesture;

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns></returns>
    public override object ProvideValue(IServiceProvider service)
    {
      return gesture;
    }
  }
}