using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoomsdayTimer : MonoBehaviour
{

    public float secondsUntilDoom;
    Text doomTimerText;

    public float timeIncreaseFactor = 1f;

    DoomLaser DoomLaser;

    // Start is called before the first frame update
    void Start()
    {
        doomTimerText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        secondsUntilDoom-=Time.deltaTime* timeIncreaseFactor;
        if(DoomLaser == null) {
            DoomLaser = FindObjectOfType<DoomLaser>();
        }

        secondsUntilDoom = Mathf.Max(secondsUntilDoom, 0);

        if (secondsUntilDoom < 1200 && DoomLaser.doomLaserStage == DoomLaserStages.Dormant) {
            iTween.PunchScale(doomTimerText.gameObject, new Vector3(1, 1, 1), 2f);
            DoomLaser.AdvanceStage();
        }
        if (secondsUntilDoom < 960 && DoomLaser.doomLaserStage == DoomLaserStages.Stage1) {
            iTween.PunchScale(doomTimerText.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 2f);
            DoomLaser.AdvanceStage();
        }
        if (secondsUntilDoom < 720 && DoomLaser.doomLaserStage == DoomLaserStages.Stage2) {
            iTween.PunchScale(doomTimerText.gameObject, new Vector3(2, 2, 2), 2f);
            DoomLaser.AdvanceStage();
        }
        if (secondsUntilDoom < 500 && DoomLaser.doomLaserStage == DoomLaserStages.Stage3) {
            iTween.PunchScale(doomTimerText.gameObject, new Vector3(2.5f, 2.5f, 2.5f), 2f);
            DoomLaser.AdvanceStage();
        }
        if (secondsUntilDoom < 240 && DoomLaser.doomLaserStage == DoomLaserStages.Stage4) {
            iTween.PunchScale(doomTimerText.gameObject, new Vector3(3, 3, 3), 2f);
            DoomLaser.AdvanceStage();
        }
        if (secondsUntilDoom <= 0 && DoomLaser.doomLaserStage == DoomLaserStages.Firing) {
            iTween.PunchScale(doomTimerText.gameObject, new Vector3(4, 4, 4), 1f);
            DoomLaser.AdvanceStage();
        }

        int hours = (Mathf.FloorToInt(secondsUntilDoom)) / 3600;
        int minutes = ((Mathf.FloorToInt(secondsUntilDoom)) % 3600)/60;
        int seconds = (Mathf.FloorToInt(secondsUntilDoom)) % 60;

        doomTimerText.text = string.Format("Time until laser fires \n {0}:{1}:{2}", hours, minutes, seconds);
    }
}
