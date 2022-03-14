using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    private Camera currentCamera;
    private RectTransform Coin;
    GameObject t;
    // Start is called before the first frame update
    void Start()
    {
        currentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Coin = transform.GetChild(0).GetComponent<RectTransform>();
        t = Instantiate(transform.GetChild(0).gameObject);
        t.transform.parent = GameObject.Find("where").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        print(currentCamera.ScreenToWorldPoint(transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition3D));
        Vector3 vec = currentCamera.WorldToScreenPoint(GameObject.Find("PointOfBarn").transform.position);
        t.transform.position = vec;

    }
}
