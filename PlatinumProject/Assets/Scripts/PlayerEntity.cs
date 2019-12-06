using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;


// Code crée et géré par Corentin
public class PlayerEntity : MonoBehaviour
{
    // Player
    [Header("Player")]
    public int playerID = 0;
    public int teamID;
    private bool isDead = false;
    public TextMesh text;
    
    
    // Move
    [Header("Move")]
    [Range(0f, 100f)]  public float acceleration = 20f;
    public float moveSpeedMax = 10f;
    private Vector2 moveDir;
    private Vector2 speed = Vector2.zero;
    private Vector2 orientDir = Vector2.right;

    // Frictions
    [Header("Friction")]
    [Range(0f, 100f)] public float friction;
    [Range(0f, 100f)] public float turnFriction;

    // Gravity
    [Header("Gravity")]
    public float gravity = 20f;
    private float verticalSpeed = 0f;
    public float verticalSpeedMax = 10f;

    // Ground
    [Header("Ground")]
    public Transform groundCheckPoint;
    public float groundY = 0f;
    private bool isGrounded = false;

    // Object Models
    [Header("Models")]
    public List<GameObject> modelObjs;

    // Pick Up
    [Header("Pick Up")]
    public PickupableObject pickedObject;
    public GameObject pointToHold;
    private bool canPick = false;
    private bool isHoldingItem = false;
    private PickupableObject targetObjet;


    // Throw
    [Header("Throw")]
    private bool canThrow = false;
    private Vector2 throwDir;
    private float power = 0f;

    // Knockback
    private bool isKnocked = false;

    //Canon
    private bool isInsideCanon = false;
    private Canon canon = null;

    // Smoke Animation
    [Header("Smoke")]
    public GameObject smoke;
    public float startTimeBtwSpawns;
    public float durationTime;
    private float timeBtwSpawns;

    //Rigidbody
    [Header("Rigidbody")]
    public Rigidbody _rigidbody;

    // Debug
    [Header("Debug")]
    public bool _debugMode = false;

    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateGroundCheck();
        UpdateGravity();
        UpdateMove();
        UpdateModelOrient();
        UpdatePosition();
        UpdateSmoke();

