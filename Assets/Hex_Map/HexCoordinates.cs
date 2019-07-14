using UnityEngine;

//创建新的坐标系，表示6方向偏移
/*
 * ← == x-1,y+1 == (-1,1,0)
 * → == x+1,y-1 == (1,-1,0)
 * ↖ == x-1,z+1 == (-1,0,1)
 * ↘ == x+1,z-1 == (1,0,-1)
 * ↙ == y+1,z-1 == (0,1,-1)
 * ↗ == y-1,z+1 == (0,-1,1)
*/

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;

    //这样定义一个访问方法，会自己创建一个变量为X    ?
    //public int X
    //{
    //    get;
    //    set;
    //}

    public int X
    {
        get
        {
            return x;
        }
    }
    public int Z
    {
        get
        {
            return z;
        }
    }
    //原XZ只能表示4个方向的偏移，增加一个轴表示另外两个方向，注意三个基底不正交，所以数学上不光滑
    public int Y
    {
        get
        {
            return -X - Z;
        }
    }
    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }


    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        //减去对应的起始偏移量，保证x六边形的轴为斜（实际上就是让x轴与z轴呈120度）
        return new HexCoordinates(x - z / 2, z);
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        //在此计算哪一个单元格被触发了    
        //数学推导三坐标轴x,y,z互成120度,分别求得position投影到x,y轴的坐标
        float x = (1.732050f * position.x - position.z) / (3f * HexMetrics.outerRadius);
        float y = (-1.732050f * position.x - position.z) / (3f * HexMetrics.outerRadius);
        int CoordinateX = Mathf.RoundToInt(x);
        int CoordinateY = Mathf.RoundToInt(y);
        int CoordinateZ = Mathf.RoundToInt(-x - y);
        if (CoordinateX + CoordinateY + CoordinateZ != 0)
        {
            //在冲突区域计算舍入值最大的坐标进行重新计算（未推导）
            float dX = Mathf.Abs(x - CoordinateX);
            float dY = Mathf.Abs(y - CoordinateY);
            float dZ = Mathf.Abs(-x - y - CoordinateZ);

            if (dX > dY && dX > dZ)
            {
                CoordinateX = -CoordinateY - CoordinateZ;
            }
            else if (dZ > dY)
            {
                CoordinateZ = -CoordinateX - CoordinateY;
            }
            //这里没有写dy 大于的情况原因是如果X和Z没错，最后都会重新构建一个Y，所以无需判断
            // Debug.Log("touched at " + CoordinateX + " " + CoordinateY + " " + CoordinateZ);
        }
        return new HexCoordinates(CoordinateX, CoordinateZ);
    }
}



