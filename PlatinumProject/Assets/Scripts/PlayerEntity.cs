using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;


// Code crée et géré par Corentin
public class PlayerEntity : MonoBehaviour
{
    enum STATE_DEATH {Knockbacked, Suicide}

    public PlayerController controller;

    // Player
    [Header("Player")]
    public int playerID = 0;
    public int teamID;
    
    // Move
    [Header("Move")]
    [Range(0f, 500)]  public float acceleration = 20f;
    public float moveSpeedMax = 10f;
    private Vector2 moveDir;
    private Vector2 speed = Vector2.zero;
    private Vector2 orientDir = Vector2.right;

    // Frictions
    [Header("Friction")]
    [Range(0f, 100f)] public float friction;
    [Range(0f, 500f)] public float turnFriction;

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
    private Vector3 distToTarget;
    private List<PickupableObject> listObjCanPick;

    // Throw
    [Header("Throw")]
    private bool canThrow = false;
    private Vector2 throwDir;
    private float power = 0f;

    // Knockback
    [Header("Knockback")]
    public float knockPower;
    private bool isKnocked = false;
    private Collider colliderOtherPlayer;

    //Canon
    [Header("Canon")]
    private bool isInsideCanon = false;
    private Canon canon = null;

    // Smoke Animation
    [Header("Smoke")]
    public GameObject smoke;
    public float startTimeBtwSpawns;
    public float durationTime;
    private float timeBtwSpawns;

    //Death
    [Header("Death")]
    public float multiplierKnock;
    public float distToJump;
    public AnimationCurve jumpToDie;
    private float timingJump = 0f;
    private bool isDead = false;
    private STATE_DEATH typeDeath;
    private Vector3 positionWhenDie;
    private Vector3 targetDrop;

    //Rigidbody
    [Header("Rigidbody")]
    public Rigidbody _rigidbody;

    // Debug
    [Header("Debug")]
    public bool _debugMode = false;

    //Vibration
    [Header("Vibration Manette")]
    public int motorIndex;
    public float motorLevel;
    public float duration;

    //Confettis
    [Header("Confettis")]
    public GameObject confettis;

