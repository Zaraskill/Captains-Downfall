using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//code crée et géré par Corentin
public class UIManager : MonoBehaviour
{
    public const int WINNER_P1 = 1;
    public const int WINNER_P2 = 2;
    public const int WINNER_P3 = 3;
    public const int WINNER_P4 = 4;
    public const int WINNER_TEAM1 = 5;
    public const int WINNER_TEAM2 = 6;

    public static UIManager managerUI;

    public Canvas canvasUI;

    // Main Menu
    [Header("MainMenu")]
    public GameObject mainMenu;

    //Player Selection

    //Prepare Round
    [Header("Begin Round")]
    public GameObject beginRound;
    public List<Image> characterSlotsFFA;
    public List<Image> characterSlotsTF;
    public GameObject roundFFA;
    public GameObject roundTF;

    //End Round
    [Header("End Round")]
    public GameObject endRound;
    public Image displayWinner;
    public Image boardLadder;
    public Image playerOne;
    public Image playerTwo;
    public Image playerThree;
    public Image playerFour;
    public List<Sprite> listWinner;
    private Image tmpImage;

    //Database Skin
    [Header("Skins Players")]
    public List<Sprite> playerSkinsFFALeft;
    public List<Sprite> playerSkinsFFARight;
    public List<Sprite> playerSkinsTFLeft;
    public List<Sprite> playerSkinsTFRight;
    public List<Sprite> listNumberPlayer;

    //Final Results
    [Header("Final Display")]
    public GameObject finalDisplay;
    public GameObject oneWinner;
    public GameObject twoWinners;
    public List<Sprite> listPlayersPodium;

    //Pause

    //Credits
    [Header("Credits")]
    public GameObject displayCredits;
    public GameObject mainMenuToDisplay;
    public GameObject tutoUI;
    public GameObject navBar;
    public GameObject pressAToContinue;
    public GameObject tutoMove;
    public GameObject tutoObjects;

    //Options


    void Awake()
    {
        if (managerUI != null)
        {

        }
        else
        {
            managerUI = this;
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

    #endregion

    #region Ending round Fonctions

    public void DisplayRoundEnding(int caseVictory)
    {
        endRound.SetActive(true);
        switch (caseVictory)
        {
            case WINNER_P1:
                displayWinner.sprite = listWinner[0];
                tmpImage = playerOne;
                GenerateAnimation(0);
                break;
            case WINNER_P2:
                displayWinner.sprite = listWinner[1];
                tmpImage = playerTwo;
                GenerateAnimation(1);
                break;
            case WINNER_P3:
                displayWinner.sprite = listWinner[2];
                tmpImage = playerThree;
                GenerateAnimation(2);
                break;
            case WINNER_P4:
                displayWinner.sprite = listWinner[3];
                tmpImage = playerFour;
                GenerateAnimation(3);
                break;
            case WINNER_TEAM1:
                DisplayWinnerTeam(WINNER_TEAM1);
                GenerateAnimationTeam(WINNER_TEAM1);
                break;
            case WINNER_TEAM2:
                DisplayWinnerTeam(WINNER_TEAM2);
                GenerateAnimationTeam(WINNER_TEAM2);
                break;
            default:
                break;
        }
    }

    private void DisplayWinnerTeam(int caseVictory)
    {
        if (caseVictory == WINNER_TEAM1)
        {
            List<PlayerEntity> tmpTeam = GameManager.managerGame.GetTeamOne();
            DisplayWinnerUI(tmpTeam);
        }
        else if (caseVictory == WINNER_TEAM2)
        {
            List<PlayerEntity> tmpTeam = GameManager.managerGame.GetTeamTwo();
            DisplayWinnerUI(tmpTeam);
        }
    }

    private void DisplayWinnerUI(List<PlayerEntity> team)
    {
        if ( (team[0].playerID == 0 && team[1].playerID == 1) || (team[1].playerID == 0 && team[0].playerID == 1) ) //J1 et J2
        {
            displayWinner.sprite = listWinner[4];
        }
        else if ((team[0].playerID == 0 && team[1].playerID == 2) || (team[1].playerID == 0 && team[0].playerID == 2)) //J1 et J3
        {
            displayWinner.sprite = listWinner[5];
        }
        else if ((team[0].playerID == 0 && team[1].playerID == 3) || (team[1].playerID == 0 && team[0].playerID == 3)) //J1 et J4
        {
            displayWinner.sprite = listWinner[6];
        }
        else if ((team[0].playerID == 1 && team[1].playerID == 2) || (team[1].playerID == 1 && team[0].playerID == 2)) //J2 et J3
        {
            displayWinner.sprite = listWinner[7];
        }
        else if ((team[0].playerID == 1 && team[1].playerID == 3) || (team[1].playerID == 1 && team[0].playerID == 3)) //J2 et J4
        {
            displayWinner.sprite = listWinner[8];
        }
        else if ((team[0].playerID == 2 && team[1].playerID == 3) || (team[1].playerID == 2 && team[0].playerID == 3)) //J3 et J4
        {
            displayWinner.sprite = listWinner[9];
        }
    }

    private void GenerateAnimation(int player)
    {
        if (GameManager.managerGame.GetPointsPlayers(player) == 1)
        {
            tmpImage.GetComponent<Animator>().SetInteger("points", 1);
        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 2)
        {
            tmpImage.GetComponent<Animator>().SetInteger("points", 2);
        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 3)
        {
            tmpImage.GetComponent<Animator>().SetInteger("points", 3);
        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 4)
        {
            tmpImage.GetComponent<Animator>().SetInteger("points", 4);
        }
        else if (GameManager.managerGame.GetPointsPlayers(player) == 5)
        {
            tmpImage.GetComponent<Animator>().SetInteger("points", 5);
        }
        tmpImage = null;
    }

    private void GenerateAnimationTeam(int team)
    {
        if (team == WINNER_TEAM1)
        {
            foreach (PlayerEntity player in GameManager.managerGame.GetTeamOne())
            {
                UpdateImage(player);
                GenerateAnimation(player.playerID);
            }
        }
        else if (team == WINNER_TEAM2)
        {
            foreach (PlayerEntity player in GameManager.managerGame.GetTeamTwo())
            {
                UpdateImage(player);
                GenerateAnimation(player.playerID);
            }
        }
    }

    public void EndRound()
    {
        endRound.SetActive(false);
    }

    private void UpdateImage(PlayerEntity player)
    {
        if (player.playerID == 0)
        {
            tmpImage = playerOne;
        }
        else if (player.playerID == 1)
        {
            tmpImage = playerTwo;
        }
        else if (player.playerID == 2)
        {
            tmpImage = playerThree;
        }
        else if (player.playerID == 3)
        {
            tmpImage = playerFour;
        }
    }

    #endregion

    #region Final Displays Fonctions

    public void DisplayPodiumWinner(List<int> listWinner)
    {
        finalDisplay.SetActive(true);
        switch (listWinner.Count)
        {           
            case 1:
                oneWinner.SetActive(true);
                Image winner = oneWinner.GetComponentInChildren<Image>();
                winner.sprite = listPlayersPodium[listWinner[0]];
                break;
            case 2:
                twoWinners.SetActive(true);
                Image[] winners = twoWinners.GetComponentsInChildren<Image>();
                for (int index = 0; index < winners.Length; index++)
                {
                    winners[index].sprite = listPlayersPodium[listWinner[index]];
                }
                
                break;
            default:
                break;
        }
    }

    #endregion

}
