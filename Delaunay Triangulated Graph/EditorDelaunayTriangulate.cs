#if UNITY_EDITOR
using Assets.GameEditor.Scene.Component;
using Assets.GameEditor.Scene.Component.Delaunay_Triangulated_Graph;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("游戏元素/三角剖分")]
public class EditorDelaunayTriangulate : EditorBaseComponent
{
    #region 创建菜单

    [MenuItem("GameObject/游戏元素/三角剖分")]
    private static void Create() { CreateGameObject<EditorDelaunayTriangulate>(); }

    #endregion

    public int length;
    List<Vector3> _worldbox = new List<Vector3>();
    /// <summary>
    /// 点集
    /// </summary>
    public List<Vector3> _vertices = new List<Vector3>();
    //public List<List<Vector3>> AreaList = new List<List<Vector3>>();
    public List<EditorSceneObstacle> _obstacles = new List<EditorSceneObstacle>();

    #region 2D三角剖分参数

    /// <summary>
    /// 超级三角形
    /// </summary>
    Triangle SuperTriangle;

    /// <summary>
    /// 边链表
    /// </summary>
    List<Edge> _edges = new List<Edge>();

    /// <summary>
    /// 三角形链表
    /// </summary>
    List<Triangle> _triangle = new List<Triangle>();
    #endregion


    #region 3D三角剖分参数

    /// <summary>
    /// 超级三角形
    /// </summary>
    Tetrahedron SuperTetrahedron;

    /// <summary>
    /// 面链表
    /// </summary>
    List<Surface> _surface = new List<Surface>();

    /// <summary>
    /// 三角形链表
    /// </summary>
    List<Tetrahedron> _tetrahedron = new List<Tetrahedron>();
    List<Tetrahedron> _tetrahedronObstacle = new List<Tetrahedron>();


    #endregion




    [Button("重置所有参数")]
    private void ResetNum()
    {
        isDrawSuperTriangle = false;
        isTwoSubdivision = false;
        _vertices.Clear();
        //_tetrahedronAmount.Clear();
        isDrawSuperTetrahedron = false;
        isThreeSubdivision = false;
        _worldbox.Clear();
        _tetrahedronObstacle.Clear();
    }

    [Button("构造世界盒子")]
    public void WorldBox()
    {
        Vector3 Pos = transform.position;
        _worldbox.Add(Pos + new Vector3(-length, -length, -length));
        _worldbox.Add((Pos + new Vector3(-length, -length, length)));
        _worldbox.Add((Pos + new Vector3(-length, length, -length)));
        _worldbox.Add((Pos + new Vector3(-length, length, length)));
        _worldbox.Add((Pos + new Vector3(length, -length, -length)));
        _worldbox.Add((Pos + new Vector3(length, length, -length)));
        _worldbox.Add((Pos + new Vector3(length, -length, length)));
        _worldbox.Add((Pos + new Vector3(length, length, length)));
    }

    [Button("构造点集并进行排序")]
    public void SortList()
    {
        if(_vertices!=null)
        {
            _vertices.Clear();
        }
        //var points = GetComponentsInChildren<EditorDelaunayTriangulatePoint>().Select(r => r.transform.position).ToArray();
        foreach (var point in _worldbox)
        {
            _vertices.Add(point);
        }
        foreach (var _obstacle in _obstacles)
        {
            var obstacle = _obstacle._nodelist;
            foreach (var obstacleDot in obstacle)
            {
                _vertices.Add(obstacleDot);
            }
        }


        _vertices = _vertices.OrderBy(o => o.x).ThenBy(o => o.y).ThenBy(o => o.z).ToList();
    }










