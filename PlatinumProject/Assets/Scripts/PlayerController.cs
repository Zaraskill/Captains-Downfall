using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 5.0f;
    public GameObject balle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var z = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        this.transform.Translate(x, 0, z);


        if (Input.GetButtonDown("Fire3"))
        {
            Instantiate(balle, transform.position, transform.rotation);

        }


    }
}
