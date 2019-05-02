using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpawnQueueItemTutorial {
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

        image.GetComponentInParent<SpawnUIButtonScript>().SpawnQueueItemTutorial = this;
    }

    public void EnqueuePrefab() {
        if (
        gameManager.GetMinerals() >= thingToSpawn.cost &&
        (spawnComponent.canQueue ||
        (!spawnComponent.spawnQueue.Contains(this) && spawnComponent.objectToSpawn != thingToSpawn))) {

            gameManager.UpdateMinerals(-thingToSpawn.cost);
            spawnComponent.spawnQueue.Enqueue(this);
            nrOfQueue++;

            queueLength.text = "Queue: " + (Mathf.Max(nrOfQueue, 0)).ToString();
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
        queueLength.text = "Queue: " + (Mathf.Max(nrOfQueue, 0)).ToString();
    }

    public void CompleteQueueItem() {
        GameObject.FindObjectOfType<InputManager>().OnBuildingPlacedSuccessfully -= CompleteQueueItem;
        UICooldownImage.rectTransform.sizeDelta = new Vector2(UICooldownImage.rectTransform.sizeDelta.x, 0);
        spawnComponent.ContinueQueue();

        TutorialInputManager2.buildings.Add(thingToSpawn.name);
    }

    public void Cancel() {
        if (spawnComponent.currentSpawnQueueItem == this) {
            gameManager.UpdateMinerals(thingToSpawn.cost);
            CompleteQueueItem();
        }
        else {
            if (spawnComponent.spawnQueue.Contains(this)) {
                //spawnComponent.spawnQueue.
                Queue<SpawnQueueItemTutorial> tmpqueue = new Queue<SpawnQueueItemTutorial>();

                bool hasCanceledOnce = false;
                while (spawnComponent.spawnQueue.Count > 0) {
                    SpawnQueueItemTutorial spawnQueueItem = spawnComponent.spawnQueue.Dequeue();
                    if (spawnQueueItem == this && !hasCanceledOnce) {
                        hasCanceledOnce = true;
                        spawnQueueItem.nrOfQueue--;
                        spawnQueueItem.queueLength.text = "Queue: " + nrOfQueue.ToString();
                        continue;
                    }
                    else {
                        tmpqueue.Enqueue(spawnQueueItem);
                    }
                }

                while (tmpqueue.Count > 0) {
                    spawnComponent.spawnQueue.Enqueue(tmpqueue.Dequeue());
                }
            }
        }
    }
}


public class SpawnComponentTutorial : MonoBehaviour {
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

    public FactoryDoor FactoryDoor;

    public AudioSource constructionComplete;
    bool hasPlayedAudio = false;

    virtual protected void Start() {

        foreach (InfantryUnit iu in UnitPrefabSO) {
            GameObject tmpGO = Instantiate(iu.UIGameObjectPrefab, buildingContainer);

            Image imageTmp = null;
            Text queueText = null;
            Text costTxt = null;

            //Debug.Log(tmpGO.transform.GetChild(1).name);
            for (int i = 0; i < tmpGO.transform.GetChild(1).childCount; i++) {

                Transform t = tmpGO.transform.GetChild(1).GetChild(i);

                //if (t.name == "UICooldownImage") {
                //     = t.GetComponent<Image>();
                //}
                if (t.name == "QueueLength") {
                    queueText = t.GetComponent<Text>();
                }
                else if (t.name == "MainText") {
                    t.GetComponent<Text>().text = iu.MyObject.myName;
                }
                else if (t.name == "CostText") {
                    costTxt = t.GetComponent<Text>();
                    costTxt.text = "Cost: " + iu.MyObject.cost.ToString();
                }
                else if (t.name == "PowerCost") {
                    t.GetComponent<Text>().text = "Power: " + (-iu.MyObject.powerReq).ToString();
                }
            }

            imageTmp = tmpGO.transform.GetChild(0).GetComponent<Image>();

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
                hasPlayedAudio = false;
                UICoolDownImage.color = new Color(1, 0, 0, (float)122 / 255);
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

                    if (FactoryDoor != null) {
                        FactoryDoor.OpenDoor();
                    }
                }
                else {
                    if (!hasPlayedAudio) {
                        hasPlayedAudio = true;
                        UICoolDownImage.color = new Color(0, 1, 0, (float)122 / 255);
                        constructionComplete.Play();
                        //UICoolDownImage.transform.parent.SetAsLastSibling();
                        iTween.PunchScale(UICoolDownImage.transform.parent.gameObject, new Vector3(2f, 2f, 2f), 1f);
                    }
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