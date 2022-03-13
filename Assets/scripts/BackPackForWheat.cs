using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackForWheat : MonoBehaviour
{
    public settings _settings;

    private int maxBagCapacity;
    private int currentBagCapacity;
    private Transform bag;

    private void Start()
    {
        maxBagCapacity = _settings.MaxBagCapacity;
        bag = GameObject.Find("BackPack").transform;
    }

    public bool isReadyToTakeStack()
    {
        print(maxBagCapacity);

        if (currentBagCapacity == maxBagCapacity)
        {
            return false;
        }
        else
        {
            return true;
        }

        
    }


}
