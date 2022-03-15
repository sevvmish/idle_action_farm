using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{

    public settings _settings;
    public Joystick CurrentJoystick;

    private CharacterController playerCharController;
    [SerializeField] private float playerSpeed;
    [SerializeField] private int CollectedWheatPackAmount;
    [SerializeField] private int maxBagCapacity;
    [SerializeField] private float timeBeforeActivateScytheMow = 0.8f;
    [SerializeField] private float timeForMowActiveWork = 0.15f;

    private CurrentPlayerState PlayerState;
    private UIManager UI;
    private SphereCollider scytheCollider;
    private GameObject Scythe;

    private BackPackForWheat bag;   

    // Start is called before the first frame update
    private void Start()
    {
        Screen.SetResolution(720, 1280, true);
        Camera.main.aspect = 9f / 16f;
        Application.targetFrameRate = 60;

        Scythe = GameObject.Find("ScytheV1");
        Scythe.SetActive(false);
        scytheCollider = Scythe.GetComponent<SphereCollider>();
        scytheCollider.isTrigger = false;

        playerCharController = GetComponent<CharacterController>();
        PlayerState = new CurrentPlayerState(GetComponent<Animator>());

        playerSpeed = _settings.playerSpeed;

        maxBagCapacity = _settings.MaxBagCapacity;
        bag = GameObject.Find("BackPack").GetComponent<BackPackForWheat>();
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + new Vector3(CurrentJoystick.Horizontal, 0, CurrentJoystick.Vertical));

        if (Mathf.Abs(CurrentJoystick.Horizontal) > 0 || Mathf.Abs(CurrentJoystick.Vertical) > 0)
        {
            playerCharController.Move(transform.TransformDirection(Vector3.forward) * Time.deltaTime * playerSpeed);
            Run();
        }
        else
        {
            Idle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wheat") && CollectedWheatPackAmount < maxBagCapacity)
        {
            Mow();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("wheat") && CollectedWheatPackAmount < maxBagCapacity)
        {            
            Mow();
        }

        if (other.CompareTag("wheatpack") && bag.isReadyToTakeStack())
        {
            ObtainReadyWheatPack(other.gameObject);
        }

        if (other.CompareTag("base") && bag.isReadyToGiveAwayStack())
        {            
            giveAwayReadyWheatPack(other.transform);
        }
    }

    //animation uses for better chop
    public void ActivateScythe()
    {
        StartCoroutine(ActiveWorkOfScythe());
    }

    //animation uses for better chop
    public void DeactivateScythe()
    {
        Scythe.SetActive(false);
    }

    private void giveAwayReadyWheatPack(Transform placeToGiveAwayWheat)
    {
        CollectedWheatPackAmount = bag.GiveAwayReadyWheatPack(placeToGiveAwayWheat);
        UI.SetWheatAmount(CollectedWheatPackAmount);
    }

    private void ObtainReadyWheatPack(GameObject pack)
    {
        CollectedWheatPackAmount = bag.TakeReadyWheatPack(pack.GetComponent<ReadyWheatPack>());
        UI.SetWheatAmount(CollectedWheatPackAmount);
    }

    private void Run()
    {
        PlayerState.SetState(CurrentPlayerState.states.run);
    }

    private void Idle()
    {
        PlayerState.SetState(CurrentPlayerState.states.idle);
    }

    private void Mow()
    {
        PlayerState.SetState(CurrentPlayerState.states.mow);
    }
    
    
    private IEnumerator ActiveWorkOfScythe()
    {
        Scythe.SetActive(true);
        yield return new WaitForSeconds(timeBeforeActivateScytheMow);

        scytheCollider.isTrigger = true;        
        yield return new WaitForSeconds(timeForMowActiveWork);
        
        scytheCollider.isTrigger = false;       
    }
    
}




public class CurrentPlayerState
{    
    private Animator currentAnimator;
    
    public enum states
    {
        idle,
        run,
        mow
    }

    public CurrentPlayerState(Animator _animator)
    {
        currentAnimator = _animator;
    }



    public void SetState(states newState)
    {
        switch ((int)newState)
        {
            case 0:
                Idle();
                break;

            case 1:
                Run();
                break;

            case 2:
                Mow();
                break;

        }
    }

    private void Idle()
    {
        if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
        {            
            return;
        }

        
        resetMow();
        currentAnimator.StopPlayback();
        currentAnimator.Play("idle");
    }

    private void Run()
    {
        if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("run"))
        {
            return;
        }
        
        resetMow();
        currentAnimator.StopPlayback();
        currentAnimator.Play("run");
    }

    private void Mow()
    {
        if (!currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("run") && !currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
        {
            currentAnimator.SetLayerWeight(0, 1f);
        }
       
        if (currentAnimator.GetCurrentAnimatorStateInfo(1).IsTag("mow"))
        {
            return;
        }
        
        currentAnimator.SetLayerWeight(1, 1f);
        currentAnimator.StopPlayback();
        currentAnimator.Play("mow");
    }

    private void resetMow()
    {
        if (!currentAnimator.GetCurrentAnimatorStateInfo(1).IsTag("mow"))
        {
            currentAnimator.SetLayerWeight(1, 0);
        }
    }

  

}
