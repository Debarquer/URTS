using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team { A, B, C, D };
public class GameManager : MonoBehaviour
{
    float minerals = 1500;
    float power = 0;

    public Text mineralsText;
    public Text powerText;

    public Canvas RadarCanvas;

    private void Start() {
        RadarCanvas.enabled = false;
    }

    public float GetMinerals() {
        return minerals;
    }

    public void UpdateMinerals(float amount) {
        minerals += amount;
        minerals = Mathf.Clamp(minerals, 0, 999999);
        mineralsText.text = "Minerals: " + minerals;
    }

    public void UpdatePower(float amount) {
        power += amount;
        power = Mathf.Clamp(power, -999999, 999999);
        powerText.text = "Power: " + power;

        if (power - amount <= 0 && power > 0) {
            // Power restored
            ActivatePoweredBuildings();
        }
        else if(power - amount > 0 && power <= 0) {
            // Power lost
            DeactivatePoweredBuildings();
        }
    }

    public float GetPower() {
        return power;
    }

    public void EnableRadar(bool enabled) {
        RadarCanvas.enabled = enabled;
    }

    private void DeactivatePoweredBuildings() {
        foreach(MyObject myObject in FindObjectsOfType<MyObject>()) {
            myObject.Disable();
        }
    }

    private void ActivatePoweredBuildings() {
        foreach (MyObject myObject in FindObjectsOfType<MyObject>()) {
            myObject.Activate();
        }
    }
}
