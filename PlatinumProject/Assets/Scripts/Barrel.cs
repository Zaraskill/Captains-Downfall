using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float knockPower;
    private Vector3 orientDir = Vector3.zero;

    private bool isExploding;

    public List<PlayerEntity> playerIntoArea;


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
            Debug.Log("ça touche");
            isExploding = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerEntity player = collision.GetComponentInParent<PlayerEntity>();
        if(player != null)
        {
            playerIntoArea.Add(player);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        PlayerEntity player = collision.GetComponentInParent<PlayerEntity>();
        if (player != null)
        {
            playerIntoArea.Remove(player);
        }
    }

    private void Explosion()
    {
        for(int i = 0; i < playerIntoArea.Count; i++)
        {
            Vector2 direction = new Vector2(playerIntoArea[i].transform.position.x, transform.position.y);
            Vector2 directionNormalized = direction.normalized;
            playerIntoArea[i].Knockback(-directionNormalized, knockPower);
            isExploding = false;
        }
    }
}
