using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


class SpawnQueueItemTutorial {
    public TutorialInputManager2 TutorialInputManager2;

    public Image UICooldownImage;
    public Text queueLength;
    public Text costText;
    public MyObject thingToSpawn;
    protected SpawnComponentTutorial spawnComponent;

    GameManager gameManager;

    int nrOfQueue;

    public SpawnQueueItemTutorial(Image image, Text text, MyObject myObject, SpawnComponentTutorial spawnComponent, Text costText) {
        UICooldownImage = image;
        queueLength = text;
        thingToSpawn = myObject;
        this.spawnComponent = spawnComponent;
        this.costText = costText;
        nrOfQueue = 0;
        gameManager = GameObject.FindObjectOfType<GameManager>();

        TutorialInputManager2 = GameObject.FindObjectOfType<TutorialInputManager2>();
    }

    public void EnqueuePrefab() {
        if (
        gameManager.GetMinerals() >= thingToSpawn.cost &&
        (spawnComponent.canQueue ||
        (!spawnComponent.spawnQueue.Contains(this) && spawnComponent.objectToSpawn != thingToSpawn))) {

            gameManager.UpdateMinerals(-thingToSpawn.cost);
            spawnComponent.spawnQueue.Enqueue(this);
            nrOfQueue++;

            queueLength.text = (Mathf.Max(nrOfQueue, 0)).ToString();
        }
        else if (spawnComponent.spawntimerCurr >= spawnComponent.spawntimerMax) {
            // We are not allowed to queue duplicates and we are already in the queue
            // Check if the building is ready to be placed
            // If so, initiate building placement mode


            GameObject.FindObjectOfType<InputManager>().InitiateBuildingPlacement(thingToSpawn);
            GameObject.FindObjectOfType<InputManager>().OnBuildingPlacedSuccessfully += CompleteQueueItem;
        }
    }

    public void Dequeue() {
        nrOfQueue--;
        queueLength.text = (Mathf.Max(nrOfQueue, 0)).ToString();
    }

    public void CompleteQueueItem() {
        GameObject.FindObjectOfType<InputManager>().OnBuildingPlacedSuccessfully -= CompleteQueueItem;
        UICooldownImage.rectTransform.sizeDelta = new Vector2(UICooldownImage.rectTransform.sizeDelta.x, 0);
        spawnComponent.ContinueQueue();

        TutorialInputManager2.buildings.Add(thingToSpawn.name);
    }

    public void Cancel() {
        gameManager.UpdateMinerals(thingToSpawn.cost);
        CompleteQueueItem();
    }
}


class SpawnComponentTutorial : MonoBehaviour {
    [HideInInspector] public float spawntimerMax = 5f;
    [HideInInspector] public float spawntimerCurr = 0f;
    protected Image UICoolDownImage;
    protected float spriteMaxSize = 100;

    public List<InfantryUnit> UnitPrefabSO;
    public Dictionary<InfantryUnit, SpawnQueueItemTutorial> infantryUnitToSpawnQueueItem = new Dictionary<InfantryUnit, SpawnQueueItemTutorial>();
    [HideInInspector] public MyObject objectToSpawn = null;
    public Queue<SpawnQueueItemTutorial> spawnQueue = new Queue<SpawnQueueItemTutorial>();
    public SpawnQueueItemTutorial currentSpawnQueueItem;

    protected Text queueLength;

    public Transform buildingContainer;

    public bool manuallyPlaced = false;
    public bool canQueue = true;

    public Canvas canvas;

    public Transform spawnPoint;
    public Transform waypointLocation;

    virtual protected void Start() {

        foreach (InfantryUnit iu in UnitPrefabSO) {
            GameObject tmpGO = Instantiate(iu.UIGameObjectPrefab, buildingContainer);

            Image imageTmp = null;
            Text queueText = null;
            Text costTxt = null;

            for (int i = 0; i < tmpGO.transform.childCount; i++) {
                if (tmpGO.transform.GetChild(i).name == "UICooldownImage") {
                    imageTmp = tmpGO.transform.GetChild(i).GetComponent<Image>();
                }
                else if (tmpGO.transform.GetChild(i).name == "QueueLength") {
                    queueText = tmpGO.transform.GetChild(i).GetComponent<Text>();
                }
                else if (tmpGO.transform.GetChild(i).name == "MainText") {
                    tmpGO.transform.GetChild(i).GetComponent<Text>().text = iu.name;
                }
                else if (tmpGO.transform.GetChild(i).name == "CostText") {
                    costTxt = tmpGO.transform.GetChild(i).GetComponent<Text>();
                    costTxt.text = iu.MyObject.cost.ToString();
                }
            }

            infantryUnitToSpawnQueueItem[iu] = new SpawnQueueItemTutorial(imageTmp, queueText, iu.MyObject, this, costTxt);

            Button tmpButton = tmpGO.GetComponentInChildren<Button>();
            tmpButton.onClick.AddListener(delegate { infantryUnitToSpawnQueueItem[iu].EnqueuePrefab(); });
        }

        ClickableComponent clickableComponent = GetComponent<ClickableComponent>();
        if (clickableComponent != null) {
            clickableComponent.OnClick += ToggleBuildingUI;
            clickableComponent.OnUnClick += DisableBuildingUI;
        }
        else {
            Debug.LogWarning("SpawnComponent warning: No clickableComponent found", this);
        }
        SelectableComponent selectableComponent = GetComponent<SelectableComponent>();
        if (selectableComponent != null) {
            selectableComponent.OnSelect += ToggleBuildingUI;
            selectableComponent.OnDeselect += DisableBuildingUI;
        }
        else {
            Debug.LogWarning("SpawnComponent warning: No selectableComponent found", this);
        }
    }

    protected void Update() {
        if (spawnQueue.Count > 0 || objectToSpawn != null) {
            if (objectToSpawn == null) {
                currentSpawnQueueItem = spawnQueue.Dequeue();
                objectToSpawn = currentSpawnQueueItem.thingToSpawn;
                UICoolDownImage = currentSpawnQueueItem.UICooldownImage;
                queueLength = currentSpawnQueueItem.queueLength;
                currentSpawnQueueItem.Dequeue();
            }
            spawntimerCurr += Time.deltaTime;

            if (spawntimerCurr > spawntimerMax) {
                spawntimerCurr = spawntimerMax;
                if (!manuallyPlaced) {
                    spawntimerCurr = 0;
                    UICoolDownImage.rectTransform.sizeDelta = new Vector2(UICoolDownImage.rectTransform.sizeDelta.x, 0);

                    MyObject tmp = Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);

                    currentSpawnQueueItem.TutorialInputManager2.buildings.Add(currentSpawnQueueItem.thingToSpawn.name);

                    tmp.Activate();
                    objectToSpawn = null;
                    currentSpawnQueueItem = null;
                    tmp.team = GetComponent<MyObject>().team;
                    tmp.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = waypointLocation.position;

                }
            }

            float spriteSizeFrac = (spawntimerCurr / spawntimerMax);
            UICoolDownImage.rectTransform.sizeDelta = new Vector2(UICoolDownImage.rectTransform.sizeDelta.x, spriteMaxSize * spriteSizeFrac);
        }
        else {
            spawntimerCurr = 0;
        }
    }

    public void ContinueQueue() {
        spawntimerCurr = 0;
        objectToSpawn = null;
        currentSpawnQueueItem = null;
    }

    protected void ToggleBuildingUI() {
        canvas.enabled = !canvas.enabled;
    }

    protected void DisableBuildingUI() {
        canvas.enabled = false;
    }
}