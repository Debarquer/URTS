using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.name == "Player") {
            GenerateMazeWindu_MakeMazeWinduDepthFirstSearch generateMazeWindu_MakeMazeWinduDepthFirstSearch = FindObjectOfType<GenerateMazeWindu_MakeMazeWinduDepthFirstSearch>();
            Vector2Int vector2Int = generateMazeWindu_MakeMazeWinduDepthFirstSearch.WorldPositionToMazeWinduPositon(transform.position);
            generateMazeWindu_MakeMazeWinduDepthFirstSearch.StartResetWallaces(vector2Int);
            Destroy(gameObject);
        }
    }
}