        if(pickedObject == null)
        {
            isHoldingItem = false;
        }
    }

    private void OnGUI()
    {
        if (!_debugMode)
        {
            return;
        }

        GUILayout.BeginVertical();
        GUILayout.Label("Speed = " + speed);
        GUILayout.Label("Team = " + teamID);
        GUILayout.Label("moveDir = " + moveDir);
        GUILayout.Label(canPick ? "canPick" : "cantPick");
        GUILayout.Label(isInsideCanon ? "inCanon" : "outCanon");
        GUILayout.Label(isHoldingItem ? "hold" : "empty");
        GUILayout.Label("power = " + power);
        GUILayout.EndVertical();
    }

    private void UpdatePosition()
    {
        _rigidbody.velocity = new Vector3(speed.x, verticalSpeed, speed.y);
    }


    #region Move Fonctions

    private void UpdateMove()
    {
        if (moveDir != Vector2.zero)
        {
            float turnAngle = Vector2.SignedAngle(speed, moveDir);
            turnAngle = Mathf.Abs(turnAngle);
            float frictionRatio = turnAngle / 180f;
            float turnFrictionWithRatio = turnFriction * frictionRatio;

            speed += moveDir * acceleration * Time.fixedDeltaTime;
            if (speed.sqrMagnitude > moveSpeedMax * moveSpeedMax && !isKnocked)
            {
                speed = speed.normalized * moveSpeedMax;
            }

            Vector2 frictionDir = speed.normalized;
            speed -= frictionDir * turnFrictionWithRatio * Time.fixedDeltaTime;

            orientDir = speed.normalized;
        }
        else if (speed != Vector2.zero)
        {
            Vector2 frictionDir = speed.normalized;
            float frictionToApply = friction * Time.fixedDeltaTime;
            if (speed.sqrMagnitude <= frictionToApply * frictionToApply)
            {
                isKnocked = false;
                speed = Vector2.zero;
            }
            else
            {
                speed -= frictionToApply * frictionDir;
            }
        }
        else if (speed == Vector2.zero)
        {
            isKnocked = false;
        }
    }

    public void Move(Vector2 dir)
    {
        if (!isKnocked)
        {
            moveDir = dir;
        }
    }

    private void UpdateModelOrient()
    {
        float angle = -Vector2.SignedAngle(Vector2.right, orientDir);
        Vector3 eulerAngles = modelObjs[0].transform.eulerAngles;
        eulerAngles.y = angle;
        modelObjs[0].transform.eulerAngles = eulerAngles;
    }

    #endregion

    #region Gravity Fonctions

    private void UpdateGravity()
    {
        if (isGrounded)
        {
            return;
        }
        verticalSpeed -= gravity * Time.fixedDeltaTime;
        if (verticalSpeed < -verticalSpeedMax)
        {
            verticalSpeed = -verticalSpeedMax;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    #endregion

    #region Ground Fonctions

    private void UpdateGroundCheck()
    {
        if (groundCheckPoint.position.y < groundY)
        {
            isGrounded = true;
            verticalSpeed = 0f;
            float offsetY = groundCheckPoint.position.y - transform.position.y;
            transform.position = new Vector3(transform.position.x, groundY - offsetY, transform.position.z);
        }
        else
        {
            isGrounded = false;
        }
    }

    #endregion

    #region PickUp Fonctions

    public bool CanPick()
    {
        return canPick;
    }

    public bool IsHoldingItem()
    {
        return isHoldingItem;
    }

    public void PickItem()
    {
        if (isHoldingItem)
        {
            return;
        }
        pickedObject = targetObjet;
        if (pickedObject.isPickable)
        {
            pickedObject.Picked();
            targetObjet = null;
            pickedObject.transform.SetParent(modelObjs[0].transform);
            pickedObject.transform.position = pointToHold.transform.position;
            canPick = false;
            isHoldingItem = true;
            canThrow = true;
        }
    }

    #endregion

    #region Throw Fonctions

    public bool CanThrow()
    {
        return canThrow;
    }

    public void Throw()
    {
        if(canThrow)
        {
            pickedObject.transform.parent = null;
            pickedObject.Throw(orientDir);
            pickedObject = null;
            isHoldingItem = false;
            canThrow = false;
        }
        
    }

    #endregion

    #region Knockback Fonctions

    public void Knockback(Vector2 knockDir, float powerKnock)
    {
        //SoundManager.managerSound.MakeHitSound();
        isKnocked = true;
        orientDir = knockDir;
        moveDir = Vector2.zero;
        speed = knockDir * powerKnock;
    }

    public bool IsKnocked()
    {
        return isKnocked;
    }

    #endregion

    #region Canon Fonctions

    public void GoInsideCanon(Canon canon)
    {
        this.canon = canon;
        if (isHoldingItem)
        {
            isHoldingItem = false;
            Destroy(pickedObject.gameObject);
            GameManager.managerGame.SpawnObject();
            pickedObject = null;
            canThrow = false;
            canPick = false;
        }
        isInsideCanon = true;
    }

    public void QuitCanon()
    {
        isInsideCanon = false;
        canPick = true;
        canon.ForcedEjection();
        canon = null;
    }

    public bool IsInsideCanon()
    {
        return isInsideCanon;
    }

    public void OutCanon()
    {
        isInsideCanon = false;
    }

    #endregion

    #region Collisions/Trigger Fonctions

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickable" && !isHoldingItem)
        {
            PickupableObject objectPick = other.GetComponent<PickupableObject>();
            if (objectPick.IsPickable())
            {
                canPick = true;
                targetObjet = objectPick;
            }

        }
        else if (other.gameObject.tag == "DeathZone" && !isDead)
        {
            groundY = -2f;
            TimeToDie();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickable" && !isHoldingItem)
        {
            canPick = false;
            targetObjet = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DeathZone")
        {
            SoundManager.managerSound.MakeDeathSound();
            CameraShaker.Instance.ShakeOnce(1f, 1f, 0.1f, 1f);
            TimeToDie();
        }
        else if (collision.gameObject.tag == "Wall")
        {
            BreakableWalls wall = collision.gameObject.GetComponent<BreakableWalls>();
            if (IsKnocked())
            {
                wall.TakeDamage();
            }
            HittingWall();
            
        }
        else if (collision.gameObject.tag == "Pillar")
        {
            HittingWall();
        }
        else if (collision.gameObject.tag == "Pickable" && !collision.gameObject.GetComponent<PickupableObject>().isPickable)
        {
            SoundManager.managerSound.MakeHitSound();
        }
    }

    public void HittingWall()
    {
        isKnocked = false;
        speed = Vector2.zero;
        _rigidbody.velocity = Vector3.zero;
    }

    #endregion

    #region Animations Smoke Fonctions

    private void UpdateSmoke()
    {
        if (moveDir != Vector2.zero)
        {
            GameObject _instance = Instantiate(smoke, transform.position, Quaternion.identity);
            Destroy(_instance, 2f);
        }
    }

    #endregion

    #region Death Fonctions

    private void TimeToDie()
    {
        if (!isDead)
        {
            isDead = true;
            GameManager.managerGame.DeadPlayer(playerID);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Respawn()
    {
        isDead = false;
        groundY = 0f;
        speed = Vector2.zero;
    }

    #endregion

}
