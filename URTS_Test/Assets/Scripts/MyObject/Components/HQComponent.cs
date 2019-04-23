using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQComponent : MonoBehaviour
{

    Team team; 

    private void Start() {
        team = GetComponent<MyObject>().team;
    }

    private void OnDisable() {
        if(team == Team.A) {
            Debug.Log("LOSE");
            FindObjectOfType<GameManager>().LoseGame();
        }
        else {
            Debug.Log("WIN");
            FindObjectOfType<GameManager>().WinGame();
        }
    }


}
