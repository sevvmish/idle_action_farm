using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Settings")]
public class settings : ScriptableObject
{    
    public float playerSpeed = 4f;    
    public int HowManyChopsBeforeCutOff = 1;
    public int MaxBagCapacity = 40;
}
