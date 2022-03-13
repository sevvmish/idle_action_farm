using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WheatPack : MonoBehaviour
{
    public settings _settings;

    private int HowManyChopsBeforeCutOff = 1;
    private float CoolDownForNextTrigger = 1f;

    private bool isCoolDownForTrigger;
    private int currentChopsBeforeCutOff;
    private DateTime lastMow;    
    private Transform[] wheats = new Transform[6];
    private WheatData[] initData = new WheatData[6];

    private float timeForMowAnimation = 1f;
    private float timeForWheatGrow = 10f;

    private BoxCollider boxCollider;

    private void OnEnable()
    {
        HowManyChopsBeforeCutOff = _settings.HowManyChopsBeforeCutOff;
        isCoolDownForTrigger = false;
        currentChopsBeforeCutOff = HowManyChopsBeforeCutOff;
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;

        for (int i = 0; i < wheats.Length; i++)
        {            
            wheats[i] = transform.GetChild(i);
            initData[i] = new WheatData(wheats[i].localPosition, wheats[i].rotation, wheats[i].localScale);            
        }
               
        lastMow = DateTime.Now;
    }


    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("scythe") && !isCoolDownForTrigger)
        {            
            ChopThisPack();
        }        
    }

    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("scythe") && !isCoolDownForTrigger)
        {            
            ChopThisPack();
        }
    }
    

    private void ChopThisPack()
    {
        StartCoroutine(coolDownForCutOff());

        currentChopsBeforeCutOff--;

        if (currentChopsBeforeCutOff == 0)
        {
            boxCollider.enabled = false;
            ChopEffect();
            StartCoroutine(grow());
            StartCoroutine(createReadyPack());
        }
        
    }

    private void ChopEffect() 
    {
        Vector3 new_position = new Vector3(transform.position.x, transform.position.y+0.1f, transform.position.z);
        Sequence seq = DOTween.Sequence();
      
        for (int i = 0; i < wheats.Length; i++)
        {

            seq.Join(wheats[i].transform.DOMove(new Vector3(transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f), transform.position.y + 0.1f, transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f)), timeForMowAnimation, false));
            seq.Join(wheats[i].transform.DORotate(new Vector3(80, UnityEngine.Random.Range(-70f, 70f) , 0), timeForMowAnimation, RotateMode.Fast));
            
        }

        seq.AppendInterval(timeForMowAnimation);

        for (int i = 0; i < wheats.Length; i++)
        {
            seq.Join(wheats[i].transform.DOLocalMove(new Vector3(0, 0.5f, 0), timeForMowAnimation/2, false));
            seq.Join(wheats[i].transform.DOScale(0, timeForMowAnimation/2f));
        }

        for (int i = 0; i < wheats.Length; i++)
        {
            seq.Join(wheats[i].transform.DOLocalMove(Vector3.zero, 0.01f, false));            
        }

        
    }

    private IEnumerator coolDownForCutOff()
    {
        isCoolDownForTrigger = true;
        yield return new WaitForSeconds(CoolDownForNextTrigger);
        isCoolDownForTrigger = false;
    }

    private IEnumerator createReadyPack()
    {
        yield return new WaitForSeconds(timeForMowAnimation * 1.5f);
        GameObject pack = Instantiate(Resources.Load<GameObject>("ReadyWheatStack"), Vector3.zero, Quaternion.identity, transform);
        pack.transform.localPosition = Vector3.zero;

    }

    private IEnumerator grow()
    {
        yield return new WaitForSeconds(2f);
        Sequence seq1 = DOTween.Sequence();

        for (int i = 0; i < wheats.Length; i++)
        {
            seq1.Join(wheats[i].transform.DOLocalMove(initData[i].position, 0.01f, false));
            seq1.Join(wheats[i].transform.DORotate(initData[i].rotation.eulerAngles, 0.01f, RotateMode.Fast));

            seq1.Join(wheats[i].transform.DOScaleX(initData[i].scale.x, 0.01f));
            seq1.Join(wheats[i].transform.DOScaleY(initData[i].scale.y, timeForWheatGrow - 2f));
            seq1.Join(wheats[i].transform.DOScaleZ(initData[i].scale.z, timeForWheatGrow - 2f));
        }

        yield return new WaitForSeconds(timeForWheatGrow - 2f);

        currentChopsBeforeCutOff = HowManyChopsBeforeCutOff;
        boxCollider.enabled = true;
    }

    public struct WheatData
    {
        public Vector3 position, scale;
        public Quaternion rotation;


        public WheatData(Vector3 _pos, Quaternion _rot, Vector3 _scale)
        {
            position = _pos;
            rotation = _rot;
            scale = _scale;
        }
    }
}
