﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 5f;
    public bool isRotating = false;

    [Header("Éjection")]
    public float timeInsideCanon = 0f;
    public float timeToExpel = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRotating)
        {
            UpdateRotate();
            EjectPlayer();
        }
    }

    private void UpdateRotate()
    {
        transform.Rotate(0, rotateSpeed, 0);
    }

    private void EjectPlayer()
    {
        timeInsideCanon += Time.fixedDeltaTime;
        if(timeInsideCanon > timeToExpel || Input.GetButtonDown("PickUp"))
        {
            // Ejection du Player

            timeInsideCanon = 0f;
            isRotating = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isRotating = true;
        }
    }
}
