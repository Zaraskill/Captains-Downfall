using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

// Code créé et géré par Siméon
public class Barrel : MonoBehaviour
{
    [Header("Players To Knock")]
    public List<PlayerEntity> playerIntoArea;
    public List<PickupableObject> objetIntoArea;

    [Header("Explosion")]
    public float knockPower = 0f;
    public bool isTouchingPlayer;
    public bool isExploding;
    public GameObject explosionEffect;

    [Header("Camera Shaker")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    [Header("Components")]
    private PlayerEntity playerCollisionned;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTouchingPlayer)
        {
            Explosion();
        }
        else if(isExploding)
        {
            Destruction();
        }
    }

    #region Collisions/Trigger Fonctions

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            return;
        }
        else
        {
            if (collision.gameObject.CompareTag("Pickable"))
            {
                RemoveObject(collision.gameObject.GetComponent<PickupableObject>());
            }
            isTouchingPlayer = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerIntoArea.Add(collision.gameObject.GetComponent<PlayerEntity>());
        }
        else if (collision.gameObject.CompareTag("Pickable"))
        {
            objetIntoArea.Add(collision.gameObject.GetComponent<PickupableObject>());
            collision.gameObject.GetComponent<PickupableObject>().GoInsideRangeBarrel(this);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIntoArea.Remove(collision.gameObject.GetComponent<PlayerEntity>());
        }
        else if (collision.gameObject.CompareTag("Pickable"))
        {
            RemoveObject(collision.gameObject.GetComponent<PickupableObject>());
            collision.gameObject.GetComponent<PickupableObject>().ExitInsideRangeBarrel();
        }
    }
    #endregion

    #region Destruction Fonctions

    private void Explosion()
    {
        animator.SetBool("isExploding", true);
    }

    private void Destruction()
    {
        if (playerIntoArea.Count >= 1)
        {
            for (int i = playerIntoArea.Count; 0 <= --i;)
            {
                if(playerIntoArea[i])
                {
                Vector3 orientDir = (playerIntoArea[i].transform.position - transform.position);
                Vector3 directionNormalized = orientDir.normalized;
                playerIntoArea[i].Knockback(new Vector2(directionNormalized.x, directionNormalized.z), knockPower);
                }
            }            
        }

        if (objetIntoArea.Count >= 1)
        {
            for (int i = objetIntoArea.Count; 0 <= --i;)
            {
                if(objetIntoArea[i])
                {
                Vector3 orientDir = (objetIntoArea[i].transform.position - transform.position);
                Vector3 directionNormalized = orientDir.normalized;
                objetIntoArea[i].Throw(new Vector2(directionNormalized.x, directionNormalized.z));
                }
            }            
        }

        SoundManager.managerSound.MakeBarrelExplosionSound();
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    #endregion

    private void RemoveObject(PickupableObject objectToRemove)
    {
        while (objetIntoArea.Contains(objectToRemove))
        {
            objetIntoArea.Remove(objectToRemove);
        }
    }

}
