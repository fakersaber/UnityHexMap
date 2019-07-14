////Attach this script to your Camera
////This draws a line in the Scene view going through a point 200 pixels from the lower-left corner of the screen
////To see this, enter Play Mode and switch to the Scene tab. Zoom into your Camera's position.
//using UnityEngine;
//using System.Collections;

//public class CameraRay : MonoBehaviour
//{
//    Camera cam;

//    void Start()
//    {
//        cam = GetComponent<Camera>();
//    }

//    void Update()
//    {
//        Ray ray = cam.ScreenPointToRay(new Vector3(0, 0, 0));
//        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
//        ray = cam.ScreenPointToRay(Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0)));
//        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
//    }
//}