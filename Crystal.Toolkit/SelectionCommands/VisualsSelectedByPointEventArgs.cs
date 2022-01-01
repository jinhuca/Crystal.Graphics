namespace Crystal.Graphics
{
  /// <summary>
  /// Provides event data for the VisualsSelected event of the <see cref="PointSelectionCommand" />.
  /// </summary>
  public class VisualsSelectedByPointEventArgs : VisualsSelectedEventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VisualsSelectedByPointEventArgs"/> class.
    /// </summary>
    /// <param name="selectedVisuals">The selected visuals.</param>
    /// <param name="position">The position.</param>
    /// <remarks>
    /// For the visuals selected by point, they are sorted by distance in ascending order.
    /// </remarks>
    public VisualsSelectedByPointEventArgs(IList<Visual3D> selectedVisuals, Point position)
        : base(selectedVisuals, true)
    {
      Position = position;
    }

    /// <summary>
    /// Gets the rectangle of selection.
    /// </summary>
    public Point Position { get; private set; }
  }
}