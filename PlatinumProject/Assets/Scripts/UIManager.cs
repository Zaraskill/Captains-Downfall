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
    public Image displayFightFFA;
    public Image displayVSFFA;
    public Image displayFightTF;
    public Image displayVSTF;
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

    #region Begin Round Fonctions

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
            GenerateAnimationsBeginRound(1);
        }
        else if (roundType == 2)
        {
            roundTF.SetActive(true);
            DisplayTeamOne();
            DisplayTeamTwo();
            GenerateAnimationsBeginRound(2);
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

    public void GenerateAnimationsBeginRound(int roundType)
    {
        if (roundType == 1)
        {
            displayFightFFA.GetComponent<Animator>().SetTrigger("display");
            displayVSFFA.GetComponent<Animator>();
        }
        else if (roundType == 2)
        {
            displayFightTF.GetComponent<Animator>();
            displayVSTF.GetComponent<Animator>();
        }
    }

    #endregion

    #region Ending round Fonctions

    public void DisplayRoundEnding(int caseVictory)
    {
        endRound.SetActive(true);
        switch (caseVictory)
        {
            case 0:
                GenerateAnimation(0);
                break;
            case 1:
                GenerateAnimation(1);
                break;
            case 2:
                GenerateAnimation(2);
                break;
            case 3:
                GenerateAnimation(3);
                break;
            case 4:
                break;
            case 5:
                break;
            default:
                break;
        }
    }

    private void GenerateAnimation(int player)
    {
        if (GameManager.managerGame.GetPointsPlayers(player) == 1)
        {

        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 2)
        {

        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 3)
        {

        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 4)
        {

        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 5)
        {

        }
    }

    private void GenerateAnimationTeam(int team)
    {
        if (team == 4)
        {
            foreach (PlayerEntity player in GameManager.managerGame.GetTeamOne())
            {
                GenerateAnimation(player.playerID);
            }
        }
        else if (team == 5)
        {
            foreach (PlayerEntity player in GameManager.managerGame.GetTeamTwo())
            {
                GenerateAnimation(player.playerID);
            }
        }
    }

    public void EndRound()
    {
        endRound.SetActive(false);
    }

    #endregion

}