    #region 2DButton
    [HideInInspector]
    [Button("构造超级三角形")]
    public void DrawSuperTriangle()
    {
        SuperTriangle = FindSuper_Triangle(_vertices);
        isDrawSuperTriangle = true;
        SuperTriangle.GetTriangleExcenterRadius();
        _vertices.Add(SuperTriangle.P1);
        _vertices.Add(SuperTriangle.P2);
        _vertices.Add(SuperTriangle.P3);
    }
    bool isDrawSuperTriangle;
    [HideInInspector]
    [Button("三角剖分")]
    public void Draw_TwoD_Subdivision()
    {
        isTwoSubdivision = true;
        try
        {
            TwoD_Subdivision();
        }
        catch (Exception i)
        {
            throw;
        }
        //Debug.LogError(_triangle.Count);
    }
    bool isTwoSubdivision;

    #endregion





    #region 3DButton

    //[Button("构造超级四面体")]
    //public void DrawSuperTetrahedron()
    //{
    //    SuperTetrahedron = FindSuper_Tetrahedron(_vertices);
    //    //Debug.LogError(SuperTetrahedron.CalculateVolume(new Tetrahedron(new Vector3(0, 0, 0), new Surface(new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)))));
    //    isDrawSuperTetrahedron = true;
    //    //SuperTriangle.GetTriangleExcenterRadius();
    //    _vertices.Add(SuperTetrahedron.P1);
    //    _vertices.Add(SuperTetrahedron.P2);
    //    _vertices.Add(SuperTetrahedron.P3);
    //}
    bool isDrawSuperTetrahedron;

    [Button("3D三角剖分")]
    public void Draw_ThreeD_Subdivision()
    {
        SuperTetrahedron = FindSuper_Tetrahedron(_vertices);
        //Debug.LogError(SuperTetrahedron.CalculateVolume(new Tetrahedron(new Vector3(0, 0, 0), new Surface(new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)))));
        isDrawSuperTetrahedron = true;
        //SuperTriangle.GetTriangleExcenterRadius();
        _vertices.Add(SuperTetrahedron.P1);
        _vertices.Add(SuperTetrahedron.P2);
        _vertices.Add(SuperTetrahedron.P3);
        _vertices.Add(SuperTetrahedron.P4);
        try
        {
            ThreeD_Subdivision();
        }
        catch (Exception i)
        {
            throw;
        }

        Debug.LogError(_tetrahedron.Count);
        Debug.LogError(_tetrahedronObstacle.Count);
        isDrawSuperTetrahedron = false;
        isThreeSubdivision = true;
    }
    //[Button("分区域3D三角剖分")]
    //public void Draw_NewThreeD_Subdivision()
    //{
    //    isThreeSubdivision = true;
    //    Space_Subdivision();
    //    Debug.LogError(AreaList.Count);
    //    foreach (var nodelist in AreaList)
    //    {
    //        if (nodelist.Count >= 4)
    //        {
    //            SuperTetrahedron = FindSuper_Tetrahedron(nodelist);
    //            //Debug.LogError(SuperTetrahedron.CalculateVolume(new Tetrahedron(new Vector3(0, 0, 0), new Surface(new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)))));
    //            //SuperTriangle.GetTriangleExcenterRadius();
    //            nodelist.Add(SuperTetrahedron.P1);
    //            nodelist.Add(SuperTetrahedron.P2);
    //            nodelist.Add(SuperTetrahedron.P3);
    //            try
    //            {
    //                NewThreeD_Subdivision(nodelist);
    //            }
    //            catch (Exception i)
    //            {
    //                throw;
    //            }
    //        }

    //    }

    //    Debug.LogError(_tetrahedronAmount.Count);
    //    isDrawSuperTetrahedron = false;
    //}

    bool isThreeSubdivision;

    #endregion








