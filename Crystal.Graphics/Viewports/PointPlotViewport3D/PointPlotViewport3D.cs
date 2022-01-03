﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.Graphics
{
  public class PointPlotViewport3D : PlotViewport3D
  {
    #region Private fields

    private readonly ModelVisual3D visualChild;

    #endregion Private fields

    #region Constructors

    static PointPlotViewport3D()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PointPlotViewport3D), new FrameworkPropertyMetadata(typeof(PointPlotViewport3D)));
    }

    public PointPlotViewport3D()
    {
      visualChild = new ModelVisual3D();
      Children.Add(visualChild);
    }

    #endregion Constructors

    #region Private Methods

    private static new void ModelChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
    {
      ((PointPlotViewport3D)dpObj).UpdateModel();
    }

    private void UpdateModel()
    {
      visualChild.Content = CreateModel();
    }

    private Model3D CreateModel()
    {
      Model3DGroup? plotModel = new();
      if(Points == null || Values == null) return plotModel;

      double minX = Points.Min(p => p.X);
      double maxX = Points.Max(p => p.X);
      double minY = Points.Min(p => p.Y);
      double maxY = Points.Max(p => p.Y);
      double minZ = Points.Min(p => p.Z);
      double maxZ = Points.Max(p => p.Z);
      double minValue = Values.Min();
      double maxValue = Values.Max();

      var valueRange = maxValue - minValue;
      var scatterMeshBuilder = new MeshBuilder(true, true);
      var oldTCCount = 0;

      for(var i = 0; i < Points.Length; ++i)
      {
        scatterMeshBuilder.AddSphere(Points[i], SphereSize, 16, 16);
        var u = (Values[i] - minValue) / valueRange;
        var newTCCount = scatterMeshBuilder.TextureCoordinates.Count;

        for(var j = oldTCCount; j < newTCCount; ++j)
        {
          scatterMeshBuilder.TextureCoordinates[j] = new Point(u, u);
        }
        oldTCCount = newTCCount;
      }

      MeshGeometry3D? mesh = scatterMeshBuilder.ToMesh(true);
      Material? temp = MaterialHelper.CreateMaterial(SurfaceBrush, null, null, 1, 0);

      var scatterModel = new GeometryModel3D(mesh, temp);

      scatterModel.BackMaterial = scatterModel.Material;

      #region Create bounding box with axes indications

      var axesMeshBuilder = new MeshBuilder();

      for(double x = minX; x <= Math.Ceiling(maxX) + 1; x += IntervalX)
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          $"{x:0}", XAxisLabelBrush, true, XAxisLabelFontSize, new Point3D(x, minY - FontSize * 2.5, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }
      GeometryModel3D xAxisTitle = TextCreator.CreateTextLabelModel3D(
        XAxisTitleContent, XAxisTitleBrush, true, XAxisTitleFontSize, new Point3D((minX + maxX) * 0.5, minY - FontSize * 6, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
      plotModel.Children.Add(xAxisTitle);

      for(double y = minY; y <= Math.Ceiling(maxY) + 1; y += IntervalY)
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(
          $"{y:0}", YAxisLabelBrush, true, YAxisLabelFontSize, new Point3D(minX - FontSize * 3, y, minZ), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
        plotModel.Children.Add(label);
      }
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(YAxisTitleContent, YAxisTitleBrush, true, YAxisTitleFontSize, new Point3D(minX - (FontSize * 10), (minY + maxY) * 0.5, minZ), new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0));
        plotModel.Children.Add(label);
      }

      double z0 = (int)(minZ / IntervalZ) * IntervalZ;
      for(double z = z0; z <= Math.Ceiling(maxZ); z += IntervalZ)
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D($"{z:0}", ZAxisLabelBrush, true, ZAxisLabelFontSize, new Point3D(minX - FontSize * 3, maxY, z), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
        plotModel.Children.Add(label);
      }
      {
        GeometryModel3D label = TextCreator.CreateTextLabelModel3D(ZAxisTitleContent, ZAxisTitleBrush, true, ZAxisTitleFontSize, new Point3D(minX - FontSize * 10, maxY, (minZ + maxZ) * 0.5), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
        plotModel.Children.Add(label);
      }

      var axisBoundingBox = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
      axesMeshBuilder.AddBoundingBox(axisBoundingBox, BoundingBoxThickness);

      var axesModel = new GeometryModel3D(axesMeshBuilder.ToMesh(), BoundingBoxMaterial);

      #endregion Create bounding box with axes indications

      plotModel.Children.Add(scatterModel);
      plotModel.Children.Add(axesModel);

      return plotModel;
    }

    #endregion Private Methods

    #region Data Dependency Properties

    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
      nameof(Points), typeof(Point3D[]), typeof(PointPlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    public Point3D[] Points
    {
      get => (Point3D[])GetValue(PointsProperty);
      set => SetValue(PointsProperty, value);
    }

    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
      nameof(Values), typeof(double[]), typeof(PointPlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    public double[] Values
    {
      get => (double[])GetValue(ValuesProperty);
      set => SetValue(ValuesProperty, value);
    }

    public static readonly DependencyProperty SurfaceBrushProperty = DependencyProperty.Register(
      nameof(SurfaceBrush), typeof(Brush), typeof(PointPlotViewport3D), new UIPropertyMetadata(null, ModelChanged));

    public Brush SurfaceBrush
    {
      get => (Brush)GetValue(SurfaceBrushProperty);
      set => SetValue(SurfaceBrushProperty, value);
    }

    #endregion Data Dependency Properties
  }
}
