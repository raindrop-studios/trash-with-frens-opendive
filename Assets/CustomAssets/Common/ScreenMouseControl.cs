using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMouseControl : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;
    [SerializeField] private float capOffset = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 mousePos = Input.mousePosition;

        //if(mousePos.x >= Screen.width)
        //{
        //    Debug.Log("right");
        //}
        //if (mousePos.x <= 0)
        //{
        //    Debug.Log("left");
        //}
        //if (mousePos.y >= Screen.height)
        //{
        //    Debug.Log("top");
        //}
        //if (mousePos.y <= 0)
        //{
        //    Debug.Log("down");
        //}
        //Debug.Log("mousePos.x " + mousePos.x);
        //Debug.Log("mousePos.y " + mousePos.y);

        //Debug.Log("screen width " + Screen.currentResolution.width);
        //Debug.Log("screen heigh " + Screen.currentResolution.height);
    }
}
