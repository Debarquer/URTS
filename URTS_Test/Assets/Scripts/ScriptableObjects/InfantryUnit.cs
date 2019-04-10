using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Infantry Unit ", menuName = "Units/Infantry/Infantry Unit", order = 1)]
public class InfantryUnit : ScriptableObject
{
    public MyObject MyObject;
    public GameObject UIGameObjectPrefab;
}
