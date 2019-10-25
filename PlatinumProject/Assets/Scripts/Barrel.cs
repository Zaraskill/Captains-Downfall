using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float knockPower;
    private Vector3 orientDir = Vector3.zero;

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

    }
}
