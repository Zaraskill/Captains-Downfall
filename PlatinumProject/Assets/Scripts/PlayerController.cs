using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    public PlayerEntity entity;
    //public PlayerEntity entity2;

    private Player mainPlayer;
    //private Player mainPlayer2;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer(entity.playerID);

        //mainPlayer2 = ReInput.players.GetPlayer("Player1");
    }

    // Update is called once per frame
    void Update()
    {
        float dirX = mainPlayer.GetAxis("HorizontalMove");
        float dirY = mainPlayer.GetAxis("VerticalMove");

        //float dirX2 = mainPlayer2.GetAxis("HorizontalMove");
        //float dirY2 = mainPlayer2.GetAxis("VerticalMove");

        Vector2 moveDir = new Vector2(dirX, dirY);
        moveDir.Normalize();

        //Vector2 moveDir2 = new Vector2(dirX2, dirY2);
        //moveDir2.Normalize();

        entity.Move(moveDir);
        //entity2.Move(moveDir2);

        if (mainPlayer.GetButtonDown("PickUp") && entity.CanPick())
        {
            entity.PickItem();
        }
    }
}
