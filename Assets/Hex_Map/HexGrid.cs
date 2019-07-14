using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    private HexMesh hexMesh;
    private HexCell[] cells;
    private Canvas gridCanvas;
    

    //减少栈GC，描述Cell and lable position
    private Vector3 position;
    private Vector2 anchoredPos;

    private void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();
        gridCanvas = GetComponentInChildren<Canvas>(); 

        //y轴不变
        position.y = 0f;
        cells = new HexCell[height * width];
        int index = 0;
        for (int z = 0; z < height; ++z)
        {
            for(int x = 0; x<width; ++x)
            {
                cells[index] = Instantiate<HexCell>(cellPrefab);
                //z为偶数情况： position.x = HexMetrics.innerRadius + x * 2f* HexMetrics.innerRadius= (x + 0.5f)* 2f* HexMetrics.innerRadius;
                //z为奇数情况： position.x = x * 2f* HexMetrics.innerRadius;
                //融合上式： z/2当z为偶数时与z*0.5f相同

                //当以竖直方向移动时，每移动一次x会偏移0.5 * z，所以在计算x时也可以表示为0.5*z - z/2，代表每次实际偏移值减去取整的值，得到一个内圆的偏移量

                position.x = (z & 0x1) * HexMetrics.innerRadius + x * 2f * HexMetrics.innerRadius;
                position.z = z * 1.5f * HexMetrics.outerRadius;
                //如果原本的transform有父类transform，那么第二个param为ture时会保证更换父类后原物体的position在world space下保持不变
                //在此处两个地方都是需要跟随父系一起变换，所以直接设置为false
                cells[index].transform.SetParent(this.transform,false);
                cells[index].transform.localPosition = position;
                cells[index].coordinates = HexCoordinates.FromOffsetCoordinates(x,z);
                cells[index].color = defaultColor;


                Text label = Instantiate<Text>(cellLabelPrefab);
                anchoredPos.x = position.x;
                anchoredPos.y = position.z;
                //不要又变回原位置，跟随父级一起变
                label.rectTransform.SetParent(gridCanvas.transform,false);
                label.rectTransform.anchoredPosition = anchoredPos;
                
                label.text = cells[index].coordinates.ToStringOnSeparateLines();
                index++;
            }
        }
    }

    //enum Mouse_Button {
        
    //}

    // Start is called before the first frame update
    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        //射线的原点是在近裁剪面上
        //具体算法即在ndc下取z = -1，直接与对应逆矩阵进行运算（PS：UNITY的CAMERA下的PROJECTMATRIX是做了透视除法的。。）
        //代码如下~
        //float NdcX = (2f * Input.mousePosition.x - Camera.main.pixelWidth) / Camera.main.pixelWidth;
        //float NdcY = (2f * Input.mousePosition.y - Camera.main.pixelHeight) / Camera.main.pixelHeight;
        //float NdcZ = -1f;
        //Vector3 ndc_position = new Vector3(NdcX, NdcY, NdcZ);
        //Vector3 world_position = Camera.main.cameraToWorldMatrix.MultiplyPoint(Camera.main.projectionMatrix.inverse.MultiplyPoint(ndc_position));
        //Vector3 world_point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        ////结果相同
        //Debug.Log(world_position.ToString("F4"));
        //Debug.Log(world_point.ToString("F4"));

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //射线与物体相交
        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.Log(hit.point);
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        //X可能是负数,因为做了坐标变换，需要加上coordinates.Z / 2
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        cells[index].color = touchedColor;
        //重绘整个mesh
        hexMesh.Triangulate(cells);
    }
}


////这个方案要分别从六个方向来修正误差，而且轴线无法计算。。

////初始化用于检查冲突的轴线，减去包含多少个六边形后剩下的长度，轴线也要判断是否奇偶加上一个内圆... 这里无法判断。。
//float check = position.x  - ((int)((position.x ) / (2f * HexMetrics.innerRadius))) * 2f * HexMetrics.innerRadius ;
////检查上下冲突部分
////获取x轴和y轴的小数部分
//float dx = x - (int)x;
//float dy = y - (int)y;
//if (dx > dy)
//{
//    //上冲突
//    if (check > HexMetrics.innerRadius)
//    {
//        //在轴的左边。y冲突,y轴都是负数所以+1
//        CoordinateY += 1;
//    }
//    else
//    {
//        //等于时都视为点到下一个
//        CoordinateX += 1;
//    }
//}
//else //即dx = dy时视为下冲突的y冲突
//{
//    //下冲突
//    if (check > HexMetrics.innerRadius)
//    {
//        //在轴的左边。y冲突,y轴都是负数所以+1
//        CoordinateX -= 1;
//    }
//    else
//    {
//        //等于时都视为点到下一个
//        CoordinateY -= 1;
//    }
//}
