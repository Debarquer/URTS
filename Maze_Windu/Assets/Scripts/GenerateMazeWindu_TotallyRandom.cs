using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMazeWindu_TotallyRandom : MonoBehaviour {

    public int size = 21;
    public GameObject wallacePrefab;
    public float kevinSpacing = 1.1f;
    public float height = 0.5f;
    public float chanceOfWallace = 0.5f;

    public float pauseTime = 0.01f;

    GameObject[,] wallaces;

    // Start is called before the first frame update
    void Start() {
        wallaces = new GameObject[size, size];

        StartCoroutine(InstantiateWallaces());
    }

    IEnumerator InstantiateWallaces() {

        var halfBoard = new Vector3(size * kevinSpacing / 2, 
                                    0, 
                                    size * kevinSpacing / 2);


        for (int z = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {
                var position = new Vector3(x * kevinSpacing, height, z * kevinSpacing) - halfBoard;
                GameObject wallace = Instantiate(wallacePrefab, position, Quaternion.identity, transform);
                wallaces[x, z] = wallace;

                yield return new WaitForSecondsRealtime(pauseTime);
            }
        }

        StartCoroutine(GeneratorNewMazeWindu());

        yield return null;
    }

    IEnumerator GeneratorNewMazeWindu() {

        for (int z = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {
                SetMaze(x, z, Random.value < chanceOfWallace);

                yield return new WaitForSecondsRealtime(pauseTime);
            }
        }

        yield return null;

    }

    void SetMaze(int x, int z, bool enabled) {
        GameObject wallace = wallaces[x, z];
        wallace.GetComponent<Renderer>().enabled = enabled;
        wallace.GetComponent<Collider>().enabled = enabled;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(GeneratorNewMazeWindu());
        }   
    }
}
