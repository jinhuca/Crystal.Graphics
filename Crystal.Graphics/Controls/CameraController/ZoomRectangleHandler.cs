using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Handles rectangle zooming.
  /// </summary>
  internal class ZoomRectangleHandler : MouseGestureHandler
  {
    /// <summary>
    /// The zoom rectangle.
    /// </summary>
    private Rect zoomRectangle;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZoomRectangleHandler"/> class.
    /// </summary>
    /// <param name="controller">
    /// The controller.
    /// </param>
    public ZoomRectangleHandler(CameraController controller)
        : base(controller)
    {
    }

    /// <summary>
    /// Occurs when the manipulation is completed.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Completed(ManipulationEventArgs e)
    {
      base.Completed(e);
      Controller.HideRectangle();
      ZoomRectangle(zoomRectangle);
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Delta(ManipulationEventArgs e)
    {
      base.Delta(e);

      var ar = Controller.ActualHeight / Controller.ActualWidth;
      var delta = MouseDownPoint - e.CurrentPosition;

      if(Math.Abs(delta.Y / delta.X) < ar)
      {
        delta.Y = Math.Sign(delta.Y) * Math.Abs(delta.X * ar);
      }
      else
      {
        delta.X = Math.Sign(delta.X) * Math.Abs(delta.Y / ar);
      }

      zoomRectangle = new Rect(MouseDownPoint, MouseDownPoint - delta);

      Controller.UpdateRectangle(zoomRectangle);
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    public override void Started(ManipulationEventArgs e)
    {
      base.Started(e);
      zoomRectangle = new Rect(MouseDownPoint, MouseDownPoint);
      Controller.ShowRectangle(zoomRectangle, Colors.LightGray, Colors.Black);
    }

    /// <summary>
    /// Zooms to the specified rectangle.
    /// </summary>
    /// <param name="rectangle">
    /// The zoom rectangle.
    /// </param>
    public void ZoomRectangle(Rect rectangle)
    {
      if(!Controller.IsZoomEnabled)
      {
        return;
      }

      if(rectangle.Width < 10 || rectangle.Height < 10)
      {
        return;
      }

      Camera.ZoomToRectangle(Viewport, rectangle);
      Controller.OnZoomedByRectangle();
    }

    /// <summary>
    /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
    /// </summary>
    /// <returns>True if the execution can continue.</returns>
    protected override bool CanExecute()
    {
      return Controller.IsZoomEnabled;
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>A cursor.</returns>
    protected override Cursor GetCursor()
    {
      return Controller.ZoomRectangleCursor;
    }

  }
}