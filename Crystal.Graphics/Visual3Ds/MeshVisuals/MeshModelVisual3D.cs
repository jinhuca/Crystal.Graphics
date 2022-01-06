using System.ComponentModel;

namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a base class for elements that contain one <see cref="GeometryModel3D"/> and front and back <see cref="Material"/>s.
  /// </summary>
  /// <remarks>
  /// Derived classes should override the Tessellate method to generate the geometry.
  /// </remarks>
  public abstract class MeshModelVisual3D : ModelVisual3D, IEditableObject
  {
    /// <summary>
    /// Gets or sets the material.
    /// </summary>
    /// <value>The material.</value>
    public Material Material
    {
      get => (Material)GetValue(MaterialProperty);
      set => SetValue(MaterialProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Material"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register(
      nameof(Material), typeof(Material), typeof(MeshModelVisual3D), new UIPropertyMetadata(MaterialHelper.CreateMaterial(Brushes.Blue), MaterialChanged));

    /// <summary>
    /// Gets or sets the back material.
    /// </summary>
    /// <value>The back material.</value>
    public Material BackMaterial
    {
      get => (Material)GetValue(BackMaterialProperty);
      set => SetValue(BackMaterialProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BackMaterial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackMaterialProperty = DependencyProperty.Register(
      nameof(BackMaterial), typeof(Material), typeof(MeshModelVisual3D), new UIPropertyMetadata(MaterialHelper.CreateMaterial(Brushes.LightBlue), MaterialChanged));

    /// <summary>
    /// Gets or sets the fill brush. This brush will be used for both the Material and BackMaterial.
    /// </summary>
    /// <value>The fill brush.</value>
    public Brush Fill
    {
      get => (Brush)GetValue(FillProperty);
      set => SetValue(FillProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Fill"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
      nameof(Fill), typeof(Brush), typeof(MeshModelVisual3D), new UIPropertyMetadata(null, FillChanged));

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="MeshModelVisual3D"/> is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the element is visible; otherwise, <c>false</c>.
    /// </value>
    public bool Visible
    {
      get => (bool)GetValue(VisibleProperty);
      set => SetValue(VisibleProperty, value);
    }

    /// <summary>
    /// The visibility property.
    /// </summary>
    public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register(
      nameof(Visible), typeof(bool), typeof(MeshModelVisual3D), new UIPropertyMetadata(true, VisibleChanged));

    /// <summary>
    /// A flag that is set when the element is in editing mode (<see cref="IEditableObject"/>, <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> and <see cref="M:System.ComponentModel.IEditableObject.EndEdit"/>).
    /// </summary>
    private bool isEditing;

    /// <summary>
    /// A flag that is set when the geometry is changed.
    /// </summary>
    private bool isGeometryChanged;

    /// <summary>
    /// A flag that is set when the material is changed.
    /// </summary>
    private bool isMaterialChanged;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MeshModelVisual3D" /> class.
    /// </summary>
    protected MeshModelVisual3D()
    {
      Content = new GeometryModel3D();
      UpdateModel();
    }

    /// <summary>
    ///   Gets the geometry model.
    /// </summary>
    /// <value>The geometry model.</value>
    public GeometryModel3D? Model => Content as GeometryModel3D;

    /// <summary>
    /// Begins an edit on the object.
    /// </summary>
    public void BeginEdit()
    {
      isEditing = true;
      isGeometryChanged = false;
      isMaterialChanged = false;
    }

    /// <summary>
    /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
    /// </summary>
    public void CancelEdit()
    {
      isEditing = false;
    }

    /// <summary>
    /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
    /// </summary>
    public void EndEdit()
    {
      isEditing = false;
      if(isGeometryChanged)
      {
        OnGeometryChanged();
      }

      if(isMaterialChanged)
      {
        OnMaterialChanged();
      }
    }

    /// <summary>
    /// Forces an update of the geometry and materials.
    /// </summary>
    public void UpdateModel()
    {
      OnGeometryChanged();
      OnMaterialChanged();
    }

    /// <summary>
    /// The visible flag changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void VisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MeshModelVisual3D)d).OnGeometryChanged();
    }

    /// <summary>
    /// The geometry was changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MeshModelVisual3D)d).OnGeometryChanged();
    }

    /// <summary>
    /// The Material or BackMaterial property was changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MeshModelVisual3D)d).OnMaterialChanged();
    }

    /// <summary>
    /// The Fill property was changed.
    /// </summary>
    protected virtual void OnFillChanged()
    {
      Material = MaterialHelper.CreateMaterial(Fill);
      BackMaterial = Material;
    }

    /// <summary>
    /// Handles changes in geometry or visible state.
    /// </summary>
    protected virtual void OnGeometryChanged()
    {
      if(!isEditing)
      {
        Model.Geometry = Visible ? Tessellate() : null;
      }
      else
      {
        // flag the geometry as changed, the geometry will be updated when the <see cref="M:System.ComponentModel.IEditableObject.EndEdit"/> is called.
        isGeometryChanged = true;
      }
    }

    /// <summary>
    /// Handles changes in material/back material.
    /// </summary>
    protected virtual void OnMaterialChanged()
    {
      if(!isEditing)
      {
        Model.Material = Material;
        Model.BackMaterial = BackMaterial;
      }
      else
      {
        isMaterialChanged = true;
      }
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected abstract MeshGeometry3D? Tessellate();

    /// <summary>
    /// Called when Fill is changed.
    /// </summary>
    /// <param name="d">
    /// The mesh element.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MeshModelVisual3D)d).OnFillChanged();
    }
  }
}