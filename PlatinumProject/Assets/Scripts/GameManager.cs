using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

// Code crée et géré par Corentin
public class GameManager : MonoBehaviour
{
    public static GameManager managerGame;

    public enum STATE_PLAY {MainMenu, Credits, TutoMove, TutoObjects, PlayerSelection, PrepareFirstParty, PrepareParty, Party, EndParty, Results, DisplayResultsRound, DisplayResultsFinal}

    private STATE_PLAY gameState;
    public STATE_PLAY startState;

    //Spawn Objects
    [Header("Spawn Objects")]
    public float maxObjectsInGame;
    public GameObject[] listPrefabsPickableItems;
    public GameObject spawnZone;
    public Transform[] arrayItemsSpawnPoints;
    private int randomObject;

    //Spawn Barrel
    [Header("Spawn Barrels")]
    public int maxBarrelsInGame;
    public Transform[] arrayBarrelsSpawnPoints;
    public GameObject barrelPrefab;
    public float timerSpawnFx;
    public float maxTimerSpawnFx;
    public GameObject redZone;
    public GameObject poofAppears;
    private List<Transform> usedBarrelSpawnPoints = new List<Transform>();

    //Gestion Map
    private BreakableWalls[] listWalls;
    private Barrel[] listBarrels;
    private bool isWaitingForInput = false;

    //Gestion players
    [Header("Players")]    
    public List<PlayerEntity> listPlayers;
    public List<GameObject> listSpawnPoint;
    private List<PlayerEntity> listAlivePlayers;
    private List<int> listPointsPlayers;
    private List<int> listWinnerPlayers;
    public int nbPlayersAlive;
    private int idPlayerwinner;
    private bool isTeam = false;
    private bool isSuddenDeath = false;
    private List<PlayerEntity> teamOne;
    private List<PlayerEntity> teamTwo;
    public List<GameObject> listPointForCamera;

    //Gestion controllers
    private IList<Joystick> listControllers;

    //Gestion Endround
    private float timer;
    public float timerStart = 3f;

    private Animator animCredits;

    public float timeBtwTutos;

    private bool isCoroutineDone = false;

