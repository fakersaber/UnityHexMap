using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.8660254f;

    //pointy side up of orientate a hexagon
    public static Vector3[] Edges =
    {
        new Vector3(0f, 0f,outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius ,0f, -0.5f * outerRadius),
        new Vector3(-innerRadius ,0f, 0.5f * outerRadius),
        new Vector3(0f, 0f,outerRadius)
    };

}
