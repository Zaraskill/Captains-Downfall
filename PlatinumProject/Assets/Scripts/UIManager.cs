using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//code crée et géré par Corentin
public class UIManager : MonoBehaviour
{

    public static UIManager managerUI;

    public Canvas canvasUI;

    // Main Menu
    [Header("MainMenu")]
    public Image titleImage;

    //Player Selection

    //Prepare Round
    [Header("Begin Round")]
    public GameObject beginRound;
    public Image fightType;
    public Image displayCharacter;
    public List<Image> characterSlotsFFA;
    public List<Image> characterSlotsTF;
    public GameObject roundFFA;
    public GameObject roundTF;

    //End Round
    [Header("End Round")]
    public GameObject endRound;
    public Image displayWinner;
    public Image boardLadder;

    //Database Skin
    [Header("Skins Players")]
    public List<Sprite> playerSkinsFFALeft;
    public List<Sprite> playerSkinsFFARight;
    public List<Sprite> playerSkinsTFLeft;
    public List<Sprite> playerSkinsTFRight;

    //Pause

    //Credits
    public Image displayCredits;

    //Options


    void Awake()
    {
        if (managerUI != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            managerUI = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayRoundBeginning(int roundType)
    {
        beginRound.SetActive(true);
        if (roundType == 1)
        {
            roundFFA.SetActive(true);
            for (int index = 0; index < characterSlotsFFA.Count; index++)
            {
                if (index % 2 == 0)
                {
                    characterSlotsFFA[index].sprite = playerSkinsFFALeft[index];
                }
                else
                {
                    characterSlotsFFA[index].sprite = playerSkinsFFARight[index];
                }
                
            }

        }
        else if (roundType == 2)
        {
            roundTF.SetActive(true);
            DisplayTeamOne();
            DisplayTeamTwo();
        }
    }

    private void DisplayTeamOne()
    {
        List<PlayerEntity> team = GameManager.managerGame.GetTeamOne();
        characterSlotsTF[0].sprite = playerSkinsTFLeft[team[0].playerID];
        characterSlotsTF[1].sprite = playerSkinsTFLeft[team[1].playerID];

    }

    private void DisplayTeamTwo()
    {
        List<PlayerEntity> team = GameManager.managerGame.GetTeamTwo();
        characterSlotsTF[2].sprite = playerSkinsTFRight[team[0].playerID];
        characterSlotsTF[3].sprite = playerSkinsTFRight[team[1].playerID];

    }

    public void StartRound()
    {
        roundFFA.SetActive(false);
        roundTF.SetActive(false);
        beginRound.SetActive(false);
    }

}
