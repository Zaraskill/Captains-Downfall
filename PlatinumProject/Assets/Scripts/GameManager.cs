using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

// Code crée et géré par Corentin
public class GameManager : MonoBehaviour
{
    public static GameManager managerGame;

    enum STATE_PLAY { PlayerSelection, PrepareParty, PrepareSuddenDeath, Party, Results, DisplayResultsRound, DisplayResultsFinal}

    private STATE_PLAY gameState;

    //Spawn Objects
    [Header("Spawn Objects")]
    public float maxObjectsInGame;
    public GameObject[] listPrefabsPickableItems;
    public GameObject spawnZone;
    public Transform[] arrayItemsSpawnPoints;
    private int randomObject;

    //Gestion Map
    private BreakableWalls[] listWalls;
    private Barrel[] listBarrels;

    //Gestion players
    [Header("Players")]    
    public List<PlayerEntity> listPlayers;
    public List<GameObject> listSpawnPoint;
    private List<PlayerEntity> listAlivePlayers;
    private Player mainPlayer;
    private List<int> listPointsPlayers;
    private List<int> listWinnerPlayers;
    public int nbPlayersAlive;
    private int idPlayerwinner;
    private bool isTeam = false;
    private bool isSuddenDeath = false;
    private List<PlayerEntity> teamOne;
    private List<PlayerEntity> teamTwo;

    //Affichage résultats
    [Header("Affichage Résultats")]
    public Canvas displayResults;
    public Text displayWinner;
    public Text displayPointsRounds;
    private float startTimer = 3f;
    private float timer;
    

    void Awake()
    {
        if (managerGame != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            managerGame = this;
            DontDestroyOnLoad(this.gameObject);
        }
        listPointsPlayers = new List<int>()
        {
            0,0,0,0
        };
        listWalls = FindObjectsOfType<BreakableWalls>();
        listBarrels = FindObjectsOfType<Barrel>();
        listWinnerPlayers = new List<int>();
        teamOne = new List<PlayerEntity>();
        teamTwo = new List<PlayerEntity>();
        mainPlayer = ReInput.players.GetPlayer(0);
        timer = startTimer;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = STATE_PLAY.PrepareParty;
        for (int i = 0; i < listPlayers.Count; i++)
        {
            listPlayers[i].GetComponentInChildren<TextMesh>().text = "J" + (i + 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case STATE_PLAY.PlayerSelection:
                Debug.Log(gameState);
                break;
            case STATE_PLAY.PrepareParty:
                Debug.Log(gameState);
                PrepareParty();
                break;
            case STATE_PLAY.PrepareSuddenDeath:
                Debug.Log(gameState);
                PrepareSuddenDeath();
                break;
            case STATE_PLAY.Party:
                Debug.Log(gameState);
                if (nbPlayersAlive == 2 && isTeam)
                {
                    if (teamOne.Count == 0)
                    {
                        idPlayerwinner = 5;
                        Debug.Log(idPlayerwinner);
                        gameState = STATE_PLAY.Results;
                    }
                    else if (teamTwo.Count == 0)
                    {
                        idPlayerwinner = 4;
                        Debug.Log(idPlayerwinner);
                        gameState = STATE_PLAY.Results;
                    }
                }
                else if (nbPlayersAlive == 1)
                {                    
                    idPlayerwinner = listAlivePlayers[0].playerID;
                    Debug.Log(idPlayerwinner);
                    gameState = STATE_PLAY.Results;      
                }
                break;
            case STATE_PLAY.Results:
                Debug.Log(gameState);
                UpdatePoints();
                UIManager.managerUI.DisplayRoundEnding(idPlayerwinner);
                CheckWinner();
                break;
            case STATE_PLAY.DisplayResultsRound:
                Debug.Log(gameState);
                WaitingForInput();                
                break;
            case STATE_PLAY.DisplayResultsFinal:
                Debug.Log(gameState);
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
        int randomSpawnPoints = Random.Range(0, arrayItemsSpawnPoints.Length);
        Instantiate(listPrefabsPickableItems[randomObject], arrayItemsSpawnPoints[randomSpawnPoints].transform.position, Quaternion.identity);
    }

    private void GenerateObjects()
    {
        randomObject = Random.Range(0, listPrefabsPickableItems.Length);
        for (int i = 0; i < maxObjectsInGame; i++)
        {
            SpawnObject();
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
            playerID.gameObject.GetComponentInChildren<TextMesh>().color = Color.blue;
            listTemp.Remove(playerID);
            //playerID.body.color = Color.blue;
        }
        foreach (PlayerEntity player in listTemp)
        {
            player.teamID = 2;
            player.gameObject.GetComponentInChildren<TextMesh>().color = Color.red;
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
        if (listWinnerPlayers.Count == 1)
        {
            gameState = STATE_PLAY.DisplayResultsFinal;
        }
        else if (listWinnerPlayers.Count >= 2)
        {

        }
        else
        {
            gameState = STATE_PLAY.DisplayResultsRound;
        }
        idPlayerwinner++;
    }

    private void RespawnPlayers()
    {
        foreach (PlayerEntity player in listPlayers)
        {
            player.Respawn();
            if (player.playerID == 0)
            {
                player.gameObject.GetComponentInChildren<TextMesh>().color = Color.blue;
            }
            else if ( player.playerID == 1)
            {
                player.gameObject.GetComponentInChildren<TextMesh>().color = Color.red;
            }
            else if (player.playerID == 2)
            {
                player.gameObject.GetComponentInChildren<TextMesh>().color = Color.green;
            }
            else if (player.playerID == 3)
            {
                player.gameObject.GetComponentInChildren<TextMesh>().color = Color.yellow;
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

    private void PrepareParty()
    {
        if (timer <= 0f)
        {
            UIManager.managerUI.StartRound();
            gameState = STATE_PLAY.Party;
            timer = startTimer;
        }
        else if (timer < startTimer)
        {
            timer -= Time.deltaTime;
        }
        else
        {
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
        }                
    }

    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(6);
    }

    private void PrepareSuddenDeath()
    {
        DestroyTeam();
        ClearMap();
        listAlivePlayers.Clear();
        idPlayerwinner = -1;
        foreach (int index in listWinnerPlayers)
        {
            listAlivePlayers.Add(listPlayers[index]);
        }
        nbPlayersAlive = listWinnerPlayers.Count;
        listWinnerPlayers.Clear();
        isSuddenDeath = true;
        GenerateObjects();
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
        foreach (Barrel barrel in listBarrels)
        {
            barrel.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Input Fonctions

    private void WaitingForInput()
    {
        if (mainPlayer.GetButtonDown("UISubmit"))
        {
            if (listWinnerPlayers.Count == 1)
            {
                gameState = STATE_PLAY.DisplayResultsFinal;
            }
            else if (listWinnerPlayers.Count > 1)
            {
                UIManager.managerUI.EndRound();
                gameState = STATE_PLAY.PrepareSuddenDeath;
            }
            else
            {
                UIManager.managerUI.EndRound();
                gameState = STATE_PLAY.PrepareParty;
            }
        }
    }

    #endregion

}
