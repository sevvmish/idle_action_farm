using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WheatPack : MonoBehaviour
{
    public settings _settings;

    [SerializeField] private int HowManyChopsBeforeCutOff;
    [SerializeField] private float CoolDownForNextCut = 1f;

    private bool isCoolDownForTrigger;
    private int currentChopsBeforeCutOff;
     
    private Transform[] wheats = new Transform[6];
    private WheatData[] initData = new WheatData[6];

    private float timeForMowAnimation = 1f;
    private float timeForWheatGrow = 10f;

    private BoxCollider boxCollider;
    private GameObject chopParticleEffect; 

    private void OnEnable()
    {
        HowManyChopsBeforeCutOff = _settings.HowManyChopsBeforeCutOff;
        isCoolDownForTrigger = false;
        currentChopsBeforeCutOff = HowManyChopsBeforeCutOff;
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;

        chopParticleEffect = transform.GetChild(8).gameObject;
        chopParticleEffect.SetActive(false);

        for (int i = 0; i < wheats.Length; i++)
        {            
            wheats[i] = transform.GetChild(i);
            initData[i] = new WheatData(wheats[i].localPosition, wheats[i].rotation, wheats[i].localScale);            
        }

        //make unique materials
        for (int i = 0; i < wheats.Length; i++)
        {
            Material newMaterial = Instantiate(transform.GetChild(i).GetComponent<MeshRenderer>().material);
            newMaterial.SetFloat("color_slider", 1f);
            wheats[i].GetComponent<MeshRenderer>().material = newMaterial;
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
        StartCoroutine(startChopParticleEffect());

        currentChopsBeforeCutOff--;

        if (currentChopsBeforeCutOff == 0)
        {
            boxCollider.enabled = false;
            StartCoroutine(ChopEffect());
            
        }
        
    }

    private IEnumerator startChopParticleEffect()
    {
        if (chopParticleEffect.activeSelf) chopParticleEffect.SetActive(false);
        chopParticleEffect.SetActive(true);
        yield return new WaitForSeconds(2);
        chopParticleEffect.SetActive(false);
    }

    private IEnumerator ChopEffect() 
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

        StartCoroutine(createReadyPack());

        yield return new WaitForSeconds(timeForMowAnimation*2);

        StartCoroutine(growAgain());
        
    }

    private IEnumerator coolDownForCutOff()
    {
        isCoolDownForTrigger = true;
        yield return new WaitForSeconds(CoolDownForNextCut);
        isCoolDownForTrigger = false;
    }

    private IEnumerator createReadyPack()
    {
        
        yield return new WaitForSeconds(timeForMowAnimation);
        GameObject pack = Instantiate(Resources.Load<GameObject>("ReadyWheatStack"), Vector3.zero, Quaternion.identity, transform);
        pack.transform.localPosition = Vector3.zero;
    }

    private IEnumerator growAgain()
    {
        
        yield return new WaitForSeconds(Time.deltaTime);

        Sequence seq1 = DOTween.Sequence();
        
        for (int i = 0; i < wheats.Length; i++)
        {
            wheats[i].GetComponent<MeshRenderer>().material.SetFloat("color_slider", 0);
            seq1.Join(wheats[i].transform.DOScale(Vector3.zero, 0));
            seq1.Join(wheats[i].transform.DOLocalMove(initData[i].position, 0, false));
            seq1.Join(wheats[i].transform.DORotate(initData[i].rotation.eulerAngles, 0, RotateMode.Fast));
            seq1.Join(wheats[i].transform.DOScaleX(initData[i].scale.x, 0));            
        }

        for (int i = 0; i < wheats.Length; i++)
        {
            seq1.Join(wheats[i].transform.DOScaleY(initData[i].scale.y * 0.6f, timeForWheatGrow * 0.9f));
            seq1.Join(wheats[i].transform.DOScaleZ(initData[i].scale.z * 0.6f, timeForWheatGrow * 0.9f));
            seq1.Join(wheats[i].GetComponent<MeshRenderer>().material.DOFloat(0.8f, "color_slider", timeForWheatGrow * 0.9f));
        }

        yield return new WaitForSeconds(timeForWheatGrow * 0.9f);

        for (int i = 0; i < wheats.Length; i++)
        {
            seq1.Join(wheats[i].transform.DOScaleY(initData[i].scale.y, timeForWheatGrow * 0.1f));
            seq1.Join(wheats[i].transform.DOScaleZ(initData[i].scale.z, timeForWheatGrow * 0.1f));
            seq1.Join(wheats[i].GetComponent<MeshRenderer>().material.DOFloat(1, "color_slider", 0.1f));
        }

        yield return new WaitForSeconds(timeForWheatGrow * 0.1f);

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
