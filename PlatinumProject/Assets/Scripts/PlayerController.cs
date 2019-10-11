using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 5.0f;
    public GameObject balle;
    public bool isHoldingObject = false;
    public bool isNearPickableObject = false;


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


        if (Input.GetButtonDown("Fire2") && isHoldingObject)
        {
            GameObject baballe = Instantiate(balle, transform.position, transform.rotation);
            Vector3 movement = new Vector3(1000, 0, 0);
            baballe.GetComponent<Rigidbody>().AddForce(movement);
            isHoldingObject = false;

        }
        else if (Input.GetButtonDown("Fire2") && !isHoldingObject && isNearPickableObject)
        {
            isHoldingObject = true;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickable")
        {
            isNearPickableObject = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pickable")
        {
            isNearPickableObject = false;
        }
    }
}
