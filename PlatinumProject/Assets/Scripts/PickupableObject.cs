using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code créé et géré par Siméon
public class PickupableObject : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;
    public float verticalSpeedOn = 10f;
    private float verticalSpeed = 0f;
    public float groundY = 0f;
    private bool isGrounded = false;
    public bool isPickable = true;

    private Vector2 orient = Vector2.zero;
    private float powerKnock = 0f;

    private float timerSpawn = 5f;
    private float timer;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        timer = timerSpawn;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateGroundCheck();
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _rigidbody.velocity = new Vector3(velocity.x, verticalSpeed, velocity.y);
    }

    private void UpdateGroundCheck()
    {
        if (!isPickable)
        {
            return;
        }
        if ( (transform.position.y <= groundY && isPickable) || (!isPickable && velocity == Vector2.zero) )
        {
            isGrounded = true;
            verticalSpeed = 0f;
            transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        }
        else
        {
            isGrounded = false;
            verticalSpeed =  -verticalSpeedOn;
        }
    }

    public void Throw(Vector2 orient, float power)
    {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<TrailRenderer>().enabled = true;
        this.orient = orient;
        powerKnock = power;
        velocity = orient * power;
    }

    public void Picked()
    {
        isPickable = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public bool IsPickable()
    {
        return isPickable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && !isPickable)
        {
            collision.gameObject.GetComponent<PlayerEntity>().Knockback(orient, powerKnock);
            GameManager.managerGame.SpawnObject();
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            GameManager.managerGame.SpawnObject();
            BreakableWalls wall = collision.gameObject.GetComponent<BreakableWalls>();
            wall.TakeDamage();
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            GameManager.managerGame.SpawnObject();
            Destroy(this.gameObject);
        }
    }
}
