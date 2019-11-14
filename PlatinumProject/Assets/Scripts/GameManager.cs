using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //Gestion players
    [Header("Players")]    
    public List<PlayerEntity> listPlayers;
    private List<PlayerEntity> listAlivePlayers;
    private List<int> listPointsPlayers;
    private List<int> listWinnerPlayers;
    public int nbPlayersAlive;
    private int idPlayerwinner;
    private bool isTeam = false;
    private List<PlayerEntity> teamOne;
    private List<PlayerEntity> teamTwo;

    //Affichage résultats
    [Header("Affichage Résultats")]
    public Canvas displayResults;
    public Text displayWinner;

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
        listWinnerPlayers = new List<int>();
        teamOne = new List<PlayerEntity>();
        teamTwo = new List<PlayerEntity>();
    }
    // Start is called before the first frame update
    void Start()
    {
        gameState = STATE_PLAY.Party;
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
        for (int i = 0; i < 15; i++)
        {
            SpawnObject();
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
            listTemp.Remove(playerID);           
        }
        foreach (PlayerEntity player in listTemp)
        {
            player.teamID = 2;
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

    #endregion

    #region Events Fonctions

    private void PrepareParty()
    {
        DestroyTeam();
        listWinnerPlayers.Clear();
        idPlayerwinner = -1;
        listAlivePlayers = listPlayers;
        nbPlayersAlive = 4;
        if (Random.Range(0, 2) == 1)
        {
            isTeam = true;
            CreationTeam();
        }
        else
        {
            isTeam = false;
        }
        GenerateObjects();
    }

    private void PrepareSuddenDeath()
    {
        DestroyTeam();
        idPlayerwinner = -1;
    }

    #endregion

    #region UI Fonctions

    private void DisplayRoundResults()
    {
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
                if (i == 0)
                {
                    displayText = " " + teamOne[i].playerID++;
                }
                else
                {
                    displayText = " et " + teamOne[i].playerID++;
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
                if (i == 0)
                {
                    displayText = " " + teamTwo[i].playerID;
                }
                else
                {
                    displayText = " et " + teamTwo[i].playerID;
                }
                displayWinner.text += displayText;
            }
        }
    }

    private void DisplayFinalResults()
    {

    }

    #endregion

    #region Input Fonctions

    private void WaitingForInput()
    {
        if (Input.GetButtonDown("UISubmit"))
        {
            if (listPointsPlayers.Count == 1)
            {
                gameState = STATE_PLAY.DisplayResultsFinal;
            }
            else if (listPointsPlayers.Count > 1)
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
