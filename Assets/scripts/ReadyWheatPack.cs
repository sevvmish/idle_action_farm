using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReadyWheatPack : MonoBehaviour
{
    private float angle;
    private BoxCollider boxCollider;
    
    private void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        Invoke("SetReadyToInteract", 0.5f);
        transform.DOShakeScale(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * 40f;
        transform.rotation = Quaternion.  AngleAxis(angle, Vector3.up);        
    }

   

    private void SetReadyToInteract()
    {
        boxCollider.enabled = true;
    }

}
