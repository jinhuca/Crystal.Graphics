namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a visual element that shows translation and rotation manipulators.
  /// </summary>
  public class CombinedManipulator : ModelVisual3D
  {
    /// <summary>
    /// Gets or sets a value indicating whether this instance can rotate X.
    /// </summary>
    /// <value> <c>true</c> if this instance can rotate X; otherwise, <c>false</c> . </value>
    public bool CanRotateX
    {
      get => (bool)GetValue(CanRotateXProperty);
      set => SetValue(CanRotateXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanRotateX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanRotateXProperty = DependencyProperty.Register(
      nameof(CanRotateX), typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));
    
    /// <summary>
    /// Gets or sets a value indicating whether this instance can rotate Y.
    /// </summary>
    /// <value> <c>true</c> if this instance can rotate Y; otherwise, <c>false</c> . </value>
    public bool CanRotateY
    {
      get => (bool)GetValue(CanRotateYProperty);
      set => SetValue(CanRotateYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanRotateY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanRotateYProperty = DependencyProperty.Register(
      nameof(CanRotateY), typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

    /// <summary>
    /// Gets or sets a value indicating whether this instance can rotate Z.
    /// </summary>
    /// <value> <c>true</c> if this instance can rotate Z; otherwise, <c>false</c> . </value>
    public bool CanRotateZ
    {
      get => (bool)GetValue(CanRotateZProperty);
      set => SetValue(CanRotateZProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanRotateZ"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanRotateZProperty = DependencyProperty.Register(
      nameof(CanRotateZ), typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

    /// <summary>
    /// Gets or sets a value indicating whether this instance can translate X.
    /// </summary>
    /// <value> <c>true</c> if this instance can translate X; otherwise, <c>false</c> . </value>
    public bool CanTranslateX
    {
      get => (bool)GetValue(CanTranslateXProperty);
      set => SetValue(CanTranslateXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanTranslateX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanTranslateXProperty = DependencyProperty.Register(
      nameof(CanTranslateX), typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

    /// <summary>
    /// Gets or sets a value indicating whether this instance can translate Y.
    /// </summary>
    /// <value> <c>true</c> if this instance can translate Y; otherwise, <c>false</c> . </value>
    public bool CanTranslateY
    {
      get => (bool)GetValue(CanTranslateYProperty);
      set => SetValue(CanTranslateYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanTranslateY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanTranslateYProperty = DependencyProperty.Register(
      nameof(CanTranslateY), typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

    /// <summary>
    /// Gets or sets a value indicating whether this instance can translate Z.
    /// </summary>
    /// <value> <c>true</c> if this instance can translate Z; otherwise, <c>false</c> . </value>
    public bool CanTranslateZ
    {
      get => (bool)GetValue(CanTranslateZProperty);
      set => SetValue(CanTranslateZProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanTranslateZ"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanTranslateZProperty = DependencyProperty.Register(
      nameof(CanTranslateZ), typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

    /// <summary>
    /// Gets or sets the diameter.
    /// </summary>
    /// <value> The diameter. </value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);
      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
      nameof(Diameter), typeof(double), typeof(CombinedManipulator), new UIPropertyMetadata(2.0));

    /// <summary>
    /// Gets or sets the target transform.
    /// </summary>
    /// <value> The target transform. </value>
    public Transform3D TargetTransform
    {
      get => (Transform3D)GetValue(TargetTransformProperty);
      set => SetValue(TargetTransformProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TargetTransform"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetTransformProperty = DependencyProperty.Register(
      nameof(TargetTransform), typeof(Transform3D), typeof(CombinedManipulator), new FrameworkPropertyMetadata(Transform3D.Identity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// The rotate x manipulator.
    /// </summary>
    private readonly RotateManipulator rotateXManipulator;

    /// <summary>
    /// The rotate y manipulator.
    /// </summary>
    private readonly RotateManipulator rotateYManipulator;

    /// <summary>
    /// The rotate z manipulator.
    /// </summary>
    private readonly RotateManipulator rotateZManipulator;

    /// <summary>
    /// The translate x manipulator.
    /// </summary>
    private readonly TranslateManipulator translateXManipulator;

    /// <summary>
    /// The translate y manipulator.
    /// </summary>
    private readonly TranslateManipulator translateYManipulator;

    /// <summary>
    /// The translate z manipulator.
    /// </summary>
    private readonly TranslateManipulator translateZManipulator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedManipulator" /> class.
    /// </summary>
    public CombinedManipulator()
    {
      translateXManipulator = new TranslateManipulator
      {
        Direction = new Vector3D(1, 0, 0),
        Color = Colors.Red
      };
      translateYManipulator = new TranslateManipulator
      {
        Direction = new Vector3D(0, 1, 0),
        Color = Colors.Green
      };
      translateZManipulator = new TranslateManipulator
      {
        Direction = new Vector3D(0, 0, 1),
        Color = Colors.Blue
      };
      rotateXManipulator = new RotateManipulator { Axis = new Vector3D(1, 0, 0), Color = Colors.Red };
      rotateYManipulator = new RotateManipulator { Axis = new Vector3D(0, 1, 0), Color = Colors.Green };
      rotateZManipulator = new RotateManipulator { Axis = new Vector3D(0, 0, 1), Color = Colors.Blue };

      BindingOperations.SetBinding(this, TransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      BindingOperations.SetBinding(translateXManipulator, Manipulator.TargetTransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      BindingOperations.SetBinding(translateYManipulator, Manipulator.TargetTransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      BindingOperations.SetBinding(translateZManipulator, Manipulator.TargetTransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      BindingOperations.SetBinding(rotateXManipulator, RotateManipulator.DiameterProperty, new Binding(nameof(Diameter)) { Source = this });
      BindingOperations.SetBinding(rotateYManipulator, RotateManipulator.DiameterProperty, new Binding(nameof(Diameter)) { Source = this });
      BindingOperations.SetBinding(rotateZManipulator, RotateManipulator.DiameterProperty, new Binding(nameof(Diameter)) { Source = this });
      BindingOperations.SetBinding(rotateXManipulator, Manipulator.TargetTransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      BindingOperations.SetBinding(rotateYManipulator, Manipulator.TargetTransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      BindingOperations.SetBinding(rotateZManipulator, Manipulator.TargetTransformProperty, new Binding(nameof(TargetTransform)) { Source = this });
      UpdateChildren();
    }

    /// <summary>
    /// Gets or sets the offset of the visual (this vector is added to the Position point).
    /// </summary>
    /// <value> The offset. </value>
    public Vector3D Offset
    {
      get => translateXManipulator.Offset;
      set
      {
        translateXManipulator.Offset = value;
        translateYManipulator.Offset = value;
        translateZManipulator.Offset = value;
        rotateXManipulator.Offset = value;
        rotateYManipulator.Offset = value;
        rotateZManipulator.Offset = value;
      }
    }

    /// <summary>
    /// Gets or sets the pivot point of the manipulator.
    /// </summary>
    /// <value> The position. </value>
    public Point3D Pivot
    {
      get => rotateXManipulator.Pivot;
      set
      {
        rotateXManipulator.Pivot = value;
        rotateYManipulator.Pivot = value;
        rotateZManipulator.Pivot = value;
      }
    }

    /// <summary>
    /// Gets or sets the position of the manipulator.
    /// </summary>
    /// <value> The position. </value>
    public Point3D Position
    {
      get => translateXManipulator.Position;
      set
      {
        translateXManipulator.Position = value;
        translateYManipulator.Position = value;
        translateZManipulator.Position = value;
        rotateXManipulator.Position = value;
        rotateYManipulator.Position = value;
        rotateZManipulator.Position = value;
      }
    }

    /// <summary>
    /// Binds this manipulator to a given Visual3D.
    /// </summary>
    /// <param name="source">
    /// Source Visual3D which receives the manipulator transforms.
    /// </param>
    public virtual void Bind(ModelVisual3D source)
    {
      BindingOperations.SetBinding(this, TargetTransformProperty, new Binding(nameof(Transform)) { Source = source });
      BindingOperations.SetBinding(this, TransformProperty, new Binding(nameof(Transform)) { Source = source });
    }

    /// <summary>
    /// Releases the binding of this manipulator.
    /// </summary>
    public virtual void UnBind()
    {
      BindingOperations.ClearBinding(this, TargetTransformProperty);
      BindingOperations.ClearBinding(this, TransformProperty);
    }

    /// <summary>
    /// Updates the child visuals.
    /// </summary>
    protected void UpdateChildren()
    {
      Children.Clear();
      if(CanTranslateX)
      {
        Children.Add(translateXManipulator);
      }

      if(CanTranslateY)
      {
        Children.Add(translateYManipulator);
      }

      if(CanTranslateZ)
      {
        Children.Add(translateZManipulator);
      }

      if(CanRotateX)
      {
        Children.Add(rotateXManipulator);
      }

      if(CanRotateY)
      {
        Children.Add(rotateYManipulator);
      }

      if(CanRotateZ)
      {
        Children.Add(rotateZManipulator);
      }
    }

    /// <summary>
    /// Handles changes in properties related to the child visuals.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void ChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CombinedManipulator)d).UpdateChildren();
    }
  }
}