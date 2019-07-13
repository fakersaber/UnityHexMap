using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;



    private void Awake()
    {
        //手动创建mesh
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }


    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        for(int i = 0; i < cells.Length; ++i)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = this.vertices.ToArray();
        hexMesh.triangles = this.triangles.ToArray();
        hexMesh.RecalculateNormals();
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
        }

    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
