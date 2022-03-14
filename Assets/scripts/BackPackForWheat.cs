using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackPackForWheat : MonoBehaviour
{
    public settings _settings;

    [SerializeField] private int maxBagCapacity;
    [SerializeField] private float delayForGivingAwayPacks;
    private bool isGivingAwayBusy;
    private List<ReadyWheatPack> ReadyWheatPacksCollected = new List<ReadyWheatPack>();

    
    private void Start()
    {        
        maxBagCapacity = _settings.MaxBagCapacity;
        delayForGivingAwayPacks = _settings.delayForGivingAwayPacks;
    }

    public bool isReadyToTakeStack()
    {        
        if (ReadyWheatPacksCollected.Count == maxBagCapacity)
        {            
            return false;
        }
        else
        {
            return true;
        }        
    }

    public bool isReadyToGiveAwayStack()
    {
        if (ReadyWheatPacksCollected.Count == 0) return false;
        return !isGivingAwayBusy;
    }


    public int TakeReadyWheatPack(ReadyWheatPack pack)
    {
        Sequence seq = DOTween.Sequence();
        pack.TakenByPlayer();
        pack.transform.parent = transform;
        seq.Append(pack.transform.DOLocalMove(new Vector3(Random.Range(0.1f, 0.201f), Random.Range(-1.2f, - 0.8f), Random.Range(-0.19f, -0.0101f)), 1));
        seq.Join(pack.transform.DOLocalRotate(new Vector3(0, Random.Range(80f,100f), 0), 1));
        seq.Join(pack.transform.DOScale(Vector3.one, 1));
        ReadyWheatPacksCollected.Add(pack);
        return ReadyWheatPacksCollected.Count;
    }

    public int GiveAwayReadyWheatPack(Transform placeToGiveAwayWheat)
    {
        if (ReadyWheatPacksCollected.Count == 0) return 0;

        StartCoroutine(waitDelayForGiveAwayPack());

        Transform currentPackTransform = ReadyWheatPacksCollected[ReadyWheatPacksCollected.Count - 1].GetComponent<Transform>();
        currentPackTransform.parent = placeToGiveAwayWheat;
        Sequence seq = DOTween.Sequence();
        seq.Append(currentPackTransform.transform.DOLocalMove(new Vector3(0, 2, 1), 0.5f));
        seq.Join(currentPackTransform.transform.DOLocalRotate(Vector3.zero, 0.5f));
                
        StartCoroutine(destroyGivenReadyWheatPacks(ReadyWheatPacksCollected[ReadyWheatPacksCollected.Count - 1].gameObject));
        ReadyWheatPacksCollected.Remove(ReadyWheatPacksCollected[ReadyWheatPacksCollected.Count - 1]);

        return ReadyWheatPacksCollected.Count;
    }

    private IEnumerator destroyGivenReadyWheatPacks(GameObject _gameobject)
    {
        yield return new WaitForSeconds(2);
        Destroy(_gameobject);
    }

    
    private IEnumerator waitDelayForGiveAwayPack()
    {
        isGivingAwayBusy = true;
        yield return new WaitForSeconds(delayForGivingAwayPacks);
        isGivingAwayBusy = false;
    }
    

}
