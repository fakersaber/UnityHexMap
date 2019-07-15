using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    private Mesh hexMesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private MeshCollider meshCollider;
    private MeshRenderer meshRender;


    //扫描线位置，其实位置也要加上渐变距离和不显示距离
    private float LinePosition = -HexMetrics.innerRadius - 5f* HexMetrics.innerRadius;
    List<Color> colors;

    private void Awake()
    {
        //手动创建mesh
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshRender = GetComponent<MeshRenderer>();
        //添加碰撞盒组件
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }


    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        //对于所有着色器用Shader.SetGlobalxxx()


        //设置渐变线位置，考虑平滑每帧移动，跳动的原因是在最右边时没有淡出效果，让线再多移动显示贴图加上渐变贴图的距离
        LinePosition += 3f;

        if (LinePosition > 16 * HexMetrics.innerRadius)
        {
            LinePosition = -HexMetrics.innerRadius - 3f * HexMetrics.innerRadius;
        }

        meshRender.material.SetFloat("_LinePos", LinePosition);

        for (int i = 0; i < cells.Length; ++i)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = this.vertices.ToArray();
        hexMesh.triangles = this.triangles.ToArray();
        //顶点颜色他lei了？
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();
        //对该材质的shader传递数据，不要直接使用全局shader传递数据
        meshCollider.sharedMesh = hexMesh;
    }

    private void Triangulate(HexCell cell)
    {
        //根据cell的中心位置生成六边形

        for(int i = 0; i < 6; ++i)
        {
            //注意可能最后一条边越界，可以除余或者直接再加一条边
            int vertexIndex = vertices.Count;
            vertices.Add(cell.transform.localPosition);
            vertices.Add(cell.transform.localPosition + HexMetrics.Edges[i]);
            vertices.Add(cell.transform.localPosition + HexMetrics.Edges[i+1]);

            //对应顶点索引,这里公用顶点暂未优化
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            colors.Add(cell.color);
            colors.Add(cell.color);
            colors.Add(cell.color);
        }

    }


}