    private Animator animator;
    private Collider colliderItemPicked = null;
    private bool _isPlayerRepulsed;


    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void Start()
    {
        listObjCanPick = new List<PickupableObject>();
        distToTarget = new Vector3(10000, 0, 0);
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

        if(colliderItemPicked != null)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<CapsuleCollider>(), colliderItemPicked, true);
        }
    }

    private void LateUpdate()
    {
        _isPlayerRepulsed = false;
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
        GUILayout.Label("" + isDead);
        GUILayout.EndVertical();
    }

    private void UpdatePosition()
    {
        if (isDead && typeDeath == STATE_DEATH.Suicide)
        {
            _rigidbody.velocity = Vector3.zero;
            timingJump += Time.fixedDeltaTime;            
            transform.position = new Vector3(Mathf.Lerp(positionWhenDie.x,targetDrop.x,timingJump), jumpToDie.Evaluate(timingJump), Mathf.Lerp(positionWhenDie.z, targetDrop.z, timingJump));
        }
        else
        {
            _rigidbody.velocity = new Vector3(speed.x, verticalSpeed, speed.y);
        }
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
        if (!isKnocked && !isDead)
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
        if (isHoldingItem || listObjCanPick.Count == 0)
        {
            return;
        }
        if (listObjCanPick.Count > 0)
        {
            foreach(PickupableObject targetedObject in listObjCanPick)
            {
                CalculDistToObj(targetedObject);
            }
            distToTarget = new Vector3(10000, 0, 0);
            pickedObject = targetObjet;
            colliderItemPicked = pickedObject.GetComponent<BoxCollider>();
            if (pickedObject.isPickable)
            {                
                pickedObject.Picked();
                targetObjet = null;
                pickedObject.transform.SetParent(modelObjs[0].transform);
                pickedObject.transform.position = pointToHold.transform.position;
                canPick = false;
                isHoldingItem = true;
                canThrow = true;
                listObjCanPick.Remove(pickedObject);
            }
        }
        
        
        
    }

    private void CalculDistToObj(PickupableObject target)
    {
        if(target != null)
        {
            Vector3 playerPos = transform.position;
            Vector3 objectPos = target.transform.position;
            Vector3 distPlayerObject = objectPos - playerPos;
            if (Vector3.Dot(orientDir, distPlayerObject) < 0)
            {
                if (distPlayerObject.x < 0)
                {
                    distPlayerObject.x = distPlayerObject.x - 100;
                }
                else
                {
                    distPlayerObject.x = distPlayerObject.x + 100;
                }
            }
            if (distPlayerObject.magnitude < distToTarget.magnitude)
            {
                distToTarget = distPlayerObject;
                targetObjet = target;
            }
        }
    }

    public void ObjectDestroyedInRange(PickupableObject pickItem)
    {
        if (listObjCanPick.Contains(pickItem))
        {
            listObjCanPick.Remove(pickItem);
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
            if(listObjCanPick.Count > 0)
            {
                canPick = true;
            }
            animator.SetBool("ThrowBool", true);
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
        animator.SetBool("HitBool", true);
        controller.mainPlayer.SetVibration(motorIndex, motorLevel, duration);
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
        if (other.tag == "Pickable" && other.GetComponent<PickupableObject>().IsPickable())
        {
            listObjCanPick.Add(other.GetComponent<PickupableObject>());
            other.GetComponent<PickupableObject>().NewPlayerInRange(this);
            canPick = true;
        }
        else if (other.gameObject.tag == "DeathZone" && !isDead)
        {
            if (isKnocked)
            {
                typeDeath = STATE_DEATH.Knockbacked;
            }
            else
            {
                typeDeath = STATE_DEATH.Suicide;
            }
            TimeToDie();
        }
        else if (other.gameObject.CompareTag("OutZone") && isDead)
        {
                Vector3 centerScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.transform.position.z);
                Vector3 centerWorldPosition = Camera.main.ScreenToWorldPoint(centerScreenPosition);

                GameObject _instance = Instantiate(confettis, transform.position, Quaternion.identity);
                _instance.transform.LookAt(centerWorldPosition);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickable" && !isHoldingItem)
        {
            other.GetComponent<PickupableObject>().PlayerLeaveRange(this);
            listObjCanPick.Remove(other.GetComponent<PickupableObject>());
            if (listObjCanPick.Count == 0)
            {
                canPick = false;
            }
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
        else if (collision.gameObject.CompareTag("Player") && isKnocked && !_isPlayerRepulsed)
        {
            PlayerEntity otherPlayer = collision.gameObject.GetComponent<PlayerEntity>();
            otherPlayer.Knockback(speed.normalized, speed.magnitude);
            HittingWall();
            otherPlayer._isPlayerRepulsed = true;
            _isPlayerRepulsed = true;
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
        if (moveDir != Vector2.zero && !isDead)
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
            if (typeDeath == STATE_DEATH.Knockbacked)
            {                
                if (speed == Vector2.zero)
                {
                    speed = orientDir * multiplierKnock * multiplierKnock;
                }
                speed *= multiplierKnock;
            }
            else if (typeDeath == STATE_DEATH.Suicide)
            {
                _rigidbody.velocity = Vector3.zero;
                positionWhenDie = transform.position;
                Vector3 distJump = orientDir * distToJump;
                targetDrop = new Vector3(positionWhenDie.x + distJump.x, positionWhenDie.y, positionWhenDie.z + distJump.y);
            }
            Debug.Log(typeDeath);
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
        isKnocked = false;listObjCanPick.Clear();
        groundY = 0f;
        speed = Vector2.zero;
        orientDir = Vector2.right;
    }

    #endregion

    
}
