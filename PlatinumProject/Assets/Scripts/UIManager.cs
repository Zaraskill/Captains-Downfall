using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager managerUI;

    public Canvas canvasUI;

    // Main Menu
    [Header("MainMenu")]
    public Image titleImage;

    //Player Selection

    //Prepare Round
    public Image fightType;
    public Image displayCharacter;

    //End Round
    public Text displayWinner;
    public Image boardLadder;
    public List<Image> playerSkins;

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
}
