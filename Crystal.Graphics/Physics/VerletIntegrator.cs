namespace Crystal.Graphics
{
  /// <summary>
  /// Provides a verlet integrator.
  /// </summary>
  /// <remarks>
  /// http://en.wikipedia.org/wiki/Verlet_integration
  /// http://www.gamasutra.com/resource_guide/20030121/jacobson_01.shtml
  /// http://code.google.com/p/verlet/
  /// http://www.gamedev.net/reference/articles/article2200.asp
  /// </remarks>
  public class VerletIntegrator
  {
    /// <summary>
    /// The dtprev.
    /// </summary>
    private double dtprev;

    /// <summary>
    /// Initializes a new instance of the <see cref = "VerletIntegrator" /> class.
    /// </summary>
    public VerletIntegrator()
    {
      Damping = 0.995f;
      Iterations = 4;

      Constraints = new List<Constraint>();
    }

    /// <summary>
    /// Gets or sets the accelerations.
    /// </summary>
    /// <value>The accelerations.</value>
    public Vector3D[] Accelerations { get; private set; }

    /// <summary>
    /// Gets or sets the constraints.
    /// </summary>
    /// <value>The constraints.</value>
    public List<Constraint> Constraints { get; }

    /// <summary>
    /// Gets or sets the damping.
    /// </summary>
    /// <value>The damping.</value>
    public double Damping { get; set; }

    /// <summary>
    /// Gets or sets the inverse mass.
    /// </summary>
    /// <value>The inverse mass.</value>
    public double[] InverseMass { get; private set; }

    /// <summary>
    /// Gets or sets the iterations.
    /// </summary>
    /// <value>The iterations.</value>
    public int Iterations { get; set; }

    /// <summary>
    /// Gets or sets the positions.
    /// </summary>
    /// <value>The positions.</value>
    public Point3D[] Positions { get; private set; }

    /// <summary>
    /// Gets or sets the positions0.
    /// </summary>
    /// <value>The positions0.</value>
    public Point3D[] Positions0 { get; private set; }

    /// <summary>
    /// Adds the constraint.
    /// </summary>
    /// <param name="A">
    /// The A.
    /// </param>
    /// <param name="B">
    /// The B.
    /// </param>
    /// <param name="relax">
    /// The relax.
    /// </param>
    public void AddConstraint(int A, int B, double relax)
    {
      var c = new DistanceConstraint(A, B)
      {
        Restlength = (Positions[A] - Positions[B]).Length,
        RelaxationFactor = relax,
        Iterations = Iterations
      };
      Constraints.Add(c);
    }

    /// <summary>
    /// Adds the floor.
    /// </summary>
    /// <param name="friction">
    /// The friction.
    /// </param>
    public void AddFloor(double friction)
    {
      for(var i = 0; i < Positions.Length; i++)
      {
        var c = new FloorConstraint(i, friction);
        Constraints.Add(c);
      }
    }

    /// <summary>
    /// Adds the sphere.
    /// </summary>
    /// <param name="center">
    /// The center.
    /// </param>
    /// <param name="radius">
    /// The radius.
    /// </param>
    public void AddSphere(Point3D center, double radius)
    {
      for(var i = 0; i < Positions.Length; i++)
      {
        var c = new SphereConstraint(i, center, radius);
        Constraints.Add(c);
      }
    }

    /// <summary>
    /// Applies the gravity.
    /// </summary>
    /// <param name="gravity">
    /// The gravity.
    /// </param>
    public void ApplyGravity(Vector3D gravity)
    {
      for(var i = 0; i < Positions.Length; i++)
      {
        Accelerations[i] = gravity * InverseMass[i];
      }
    }

    /// <summary>
    /// Creates the constraints by mesh.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    /// <param name="relax">
    /// The relax.
    /// </param>
    public void CreateConstraintsByMesh(MeshGeometry3D mesh, double relax)
    {
      for(var i = 0; i < mesh.TriangleIndices.Count; i += 3)
      {
        var i0 = mesh.TriangleIndices[i];
        var i1 = mesh.TriangleIndices[i + 1];
        var i2 = mesh.TriangleIndices[i + 2];
        AddConstraint(i0, i1, relax);
        AddConstraint(i1, i2, relax);
        AddConstraint(i2, i0, relax);
      }
    }

    /// <summary>
    /// Fixes the specified position.
    /// </summary>
    /// <param name="i">
    /// The i.
    /// </param>
    public void FixPosition(int i)
    {
      InverseMass[i] = 0;
    }

    /// <summary>
    /// Inits the specified mesh.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public void Init(MeshGeometry3D mesh)
    {
      Resize(mesh.Positions.Count);
      for(var i = 0; i < mesh.Positions.Count; i++)
      {
        Positions[i] = mesh.Positions[i];
        Positions0[i] = Positions[i];
        Accelerations[i] = new Vector3D();
      }
    }

    /// <summary>
    /// Resizes the arrays.
    /// </summary>
    /// <param name="n">
    /// The n.
    /// </param>
    public void Resize(int n)
    {
      Positions = new Point3D[n];
      Positions0 = new Point3D[n];
      Accelerations = new Vector3D[n];
      InverseMass = new double[n];
    }

    /// <summary>
    /// Sets the force.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="force">
    /// The force.
    /// </param>
    public void SetForce(int index, Vector3D force)
    {
      Accelerations[index] = force * InverseMass[index];
    }

    /*   public void InitPoint(Point3D v)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            Positions[i] = v;
            Positions0[i] = v;
            Accelerations[i] = new Vector3D();
        }
    }*/

    /// <summary>
    /// Sets the inverse mass.
    /// </summary>
    /// <param name="invmass">
    /// The invmass.
    /// </param>
    public void SetInverseMass(double invmass)
    {
      for(var i = 0; i < Positions.Length; i++)
      {
        InverseMass[i] = invmass;
      }
    }

    /// <summary>
    /// Times the step.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    public void TimeStep(double dt)
    {
      Integrate(dt);

      for(var j = 0; j < Iterations; j++)
      {
        SatisfyConstraints(j);
      }
    }

    /// <summary>
    /// Transfers the positions.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public void TransferPositions(MeshGeometry3D mesh)
    {
      lock(Positions)
      {
        var pc = new Point3DCollection(Positions.Length);
        for(var i = 0; i < Positions.Length; i++)
        {
          pc.Add(Positions[i]);
        }

        mesh.Positions = pc;
      }
    }

    // Time corrected verlet integration
    // http://www.gamedev.net/reference/articles/article2200.asp
    /// <summary>
    /// The integrate.
    /// </summary>
    /// <param name="dt">
    /// The dt.
    /// </param>
    private void Integrate(double dt)
    {
      if(dtprev == 0)
      {
        dtprev = dt;
      }

      lock(Positions)
      {
        for(var i = 0; i < Positions.Length; i++)
        {
          if(InverseMass[i] == 0)
          {
            continue;
          }

          var temp = Positions[i];
          Positions[i] += (Positions[i] - Positions0[i]) * dt / dtprev * Damping
                               + Accelerations[i] * dt * dt;
          Positions0[i] = temp;
        }
      }

      dtprev = dt;
    }

    /// <summary>
    /// The satisfy constraints.
    /// </summary>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    private void SatisfyConstraints(int iteration)
    {
      foreach(var c in Constraints)
      {
        c.Satisfy(this, iteration);
      }
    }

  }

  /// <summary>
  /// Constraint base class.
  /// </summary>
  public abstract class Constraint
  {
    /// <summary>
    /// Satisfies the constraint.
    /// </summary>
    /// <param name="vs">
    /// The verlet system.
    /// </param>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    public abstract void Satisfy(VerletIntegrator vs, int iteration);

  }

  /// <summary>
  /// A sphere constraint.
  /// </summary>
  public class SphereConstraint : Constraint
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SphereConstraint"/> class.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="center">
    /// The center.
    /// </param>
    /// <param name="radius">
    /// The radius.
    /// </param>
    public SphereConstraint(int index, Point3D center, double radius)
    {
      Index = index;
      Center = center;
      Radius = radius;
      RadiusSquared = radius * radius;
    }

    /// <summary>
    /// Gets or sets the center.
    /// </summary>
    /// <value>The center.</value>
    public Point3D Center { get; set; }

    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    /// <value>The index.</value>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius.</value>
    public double Radius { get; set; }

    /// <summary>
    /// Gets or sets the radius squared.
    /// </summary>
    /// <value>The radius squared.</value>
    public double RadiusSquared { get; set; }

    /// <summary>
    /// Satisfies the constraint.
    /// </summary>
    /// <param name="vs">
    /// The verlet system.
    /// </param>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    public override void Satisfy(VerletIntegrator vs, int iteration)
    {
      var vec = Point3D.Subtract(vs.Positions[Index], Center);
      if(vec.LengthSquared < RadiusSquared)
      {
        vec.Normalize();
        vs.Positions[Index] = Center + vec * Radius * 1.1;
        vs.Positions0[Index] = vs.Positions[Index];
      }
    }

  }

  /// <summary>
  /// A floor constraint.
  /// </summary>
  public class FloorConstraint : Constraint
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FloorConstraint"/> class.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="friction">
    /// The friction.
    /// </param>
    public FloorConstraint(int index, double friction = 1.0)
    {
      Index = index;
      Friction = friction;
    }

    /// <summary>
    /// Gets or sets the friction.
    /// </summary>
    /// <value>The friction.</value>
    public double Friction { get; set; }

    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    /// <value>The index.</value>
    public int Index { get; set; }

    /// <summary>
    /// Satisfies the constraint.
    /// </summary>
    /// <param name="vs">
    /// The verlet system.
    /// </param>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    public override void Satisfy(VerletIntegrator vs, int iteration)
    {
      var i = Index;
      var x = vs.Positions[i];
      if(x.Z <= 0)
      {
        if(Friction != 0)
        {
          var f = -x.Z * Friction;
          var v = vs.Positions[i] - vs.Positions0[i];
          v.Z = 0;

          if(v.X > 0)
          {
            v.X -= f * v.X;
            if(v.X < 0)
            {
              v.X = 0;
            }
          }
          else
          {
            v.X += f;
            if(v.X > 0)
            {
              v.X = 0;
            }
          }

          if(v.Y > 0)
          {
            v.Y -= f * v.Y;
            if(v.Y < 0)
            {
              v.Y = 0;
            }
          }
          else
          {
            v.Y += f;
            if(v.Y > 0)
            {
              v.Y = 0;
            }
          }

          vs.Positions0[i] = vs.Positions[i] - v;
        }

        vs.Positions[i].Z = 0;
      }
    }

  }

  /// <summary>
  /// A distance constraint.
  /// </summary>
  public class DistanceConstraint : Constraint
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceConstraint"/> class.
    /// </summary>
    /// <param name="A">
    /// The A.
    /// </param>
    /// <param name="B">
    /// The B.
    /// </param>
    public DistanceConstraint(int A, int B)
    {
      Index1 = A;
      Index2 = B;
    }

    /// <summary>
    /// Gets or sets the index1.
    /// </summary>
    /// <value>The index1.</value>
    public int Index1 { get; set; }

    /// <summary>
    /// Gets or sets the index2.
    /// </summary>
    /// <value>The index2.</value>
    public int Index2 { get; set; }

    /// <summary>
    /// Gets or sets the iterations.
    /// </summary>
    /// <value>The iterations.</value>
    public int Iterations { get; set; }

    /// <summary>
    /// Gets or sets the relaxation factor.
    /// </summary>
    /// <value>The relaxation factor.</value>
    public double RelaxationFactor { get; set; }

    /// <summary>
    /// Gets or sets the restlength.
    /// </summary>
    /// <value>The restlength.</value>
    public double Restlength { get; set; }

    /// <summary>
    /// Satisfies the constraint.
    /// </summary>
    /// <param name="vs">
    /// The verlet system.
    /// </param>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    public override void Satisfy(VerletIntegrator vs, int iteration)
    {
      if(Iterations > iteration)
      {
        var x1 = vs.Positions[Index1];
        var x2 = vs.Positions[Index2];
        var delta = x2 - x1;

        var deltalength = delta.Length;
        var diff = deltalength - Restlength;

        var div = deltalength * (vs.InverseMass[Index1] + vs.InverseMass[Index2]);

        if(Math.Abs(div) > 1e-8)
        {
          diff /= div;
          if(vs.InverseMass[Index1] != 0)
          {
            vs.Positions[Index1] += delta * diff * vs.InverseMass[Index1] * RelaxationFactor;
          }

          if(vs.InverseMass[Index2] != 0)
          {
            vs.Positions[Index2] -= delta * diff * vs.InverseMass[Index2] * RelaxationFactor;
          }
        }
      }
    }

  }
}