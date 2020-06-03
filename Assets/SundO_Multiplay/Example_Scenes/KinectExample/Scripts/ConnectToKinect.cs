using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToKinect : MonoBehaviour
{

    public K4AdotNet.Samples.Unity.Assets.Scripts.DepthStreamRenderer KinectDepthStream;

    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ParticleSystem.ShapeModule myShape = GetComponent<ParticleSystem>().shape;
            myShape.texture = KinectDepthStream._depthTexture;
        }
    }

}
