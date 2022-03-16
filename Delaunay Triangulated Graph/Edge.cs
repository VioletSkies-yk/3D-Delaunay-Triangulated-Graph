using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GameEditor.Scene.Component.Delaunay_Triangulated_Graph
{
    public class Edge
    {
        public Vector3 P1;
        public Vector3 P2;

        bool isBad;
        public Edge(Vector3 P1, Vector3 P2)
        {
            this.P1 = P1;
            this.P2 = P2;
        }

    }
}
