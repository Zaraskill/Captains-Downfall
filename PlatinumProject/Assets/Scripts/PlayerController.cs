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


        if (mainPlayer.GetButtonDown("PickUp") && entity.CanPick() && !entity.IsHoldingItem())
        {
            entity.PickItem();
        }
        else if (!entity.IsChargingPower())
        {
            if (mainPlayer.GetButtonDown("PickUp") && entity.IsHoldingItem())
            {
                entity.StartChargingPower();
            }
        }
        else if (entity.IsChargingPower())
        {
            if (mainPlayer.GetButton("PickUp"))
            {
                entity.ImprovePower();
            }
            else
            {
                entity.Throw();
            }
        }
    }
}
