using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Code crée et géré par Corentin
public class GameManager : MonoBehaviour
{
    public static GameManager managerGame;

    enum STATE_PLAY { PlayerSelection, Party, Results, DisplayResults}

    private STATE_PLAY gameState;
    

    //Spawn Objects
    [Header("Spawn Objects")]
    public float maxObjectsInGame;
    public GameObject[] listPrefabsPickableItems;
    public GameObject spawnZone;

    //Gestion players
    [Header("Players")]    
    public List<PlayerEntity> listPlayers;
    private List<PlayerEntity> listAlivePlayers;
    public int nbPlayersAlive;
    private Dictionary<int, bool> players;
    private int idPlayerwinner;
    private bool isTeam = false;
    private List<PlayerEntity> teamOne;
    private List<PlayerEntity> teamTwo;

    //Gestion Events
    [Header("Events")]
    public float startCooldownEvent;
    public float cooldownEvent;
    private bool isEventComing = false;
    private bool isEventHere = false;

    //Affichage résultats
    [Header("Affichage Résultats")]
    public Canvas displayResults;
    public Text displayWinner;

    void Awake()
    {
        if (managerGame != null)
        {
            Debug.LogError("Too many instances!");
        }
        else
        {
            managerGame = this;
        }
        players = new Dictionary<int, bool>
        {
            { 0, true },
            { 1, true },
            { 2, true },
            { 3, true }
        };
        nbPlayersAlive = 4;
        teamOne = new List<PlayerEntity>();
        teamTwo = new List<PlayerEntity>();
        listAlivePlayers = new List<PlayerEntity>(listPlayers);
        cooldownEvent = startCooldownEvent;
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
            case STATE_PLAY.Party:
                if (nbPlayersAlive <= 2 && isTeam)
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
                    foreach(int idPlayer in players.Keys)
                    {
                        if (players[idPlayer])
                        {
                            idPlayerwinner = idPlayer;
                            gameState = STATE_PLAY.Results;
                        }
                    }
                }
                else
                {
                    if (nbPlayersAlive > 2 )
                    {
                        if (!isEventComing && !isEventHere)
                        {
                            if (Random.Range(0, 2) == 1)
                            {
                                StartComingEvent();
                            }
                        }
                        else if (isEventComing)
                        {
                            if (cooldownEvent <= 0f)
                            {
                                StartEvent();
                            }
                            else
                            {
                                cooldownEvent -= Time.deltaTime;
                            }
                        }
                        else if (isEventHere)
                        {
                            PrepareEvent();
                        }
                    }                    
                }

                break;
            case STATE_PLAY.Results:
                idPlayerwinner++;
                gameState = STATE_PLAY.DisplayResults;
                break;
            case STATE_PLAY.DisplayResults:
                displayResults.gameObject.SetActive(true);
                if (idPlayerwinner <= 4 )
                {
                    displayWinner.text = "Le joueur " + idPlayerwinner + " est le grand vainqueur!!";
                }
                else if (idPlayerwinner == 5)
                {
                    displayWinner.text = "L'équipe";
                    for (int i = 0; i < teamOne.Count; i++)
                    {
                        string displayText;
                        if (i == 0)
                        {
                            displayText = " joueur " + teamOne[i].playerID;
                        }
                        else
                        {
                            displayText = " et joueur " + teamOne[i].playerID;
                        }
                        displayWinner.text += displayText;
                    }
                    displayWinner.text += "sont les vainqueurs!!";
                }
                else if (idPlayerwinner == 6)
                {
                    displayWinner.text = "L'équipe";
                    for (int i = 0; i < teamTwo.Count; i++)
                    {
                        string displayText;
                        if (i == 0)
                        {
                            displayText = " joueur " + teamTwo[i].playerID;
                        }
                        else
                        {
                            displayText = " et joueur " + teamTwo[i].playerID;
                        }
                        displayWinner.text += displayText;
                    }
                    displayWinner.text += "sont les vainqueurs!!";
                }                
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
        Instantiate(listPrefabsPickableItems[Random.Range(0, listPrefabsPickableItems.Length - 1)], position, Quaternion.identity);
    }

    #endregion

    #region Players Fonctions

    public void DeadPlayer(int playerID)
    {
        players[playerID] = false;
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
        foreach (PlayerEntity player in listPlayers)
        {
            player.teamID = 0;
        }
        teamOne.Clear();
        teamTwo.Clear();
    }

    #endregion

    #region Events Fonctions

    private void StartComingEvent()
    {
        isEventComing = true;
    }

    private void StartEvent()
    {
        cooldownEvent = startCooldownEvent;
        isEventComing = false;
        isEventHere = true;
    }

    private void PrepareEvent()
    {
        if (isTeam)
        {
            isTeam = false;
            DestroyTeam();
        }
        else
        {
            isTeam = true;
            CreationTeam();
        }
        isEventHere = false;
    }

    #endregion

}
