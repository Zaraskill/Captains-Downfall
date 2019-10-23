using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{
    public float timeBtwSpawns;
    public float StartTimeBtwSpawns;

    public GameObject Smoke;
    private PlayerEntity player;

    void Start()
    {
        player = GetComponent<PlayerEntity>();
    }

    void Update()
    {
        if(player.moveInput != 0){
            if (timeBtwSpawns <= 0)
            {
                //spawn Smoke game object
                GameObject instance = (GameObject)Instantiate(Smoke, transform.position, Quaternion.identity);
                Destroy(instance, 8f);
                timeBtwSpawns = StartTimeBtwSpawns;
            }
            else
            {
                timeBtwSpawns -= Time.deltaTime;
            }
        }
        
    }
}
