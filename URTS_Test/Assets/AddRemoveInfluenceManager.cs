using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRemoveInfluenceManager : MonoBehaviour
{
    [SerializeField] List<AddRemoveInfluence> addRemoveInfluences = new List<AddRemoveInfluence>();

    public void AddAddRemoveInfluence(AddRemoveInfluence addRemoveInfluence) {
        addRemoveInfluences.Add(addRemoveInfluence);
    }

    public void ShowAddRemoveInfluences() {
        foreach(AddRemoveInfluence addRemoveInfluence in addRemoveInfluences) {
            addRemoveInfluence.gameObject.SetActive(true);
        }
    }

    public void HideAddRemoveInfluences() {
        foreach (AddRemoveInfluence addRemoveInfluence in addRemoveInfluences) {
            addRemoveInfluence.gameObject.SetActive(false);
        }
    }
}
