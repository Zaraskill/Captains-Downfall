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
    private int randomObject;

    //Gestion Map
    private BreakableWalls[] listWalls;

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
        listWinnerPlayers = new List<int>();
        teamOne = new List<PlayerEntity>();
        teamTwo = new List<PlayerEntity>();
        mainPlayer = ReInput.players.GetPlayer(0);
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
                break;
            case STATE_PLAY.PrepareParty:
                PrepareParty();
                break;
            case STATE_PLAY.PrepareSuddenDeath:
                PrepareSuddenDeath();
                break;
            case STATE_PLAY.Party:
                if (nbPlayersAlive == 2 && isTeam)
                {
                    if (teamOne.Count == 0)
                    {
                        idPlayerwinner = 5;
                        gameState = STATE_PLAY.Results;
                    }
                    else if (teamTwo.Count == 0)
                    {
                        idPlayerwinner = 4;
                        gameState = STATE_PLAY.Results;
                    }
                }
                else if (nbPlayersAlive == 1)
                {                    
                    idPlayerwinner = listAlivePlayers[0].playerID;
                    gameState = STATE_PLAY.Results;      
                }
                break;
            case STATE_PLAY.Results:
                UpdatePoints();
                CheckWinner();
                break;
            case STATE_PLAY.DisplayResultsRound:
                DisplayRoundResults();
                WaitingForInput();
                break;
            case STATE_PLAY.DisplayResultsFinal:
                DisplayFinalResults();
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
        float x = Random.Range(spawnZone.GetComponent<BoxCollider>().bounds.min.x, spawnZone.GetComponent<BoxCollider>().bounds.max.x);
        float y = spawnZone.GetComponent<BoxCollider>().bounds.min.y;
        float z = Random.Range(spawnZone.GetComponent<BoxCollider>().bounds.min.z, spawnZone.GetComponent<BoxCollider>().bounds.max.z);
        Vector3 position = new Vector3(x, y, z);
        Instantiate(listPrefabsPickableItems[randomObject], position, Quaternion.identity);
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

    #endregion

    #region Events Fonctions

    private void PrepareParty()
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
        }
        else
        {
            Debug.Log("FFA");
            isTeam = false;
        }
        GenerateObjects();
        displayResults.gameObject.SetActive(false);
        gameState = STATE_PLAY.Party;
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
    }

    #endregion

    #region UI Fonctions

    private void DisplayRoundResults()
    {
        int idPlayer = 0;
        displayResults.gameObject.SetActive(true);
        if (idPlayerwinner <= 4)
        {
            displayWinner.text = "Victoire du joueur " + idPlayerwinner;
        }
        else if (idPlayerwinner == 5)
        {
            displayWinner.text = "Victoire de l'équipe";
            for (int i = 0; i < teamOne.Count; i++)
            {
                string displayText;
                idPlayer = teamOne[i].playerID + 1;
                if (i == 0)
                {
                    displayText = " " + idPlayer;
                }
                else
                {
                    displayText = " et " + idPlayer + "\n";
                }
                displayWinner.text += displayText;
            }
        }
        else if (idPlayerwinner == 6)
        {
            displayWinner.text = "Victoire de l'équipe";
            for (int i = 0; i < teamTwo.Count; i++)
            {
                string displayText;
                idPlayer = teamTwo[i].playerID + 1;
                if (i == 0)
                {
                    displayText = " " + idPlayer;
                }
                else
                {
                    displayText = " et " + idPlayer + "\n";
                }
                displayWinner.text += displayText;
            }
        }
        displayPointsRounds.text = "Scores : ";
        for (int index = 0; index < listPointsPlayers.Count; index++)
        {
            idPlayer = index + 1;
            displayPointsRounds.text += "joueur " + idPlayer + ", points : " + listPointsPlayers[index] + "\n";
        }
    }

    private void DisplayFinalResults()
    {

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
                gameState = STATE_PLAY.PrepareSuddenDeath;
            }
            else
            {
                gameState = STATE_PLAY.PrepareParty;
            }
        }
    }

    #endregion

}
