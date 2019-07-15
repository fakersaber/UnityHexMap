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
    public Color defaultColor = Color.black;
    public Color touchedColor = Color.magenta;

    private HexMesh hexMesh;
    private HexCell[] cells;
    private Canvas gridCanvas;

    private float ColorChangeSpeed = 0.5f;
    private const int ScanCellNum = 2;
    private int startColorCell = 0;


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

                position.x = (z & 0x1) * HexMetrics.innerRadius + x * 2f * HexMetrics.innerRadius;
                position.z = z * 1.5f * HexMetrics.outerRadius;

                cells[index].transform.SetParent(this.transform,false);
                cells[index].transform.localPosition = position;
                cells[index].coordinates = HexCoordinates.FromOffsetCoordinates(x,z);
                cells[index].color = defaultColor;


                //Text label = Instantiate<Text>(cellLabelPrefab);
                //anchoredPos.x = position.x;
                //anchoredPos.y = position.z;

                //label.rectTransform.SetParent(gridCanvas.transform,false);
                //label.rectTransform.anchoredPosition = anchoredPos;
                
                //label.text = cells[index].coordinates.ToStringOnSeparateLines();
                index++;
            }
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        hexMesh.Triangulate(cells);
    }


    void Update()
    {
        hexMesh.Triangulate(cells);
        //if (Input.GetMouseButtonDown(0))
        //{
        //    HandleInput();
        //}
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
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

        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

        cells[index].color = touchedColor;

        hexMesh.Triangulate(cells);
    }


    private void ChangeCellColor()
    {
        if (ColorChangeSpeed < 0.5f)
        {
            ColorChangeSpeed += Time.deltaTime;
        }
        else
        {
            ColorChangeSpeed = 0f;
            int CurCellIndex = 0;

            for (int z = 0; z < height; ++z)
            {
                for (int i = 0; i < width; ++i)
                {
                    CurCellIndex = i + z * width;
                    if (i >= startColorCell && i < startColorCell + HexGrid.ScanCellNum)
                    {
                        cells[CurCellIndex].color = Color.black;
                    }
                    else
                    {
                        cells[CurCellIndex].color = defaultColor;
                    }

                }
            }
            hexMesh.Triangulate(cells);

            startColorCell += HexGrid.ScanCellNum;
            if (startColorCell == width)
            {
                startColorCell = 0;
            }
        }
    } 


    private void ScreenToNearWorld()
    {
        float NdcX = (2f * Input.mousePosition.x - Camera.main.pixelWidth) / Camera.main.pixelWidth;
        float NdcY = (2f * Input.mousePosition.y - Camera.main.pixelHeight) / Camera.main.pixelHeight;
        float NdcZ = -1f;
        Vector3 ndc_position = new Vector3(NdcX, NdcY, NdcZ);
        Vector3 world_position = Camera.main.cameraToWorldMatrix.MultiplyPoint(Camera.main.projectionMatrix.inverse.MultiplyPoint(ndc_position));
        Vector3 world_point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Debug.Log(world_position.ToString("F4"));
        Debug.Log(world_point.ToString("F4"));
    }
}


