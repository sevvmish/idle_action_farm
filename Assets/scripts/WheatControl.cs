using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheatControl : MonoBehaviour
{
    public float rowX = 5, rowY = 5;
    public Vector3 wheatPackPlace;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        for (float x = wheatPackPlace.x; x < (wheatPackPlace.x + rowX); x++)
        {
            for (float z = wheatPackPlace.z; z >(wheatPackPlace.z - rowY); z--)
            {
                GameObject pack = Instantiate(Resources.Load<GameObject>("WheatPack"), new Vector3(x, 0, z), Quaternion.identity, transform);
                pack.name = i.ToString();
                
                i++;
            }
        }

    }

  

}
