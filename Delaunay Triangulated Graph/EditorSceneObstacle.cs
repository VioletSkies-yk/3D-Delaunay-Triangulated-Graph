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

[AddComponentMenu("游戏元素/障碍物盒子")]
public class EditorSceneObstacle : EditorBaseComponent
{
    #region 创建菜单

    [MenuItem("GameObject/游戏元素/障碍物盒子")]
    private static void Create() { CreateGameObject<EditorSceneObstacle>(); }

    #endregion

    public int lenth = 0;
    public Vector3[] _nodelist = new Vector3[8];



    [Button("构造AABB盒子")]
    public void AABB()
    {
        Vector3 Pos = transform.position;
        _nodelist[0] = Pos + new Vector3(-lenth, -lenth, -lenth);
        _nodelist[1] = Pos + new Vector3(-lenth, -lenth, lenth);
        _nodelist[2] = Pos + new Vector3(-lenth, lenth, -lenth);
        _nodelist[3] = Pos + new Vector3(-lenth, lenth, lenth);
        _nodelist[4] = Pos + new Vector3(lenth, -lenth, -lenth);
        _nodelist[5] = Pos + new Vector3(lenth, lenth, -lenth);
        _nodelist[6] = Pos + new Vector3(lenth, -lenth, lenth);
        _nodelist[7] = Pos + new Vector3(lenth, lenth, lenth);
    }
    private void OnDrawGizmos()
    {
        foreach (var node in _nodelist)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node, 0.5f);
        }
    }

    public bool CollisionDetection(Tetrahedron t)
    {
        if (_nodelist[0].x <= t.Center.x && _nodelist[0].y <= t.Center.y && _nodelist[0].z <= t.Center.z && _nodelist[7].x >= t.Center.x && _nodelist[7].y >= t.Center.y && _nodelist[7].z >= t.Center.z &&
           _nodelist[0].x <= t.P2.x && _nodelist[0].y <= t.P2.y && _nodelist[0].z <= t.P2.z && _nodelist[7].x >= t.P2.x && _nodelist[7].y >= t.P2.y && _nodelist[7].z >= t.P2.z &&
           _nodelist[0].x <= t.P3.x && _nodelist[0].y <= t.P3.y && _nodelist[0].z <= t.P3.z && _nodelist[7].x >= t.P3.x && _nodelist[7].y >= t.P3.y && _nodelist[7].z >= t.P3.z &&
           _nodelist[0].x <= t.P4.x && _nodelist[0].y <= t.P4.y && _nodelist[0].z <= t.P4.z && _nodelist[7].x >= t.P4.x && _nodelist[7].y >= t.P4.y && _nodelist[7].z >= t.P4.z)
        //if (_nodelist[0].x <= t.Center.x && _nodelist[0].y <= t.Center.y && _nodelist[0].z <= t.Center.z && _nodelist[7].x >= t.Center.x && _nodelist[7].y >= t.Center.y && _nodelist[7].z >= t.Center.z)
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