    private void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }


    private void OnDrawGizmos()
    {
        foreach (var node in _worldbox)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(node, 1f);
        }

        if (isDrawSuperTriangle)
        {
            Gizmos.DrawLine(SuperTriangle.P2, SuperTriangle.P1);
            Gizmos.DrawLine(SuperTriangle.P3, SuperTriangle.P1);
            Gizmos.DrawLine(SuperTriangle.P3, SuperTriangle.P2);
        }
        if(isTwoSubdivision)
        {
            foreach (var t in _triangle)
            {
                Gizmos.DrawLine(t.P1, t.P2);
                Gizmos.DrawLine(t.P1, t.P3);
                Gizmos.DrawLine(t.P2, t.P3);
            }

        }


        if (isDrawSuperTetrahedron)
        {
            Gizmos.DrawLine(SuperTetrahedron.P2, SuperTetrahedron.P1);
            Gizmos.DrawLine(SuperTetrahedron.P3, SuperTetrahedron.P1);
            Gizmos.DrawLine(SuperTetrahedron.P4, SuperTetrahedron.P1);
            Gizmos.DrawLine(SuperTetrahedron.P4, SuperTetrahedron.P2);
            Gizmos.DrawLine(SuperTetrahedron.P4, SuperTetrahedron.P3);
            Gizmos.DrawLine(SuperTetrahedron.P3, SuperTetrahedron.P2);
            Gizmos.DrawSphere(SuperTetrahedron.Center, 1f);
            //isdrawsupertriangle = false;
        }



        if (isThreeSubdivision)
        {
            foreach (var t in _tetrahedron)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(t.P1, t.P2);
                Gizmos.DrawLine(t.P1, t.P3);
                Gizmos.DrawLine(t.P1, t.P4);
                Gizmos.DrawLine(t.P2, t.P3);
                Gizmos.DrawLine(t.P2, t.P4);
                Gizmos.DrawLine(t.P3, t.P4);

               
            }
            foreach (var t in _tetrahedronObstacle)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(t.P1, t.P2);
                Gizmos.DrawLine(t.P1, t.P3);
                Gizmos.DrawLine(t.P1, t.P4);
                Gizmos.DrawLine(t.P2, t.P3);
                Gizmos.DrawLine(t.P2, t.P4);
                Gizmos.DrawLine(t.P3, t.P4);


            }

        }                      

    }

    #region 2D三角剖分

    /// <summary>
    /// 找到超级三角形
    /// </summary>
    /// <returns></returns>
    public Triangle FindSuper_Triangle(List<Vector3> _vertices)
    {
        Triangle super_Triangle=new Triangle(new Vector3(0,0,0),new Edge(new Vector3(1, 0, 1), new Vector3(-1, 0, 1)));
        Vector3 Circle;
        float Radius;
        float xmin = _vertices[0].x;
        float xmax = _vertices[_vertices.Count - 1].x;
        float zmin = _vertices[0].z;
        float zmax = _vertices[_vertices.Count - 1].z;
        foreach (var dot in _vertices)
        {
            if (zmin > dot.z)
                zmin = dot.z;
            if (zmax <= dot.z)
                zmax = dot.z;
            //zmin = Math.Min(zmin, dot.z);
            //zmax = Math.Max(zmax, dot.z);
        }
        Vector3 P1 = new Vector3(xmin, 0, zmin);
        Vector3 P2 = new Vector3(xmin, 0, zmax);
        Vector3 P3 = new Vector3(xmax, 0, zmax);
        Vector3 P4 = new Vector3(xmax, 0, zmin);

        Circle = (P1 + P3) / 2;
        Radius = Mathf.Sqrt((xmax - xmin) * (xmax - xmin) + (zmax - zmin) * (zmax - zmin));

        Vector3 sP1 = new Vector3(Circle.x, 0, Circle.y + Radius * 2);
        Vector3 sP2 = new Vector3(Circle.x - Radius * 2, 0, Circle.y - Radius);
        Vector3 sP3 = new Vector3(Circle.x + Radius * 2, 0, Circle.y - Radius);

        super_Triangle = new Triangle(sP1, new Edge(sP2, sP3));


        return super_Triangle;
    }


    /// <summary>
    /// 2D三角剖分
    /// </summary>
    public void TwoD_Subdivision()
    {
        _triangle.Clear();
        _edges.Clear();
        _triangle.Add(SuperTriangle);

        for (int i = 0; i < _vertices.Count; i++)
        {
            _edges.Clear();
            int index = 0;
            while (index<_triangle.Count)
            {
                if (_triangle[index].isComtain(_vertices[i]))
                {
                    AddEdge(_edges, _triangle[index].E1);
                    AddEdge(_edges, _triangle[index].E2);
                    AddEdge(_edges, _triangle[index].E3);
                    _triangle.Remove(_triangle[index]);
                }
                else
                {
                    index++;
                }
            }
            //foreach (var t in _triangle)
            //{
            //    if (t.isComtain(_vertices[i]))
            //    {
            //        DeleteEdge(_edges, t.E1);
            //        DeleteEdge(_edges, t.E2);
            //        DeleteEdge(_edges, t.E3);
            //        _triangle.Remove(t);
            //    }
            //}
            foreach (var e in _edges)
            {
                Triangle Ttemp = new Triangle(_vertices[i], e);
                _triangle.Add(Ttemp);
            }
        }
        int Tindex = 0;
        while (Tindex < _triangle.Count)
        {
            _triangle[Tindex].Bad(SuperTriangle);
            if (_triangle[Tindex].isBad)
                _triangle.Remove(_triangle[Tindex]);
            else
                Tindex++;
        }
        _triangle.Remove(SuperTriangle);
        _vertices.Remove(SuperTriangle.P1);
        _vertices.Remove(SuperTriangle.P2);
        _vertices.Remove(SuperTriangle.P3);
    }


    /// <summary>
    /// 添加边
    /// </summary>
    /// <param name="_edges"></param>
    /// <param name="E"></param>
    public void AddEdge(List<Edge> _edges,Edge E)
    {
        bool isAdd = true;
        int index = 0;
        while (index<_edges.Count)
        {
            if (_edges[index].P1 == E.P1 && _edges[index].P2 == E.P2 || _edges[index].P1 == E.P2 && _edges[index].P2 == E.P1)
            {
                _edges.Remove(_edges[index]);
                isAdd = false;
            }
            else
            {
                index++;
            }
        }
        if(isAdd)
        {
            _edges.Add(E);
        }
    }


    #endregion










    /// <summary>
    /// 找到超级四面体
    /// </summary>
    /// <returns></returns>
    public Tetrahedron FindSuper_Tetrahedron(List<Vector3> _vertices)
    {
        Vector3 Circle;
        float Radius;
        float xmin = _vertices[0].x;
        float xmax = _vertices[_vertices.Count - 1].x;
        float ymin = _vertices[0].y;
        float ymax = _vertices[_vertices.Count - 1].y;
        float zmin = _vertices[0].z;
        float zmax = _vertices[_vertices.Count - 1].z;

        foreach (var dot in _vertices)
        {
            if (ymin > dot.y)
                ymin = dot.y;
            if (ymax <= dot.y)
                ymax = dot.y;

            if (zmin > dot.z)
                zmin = dot.z;
            if (zmax <= dot.z)
                zmax = dot.z;

        }
        Vector3 P1 = new Vector3(xmin, ymin, zmin);
        Vector3 P3 = new Vector3(xmax, ymax, zmax);

        Circle = (P1 + P3) / 2;
        Radius = Mathf.Sqrt((P1 - P3).sqrMagnitude);

        Vector3 sP1 = Circle + new Vector3(0, Radius * 3, 0);
        Vector3 sP2 = Circle + new Vector3(0, -Radius, Radius * 2 * Mathf.Sqrt(2));
        Vector3 sP3 = Circle + new Vector3(Radius * Mathf.Sqrt(6), -Radius, -Radius * Mathf.Sqrt(2));
        Vector3 sP4 = Circle + new Vector3(-Radius * Mathf.Sqrt(6), -Radius, -Radius * Mathf.Sqrt(2));

        Tetrahedron super_Tetrahedron = new Tetrahedron(sP1, new Surface(sP2,sP3,sP4));
        //Debug.LogError((sP1+sP2+sP3+sP4)/4);

        return super_Tetrahedron;
    }

    public void ThreeD_Subdivision()
    {
        _tetrahedron.Clear();
        _surface.Clear();
        _tetrahedron.Add(SuperTetrahedron);

        for (int i = 0; i < _vertices.Count; i++)
        {
            _surface.Clear();
            int index = 0;
            while (index < _tetrahedron.Count)
            {
                if (_tetrahedron[index].isComtain(_vertices[i]))
                {
                    AddSurface(_surface, _tetrahedron[index].E1);
                    AddSurface(_surface, _tetrahedron[index].E2);
                    AddSurface(_surface, _tetrahedron[index].E3);
                    AddSurface(_surface, _tetrahedron[index].E4);
                    _tetrahedron.Remove(_tetrahedron[index]);
                }
                else
                {
                    index++;
                }
            }
            foreach (var e in _surface)
            {
                Tetrahedron Ttemp = new Tetrahedron(_vertices[i], e);
                bool isOb = false;
                foreach (var o in _obstacles)
                {
                    if (o.CollisionDetection(Ttemp))
                        isOb = true;
                    else
                        isOb = false;
                }
                if (isOb)
                    _tetrahedronObstacle.Add(Ttemp);
                else
                    _tetrahedron.Add(Ttemp);
            }
        }
        int Tindex = 0;
        while (Tindex < _tetrahedron.Count)
        {
            _tetrahedron[Tindex].Bad(SuperTetrahedron);
            if (_tetrahedron[Tindex].isBad)
                _tetrahedron.Remove(_tetrahedron[Tindex]);
            else
                Tindex++;
        }
        _tetrahedron.Remove(SuperTetrahedron);
        _vertices.Remove(SuperTetrahedron.P1);
        _vertices.Remove(SuperTetrahedron.P2);
        _vertices.Remove(SuperTetrahedron.P3);
        _vertices.Remove(SuperTetrahedron.P4);
    }
    //public void NewThreeD_Subdivision(List<Vector3> verticesList)
    //{
    //    _tetrahedron.Clear();
    //    _surface.Clear();
    //    _tetrahedron.Add(SuperTetrahedron);

    //    for (int i = 0; i < verticesList.Count; i++)
    //    {
    //        _surface.Clear();
    //        int index = 0;
    //        while (index < _tetrahedron.Count)
    //        {
    //            if (_tetrahedron[index].isComtain(verticesList[i]))
    //            {
    //                AddSurface(_surface, _tetrahedron[index].E1);
    //                AddSurface(_surface, _tetrahedron[index].E2);
    //                AddSurface(_surface, _tetrahedron[index].E3);
    //                AddSurface(_surface, _tetrahedron[index].E4);
    //                //_surface.Add(_tetrahedron[index].E1);
    //                //_surface.Add(_tetrahedron[index].E2);
    //                //_surface.Add(_tetrahedron[index].E3);
    //                //_surface.Add(_tetrahedron[index].E4);
    //                _tetrahedron.Remove(_tetrahedron[index]);
    //            }
    //            else
    //            {
    //                index++;
    //            }
    //        }
    //        foreach (var e in _surface)
    //        {
    //            Tetrahedron Ttemp = new Tetrahedron(verticesList[i], e);
    //            _tetrahedron.Add(Ttemp);
    //        }
    //    }
    //    int Tindex = 0;
    //    while (Tindex < _tetrahedron.Count)
    //    {
    //        _tetrahedron[Tindex].Bad(SuperTetrahedron);
    //        if (_tetrahedron[Tindex].isBad)
    //            _tetrahedron.Remove(_tetrahedron[Tindex]);
    //        else
    //            Tindex++;
    //    }
    //    _tetrahedron.Remove(SuperTetrahedron);
    //    verticesList.Remove(SuperTetrahedron.P1);
    //    verticesList.Remove(SuperTetrahedron.P2);
    //    verticesList.Remove(SuperTetrahedron.P3);
    //    verticesList.Remove(SuperTetrahedron.P4);
    //    foreach (var tetrahedron in _tetrahedron)
    //    {
    //        _tetrahedronAmount.Add(tetrahedron);
    //    }
    //}
    public void AddSurface(List<Surface> _surface, Surface E)
    {
        bool isAdd = true;
        int index = 0;
        while (index < _surface.Count)
        {
            if (_surface[index].P1 == E.P1 && _surface[index].P2 == E.P2 && _surface[index].P3 == E.P3
                || _surface[index].P1 == E.P1 && _surface[index].P2 == E.P3 && _surface[index].P3 == E.P2
                || _surface[index].P1 == E.P3 && _surface[index].P2 == E.P2 && _surface[index].P3 == E.P1
                || _surface[index].P1 == E.P2 && _surface[index].P2 == E.P1 && _surface[index].P3 == E.P3
                || _surface[index].P1 == E.P2 && _surface[index].P2 == E.P3 && _surface[index].P3 == E.P1
                || _surface[index].P1 == E.P3 && _surface[index].P2 == E.P1 && _surface[index].P3 == E.P2)
            {
                _surface.Remove(_surface[index]);
                isAdd = false;
            }
            else
            {
                index++;
            }
        }
        if (isAdd)
        {
            _surface.Add(E);
        }
    }
    public bool Col(Vector3 node)
    {
        if (node.x>=10 && node.x<=20 && node.y>=5&&node.y<=15&&node.z>=10&&node.z<=15)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //public void Space_Subdivision()
    //{
    //    List<float> xList = new List<float>();
    //    List<float> yList = new List<float>();
    //    List<float> zList = new List<float>();
    //    xList.Clear();yList.Clear();zList.Clear();
    //    xList.Add(0); yList.Add(0); zList.Add(0);
    //    foreach (var obstacle in _obstacles)
    //    {
    //        xList.Add(obstacle._nodelist[0].x);
    //        yList.Add(obstacle._nodelist[0].y);
    //        zList.Add(obstacle._nodelist[0].z);

    //        xList.Add(obstacle._nodelist[7].x);
    //        yList.Add(obstacle._nodelist[7].y);
    //        zList.Add(obstacle._nodelist[7].z);
    //    }
    //    xList.Add(100); yList.Add(100); zList.Add(100);
    //    Vector3 min = new Vector3(0, 0, 0);
    //    Vector3 max = new Vector3(0, 0, 0);
    //    List<Vector3> temp = new List<Vector3>();
    //    for (int i = 0; i < xList.Count-1; i++)
    //    {
    //        for (int j = 0; j < yList.Count-1; j++)
    //        {
    //            for (int k = 0; k < zList.Count-1; k++)
    //            {
    //                temp.Clear();
    //                min = new Vector3(xList[i], yList[j], zList[k]);
    //                max = new Vector3(xList[i+1], yList[j+1], zList[k+1]);
    //                foreach (var node in _vertices)
    //                {
    //                    if (isInside(node, min, max))
    //                    {
    //                        temp.Add(node);
    //                    }
    //                }
    //                AreaList.Add(temp);
    //            }
    //        }
    //    }
        
    //}
    public bool isInside(Vector3 node,Vector3 minNode,Vector3 maxNode)
    {
        if(node.x>=minNode.x&& node.y >= minNode.y && node.z >= minNode.z
            && node.x <= maxNode.x && node.y <= maxNode.y && node.z <= maxNode.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
#endif
