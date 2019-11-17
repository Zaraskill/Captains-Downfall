using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

// Code créé et géré par Siméon
public class Barrel : MonoBehaviour
{
    [Header("Players To Knock")]
    public List<PlayerEntity> playerIntoArea;

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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            return;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerEntity player = collision.gameObject.GetComponent<PlayerEntity>();
        if (player != null)
        {
            playerIntoArea.Add(player);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        PlayerEntity player = collision.gameObject.GetComponent<PlayerEntity>();
        if (player != null)
        {
            playerIntoArea.Remove(player);
        }
    }

    private void Explosion()
    {
        animator.SetBool("isExploding", true);
    }

    private void Destruction()
    {
        for (int i = 0; i < playerIntoArea.Count; i++)
        {
            Vector3 orientDir = (playerIntoArea[i].transform.position - transform.position);
            Vector3 directionNormalized = orientDir.normalized;
            SoundManager.managerSound.MakeBarrelExplosionSound();
            CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
            playerIntoArea[i].Knockback(new Vector2(directionNormalized.x, directionNormalized.z), knockPower);
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }


    /* private void OnDrawGizmos()
    {
        foreach (PlayerEntity item in playerIntoArea)
        {
            //Gizmos.DrawRay(item.transform.position, item.transform.position - transform.position);
            Gizmos.DrawRay(transform.position, item.transform.position - transform.position);
        }        
    } */
}
