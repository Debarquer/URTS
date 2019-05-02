using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialInputManager : InputManager
{
    public GameObject questTogglePrefab;
    public Transform questToggleContainer;

    public QuestToggle currentQuest;
    public Queue<QuestToggle> questToggles = new Queue<QuestToggle>();

    #region Quest 1
    bool familiarized = false;
    #endregion

    #region Quest 3
    public List<AttackableComponent> Q3EnemyUnits;
    #endregion

    #region Quest 4
    Vector3 Q4cameraStartPos = Vector3.zero;
    float CameraMoveDistance = 10;
    #endregion

    #region Quest 8
    public MyObject enemySpawner;
    #endregion

    #region Quest 9
    public MyObject enemyHQ;
    public List<MyObject> enemyHQSpawners;
    #endregion

    protected override void Start() {
        base.Start();

        QuestToggle[] questTogglesInChildren = questToggleContainer.GetComponentsInChildren<QuestToggle>();
        for(int i = 0; i < questTogglesInChildren.Length; i++){
            switch (i) {
                case 0:
                    questTogglesInChildren[i].functionToBeCalled = Familiarize;
                    break;
                case 1:
                    questTogglesInChildren[i].functionToBeCalled = MoveCameraWASD;
                    break;
                case 2:
                    questTogglesInChildren[i].functionToBeCalled = MoveCameraScreenEdge;
                    break;
                case 3:
                    questTogglesInChildren[i].functionToBeCalled = MoveCameraMiddleMouseButton;
                    break;
                case 4:
                    questTogglesInChildren[i].functionToBeCalled = ZoomCamera;
                    break;
                case 5:
                    questTogglesInChildren[i].functionToBeCalled = SelectUnit;
                    break;
                case 6:
                    questTogglesInChildren[i].functionToBeCalled = SelectUnits;
                    break;
                case 7:
                    questTogglesInChildren[i].functionToBeCalled = MoveUnit;
                    break;
                case 8:
                    questTogglesInChildren[i].functionToBeCalled = AttackUnitsToTheNorth;
                    break;
                case 9:
                    questTogglesInChildren[i].functionToBeCalled = DestroySpawner;
                    break;
                case 10:
                    questTogglesInChildren[i].functionToBeCalled = DestroyEnemyHQ;
                    break;
                default:
                    break;
            }

            questToggles.Enqueue(questTogglesInChildren[i]);
            questTogglesInChildren[i].gameObject.SetActive(false);
        }
    }

    private bool Familiarize() {
        return familiarized;
    }

    public void SetFamiliarized(bool f) {
        familiarized = f;
    }

    private bool DestroyEnemyHQ() {
        if (enemyHQ == null) {
            return true;
        }

        if (!enemyHQ.gameObject.activeSelf) {
            enemyHQ.gameObject.SetActive(true);
            foreach(MyObject myObject in enemyHQSpawners) {
                myObject.gameObject.SetActive(true);
            }
        }

        HandleCameraMovement();
        HandleStandardInput();

        return false;
    }

    private bool DestroySpawner() {
        if (enemySpawner == null) {
            return true;
        }

        if (!enemySpawner.gameObject.activeSelf) {
            enemySpawner.gameObject.SetActive(true);
        }

        HandleCameraMovement();
        HandleStandardInput();

        return false;
    }

    private bool ZoomCamera() {
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > 0.01f) {
            Update_ScrollZoom();
            return true;
        }

        return false;
    }

    private bool MoveCameraMiddleMouseButton() {
        if (Input.GetMouseButton(2)) {
            Update_CameraDrag();
        }

        if (Input.GetMouseButtonUp(2)) {
            return true;
        }
        else {
            return false;
        }
    }

    private bool MoveCameraScreenEdge() {
        if (Q4cameraStartPos == Vector3.zero) {
            Q4cameraStartPos = mainCamera.transform.position;
        }
        Vector3 mouseTranslateMove = Vector3.zero;

        if (Input.mousePosition.x >= cameraTranslateXMax) {
            mouseTranslateMove.x = Input.mousePosition.x;
            mainCamera.transform.Translate(new Vector3(1, 0, 0) * cameraTranslateSpeed * Time.deltaTime);
            //mouseTranslateMove.x = 0;
        }
        if (Input.mousePosition.x <= cameraTranslateXMin) {
            mouseTranslateMove.x = Input.mousePosition.x;
            mainCamera.transform.Translate(new Vector3(-1, 0, 0) * cameraTranslateSpeed * Time.deltaTime);
            //mouseTranslateMove.x = 0;
        }
        if (Input.mousePosition.y >= cameraTranslateYMax) {
            mouseTranslateMove.y = Input.mousePosition.y;
            mainCamera.transform.Translate(new Vector3(0, 1, 0) * cameraTranslateSpeed * Time.deltaTime);
            //mouseTranslateMove.y = 0;
        }
        if (Input.mousePosition.y <= cameraTranslateYMin) {
            mouseTranslateMove.y = Input.mousePosition.y;
            mainCamera.transform.Translate(new Vector3(0, -1, 0) * cameraTranslateSpeed * Time.deltaTime);
            //mouseTranslateMove.y = 0;
        }

        if (mouseTranslateMove != Vector3.zero) {
            if (Vector3.Distance(mainCamera.transform.position, Q4cameraStartPos) > CameraMoveDistance) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    private bool MoveCameraWASD() {
        if(Q4cameraStartPos == Vector3.zero) {
            Q4cameraStartPos = mainCamera.transform.position;
        }
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        mainCamera.transform.Translate(new Vector3(x, z, 0) * cameraTranslateSpeed * Time.deltaTime);

        if(x != 0 || z != 0) {
            if(Vector3.Distance(mainCamera.transform.position, Q4cameraStartPos) > CameraMoveDistance) {
                Q4cameraStartPos = Vector3.zero;
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    private void InstantiateNewQuestToggle(string label) {
        GameObject go = Instantiate(questTogglePrefab, questToggleContainer);
        QuestToggle questToggle = go.GetComponent<QuestToggle>();
        questToggle.text.text = label;
        questToggle.checkmark.SetActive(false);

        if(currentQuest == null) {
            currentQuest = questToggle;
        }
        else {
            questToggles.Enqueue(questToggle);
        }
    }

    protected override void Update()
    {
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
        if(currentQuest == null) {
            HandleCameraMovement();
            HandleStandardInput();

            FindObjectOfType<GameManager>().WinGame();

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

                FindObjectOfType<GameManager>().WinGame();
            }
        }
    }

    bool SelectUnit() {

        HandleCameraMovement();
        HandleStandardInput();

        if (Input.GetMouseButtonDown(0)) {
            StartDrag();
        }
        if (Input.GetMouseButton(0)) {
            dragTimer += Time.deltaTime;

            RaycastHit hit = RaycastToGround();

            dragEndPos = hit.point;

            Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
            Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), Mathf.Abs((dragStartPos - midPoint).y), Mathf.Abs((dragStartPos - midPoint).z)) * 2;

            selectionImage.transform.position = midPoint;
            selectionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, extents.x);
            selectionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, extents.z);
        }
        if (Input.GetMouseButtonUp(0)) {
            if (dragTimer < 0.1f) {
                selectionImage.enabled = false;

                foreach (SelectableComponent selectedObject in selectedObjects) {
                    if (selectedObject != null) {
                        //selectedObject.GetComponentInChildren<Renderer>().material.color = Color.white;
                        selectedObject.Deselect();
                    }
                }
                selectedObjects.Clear();
                dragTimer = 0;

                ClickableComponent[] clickableComponents = RaycastForClickableComponents();
                if (clickableComponents != null && clickableComponents.Length > 0)
                    ClickableComponentManager.instance.UnclickAllClickableComponents();
                foreach (ClickableComponent clickableComponent in clickableComponents) {
                    clickableComponent.Click();
                }
            }
            dragTimer = 0;

            EndDrag();

            if (selectedObjects != null && selectedObjects.Count == 1)
                return true;
            else
                return false;
        }

        return false;
    }

    bool SelectUnits() {

        HandleCameraMovement();
        HandleStandardInput();

        if (Input.GetMouseButtonDown(0)) {
            StartDrag();
        }
        if (Input.GetMouseButton(0)) {
            dragTimer += Time.deltaTime;

            RaycastHit hit = RaycastToGround();

            dragEndPos = hit.point;

            Vector3 midPoint = (dragStartPos + dragEndPos) / 2;
            Vector3 extents = new Vector3(Mathf.Abs((dragStartPos - midPoint).x), Mathf.Abs((dragStartPos - midPoint).y), Mathf.Abs((dragStartPos - midPoint).z)) * 2;

            selectionImage.transform.position = midPoint;
            selectionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, extents.x);
            selectionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, extents.z);
        }
        if (Input.GetMouseButtonUp(0)) {
            if (dragTimer < 0.1f) {
                selectionImage.enabled = false;

                foreach (SelectableComponent selectedObject in selectedObjects) {
                    if (selectedObject != null) {
                        //selectedObject.GetComponentInChildren<Renderer>().material.color = Color.white;
                        selectedObject.Deselect();
                    }
                }
                selectedObjects.Clear();
                dragTimer = 0;

                ClickableComponent[] clickableComponents = RaycastForClickableComponents();
                if (clickableComponents != null && clickableComponents.Length > 0)
                    ClickableComponentManager.instance.UnclickAllClickableComponents();
                foreach (ClickableComponent clickableComponent in clickableComponents) {
                    clickableComponent.Click();
                }
            }
            dragTimer = 0;

            EndDrag();

            if (selectedObjects != null && selectedObjects.Count > 1)
                return true;
            else
                return false;
        }

        return false;
    }

    bool MoveUnit() {

        HandleCameraMovement();
        HandleStandardInput();

        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit = RaycastToGround();

            dragStartPos = hit.point;

            foreach (SelectableComponent selectedObject in selectedObjects) {
                MoveComponent mc = selectedObject.GetComponent<MoveComponent>();
                if (mc != null) {
                    //mc.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hit.point;
                    mc.SetAgentDestination(hit.point);
                }
            }

            return true;
        }

        return false;
    }

    bool AttackUnitsToTheNorth() {

        HandleCameraMovement();
        HandleStandardInput();

        List<AttackableComponent> attackableComponents = new List<AttackableComponent>();
        foreach(AttackableComponent attackableComponent in Q3EnemyUnits) {
            if(attackableComponent == null) {
                attackableComponents.Add(attackableComponent);
            }
        }

        foreach(AttackableComponent attackableComponent in attackableComponents) {
            Q3EnemyUnits.Remove(attackableComponent);
        }

        if(Q3EnemyUnits.Count <= 0) {
            Q3EnemyUnits.Clear();
            return true;
        }
        else {
            return false;
        }
    }
}
