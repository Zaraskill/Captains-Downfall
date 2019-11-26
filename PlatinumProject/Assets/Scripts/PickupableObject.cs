using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code créé et géré par Siméon
public class PickupableObject : MonoBehaviour
{
    [Header("Pick & Throw")]
    public float powerKnock = 50f;
    public bool isPickable = true;
    private bool isThrown = false;
    private Vector2 orient = Vector2.zero;

    [Header("Speed")]
    public float verticalSpeedOn = 10f;
    private Vector2 velocity = Vector2.zero;
    private float verticalSpeed = 0f;

    [Header("Ground")]
    public float groundY = 0f;
    private bool isGrounded = false;
    public Transform groundPosition;

    [Header("Spawn")]
    private float timerSpawn = 5f;
    private float timer;

    [Header("Components")]
    private Rigidbody _rigidbody;

    private bool isKnocked = false;
    private Vector2 orientDir = Vector2.right;
    private Vector2 moveDir;
    private Vector2 speed = Vector2.zero;

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
        if ( (groundPosition.position.y <= groundY && isPickable) || (!isPickable && velocity == Vector2.zero) )
        {
            isGrounded = true;
            verticalSpeed = 0f;
            float offsetY = groundPosition.position.y - transform.position.y;

            transform.position = new Vector3(transform.position.x, groundY - offsetY, transform.position.z);
        }
        else
        {
            isGrounded = false;
            verticalSpeed =  -verticalSpeedOn;
        }
    }

    public void Throw(Vector2 orient)
    {
        isThrown = true;
        _rigidbody.isKinematic = false;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<TrailRenderer>().enabled = true;
        this.orient = orient;
        velocity = orient * powerKnock;
    }

    public void Picked()
    {
        if (isPickable)
        {
            isPickable = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }        
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

    public void Knockback(Vector2 knockDir, float powerKnock)
    {
        //SoundManager.managerSound.MakeHitSound();
        isKnocked = true;
        orientDir = knockDir;
        moveDir = Vector2.zero;
        speed = knockDir * powerKnock;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isThrown)
        {
            SoundManager.managerSound.MakeHitSound();
            collision.gameObject.GetComponent<PlayerEntity>().Knockback(orient, powerKnock);
            GameManager.managerGame.SpawnObject();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall") && isThrown)
        {
            GameManager.managerGame.SpawnObject();
            BreakableWalls wall = collision.gameObject.GetComponent<BreakableWalls>();
            wall.TakeDamage();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Barrel") && isThrown)
        {
            GameManager.managerGame.SpawnObject();
            _rigidbody.velocity = Vector3.zero;
            velocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            GameManager.managerGame.SpawnObject();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Pillar"))
        {
            Destroy(gameObject);
        }
    }
}
