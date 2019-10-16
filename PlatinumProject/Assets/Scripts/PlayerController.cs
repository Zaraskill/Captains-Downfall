using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    public PlayerEntity entity;

    private Player mainPlayer;


    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer("Player0");
    }

    // Update is called once per frame
    void Update()
    {
        float dirX = mainPlayer.GetAxis("HorizontalMove");
        float dirY = mainPlayer.GetAxis("VerticalMove");

        Vector2 moveDir = new Vector2(dirX, dirY);
        moveDir.Normalize();

        entity.Move(moveDir);
    }
}
