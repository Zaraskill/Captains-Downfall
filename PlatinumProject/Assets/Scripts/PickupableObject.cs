using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code créé et géré par Siméon
public class PickupableObject : MonoBehaviour
{
    [Header("Pick & Throw")]
    public float powerKnock = 50f;
    public bool isPickable = true;
    private bool isHolded = false;
    public bool isThrown = false;
    private Vector2 orient = Vector2.zero;
    private float timing = 0f;
    public AnimationCurve curveThrow;

    [Header("Speed")]
    public float verticalSpeedOn = 10f;
    public float maxSpeedToPick = 2f;
    public float speedTravel = 20f;
    private Vector2 velocity = Vector2.zero;
    private float verticalSpeed = 0f;

    [Header("Ground")]
    [Range(0f, 1000f)] public float friction;
    public float groundY = 0f;
    private bool isGrounded = false;
    public Transform groundPosition;

    [Header("Spawn")]
    private float timerSpawn = 5f;
    private float timer;

    [Header("Barrel")]
    private bool isInRangeBarrel = false;
    private Barrel barrel;

    [Header("Components")]
    private Rigidbody _rigidbody;

    public GameObject hitParticle;

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
        UpdateMove();
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _rigidbody.velocity = new Vector3(velocity.x, verticalSpeed, velocity.y);
    }

    private void UpdateMove()
    {
        if (isThrown)
        {
            timing += Time.fixedDeltaTime;
            transform.position = new Vector3(transform.position.x, curveThrow.Evaluate(timing), transform.position.z);
        }
        if (isGrounded)
        {
            timing = 0f;
            if (velocity != Vector2.zero)
            {
                Vector2 frictionDir = velocity.normalized;
                float frictionToApply = friction * Time.fixedDeltaTime;
                if (velocity.sqrMagnitude <= frictionToApply * frictionToApply)
                {
                    velocity = Vector2.zero;
                }
                else
                {
                    velocity -= frictionToApply * frictionDir;
                }
            }
            else
            {
                isThrown = false;
            }
            if (velocity.sqrMagnitude <= (maxSpeedToPick * maxSpeedToPick) && isThrown)
            {
                isPickable = true;
                GetComponent<SphereCollider>().enabled = true;
            }
        }
    }

    private void UpdateGroundCheck()
    {
        if (isHolded)
        {
            verticalSpeed = 0f;
        }
        else if (groundPosition.position.y <= groundY)
        {
            isGrounded = true;
            verticalSpeed = 0f;
            float offsetY = groundPosition.position.y - transform.position.y;

            transform.position = new Vector3(transform.position.x, groundY - offsetY, transform.position.z);
        }
        else
        {
            isGrounded = false;
            verticalSpeed = -verticalSpeedOn;
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
        velocity = orient * speedTravel;
    }

    public void Picked()
    {
        if (isPickable)
        {
            isPickable = false;
            isHolded = true;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            if (isInRangeBarrel)
            {
                barrel.objectLeaveRange(this);
                ExitInsideRangeBarrel();
            }
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

    public void GoInsideRangeBarrel(Barrel barrel)
    {
        isInRangeBarrel = true;
        this.barrel = barrel;
    }

    public void ExitInsideRangeBarrel()
    {
        isInRangeBarrel = false;
        barrel = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isThrown)
        {
            if(hitParticle != null)
            {
                GameObject _instance = Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(_instance, 2f);
            }
            SoundManager.managerSound.MakeHitSound();
            collision.gameObject.GetComponent<PlayerEntity>().Knockback(orient, powerKnock);
            GameManager.managerGame.SpawnObject(); 
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Pillar") || collision.gameObject.CompareTag("Canon"))
        {
            if (hitParticle != null)
            {
                GameObject _instance = Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(_instance, 2f);
            }
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
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            GameManager.managerGame.SpawnObject();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Pickable"))
        {
            isThrown = false;
            _rigidbody.velocity = Vector3.zero;
            velocity = Vector2.zero;
            collision.gameObject.GetComponent<PickupableObject>().Throw(orient);
            GetComponent<SphereCollider>().enabled = true;
            isPickable = true;
        }
        else if (collision.gameObject.CompareTag("Ground") && isThrown)
        {
            if (hitParticle != null)
            {
                GameObject _instance = Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(_instance, 2f);
            }
            GameManager.managerGame.SpawnObject();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DeathZone")
        {
            GameManager.managerGame.SpawnObject();
            Destroy(gameObject);
        }
    }
}
