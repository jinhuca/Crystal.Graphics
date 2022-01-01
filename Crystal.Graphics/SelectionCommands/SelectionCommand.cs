using System.Windows.Input;

namespace Crystal.Graphics
{
  /// <summary>
  /// Provides an abstract base class for mouse selection commands.
  /// </summary>
  public abstract class SelectionCommand : ICommand
  {
    /// <summary>
    /// The viewport of the command.
    /// </summary>
    protected readonly Viewport3D Viewport;

    /// <summary>
    /// Keeps track of the old cursor.
    /// </summary>
    private Cursor oldCursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionCommand"/> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="eventHandlerModels">The selection event handler for models.</param>
    /// <param name="eventHandlerVisuals">The selection event handler for visuals.</param>
    protected SelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> eventHandlerModels, EventHandler<VisualsSelectedEventArgs> eventHandlerVisuals)
    {
      Viewport = viewport;
      ModelsSelected = eventHandlerModels;
      VisualsSelected = eventHandlerVisuals;
    }

    /// <summary>
    /// Occurs when <see cref="CanExecute" /> is changed.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Occurs when models are selected.
    /// </summary>
    private event EventHandler<ModelsSelectedEventArgs> ModelsSelected;

    /// <summary>
    /// Occurs when visuals are selected.
    /// </summary>
    private event EventHandler<VisualsSelectedEventArgs> VisualsSelected;

    /// <summary>
    /// Gets or sets the selection hit mode.
    /// </summary>
    public SelectionHitMode SelectionHitMode { get; set; }

    /// <summary>
    /// Gets the mouse down point (2D screen coordinates).
    /// </summary>
    protected Point MouseDownPoint { get; private set; }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">
    /// The parameter.
    /// </param>
    public void Execute(object parameter)
    {
      OnMouseDown(Viewport);
    }

    /// <summary>
    /// Checks whether the command can be executed.
    /// </summary>
    /// <param name="parameter">
    /// The parameter.
    /// </param>
    /// <returns>
    /// <c>true</c> if the command can be executed. Otherwise, it returns <c>false</c>.
    /// </returns>
    public bool CanExecute(object parameter)
    {
      return true;
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void Started(ManipulationEventArgs e)
    {
      MouseDownPoint = e.CurrentPosition;
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void Delta(ManipulationEventArgs e)
    {
    }

    /// <summary>
    /// Occurs when the manipulation is completed.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void Completed(ManipulationEventArgs e)
    {
    }

    /// <summary>
    /// Raises the <see cref="E:ModelsSelected" /> event.
    /// </summary>
    /// <param name="e">The <see cref="ModelsSelectedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnModelsSelected(ModelsSelectedEventArgs e)
    {
      var handler = ModelsSelected;
      if(handler != null)
      {
        handler(Viewport, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="E:VisualsSelected" /> event.
    /// </summary>
    /// <param name="e">The <see cref="VisualsSelectedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnVisualsSelected(VisualsSelectedEventArgs e)
    {
      var handler = VisualsSelected;
      if(handler != null)
      {
        handler(Viewport, e);
      }
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>
    /// A cursor.
    /// </returns>
    protected abstract Cursor GetCursor();

    /// <summary>
    /// Called when the mouse button is pressed down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    protected virtual void OnMouseDown(object sender)
    {
      Viewport.MouseMove += OnMouseMove;
      Viewport.MouseUp += OnMouseUp;

      Viewport.Focus();
      Viewport.CaptureMouse();

      Started(new ManipulationEventArgs(Mouse.GetPosition(Viewport)));

      oldCursor = Viewport.Cursor;
      Viewport.Cursor = GetCursor();
    }

    /// <summary>
    /// Called when the mouse button is released.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      Viewport.MouseMove -= OnMouseMove;
      Viewport.MouseUp -= OnMouseUp;
      Viewport.ReleaseMouseCapture();
      Viewport.Cursor = oldCursor;
      Completed(new ManipulationEventArgs(Mouse.GetPosition(Viewport)));
      e.Handled = true;
    }

    /// <summary>
    /// Called when the mouse is move on the control.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected virtual void OnMouseMove(object sender, MouseEventArgs e)
    {
      Delta(new ManipulationEventArgs(Mouse.GetPosition(Viewport)));
      e.Handled = true;
    }

    /// <summary>
    /// Called when the condition of execution is changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected virtual void OnCanExecutedChanged(object sender, EventArgs e)
    {
      var handler = CanExecuteChanged;
      if(handler != null)
      {
        handler(sender, e);
      }
    }
  }
}