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
        for (uint z = 0; z < height; ++z)
        {
            for(uint x = 0; x<width; ++x)
            {
                cells[index] = Instantiate<HexCell>(cellPrefab);
                //z为偶数情况： position.x = HexMetrics.innerRadius + x * 2f* HexMetrics.innerRadius= (x + 0.5f)* 2f* HexMetrics.innerRadius;
                //z为奇数情况： position.x = x * 2f* HexMetrics.innerRadius;
                //融合上式： z/2当z为偶数时与z*0.5f相同


                position.x = (z & 0x1ul) * HexMetrics.innerRadius + x * 2f * HexMetrics.innerRadius;
                position.z = z * 1.5f * HexMetrics.outerRadius;
                //如果原本的transform有父类transform，那么第二个param为ture时会保证更换父类后原物体的position在world space下保持不变
                //在此处两个地方都是需要跟随父系一起变换，所以直接设置为false
                cells[index].transform.SetParent(this.transform,false);
                cells[index].transform.localPosition = position;

                Text label = Instantiate<Text>(cellLabelPrefab);
                anchoredPos.x = position.x;
                anchoredPos.y = position.z;
                //不要又变回原位置，跟随父级一起变
                label.rectTransform.SetParent(gridCanvas.transform,false);
                label.rectTransform.anchoredPosition = anchoredPos;
                
                label.text = x.ToString() + "\n" + z.ToString();
                index++;
            }
        }
    }



    // Start is called before the first frame update
    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
