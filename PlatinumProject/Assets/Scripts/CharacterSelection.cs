using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelection : MonoBehaviour
{
    public string playerKey;

    private Player mainPlayer;

    public GameObject ListePlayers;

    public GameObject[] characterList;

    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer(playerKey);

        characterList = new GameObject[ListePlayers.transform.childCount];
        GameObject _instance = Instantiate(ListePlayers, transform.position, Quaternion.identity);

        for(int i = 0; i < characterList.Length; i++)
        {
            characterList[i] = _instance.transform.GetChild(i).gameObject;
            characterList[i].transform.position = transform.position;
        }

        foreach (GameObject character in characterList)
        {
            character.SetActive(false);
        }

        if (characterList[0])
        {
            characterList[0].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(mainPlayer.GetNegativeButtonDown("HorizontalMove"))
        {
            characterList[index].SetActive(false);

            index--;
            if(index < 0)
            {
                index = characterList.Length - 1;
            }

            characterList[index].SetActive(true);
        }
        else if (mainPlayer.GetButtonDown("HorizontalMove"))
        {
            characterList[index].SetActive(false);

            index++;
            if (index == characterList.Length)
            {
                index = 0;
            }

            characterList[index].SetActive(true);
        }
        else if(mainPlayer.GetButtonDown("PickUp"))
        {

        }
        //PlayerPrefs.SetInt("CharacterSelected", index);
        //PlayerPrefs.GetInt("CharacterSelected");
    }
}
