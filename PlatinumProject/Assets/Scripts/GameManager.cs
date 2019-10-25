using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Code crée et géré par Corentin
public class GameManager : MonoBehaviour
{
    public static GameManager managerGame;

    enum STATE_PLAY { PlayerSelection, Party, Results}

    private STATE_PLAY gameState;
    public float maxObjectsInGame;
    public GameObject[] listPrefabsPickableItems;
    public GameObject spawnZone;
    private Dictionary<int, bool> players;
    private int nbPlayersAlive;
    private int idPlayerwinner;
    public Canvas displayResults;
    public Text displayWinner;

    void Awake()
    {
        managerGame = this;
        players = new Dictionary<int, bool>();
        players.Add(0, true);
        players.Add(1, true);
        players.Add(2, true);
        players.Add(3, true);
        nbPlayersAlive = 4;
        //if (managerGame != null)
        //{
        //    managerGame = this;
        //}
        //else
        //{
        //    Debug.LogError("Too many instances!");
        //}
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
                if (nbPlayersAlive == 1)
                {
                    foreach(int idPlayer in players.Keys)
                    {
                        if (players[idPlayer])
                        {
                            idPlayerwinner = idPlayer;
                            gameState = STATE_PLAY.Results;
                            break;
                        }
                    }
                }
                break;
            case STATE_PLAY.Results:
                Time.timeScale = 0;
                idPlayerwinner++;
                displayWinner.text = "Le joueur " + idPlayerwinner + " est le grand vainqueur!!";
                displayResults.enabled = true;
                break;
            default:
                break;
        }
    }

    public void SpawnObject()
    {
        float x = Random.Range(spawnZone.GetComponent<BoxCollider>().bounds.min.x, spawnZone.GetComponent<BoxCollider>().bounds.max.x);
        float y = spawnZone.GetComponent<BoxCollider>().bounds.min.y;
        float z = Random.Range(spawnZone.GetComponent<BoxCollider>().bounds.min.z, spawnZone.GetComponent<BoxCollider>().bounds.max.z);
        Vector3 position = new Vector3(x, y, z);
        Instantiate(listPrefabsPickableItems[Random.Range(0, listPrefabsPickableItems.Length - 1)], position, Quaternion.identity);
    }

    public void DeadPlayer(int playerID)
    {
        players[playerID] = false;
        nbPlayersAlive--;
    }
}
