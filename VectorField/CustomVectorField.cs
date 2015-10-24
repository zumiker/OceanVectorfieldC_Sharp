using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slb.Ocean.Petrel.DomainObject.Shapes;
using Slb.Ocean.Petrel.UI;
using System.Drawing;
using Slb.Ocean.Geometry;
using System.Drawing.Drawing2D;

namespace VectorField
{
    public class CustomVectorField
    {
        private RegularHeightFieldSurface heigtfieldic;
        private RegularHeightFieldSurface azimutfieldic;
        private double[, ,] vectorField;
        private double step = 100;
        private double k = 1;
        private double oX = 0;
        private double oY = 0;
        private int sX = 1;
        private int sY = 1;
        private int n = 50;
        private bool grd = true;
        private Color clr = Color.Blue;

        public RegularHeightFieldSurface Heigtfieldic
        {
            get { return this.heigtfieldic; }
            set { this.heigtfieldic = value; }
        }
        public RegularHeightFieldSurface Azimutfieldic
        {
            get { return this.azimutfieldic; }
            set { this.azimutfieldic = value; }
        }
        public double[, ,] VectorField
        {
            get { return this.vectorField; }
            set { this.vectorField = value; }
        }
        public double gridStep
        {
            get { return this.step; }
            set { this.step = value; }
        }
        public double koeff
        {
            get { return this.k; }
            set { this.k = value; }
        }
        public double originX
        {
            get { return this.oX; }
            set { this.oX = value; }
        }
        public double originY
        {
            get { return this.oY; }
            set { this.oY = value; }
        }
        public int sizeX
        {
            get { return this.sX; }
            set { this.sX = value; }
        }
        public int sizeY
        {
            get { return this.sY; }
            set { this.sY = value; }
        }
        public int partition
        {
            get { return this.n; }
            set { this.n = value; }
        }
        public bool haveGrid
        {
            get { return this.grd; }
            set { this.grd = value; }
        }
        public Color arrowColor
        {
            get { return this.clr; }
            set { this.clr = value; }
        }
    }

    public class CustomVectorFieldMapDisplay : IMapRenderer
    {
        public void Initialize(object o, MapRendererContext ctx) 
        { 
            ctx.RenderingLayers = RenderingLayers.Solid;
        }

        public bool CanDraw(object o, MapRendererContext ctx) 
        {
            CustomVectorField cvf = o as CustomVectorField;
            return !(cvf.VectorField == null); 
        }
        public void Draw ( object o, MapRendererContext ctx ) {
            // Get the World coordinates 
            Graphics gworld = ctx.World; 
            using (Pen pn = new Pen(Color.Black))
            {
                CustomVectorField VF = (CustomVectorField)o;
                double[, ,] field = VF.VectorField;
                float originX = (float)VF.originX;
                float originY = (float)VF.originY;
                float increment = (float)VF.gridStep;
                int sizeX = VF.sizeX;
                int sizeY = VF.sizeY; 
                Pen pn1 = new Pen(VF.arrowColor);
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 7);
                Pen pn2 = new Pen(VF.arrowColor);
                pn2.CustomEndCap = bigArrow;
                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(VF.arrowColor);
                for (int i = 0; i < sizeX; i++)
                {
                    
                    for (int j = 0; j < sizeY; j++)
                    {
                        if (field[i, j, 0] > 0.005)
                        {
                            if (VF.haveGrid)
                            {
                                gworld.DrawRectangle(pn, originX + (float)i * increment, originY + (float)j * increment, increment, increment);
                            }
                            gworld.DrawLine(pn1, (float)field[i, j, 2], (float)field[i, j, 3], (float)field[i, j, 4], (float)field[i, j, 5]);
                            PointF point1 = new PointF((float)field[i, j, 4], (float)field[i, j, 5]);
                            PointF point2 = new PointF((float)field[i, j, 6], (float)field[i, j, 7]);
                            PointF point3 = new PointF((float)field[i, j, 12], (float)field[i, j, 13]);                            
                            PointF point4 = new PointF((float)field[i, j, 16], (float)field[i, j, 17]);
                            PointF point5 = new PointF((float)field[i, j, 14], (float)field[i, j, 15]);
                            PointF point6 = new PointF((float)field[i, j, 10], (float)field[i, j, 11]);
                            PointF point7 = new PointF((float)field[i, j, 8], (float)field[i, j, 9]);
                            

                            PointF[] curvePoints =
                             {
                                 point1,
                                 point2,
                                 point3,
                                 point4,
                                 point5,
                                 point6,
                                 point7
                 
                             };
                            gworld.FillPolygon(drawBrush, curvePoints);
                            //gworld.DrawLine(pn1, (float)field[i, j, 4], (float)field[i, j, 5], (float)field[i, j, 6], (float)field[i, j, 7]);
                            //gworld.DrawLine(pn1, (float)field[i, j, 4], (float)field[i, j, 5], (float)field[i, j, 8], (float)field[i, j, 9]);
                            //PointF[] f = new PointF [4];
                            //for (int rrr = 0; rrr < 4 ; i++){
                              /*  f[0].X = (int)field[i, j, 6];
                                f[0].Y = (int)field[i, j, 7];
                                f[1].X = (int)field[i, j, 4];
                                f[1].Y = (int)field[i, j, 5];
                                f[2].X = (int)field[i, j, 8];
                                f[2].Y = (int)field[i, j, 9];
                                f[3].X = (int)field[i, j, 10];
                                f[3].Y = (int)field[i, j, 11];
                                gworld.DrawPolygon(pn1, f);*/
                            //}
                                //gworld.FillPolygon(drawBrush, f);
                        }
                    }
                }
            } 
        }
        public Box2 GetBounds ( object o, MapRendererContext ctx )
        {
            CustomVectorField VF = (CustomVectorField)o; 
            Point2 begin = new Point2(VF.originX, VF.originY);
            Point2 end = new Point2(VF.originX + VF.gridStep * VF.sizeX, VF.originY + VF.gridStep * VF.sizeY);
            // Return a box that describes the area the object occupies
            return new Box2( begin, end );
        }
        public void Dispose(object o, MapRendererContext ctx) 
        {
            return; 
        }
    }
}
