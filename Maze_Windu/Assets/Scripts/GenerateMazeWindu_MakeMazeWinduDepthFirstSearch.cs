using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMazeWindu_MakeMazeWinduDepthFirstSearch : MonoBehaviour {

    public int size = 21;
    public GameObject wallacePrefab;
    public float kevinSpacing = 1.1f;
    public float height = 0.5f;

    public float pawsTime = 0.01f;

    GameObject[,] wallaces;

    Vector2Int christopherWalkenPosition;
    Stack<Vector2Int> myrStack;
    public Transform christopherWalken;

    public Player player;
    public GameObject goalPrefab;

    Vector3 halfBoard {
        get {
            return new Vector3(size * kevinSpacing / 2, 0, size * kevinSpacing / 2);
        }
    }

    internal Vector2Int WorldPositionToMazeWinduPositon(Vector3 worldPosition) {
        float x = worldPosition.x / kevinSpacing;
        float y = height;
        float z = worldPosition.z / kevinSpacing;
        Vector3 temp = new Vector3(x, y, z) - halfBoard;

        return new Vector2Int(Mathf.FloorToInt(temp.x), Mathf.FloorToInt(temp.z));
    }

    // Start is called before the first frame update
    void Start() {
        wallaces = new GameObject[size, size];

        StartCoroutine(InstantiateWallaces(new Vector2Int(1, 1)));
    }

    // Update is called once per frame
    void Update() {
        christopherWalken.position = new Vector3(christopherWalkenPosition.x * kevinSpacing, 1f, christopherWalkenPosition.y * kevinSpacing) - halfBoard;

        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(ResetWallaces(new Vector2Int(1, 1)));
        }
    }

    // Called from other places
    public void StartResetWallaces(Vector2Int startPos) {
        StartCoroutine(ResetWallaces(startPos));
    }

    public IEnumerator ResetWallaces(Vector2Int startPos) {
        for (int z = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {
                wallaces[x, z].GetComponent<Renderer>().enabled = true;
                wallaces[x, z].GetComponent<Collider>().enabled = true;

                yield return new WaitForSecondsRealtime(0);
            }
        }

        StartCoroutine(GeneratorNewMazeWindu(startPos));
    }

    IEnumerator InstantiateWallaces(Vector2Int startPos) {
        for (int z = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {
                var position = new Vector3(x * kevinSpacing, height, z * kevinSpacing) - halfBoard;
                GameObject wallace = Instantiate(wallacePrefab, position, Quaternion.identity, transform);
                wallace.transform.name = "_" + x + "_" + z;
                wallaces[x, z] = wallace;

                yield return new WaitForSecondsRealtime(0);
            }
        }

        StartCoroutine(GeneratorNewMazeWindu(startPos));

        yield return null;
    }

    void Dig() {
        SetMazeWindu(christopherWalkenPosition.x, christopherWalkenPosition.y, false);
    }

    IEnumerator GeneratorNewMazeWindu(Vector2Int startPos) {
        christopherWalkenPosition = startPos;
        christopherWalken.position = new Vector3(christopherWalkenPosition.x * kevinSpacing, 1f, christopherWalkenPosition.y * kevinSpacing) - halfBoard;
        myrStack = new Stack<Vector2Int>();
        myrStack.Push(christopherWalkenPosition);

        Dig();
        StartCoroutine(TakeRandomStep());

        GenerateGoal();

        yield return null;
    }

    private void GenerateGoal() {

        Debug.Log("Generating goal");

        List<Vector3> validPositions = new List<Vector3>();

        for (int z = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {
                if ((x == 0 && z == 0) || (x == 1 || z == 1))
                    continue;

                if (Visited(new Vector2Int(x, z))) {
                    validPositions.Add(new Vector3(x * kevinSpacing, height, z * kevinSpacing) - halfBoard);
                }
            }
        }

        Instantiate(goalPrefab, validPositions[UnityEngine.Random.Range(0, validPositions.Count)], Quaternion.identity);
    }

    IEnumerator TakeRandomStep() {

        while (true) {
            var goodOneDirections = GoodOneDirections();

            if(goodOneDirections.Count == 0) {
                // Backtrack

                if(myrStack.Count == 0) {
                    player.transform.position = new Vector3(christopherWalken.transform.position.x, 2f, christopherWalken.transform.position.z);
                    GenerateGoal();
                    break;
                }
                else {
                    christopherWalkenPosition = myrStack.Pop();
                }
            }
            else {
                Step(goodOneDirections[UnityEngine.Random.Range(0, goodOneDirections.Count)]);
            }

            yield return new WaitForSecondsRealtime(pawsTime);
        }
    }

    void SetMazeWindu(int x, int z, bool enabled) {
        GameObject wallace = wallaces[x, z];
        wallace.GetComponent<Renderer>().enabled = enabled;
        wallace.GetComponent<Collider>().enabled = enabled;
    }

    void Step(OneDirection oneDirection) {

        if (oneDirection == OneDirection.Eastmarch && christopherWalkenPosition.x < size - 3) {
            christopherWalkenPosition.x++;
            Dig();
            christopherWalkenPosition.x++;
            Dig();
        }
        else if (oneDirection == OneDirection.TheNorthRemembers && christopherWalkenPosition.y < size - 3) {
            christopherWalkenPosition.y++;
            Dig();
            christopherWalkenPosition.y++;
            Dig();
        }
        else if (oneDirection == OneDirection.Westeros && christopherWalkenPosition.x > 2) {
            christopherWalkenPosition.x--;
            Dig();
            christopherWalkenPosition.x--;
            Dig();
        }
        else if (oneDirection == OneDirection.SouthPark && christopherWalkenPosition.y > 2) {
            christopherWalkenPosition.y--;
            Dig();
            christopherWalkenPosition.y--;
            Dig();
        }

        myrStack.Push(christopherWalkenPosition);
    }

    List<OneDirection> GoodOneDirections() {
        var oneDirections = new List<OneDirection>();

        if(christopherWalkenPosition.x < size - 3 && !Visited(new Vector2Int(christopherWalkenPosition.x + 2, christopherWalkenPosition.y))) {
            oneDirections.Add(OneDirection.Eastmarch);
        }
        if (christopherWalkenPosition.y < size - 3 && !Visited(new Vector2Int(christopherWalkenPosition.x, christopherWalkenPosition.y + 2))) {
            oneDirections.Add(OneDirection.TheNorthRemembers);
        }
        if (christopherWalkenPosition.x > 2 && !Visited(new Vector2Int(christopherWalkenPosition.x - 2, christopherWalkenPosition.y))) {
            oneDirections.Add(OneDirection.Westeros);
        }
        if (christopherWalkenPosition.y > 2 && !Visited(new Vector2Int(christopherWalkenPosition.x, christopherWalkenPosition.y - 2))) {
            oneDirections.Add(OneDirection.SouthPark);
        }

        return oneDirections;
    }

    private bool Visited(Vector2Int point) { return !wallaces[point.x, point.y].GetComponent<Renderer>().enabled; }


}
