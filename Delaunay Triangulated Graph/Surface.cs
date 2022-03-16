using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GameEditor.Scene.Component.Delaunay_Triangulated_Graph;
using UnityEngine;

namespace Assets.GameEditor.Scene.Component
{
    public class Surface
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public bool isBad;
        public Surface(Vector3 P1,Vector3 P2,Vector3 P3)
        {
            this.P1 = P1;
            this.P2 = P2;
            this.P3 = P3;
        }
    }
}
