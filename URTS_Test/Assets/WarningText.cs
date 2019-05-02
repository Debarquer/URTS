using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningText : MonoBehaviour
{
    float punchTimeMax = 4f;
    float punchTimeCurr = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        punchTimeCurr += Time.deltaTime;
        if(punchTimeCurr >= punchTimeMax) {
            punchTimeCurr = 0;

            iTween.PunchScale(gameObject, new Vector3(1.5f, 1.5f, 1.5f), 2f);
        }
    }
}
