using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour
{

    public settings _settings;
    public Joystick CurrentJoystick;
        
    private CharacterController playerCharController;
    private float playerSpeed;
    private CurrentPlayerState PlayerState;
    private SphereCollider scytheCollider;
    private GameObject Scythe;

    private float timeBeforeActivateScytheMow = 0.8f;
    private float timeForMowActiveWork = 0.15f;

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
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("wheat"))
        {            
            Mow();
        }

        if (other.CompareTag("wheatpack"))
        {
            //other.gameObject.SetActive(false);
            GetComponent<BackPackForWheat>().isReadyToTakeStack();
        }
    }

    public void Run()
    {
        PlayerState.SetState(CurrentPlayerState.states.run);
    }

    public void Idle()
    {
        PlayerState.SetState(CurrentPlayerState.states.idle);
    }

    public void Mow()
    {
        PlayerState.SetState(CurrentPlayerState.states.mow);
    }


    public void ActivateScythe()
    {
        StartCoroutine(ActiveWorkOfScythe());        
    }

    public void DeactivateScythe()
    {
        Scythe.SetActive(false);
    }

    
    IEnumerator ActiveWorkOfScythe()
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
    private states currentState;
    private Animator currentAnimator;
    private bool isBusy;
    
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

        currentState = states.idle;
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

        currentState = states.run;
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
        currentState = states.mow;
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
