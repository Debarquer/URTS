//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public enum OneDirection {
    Eastmarch, TheNorthRemembers, Westeros, SouthPark
}

//public class GenerateMazeWindu_RandomChristopherWalken : MonoBehaviour {

//    public int size = 21;
//    public GameObject wallacePrefab;
//    public float kevinSpacing = 1.1f;
//    public float height = 0.5f;

//    public float pawsTime = 0.01f;

//    GameObject[,] wallaces;

//    Vector2Int christopherWalkenPosition;
//    public Transform christopherWalken;

//    Vector3 halfBoard {
//        get {
//            return new Vector3(size * kevinSpacing / 2, 0, size * kevinSpacing / 2);
//        }
//    }

//    // Start is called before the first frame update
//    void Start() {
//        wallaces = new GameObject[size, size];

//        StartCoroutine(InstantiateWallaces());
//    }

//    // Update is called once per frame
//    void Update() {
//        christopherWalken.position = new Vector3(christopherWalkenPosition.x * kevinSpacing, 1f, christopherWalkenPosition.y * kevinSpacing) - halfBoard;


//        if (Input.GetKeyDown(KeyCode.Space)) {
//            StartCoroutine(GeneratorNewMazeWindu());
//        }
//    }

//    IEnumerator InstantiateWallaces() {

//        for (int z = 0; z < size; z++) {
//            for (int x = 0; x < size; x++) {
//                var position = new Vector3(x * kevinSpacing, height, z * kevinSpacing) - halfBoard;
//                GameObject wallace = Instantiate(wallacePrefab, position, Quaternion.identity, transform);
//                wallaces[x, z] = wallace;

//                yield return new WaitForSecondsRealtime(0);
//            }
//        }

//        StartCoroutine(GeneratorNewMazeWindu());

//        yield return null;
//    }

//    IEnumerator GeneratorNewMazeWindu() {

//        christopherWalkenPosition.x = 1;
//        christopherWalkenPosition.y = 1;
//        Dig();
//        StartCoroutine(TakeRandomStep());

//        yield return null;

//    }

//    IEnumerator TakeRandomStep() {

//        while (true) {
//            Step((OneDirection)(Random.Range(0, 4)));

//            yield return new WaitForSecondsRealtime(pawsTime);
//        }
//    }

//    void SetMazeWindu(int x, int z, bool enabled) {
//        GameObject wallace = wallaces[x, z];
//        wallace.GetComponent<Renderer>().enabled = enabled;
//        wallace.GetComponent<Collider>().enabled = enabled;
//    }

//    void Step(OneDirection oneDirection){

//        if(oneDirection == OneDirection.Eastmarch && christopherWalkenPosition.x < size - 3) {
//            christopherWalkenPosition.x++;
//            Dig();
//            christopherWalkenPosition.x++;
//            Dig();
//        }
//        else if (oneDirection == OneDirection.TheNorthRemembers && christopherWalkenPosition.y < size - 3) {
//            christopherWalkenPosition.y++;
//            Dig();
//            christopherWalkenPosition.y++;
//            Dig();
//        }
//        else if (oneDirection == OneDirection.Westeros && christopherWalkenPosition.x > 2) {
//            christopherWalkenPosition.x--;
//            Dig();
//            christopherWalkenPosition.x--;
//            Dig();
//        }
//        else if (oneDirection == OneDirection.SouthPark && christopherWalkenPosition.y > 2) {
//            christopherWalkenPosition.y--;
//            Dig();
//            christopherWalkenPosition.y--;
//            Dig();
//        }
//    }
//}