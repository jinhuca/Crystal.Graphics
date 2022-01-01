﻿namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that clones all the children of another visual element.
  /// </summary>
  /// <remarks>
  /// This is useful for stereo views.
  /// </remarks>
  public class ClonedVisual3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        "Source", typeof(ModelVisual3D), typeof(ClonedVisual3D), new UIPropertyMetadata(null, SourceChanged));

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>The source.</value>
    public ModelVisual3D Source
    {
      get => (ModelVisual3D)GetValue(SourceProperty);

      set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// The source changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ClonedVisual3D)d).OnSourceChanged();
    }

    /// <summary>
    /// The source changed.
    /// </summary>
    protected virtual void OnSourceChanged()
    {
      if(Source == null)
      {
        Content = null;
        return;
      }

      var clonedModel = Source.Content.Clone();
      Content = clonedModel;
    }

  }
}