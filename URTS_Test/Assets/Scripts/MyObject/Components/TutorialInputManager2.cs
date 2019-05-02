using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInputManager2 : InputManager {

    public GameObject questTogglePrefab;
    public Transform questToggleContainer;

    public QuestToggle currentQuest;
    public Queue<QuestToggle> questToggles = new Queue<QuestToggle>();

    public GameObject buildingBlockerA;
    public GameObject buildingBlockerB;

    public List<string> buildings = new List<string>();

    #region Quest 0
    bool familiarized = false;
    #endregion

    #region Quest 1
    bool Q1Initialized = false;
    #endregion

    #region Quest 2
    bool Q2Initialized = false;
    #endregion
    
    #region Quest 3
    bool Q3Initialized = false;
    #endregion

    #region Quest 6
    bool Q6Initialized = false;
    #endregion

    #region Quest 10
    bool Q10Initialized = false;
    #endregion

    #region Quest 11
    bool Q11Initialized = false;
    #endregion

    #region Quest 12
    bool Q12Initialized = false;

    public MyObject enemyHQ;
    public MyObject enemySpawner1;
    public MyObject enemySpawner2;
    #endregion

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        QuestToggle[] questTogglesInChildren = questToggleContainer.GetComponentsInChildren<QuestToggle>();
        for (int i = 0; i < questTogglesInChildren.Length; i++) {
            switch (i) {
                case 0:
                    break;
                case 1:
                    questTogglesInChildren[i].functionToBeCalled = Familiarize;
                    break;
                case 2:
                    questTogglesInChildren[i].functionToBeCalled = RaytraceForHQ;
                    break;
                case 3:
                    questTogglesInChildren[i].functionToBeCalled = BuildPowerPlant;
                    break;
                case 4:
                    questTogglesInChildren[i].functionToBeCalled = BuildRefinery;
                    break;
                case 5:
                    questTogglesInChildren[i].functionToBeCalled = BuildBarracks;
                    break;
                case 6:
                    questTogglesInChildren[i].functionToBeCalled = SelectBarracks;
                    break;
                case 7:
                    questTogglesInChildren[i].functionToBeCalled = TrainInfantryUnit;
                    break;
                case 8:
                    questTogglesInChildren[i].functionToBeCalled = BuildFactory;
                    break;
                case 9:
                    questTogglesInChildren[i].functionToBeCalled = SelectFactory;
                    break;
                case 10:
                    questTogglesInChildren[i].functionToBeCalled = BuildTank;
                    break;
                case 11:
                    questTogglesInChildren[i].functionToBeCalled = BuildRadar;
                    break;
                case 12:
                    questTogglesInChildren[i].functionToBeCalled = BuildLaserTower;
                    break;
                case 13:
                    questTogglesInChildren[i].functionToBeCalled = RestorePower;
                    break;
                case 14:
                    questTogglesInChildren[i].functionToBeCalled = DestroyEnemyHQ;
                    break;
                default:
                    break;
            }

            if (i == 0)
                continue;

            questToggles.Enqueue(questTogglesInChildren[i]);
            questTogglesInChildren[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    override protected void Update()
    {
        if(familiarized)
            base.Update();

        //HandleCameraMovement();
        //HandleStandardInput();

        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu") {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        if (currentQuest == null && questToggles.Count > 0) {
            currentQuest = questToggles.Dequeue();
            currentQuest.gameObject.SetActive(true);
            currentQuest.BeginQuest();
        }
        if (currentQuest == null) {
            //FindObjectOfType<GameManager>().WinGame();

            return;
        }

        if(currentQuest.functionToBeCalled == null) {
            return;
        }

        if (currentQuest.functionToBeCalled()) {
            currentQuest.Complete();

            if (questToggles.Count > 0) {
                currentQuest = questToggles.Dequeue();
                currentQuest.gameObject.SetActive(true);
                currentQuest.BeginQuest();
            }
            else {
                currentQuest = null;

                //FindObjectOfType<GameManager>().WinGame();
            }
        }
    }

    bool RaytraceForHQ() {
        if (Input.GetMouseButtonUp(0)) {
            ClickableComponent[] clickableComponents = RaycastForClickableComponents();
            foreach (ClickableComponent clickableComponent in clickableComponents) {
                HQComponent hQComponent = clickableComponent.GetComponent<HQComponent>();
                if (hQComponent != null) {
                    return true;
                }
            }
        }
        
        return false;
    }

    private bool RestorePower() {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if(gameManager.GetPower() > 0) {
            return true;
        }
        else {
            return false;
        }
    }

    bool BuildPowerPlant() {
        if (!Q1Initialized) {
            Q1Initialized = true;

            buildingBlockerA.transform.position += new Vector3(0, -100, 0);
        }

        if (buildings.Contains("PowerPlant")) {
            return true;
        }

        return false;
    }

    bool BuildRefinery() {
        if (!Q2Initialized) {
            Q2Initialized = true;

            buildingBlockerB.transform.position += new Vector3(0, -100, 0);
        }

        if (buildings.Contains("Refinery")) {
            return true;
        }

        return false;
    }

    bool BuildBarracks() {
        if (!Q3Initialized) {
            Q3Initialized = true;

            buildingBlockerA.transform.position += new Vector3(0, -100, 0);
        }

        if (buildings.Contains("BarracksTutorial")) {
            return true;
        }

        return false;
    }

    private bool DestroyEnemyHQ() {
        if (!Q12Initialized) {
            Q12Initialized = true;

            enemyHQ.gameObject.SetActive(true);
            enemySpawner1.gameObject.SetActive(true);
            enemySpawner2.gameObject.SetActive(true);
        }

        if(enemyHQ == null) {
            Invoke("GetComponent<GameManager>().WinGame()", 0.5f);
            return true;
        }

        return false;
    }

    private bool BuildLaserTower() {
        if (!Q11Initialized) {
            Q11Initialized = true;

            buildingBlockerB.transform.position += new Vector3(0, -100, 0);
        }

        if (buildings.Contains("LaserTower")) {
            return true;
        }

        return false;
    }

    private bool BuildRadar() {
        if (!Q10Initialized) {
            Q10Initialized = true;

            buildingBlockerA.transform.position += new Vector3(0, -100, 0);
        }

        if (buildings.Contains("Radar")) {
            return true;
        }

        return false;
    }

    private bool BuildTank() {
        if (buildings.Contains("CubeTank") || buildings.Contains("CubeTankAntiTank")) {
            return true;
        }

        return false;
    }

    private bool SelectFactory() {
        foreach (SelectableComponent selected in selectedObjects) {
            if (selected.transform.name == "FactoryTutorial(Clone)") {
                return true;

            }
        }

        return false;
    }

    private bool BuildFactory() {
        if (!Q6Initialized) {
            Q6Initialized = true;

            buildingBlockerB.transform.position += new Vector3(0, -100, 0);
        }

        if (buildings.Contains("FactoryTutorial")) {
            return true;
        }

        return false;
    }

    private bool TrainInfantryUnit() {
        if (buildings.Contains("CubeInfantry") || buildings.Contains("CubeInfantryAntiTank")) {
            return true;
        }

        return false;
    }

    private bool SelectBarracks() {
        foreach(SelectableComponent selected in selectedObjects) {
            if(selected.transform.name == "BarracksTutorial(Clone)") {
                return true;

            }
        }

        return false;
    }

    private bool Familiarize() {
        return familiarized;
    }

    public void SetFamiliarized(bool f) {
        familiarized = f;
    }
}
