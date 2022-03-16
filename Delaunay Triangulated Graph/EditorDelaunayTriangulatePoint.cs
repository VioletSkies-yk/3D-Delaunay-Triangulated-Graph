using System.Linq;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("游戏元素/三角剖分节点")]
public class EditorDelaunayTriangulatePoint : EditorBaseComponent
{
    //#region 创建菜单

    //[MenuItem("GameObject/游戏元素/三角剖分节点")]
    //private static void Create()
    //{
    //    CreateGameObject<EditorDelaunayTriangulatePoint>((node) =>
    //    {
    //        var parent = node.GetComponentInParent<EditorDelaunayTriangulate>();
    //        if (parent == null)
    //            return;

    //        var last = parent.GetComponentsInChildren<EditorDelaunayTriangulatePoint>().Where(r => r.gameObject != node.gameObject).LastOrDefault();
    //        if (last != null)
    //            node.transform.position = last.transform.position;
    //    });
    //}

    //#endregion


    public EditorDelaunayTriangulatePoint(Vector3 Pos)
    {
        transform.position = Pos;
    }
    private void OnDrawGizmos()
    {
        //GizmosHelper.DrawComponentIcon(this);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
