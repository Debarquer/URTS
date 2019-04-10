using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class SpawnQueueItem{
    public Image UICooldownImage;
    public Text queueLength;
    public MyObject thingToSpawn;
    SpawnComponent spawnComponent;

    GameManager gameManager;

    int nrOfQueue;

    public SpawnQueueItem(Image image, Text text, MyObject myObject, SpawnComponent spawnComponent) {
        UICooldownImage = image;
        queueLength = text;
        thingToSpawn = myObject;
        this.spawnComponent = spawnComponent;
        nrOfQueue = 0;
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void EnqueuePrefab(int a, float b, string c) {
        if (
        gameManager.GetMinerals() >= thingToSpawn.cost &&
        (spawnComponent.canQueue ||
        (!spawnComponent.spawnQueue.Contains(this) && spawnComponent.objectToSpawn != thingToSpawn))) {

            spawnComponent.spawnQueue.Enqueue(this);
            nrOfQueue++;

            queueLength.text = (Mathf.Max(nrOfQueue, 0)).ToString();
        }
        else if(spawnComponent.spawntimerCurr >= spawnComponent.spawntimerMax) {
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
        gameManager.UpdateMinerals(-thingToSpawn.cost);
    }

    public void CompleteQueueItem() {
        GameObject.FindObjectOfType<InputManager>().OnBuildingPlacedSuccessfully -= CompleteQueueItem;
        UICooldownImage.rectTransform.sizeDelta = new Vector2(UICooldownImage.rectTransform.sizeDelta.x, 0);
        spawnComponent.ContinueQueue();
    }

    public void Cancel() {
        gameManager.UpdateMinerals(thingToSpawn.cost);
        CompleteQueueItem();
    }
}

class SpawnComponent : MonoBehaviour {
    [HideInInspector] public float spawntimerMax = 5f;
    [HideInInspector] public float spawntimerCurr = 0f;
    Image UICoolDownImage;
    float spriteMaxSize = 200;

    public List<InfantryUnit> UnitPrefabSO;
    public Dictionary<InfantryUnit, SpawnQueueItem> infantryUnitToSpawnQueueItem = new Dictionary<InfantryUnit, SpawnQueueItem>();
    [HideInInspector] public MyObject objectToSpawn = null;
    public Queue<SpawnQueueItem> spawnQueue = new Queue<SpawnQueueItem>();
    public SpawnQueueItem currentSpawnQueueItem;

    Text queueLength;

    public Transform buildingContainer;

    string testString = "HJASFNJDAF";

    public bool manuallyPlaced = false;
    public bool canQueue = true;

    public Canvas canvas;

    private void Start() {

        foreach(InfantryUnit iu in UnitPrefabSO) {
            GameObject tmpGO = Instantiate(iu.UIGameObjectPrefab, buildingContainer);

            Image imageTmp = null;
            Text textTmp = null;

            for (int i = 0; i < tmpGO.transform.childCount; i++) {
                if (tmpGO.transform.GetChild(i).name == "UICooldownImage") {
                    imageTmp = tmpGO.transform.GetChild(i).GetComponent<Image>();
                }
                else if (tmpGO.transform.GetChild(i).name == "QueueLength") {
                    textTmp = tmpGO.transform.GetChild(i).GetComponent<Text>();
                }
                else if (tmpGO.transform.GetChild(i).name == "MainText") {
                    tmpGO.transform.GetChild(i).GetComponent<Text>().text = iu.name;
                } 
            }

            infantryUnitToSpawnQueueItem[iu] = new SpawnQueueItem(imageTmp, textTmp, iu.MyObject, this);

            Button tmpButton = tmpGO.GetComponentInChildren<Button>();
            tmpButton.onClick.AddListener(delegate { infantryUnitToSpawnQueueItem[iu].EnqueuePrefab(5, 5, testString); });
        }

        ClickableComponent clickableComponent = GetComponent<ClickableComponent>();
        if(clickableComponent != null) {
            clickableComponent.OnClick += ToggleBuildingUI;
            clickableComponent.OnUnClick += DisableBuildingUI;
        }
    }

    private void Update() {
        if(spawnQueue.Count > 0 || objectToSpawn != null) {
            if(objectToSpawn == null) {
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

                    MyObject tmp = Instantiate(objectToSpawn, transform.position + UnityEngine.Random.insideUnitSphere, Quaternion.identity);
                    objectToSpawn = null;
                    currentSpawnQueueItem = null;
                    tmp.team = GetComponent<MyObject>().team;
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

    private void ToggleBuildingUI() {
        canvas.enabled = !canvas.enabled;
    }

    private void DisableBuildingUI() {
        canvas.enabled = false;
    }
}