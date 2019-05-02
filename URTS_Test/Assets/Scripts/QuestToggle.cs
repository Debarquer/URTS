using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestToggle : MonoBehaviour
{
    public GameObject checkmark;
    public Text text;

    bool completed = false;

    public delegate bool FunctionToBeCalledDelegate();
    public FunctionToBeCalledDelegate functionToBeCalled;

    public bool IsCompleted() { return completed; }

    public void BeginQuest() {
        //iTween.MoveFrom(gameObject, transform.position - new Vector3(400f, 0f, 0f), 1.5f);
    }

    public void Complete() {
        iTween.PunchScale(gameObject, new Vector3(1.5f, 1.5f, 1.5f), 1.5f);

        //text.color = Color.green;
        //GetComponentInChildren<Outline>().effectColor = Color.gray;

        completed = true;
        checkmark.SetActive(true);
    }
}
