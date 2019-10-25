using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float knockPower = 0f;
    //private Vector3 orientDir = Vector3.zero;

    private PlayerEntity playerCollisionned;

    private bool isExploding;

    public List<GameObject> objectsToBump;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isExploding)
        {
            Explosion();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            return;
        }
        else
        {
            isExploding = true;
        }
    }

    private void Explosion()
    {
<<<<<<< HEAD
        for(int i = 0; i < playerIntoArea.Count; i++)
        {
            Vector3 orientDir = (playerIntoArea[i].transform.position - transform.position);
            Vector3 directionNormalized = orientDir.normalized;
            playerIntoArea[i].Knockback(directionNormalized, knockPower);
            isExploding = false;
        }
=======

>>>>>>> 19a9c0a0bc03bf46bff988c68b61dde72444bc0e
    }

    private void OnDrawGizmos()
    {
        foreach (PlayerEntity item in playerIntoArea)
        {
            Gizmos.DrawRay(item.transform.position, item.transform.position - transform.position);
            //Gizmos.DrawRay(transform.position, item.transform.position - transform.position);
        }        
    }
}
