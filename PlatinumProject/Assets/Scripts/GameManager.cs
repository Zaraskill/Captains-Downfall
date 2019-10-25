using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager managerGame;

    enum STATE_PLAY { PlayerSelection, Party, Results}

    private STATE_PLAY gameState;
    public float maxObjectsInGame;
    public GameObject[] listPrefabsPickableItems;
    public GameObject spawnZone;

    void Awake()
    {
        managerGame = this;
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
        //gameState = STATE_PLAY.PlayerSelection;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case STATE_PLAY.PlayerSelection:
                break;
            case STATE_PLAY.Party:
                break;
            case STATE_PLAY.Results:
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
}