    void Awake()
    {
        if (managerGame != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            managerGame = this;
        }       
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = startState;
        listControllers = ReInput.controllers.Joysticks;
        animCredits = UIManager.managerUI.displayCredits.GetComponent<Animator>();
        timer = timerStart;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameState);
        switch (gameState)
        {
            case STATE_PLAY.MainMenu:
                break;
            case STATE_PLAY.Credits:
                WaitingForInput();
                break;
            case STATE_PLAY.TutoMove:
                WaitingForInput();
                break;
            case STATE_PLAY.TutoObjects:
                WaitingForInput();
                break;
            case STATE_PLAY.PlayerSelection:
                Debug.Log(gameState);
                break;
            case STATE_PLAY.PrepareFirstParty:
                PrepareFirstParty();
                break;
            case STATE_PLAY.PrepareParty:
                Debug.Log(gameState);
                if (!isWaitingForInput)
                {
                    PrepareParty();
                }
                WaitingForInput();                
                break;
            case STATE_PLAY.Party:
                Debug.Log(gameState);
                if (nbPlayersAlive == 2 && isTeam)
                {
                    if (teamOne.Count == 0)
                    {
                        idPlayerwinner = 5;
                        Debug.Log(idPlayerwinner);
                        Time.timeScale = 0.5f;
                        gameState = STATE_PLAY.EndParty;
                    }
                    else if (teamTwo.Count == 0)
                    {
                        idPlayerwinner = 4;
                        Debug.Log(idPlayerwinner);
                        Time.timeScale = 0.5f;
                        gameState = STATE_PLAY.EndParty;
                    }
                }
                else if (nbPlayersAlive == 1)
                {                    
                    idPlayerwinner = listAlivePlayers[0].playerID;
                    CameraManager.managerCamera.GoToPlayer(listPointForCamera[listAlivePlayers[0].playerID]);
                    Debug.Log(idPlayerwinner);
                    Time.timeScale = 0.5f;
                    gameState = STATE_PLAY.EndParty;      
                }
                break;
            case STATE_PLAY.EndParty:
                if (timer <= 0)
                {
                    Time.timeScale = 1f;
                    timer = timerStart;
                    gameState = STATE_PLAY.Results;
                }
                else
                {
                    timer -= Time.unscaledDeltaTime;
                }
                break;
            case STATE_PLAY.Results:
                Debug.Log(gameState);
                UpdatePoints();                
                CheckWinner();
                break;
            case STATE_PLAY.DisplayResultsRound:
                if (!isWaitingForInput)
                {
                    UIManager.managerUI.DisplayRoundEnding(idPlayerwinner);
                    Debug.Log(gameState);
                }            
                WaitingForInput();                
                break;
            case STATE_PLAY.DisplayResultsFinal:
                Debug.Log(gameState);
                if (!isWaitingForInput)
                {
                    UIManager.managerUI.DisplayPodiumWinner(listWinnerPlayers);
                }
                WaitingForInput();
                break;
            default:
                Application.Quit();
                break;
        }
    }

    #region Objects Fonctions

    public void SpawnObject()
    {
        List<Transform> emptyItemsSpawnPoints = new List<Transform>();
        foreach(Transform spawnPoint in arrayItemsSpawnPoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(spawnPoint.transform.position, -Vector3.up, out hit) && !hit.collider.gameObject.CompareTag("Pickable"))
            {
                emptyItemsSpawnPoints.Add(spawnPoint);
            }
        }

        if(emptyItemsSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyItemsSpawnPoints.Count);
            Instantiate(listPrefabsPickableItems[randomObject], emptyItemsSpawnPoints[randomIndex].transform.position, Quaternion.identity);
        }
    }

    private void GenerateObjects()
    {
        randomObject = Random.Range(0, listPrefabsPickableItems.Length);
        for (int i = 0; i < maxObjectsInGame; i++)
        {
            SpawnObject();
        }
    }

    public void SpawnBarrel()
    {
        List<Transform> emptyBarrelSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in arrayBarrelsSpawnPoints)
        {
            if(!usedBarrelSpawnPoints.Contains(spawnPoint))
            {
                RaycastHit hit;
                if (Physics.Raycast(spawnPoint.transform.position, Vector3.up, out hit) && !hit.collider.gameObject.CompareTag("Barrel"))
                {
                    emptyBarrelSpawnPoints.Add(spawnPoint);
                }
            }
        }

        if (emptyBarrelSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyBarrelSpawnPoints.Count);
            StartCoroutine(InstantiateNewBarrel(1.5f, 3f, randomIndex, emptyBarrelSpawnPoints));
        }
    }

    IEnumerator InstantiateNewBarrel(float timeRedZone, float timeBarrel, int index, List<Transform> listSpawnPoints)
    {
        usedBarrelSpawnPoints.Add(listSpawnPoints[index].transform);
        yield return new WaitForSeconds(timeRedZone);
        GameObject zone = Instantiate(redZone, listSpawnPoints[index].transform.position, Quaternion.identity);
        yield return new WaitForSeconds(timeBarrel);
        GameObject _instance = Instantiate(barrelPrefab, listSpawnPoints[index].transform.position, Quaternion.identity);
        _instance.transform.Rotate(-90, 0, -60);
        Instantiate(poofAppears, listSpawnPoints[index].transform.position, Quaternion.identity);
        Destroy(zone);
        usedBarrelSpawnPoints.Remove(listSpawnPoints[index].transform);
    }

    private void GenerateBarrels()
    {
        for (int i = 0; i < maxBarrelsInGame; i++)
        {
            SpawnBarrel();
        }
    }

    private void ClearMap()
    {
        PickupableObject[] listObjectsinGame = FindObjectsOfType<PickupableObject>();
        foreach(PickupableObject objects in listObjectsinGame)
        {
            Destroy(objects.gameObject);
        }
    }

    #endregion

    #region Players Fonctions

    public void DeadPlayer(int playerID)
    {
        nbPlayersAlive--;
        foreach (PlayerEntity player in listAlivePlayers)
        {
            if (player == listPlayers[playerID])
            {
                listAlivePlayers.Remove(player);
                break;
            }
        }
        if(isTeam)
        {
            if (listPlayers[playerID].teamID == 1)
            {
                foreach (PlayerEntity player in teamOne)
                {
                    if (player == listPlayers[playerID])
                    {
                        teamOne.Remove(player);
                        break;
                    }
                }
            }
            else if (listPlayers[playerID].teamID == 2)
            {
                foreach (PlayerEntity player in teamTwo)
                {
                    if (player == listPlayers[playerID])
                    {
                        teamTwo.Remove(player);
                        break;
                    }
                }
            }
            listPlayers[playerID].teamID = 0;
        }
    }

    private void CreationTeam()
    {

        List<PlayerEntity> listTemp = new List<PlayerEntity>(listAlivePlayers);
        PlayerEntity playerID;
        int id;

        while (teamOne.Count < 2)
        {
            id = Random.Range(0, listTemp.Count);
            playerID = listTemp[id];
            teamOne.Add(playerID);
            playerID.teamID = 1;
            playerID.gameObject.GetComponentInChildren<Image>().color = Color.blue;
            listTemp.Remove(playerID);
            //playerID.body.color = Color.blue;
        }
        foreach (PlayerEntity player in listTemp)
        {
            player.teamID = 2;
            player.gameObject.GetComponentInChildren<Image>().color = Color.red;
            //player.body.color = Color.red;
            teamTwo.Add(player);
        }
    }

    private void DestroyTeam()
    {
        isTeam = false;
        foreach (PlayerEntity player in listPlayers)
        {
            player.teamID = 0;
        }
        teamOne.Clear();
        teamTwo.Clear();
    }

    private void UpdatePoints()
    {
        if (idPlayerwinner < 4)
        {
            listPointsPlayers[idPlayerwinner]++;
        }
        else if (idPlayerwinner == 4)
        {
            foreach (PlayerEntity player in teamOne)
            {
                listPointsPlayers[player.playerID]++;
            }
        }
        else
        {
            foreach (PlayerEntity player in teamTwo)
            {
                listPointsPlayers[player.playerID]++;
            }
        }
        listAlivePlayers.Clear();
        
    }

    private void CheckWinner()
    {
        for (int index = 0; index < listPointsPlayers.Count; index++)
        {
            if (listPointsPlayers[index] == 5)
            {
                listWinnerPlayers.Add(index);
            }
        }
        gameState = STATE_PLAY.DisplayResultsRound;
        idPlayerwinner++;
    }

    private void RespawnPlayers()
    {
        foreach (PlayerEntity player in listPlayers)
        {
            player.Respawn();
            if (player.playerID == 0)
            {
                player.gameObject.GetComponentInChildren<Image>().color = Color.blue;
            }
            else if ( player.playerID == 1)
            {
                player.gameObject.GetComponentInChildren<Image>().color = Color.red;
            }
            else if (player.playerID == 2)
            {
                player.gameObject.GetComponentInChildren<Image>().color = Color.green;
            }
            else if (player.playerID == 3)
            {
                player.gameObject.GetComponentInChildren<Image>().color = Color.yellow;
            }
        }
    }

    public List<PlayerEntity> GetTeamOne()
    {
        return teamOne;
    }

    public List<PlayerEntity> GetTeamTwo()
    {
        return teamTwo;
    }

    #endregion

    #region Events Fonctions

    private void PrepareFirstParty()
    {
        listPointsPlayers = new List<int>()
        {
            0,0,0,0
        };
        listWalls = FindObjectsOfType<BreakableWalls>();
        listBarrels = FindObjectsOfType<Barrel>();
        listWinnerPlayers = new List<int>();
        teamOne = new List<PlayerEntity>();
        teamTwo = new List<PlayerEntity>();
        listControllers = ReInput.controllers.Joysticks;
        CameraManager.managerCamera.ResetRound();
        gameState = STATE_PLAY.PrepareParty;
        for (int i = 0; i < listPlayers.Count; i++)
        {
            listPlayers[i].GetComponentInChildren<Image>().sprite = UIManager.managerUI.listNumberPlayer[i];
        }
    }

    private void PrepareParty()
    {
        if (isWaitingForInput)
        {
            return;
        }
            CameraManager.managerCamera.ResetRound();
            DestroyTeam();
            ClearMap();
            PrepareMap();
            listWinnerPlayers.Clear();
            idPlayerwinner = -1;
            RespawnPlayers();
            listAlivePlayers = new List<PlayerEntity>(listPlayers);
            nbPlayersAlive = 4;
            if (Random.Range(0, 2) == 1)
            {
                Debug.Log("Team match");
                isTeam = true;
                CreationTeam();
                UIManager.managerUI.DisplayRoundBeginning(2);
            }
            else
            {
                Debug.Log("FFA");
                isTeam = false;
                UIManager.managerUI.DisplayRoundBeginning(1);
            }
            GenerateObjects();
            //GenerateBarrels();              
    }

    public int GetPointsPlayers(int index)
    {
        return listPointsPlayers[index];
    }

    #endregion

    #region Map Fonctions

    private void PrepareMap()
    {
        foreach(BreakableWalls wall in listWalls)
        {
            wall.gameObject.SetActive(true);
            wall.Rebuilt();
        }
        for (int index = 0; index < listPlayers.Count; index++)
        {
            listPlayers[index].transform.position = listSpawnPoint[index].transform.position;
        }
        /*foreach (Barrel barrel in listBarrels)
        {
            barrel.gameObject.SetActive(true);
        }*/
    }

    #endregion

    #region Input Fonctions

    private void WaitingForInput()
    {
        if (!isWaitingForInput)
        {
            isWaitingForInput = true;
        }
        foreach (Joystick controller in listControllers)
        {
            if(gameState == STATE_PLAY.TutoMove && !isCoroutineDone)
            {
                StartCoroutine(TransitionBetweenTutos(UIManager.managerUI.pressAToContinue));
            }
            else if (gameState == STATE_PLAY.TutoObjects && !isCoroutineDone)
            {
                StartCoroutine(TransitionBetweenTutos(UIManager.managerUI.pressAToContinue));
            }
            else if (gameState == STATE_PLAY.PrepareParty && !isCoroutineDone)
            {
                StartCoroutine(TransitionBetweenTutos(UIManager.managerUI.pressAToContinue));
            }
            else if (gameState == STATE_PLAY.DisplayResultsRound && !isCoroutineDone)
            {
                StartCoroutine(TransitionBetweenTutos(UIManager.managerUI.pressAToContinue));
                for(int i = 0; i < listPlayers.Count; i++)
                {
                    listPlayers[i].ResetMovement();
                }
            }
            if (controller.GetButtonDown(0))
            {
                if (gameState == STATE_PLAY.MainMenu)
                {
                    isWaitingForInput = false;
                    UIManager.managerUI.mainMenuToDisplay.SetActive(false);
                    UIManager.managerUI.navBar.SetActive(false);
                    UIManager.managerUI.tutoUI.SetActive(true);
                    gameState = STATE_PLAY.TutoMove;
                }
                else if (gameState == STATE_PLAY.TutoMove && UIManager.managerUI.pressAToContinue.activeSelf)
                {
                    isWaitingForInput = false;
                    isCoroutineDone = false;
                    UIManager.managerUI.pressAToContinue.SetActive(false);
                    UIManager.managerUI.tutoMove.SetActive(false);
                    UIManager.managerUI.tutoObjects.SetActive(true);
                    gameState = STATE_PLAY.TutoObjects;
                }
                else if (gameState == STATE_PLAY.TutoObjects && UIManager.managerUI.pressAToContinue.activeSelf)
                {
                    isWaitingForInput = false;
                    isCoroutineDone = false;
                    UIManager.managerUI.pressAToContinue.SetActive(false);
                    SceneManager.LoadScene(1);
                }
                else if (gameState == STATE_PLAY.PrepareParty && UIManager.managerUI.pressAToContinue.activeSelf)
                {
                    isWaitingForInput = false;
                    isCoroutineDone = false;
                    UIManager.managerUI.pressAToContinue.SetActive(false);
                    UIManager.managerUI.StartRound();
                    gameState = STATE_PLAY.Party;
                }
                else if (gameState == STATE_PLAY.DisplayResultsRound && UIManager.managerUI.pressAToContinue.activeSelf)
                {
                    isWaitingForInput = false;
                    isCoroutineDone = false;
                    if (listWinnerPlayers.Count > 0)
                    {
                        gameState = STATE_PLAY.DisplayResultsFinal;
                    }
                    else
                    {
                        gameState = STATE_PLAY.PrepareParty;
                    }
                    UIManager.managerUI.EndRound();
                }
                else if (gameState == STATE_PLAY.DisplayResultsFinal)
                {
                    isWaitingForInput = false;
                    SceneManager.LoadScene(0);
                }
            }
            else if (controller.GetButtonDown(1))
            {
                if(gameState == STATE_PLAY.Credits)
                {
                    animCredits.SetBool("isDisplayed", false);
                    UIManager.managerUI.mainMenuToDisplay.SetActive(true);
                }
            }
        }        
    }

    #endregion

    #region Get/Set Fonctions

    public bool IsWaitingForInput()
    {
        return isWaitingForInput;
    }

    #endregion

    #region Click Buttons Fonctions

    public void OnClickPlay()
    {
        gameState = STATE_PLAY.TutoMove;
    }

    public void OnClickCredits()
    {
        gameState = STATE_PLAY.Credits;
        animCredits.SetBool("isDisplayed", true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    #endregion

    IEnumerator TransitionBetweenTutos(GameObject pressToContinue)
    {
        isCoroutineDone = true;
        pressToContinue.SetActive(false);
        yield return new WaitForSeconds(timeBtwTutos);
        pressToContinue.SetActive(true);
    }

    IEnumerator TransitionWinner()
    {
        yield return new WaitForSeconds(3);
        Time.timeScale = 1f;
    }
}
