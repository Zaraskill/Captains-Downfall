using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;


// Code crée et géré par Corentin
public class PlayerController : MonoBehaviour
{

    public string playerKey;

    public PlayerEntity entity;

    private Player mainPlayer;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer(playerKey);

    }

    // Update is called once per frame
    void Update()
    {
        float dirX = mainPlayer.GetAxis("HorizontalMove");
        float dirY = mainPlayer.GetAxis("VerticalMove");


        Vector2 moveDir = new Vector2(dirX, dirY);
        moveDir.Normalize();

        entity.Move(moveDir);


        if (mainPlayer.GetButtonDown("PickUp") && entity.CanPick())
        {
            entity.PickItem();
        }
        else if (mainPlayer.GetButton("PickUp") && entity.IsHoldingItem())
        {
            entity.ImprovePower();
        }
        else if (mainPlayer.GetButtonUp("PickUp") && entity.IsHoldingItem())
        {
            entity.Throw();
        }

    }
}
