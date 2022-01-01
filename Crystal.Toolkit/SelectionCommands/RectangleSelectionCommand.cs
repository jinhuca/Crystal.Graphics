using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a command that shows a rectangle when the mouse is dragged and raises an event returning the models contained in the rectangle
  /// when the mouse button is released.
  /// </summary>
  public class RectangleSelectionCommand : SelectionCommand
  {
    /// <summary>
    /// The selection rectangle.
    /// </summary>
    private Rect selectionRect;

    /// <summary>
    /// The rectangle adorner.
    /// </summary>
    private RectangleAdorner rectangleAdorner;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="modelsSelectedEventHandler">The selection event handler.</param>
    public RectangleSelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> modelsSelectedEventHandler)
        : base(viewport, modelsSelectedEventHandler, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="visualsSelectedEventHandler">The selection event handler.</param>
    public RectangleSelectionCommand(Viewport3D viewport, EventHandler<VisualsSelectedEventArgs> visualsSelectedEventHandler)
        : base(viewport, null, visualsSelectedEventHandler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="modelsSelectedEventHandler">The selection event handler.</param>
    /// <param name="visualsSelectedEventHandler">The selection event handler.</param>
    public RectangleSelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> modelsSelectedEventHandler, EventHandler<VisualsSelectedEventArgs> visualsSelectedEventHandler)
        : base(viewport, modelsSelectedEventHandler, visualsSelectedEventHandler)
    {
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    protected override void Started(ManipulationEventArgs e)
    {
      base.Started(e);
      selectionRect = new Rect(MouseDownPoint, MouseDownPoint);
      ShowRectangle();
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    protected override void Delta(ManipulationEventArgs e)
    {
      base.Delta(e);
      selectionRect = new Rect(MouseDownPoint, e.CurrentPosition);
      UpdateRectangle();
    }

    /// <summary>
    /// The customized complete operation when the manipulation is completed.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    protected override void Completed(ManipulationEventArgs e)
    {
      HideRectangle();

      var res = Viewport.FindHits(selectionRect, SelectionHitMode);

      var selectedModels = res.Select(hit => hit.Model).ToList();

      // We do not handle the point selection, unless no models are selected. If no models are selected, we clear the
      // existing selection.
      if(selectionRect.Size.Equals(default(Size)) && selectedModels.Any())
      {
        return;
      }

      OnModelsSelected(new ModelsSelectedByRectangleEventArgs(selectedModels, selectionRect));
      var selectedVisuals = res.Select(hit => hit.Visual).ToList();
      OnVisualsSelected(new VisualsSelectedByRectangleEventArgs(selectedVisuals, selectionRect));
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>
    /// A cursor.
    /// </returns>
    protected override Cursor GetCursor()
    {
      return Cursors.Arrow;
    }

    /// <summary>
    /// Hides the selection rectangle.
    /// </summary>
    private void HideRectangle()
    {
      var myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
      if(myAdornerLayer == null) { return; }
      if(rectangleAdorner != null)
      {
        myAdornerLayer.Remove(rectangleAdorner);
      }

      rectangleAdorner = null;

      Viewport.InvalidateVisual();
    }

    /// <summary>
    /// Updates the selection rectangle.
    /// </summary>
    private void UpdateRectangle()
    {
      if(rectangleAdorner == null)
      {
        return;
      }

      rectangleAdorner.Rectangle = selectionRect;
      rectangleAdorner.InvalidateVisual();
    }

    /// <summary>
    /// Shows the selection rectangle.
    /// </summary>
    private void ShowRectangle()
    {
      if(rectangleAdorner != null)
      {
        return;
      }

      var adornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
      if(adornerLayer == null) { return; }
      rectangleAdorner = new RectangleAdorner(Viewport, selectionRect, Colors.LightGray, Colors.Black, 1, 1, 0, DashStyles.Dash);
      adornerLayer.Add(rectangleAdorner);
    }
  }
}