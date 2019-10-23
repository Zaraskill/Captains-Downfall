using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    enum STATE_PLAY { PlayerSelection, Party, Results}

    private STATE_PLAY gameState;
    public float maxObjectsInGame;
    public GameObject[] listPrefabsPickableItems;
    private PickupableObject[] objectsInGame;

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
                objectsInGame = FindObjectsOfType<PickupableObject>();

                break;
            case STATE_PLAY.Results:
                break;
            default:
                break;
        }
    }
}
