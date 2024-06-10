using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDepth : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    private void Start(){
    //get the camera and tell it to render a depthnormals texture
        cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
