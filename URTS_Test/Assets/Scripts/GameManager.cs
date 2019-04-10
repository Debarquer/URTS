﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum team { A, B, C, D };
public class GameManager : MonoBehaviour
{
    float minerals = 1500;
    float power = 0;

    public Text mineralsText;
    public Text powerText;

    public float GetMinerals() {
        return minerals;
    }

    public void UpdateMinerals(int amount) {
        minerals += amount;
        minerals = Mathf.Clamp(minerals, 0, 999999);
        mineralsText.text = "Minerals: " + minerals;
    }

    public void UpdatePower(int amount) {
        power += amount;
        power = Mathf.Clamp(power, -999999, 999999);
        powerText.text = "Power: " + power;
    }
}
