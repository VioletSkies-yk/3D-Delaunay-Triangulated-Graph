using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GameEditor.Scene.Component.Delaunay_Triangulated_Graph;
//using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Complex;
using UnityEngine;

namespace Assets.GameEditor.Scene.Component
{
    public class Tetrahedron
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 P4;
        public Vector3 Center;
        public double R;
        public Surface E1;
        public Surface E2;
        public Surface E3;
        public Surface E4;
        public bool isBad;
        public Tetrahedron(Vector3 V, Surface P)
        {
            P1 = V;
            P2 = P.P1;
            P3 = P.P2;
            P4 = P.P3;
            GetTetrahedronExcenterRadius();
            SurfaceValue();
        }
        /// <summary>
        /// 计算四面体的外接球
        /// </summary>
        public void GetTetrahedronExcenterRadius()
        {
            float x1 = P1.x; float x2 = P2.x; float x3 = P3.x; float x4 = P4.x;
            float y1 = P1.y; float y2 = P2.y; float y3 = P3.y; float y4 = P4.y;
            float z1 = P1.z; float z2 = P2.z; float z3 = P3.z; float z4 = P4.z;

            //Matrix4x4 a=new Matrix4x4();

            //a.SetRow(0, new Vector3(2 * x1 - 2 * x2, 2 * y1 - 2 * y2, 2 * z1 - 2 * z2));
            //a.SetRow(1, new Vector3(2 * x1 - 2 * x3, 2 * y1 - 2 * y3, 2 * z1 - 2 * z3));
            //a.SetRow(2, new Vector3(2 * x1 - 2 * x4, 2 * y1 - 2 * y4, 2 * z1 - 2 * z4));


            //Matrix4x4 b = new Matrix4x4();

            //b.SetRow(0, new Vector3((float)Math.Pow(x1, 2) - (float)Math.Pow(x2, 2) + (float)Math.Pow(y1, 2) - (float)Math.Pow(y2, 2) + (float)Math.Pow(z1, 2) - (float)Math.Pow(z2, 2),
            //                        (float)Math.Pow(x1, 2) - (float)Math.Pow(x3, 2) + (float)Math.Pow(y1, 2) - (float)Math.Pow(y3, 2) + (float)Math.Pow(z1, 2) - (float)Math.Pow(z3, 2),
            //                        (float)Math.Pow(x1, 2) - (float)Math.Pow(x4, 2) + (float)Math.Pow(y1, 2) - (float)Math.Pow(y4, 2) + (float)Math.Pow(z1, 2) - (float)Math.Pow(z4, 2)));
            ////float[,]a = {
            ////{2 * x1 - 2 * x2   ,   2 * y1 - 2 * y2    ,    2 * z1 - 2 * z2 },
            ////{2 * x1 - 2 * x3   ,   2 * y1 - 2 * y3    ,    2 * z1 - 2 * z3 },
            ////{2 * x1 - 2 * x4   ,   2 * y1 - 2 * y4    ,    2 * z1 - 2 * z4 }   };

            ////float[,] b = {
            ////{ (float)Math.Pow(x1,2) - (float)Math.Pow(x2,2) + (float)Math.Pow(y1,2) - (float)Math.Pow(y2,2) + (float)Math.Pow(z1,2) - (float)Math.Pow(z2,2) },
            ////{ (float)Math.Pow(x1,2) - (float)Math.Pow(x3,2) + (float)Math.Pow(y1,2) - (float)Math.Pow(y3,2) + (float)Math.Pow(z1,2) - (float)Math.Pow(z3,2) },
            ////{ (float)Math.Pow(x1,2) - (float)Math.Pow(x4,2) + (float)Math.Pow(y1,2) - (float)Math.Pow(y4,2) + (float)Math.Pow(z1,2) - (float)Math.Pow(z4,2) }   };

            ////var m1 = DenseMatrix.OfArray(a);
            ////var m2 = DenseMatrix.OfArray(b);
            ////var m1 = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseOfArray(a);
            ////var m2 = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseOfArray(b);

            //var result = HelperSolve(a * b);
            //Matrix4x4.
            //Debug.LogError(result);
            //Center = new Vector3(result.GetRow(0).x, result.GetRow(0).y, result.GetRow(0).z);

            float a11 = x2 - x1;
            float a12 = y2 - y1;
            float a13 = z2 - z1;
            float b1 = (float)0.5 * ((x2 - x1) * (x2 + x1) + (y2 - y1) * (y2 + y1) + (z2 - z1) * (z2 + z1));

            float a21 = x3 - x1;
            float a22 = y3 - y1;
            float a23 = z3 - z1;
            float b2 = (float)0.5 * ((x3 - x1) * (x3 + x1) + (y3 - y1) * (y3 + y1) + (z3 - z1) * (z3 + z1));
            
            float a31 = x4 - x1;
            float a32 = y4 - y1;
            float a33 = z4 - z1;
            float b3 = (float)0.5 * ((x4 - x1) * (x4 + x1) + (y4 - y1) * (y4 + y1) + (z4 - z1) * (z4 + z1));

            float temp = a11 * (a22 * a33 - a23 * a32) + a12 * (a23 * a31 - a21 * a33) + a13 * (a21 * a32 - a22 * a31);
            float x0 = ((a12 * a23 - a13 * a22) * b3 + (a13 * a32 - a12 * a33) * b2 + (a22 * a33 - a23 * a32) * b1) / temp;
            float y0 = -((a11 * a23 - a13 * a21) * b3 + (a13 * a31 - a11 * a33) * b2 + (a21 * a33 - a23 * a31) * b1) / temp;
            float z0 = ((a11 * a22 - a12 * a21) * b3 + (a12 * a31 - a11 * a32) * b2 + (a21 * a32 - a22 * a31) * b1) / temp;
            float radius = (float)Math.Sqrt((x0 - x1) *2 + (y0 - y1) * 2 + (z0 - z1) * 2);
            Center = new Vector3(x0,y0,z0);
            R = GetDistance(P1, Center);

        }
        private double GetDistance(Vector3 A, Vector3 B)
        {
            return Math.Sqrt(Math.Pow((A.x - B.x), 2) + Math.Pow((A.y - B.y), 2) + Math.Pow((A.z - B.z), 2));
        }
        public bool isComtain(Vector3 node)
        {
            GetTetrahedronExcenterRadius();
            if ((node - Center).sqrMagnitude <= R * R)
                return true;
            else
                return false;
            //if ((CalculateVolume(new Tetrahedron(node,new Surface(P1,P2,P3)))
            //    + CalculateVolume(new Tetrahedron(node, new Surface(P1, P2, P4)))
            //    + CalculateVolume(new Tetrahedron(node, new Surface(P4, P2, P3))))
            //    <= CalculateVolume(this))
            //    return true;
            //else
            //    return false;


        }
        public void SurfaceValue()
        {
            E1 = new Surface(P1, P2,P3);
            E2 = new Surface(P1,P2, P4);
            E3 = new Surface(P1, P3,P4);
            E4 = new Surface(P2, P3,P4);
        }
        public void Bad(Tetrahedron s)
        {
            Vector3[] su = new Vector3[4];
            su[0] = s.P1;
            su[1] = s.P2;
            su[2] = s.P3;
            su[3] = s.P4;
            if (su.Contains(P1) || su.Contains(P2) || su.Contains(P3)|| su.Contains(P4))
                isBad = true;

            int i;
            float ans;
            Vector3 s1, s2, s3;
            for (i = 0; i < 4; i++)
            {
                s1.x = su[1].x - su[0].x; s1.y = su[1].y - su[0].y; s1.z = su[1].z - su[0].z;
                s2.x = su[2].x - su[0].x; s2.y = su[2].y - su[0].y; s2.z = su[2].z - su[0].z;
                s3.x = su[3].x - su[0].x; s3.y = su[3].y - su[0].y; s3.z = su[3].z - su[0].z;
                ans = s1.x * s2.y * s3.z + s1.y * s2.z * s3.x + s1.z * s2.x * s3.y - s1.z * s2.y * s3.x - s1.x * s2.z * s3.y - s1.y * s2.x * s3.z;
                if (ans == 0)
                    isBad = true;
            }

        }
        public float CalculateVolume(Tetrahedron t)
        {
            float x1 = t.P1.x; float x2 = t.P2.x; float x3 = t.P3.x; float x4 = t.P4.x;
            float y1 = t.P1.y; float y2 = t.P2.y; float y3 = t.P3.y; float y4 = t.P4.y;
            float z1 = t.P1.z; float z2 = t.P2.z; float z3 = t.P3.z; float z4 = t.P4.z;
            Vector3 a = new Vector3(x2 - x1, y2 - y1, z2 - z1);

            Vector3 b = new Vector3(x3 - x1, y3 - y1, z3 - z1);

            Vector3 c = new Vector3(x4 - x1, y4 - y1, z4 - z1);

            float V = Math.Abs(Math.Abs(Vector3.Dot(Vector3.Cross(a, b), c)) / 6);

            return V;
        }

    }
}
