using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReadyWheatPack : MonoBehaviour
{
    private float angle;
    private BoxCollider boxCollider;
    private bool isRotating = true;
    
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
        if (isRotating)
        {
            angle += Time.deltaTime * 40f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }                
    }

    public void TakenByPlayer()
    {
        isRotating = false;
        boxCollider.enabled = false;        
    }
   

    private void SetReadyToInteract()
    {
        boxCollider.enabled = true;
    }

}
