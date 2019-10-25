using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Code crée et géré par Corentin
public class PlayerEntity : MonoBehaviour
{
    // Player
    [Header("Player")]
    public int playerID = 0;
    private bool isDead = false;
    
    // Move
    [Header("Move")]
    [Range(0f, 100f)]  public float acceleration = 20f;
    public float moveSpeedMax = 10f;
    private Vector2 moveDir;
    private Vector2 velocity = Vector2.zero;
    private Vector2 orientDir = Vector2.right;

    // Frictions
    [Header("Friction")]
    [Range(0f, 100f)] public float friction;
    [Range(0f, 100f)] public float turnFriction;

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
    public float powerUpgrade = 50f;
    [Range(0f, 150f)]  public float powerMax = 20f;
    private bool isChargingPower = false;
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

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            UpdateMove();
            UpdateModelOrient();
            UpdatePosition();
            UpdateSmoke();
        }
    }

    private void OnGUI()
    {
        if (!_debugMode)
        {
            return;
        }

        GUILayout.BeginVertical();
        GUILayout.Label("Velocity = " + velocity);
        GUILayout.Label("moveDir = " + moveDir);
        GUILayout.Label(canPick ? "canPick" : "cantPick");
        GUILayout.Label(isInsideCanon ? "inCanon" : "outCanon");
        GUILayout.Label(isHoldingItem ? "hold" : "empty");
        GUILayout.Label("power = " + power);
        GUILayout.EndVertical();
    }

    private void UpdatePosition()
    {
        Vector3 movePosition = transform.position;
        movePosition.x += velocity.x * Time.fixedDeltaTime;
        movePosition.z += velocity.y * Time.fixedDeltaTime;
        transform.position = movePosition;
    }


    #region Move Fonctions

    private void UpdateMove()
    {
        if (moveDir != Vector2.zero)
        {
            float turnAngle = Vector2.SignedAngle(velocity, moveDir);
            turnAngle = Mathf.Abs(turnAngle);
            float frictionRatio = turnAngle / 180f;
            float turnFrictionWithRatio = turnFriction * frictionRatio;

            velocity += moveDir * acceleration * Time.fixedDeltaTime;
            if (velocity.sqrMagnitude > moveSpeedMax * moveSpeedMax && !isKnocked)
            {
                velocity = velocity.normalized * moveSpeedMax;
            }

            Vector2 frictionDir = velocity.normalized;
            velocity -= frictionDir * turnFrictionWithRatio * Time.fixedDeltaTime;

            orientDir = velocity.normalized;
        }
        else if (velocity != Vector2.zero)
        {
            Vector2 frictionDir = velocity.normalized;
            float frictionToApply = friction * Time.fixedDeltaTime;
            if (velocity.sqrMagnitude <= frictionToApply * frictionToApply)
            {
                isKnocked = false;
                velocity = Vector2.zero;
            }
            else
            {
                velocity -= frictionToApply * frictionDir;
            }
        }
        else if (velocity == Vector2.zero)
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
        modelObjs[1].transform.eulerAngles = eulerAngles;
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
        pickedObject.GetComponent<Rigidbody>().useGravity = false;
        pickedObject.GetComponent<PickupableObject>().SetPickable(false);
        pickedObject.GetComponent<BoxCollider>().enabled = false;
        targetObjet = null;
        pickedObject.transform.SetParent(modelObjs[0].transform);
        pickedObject.transform.position = pointToHold.transform.position;        
        canPick = false;
        isHoldingItem = true;
        GetComponent<Rigidbody>().mass += pickedObject.GetComponent<Rigidbody>().mass;


    }

    #endregion

    #region Throw Fonctions

    public bool CanThrow()
    {
        return canThrow;
    }

    public void StartChargingPower()
    {
        isChargingPower = true;
    }

    public bool IsChargingPower()
    {
        return isChargingPower;
    }

    public void ImprovePower()
    {
        if (power >= powerMax)
        {
            power = powerMax;
            return;
        }
        power += powerUpgrade * Time.deltaTime;
    }

    public void Throw()
    {
        pickedObject.transform.parent = null;
        pickedObject.Throw(orientDir, power);
        pickedObject.GetComponent<Rigidbody>().useGravity = true;
        pickedObject.GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().mass -= pickedObject.GetComponent<Rigidbody>().mass;
        pickedObject = null;
        isHoldingItem = false;
        isChargingPower = false;
        power = 0f;
    }

    #endregion

    #region Knockback Fonctions

    public void Knockback(Vector2 knockDir, float powerKnock)
    {
        SoundManager.managerSound.MakeHitSound();
        isKnocked = true;
        orientDir = knockDir;
        moveDir = Vector2.zero;
        velocity = knockDir * powerKnock;
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
            isChargingPower = false;
            canPick = true;
        }
        isInsideCanon = true;
    }

    public void QuitCanon()
    {
        isInsideCanon = false;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DeathZone")
        {
            SoundManager.managerSound.MakeDeathSound();
            TimeToDie();
        }
    }

    public void HittingWall()
    {
        isKnocked = false;
        velocity = Vector2.zero;
    }

    #endregion

    #region Animations Smoke Fonctions

    private void UpdateSmoke()
    {
        if (moveDir != Vector2.zero)
        {
            if (timeBtwSpawns <= 0)
            {
                //spawn Smoke game object
                GameObject instance = (GameObject)Instantiate(smoke, transform.position, Quaternion.identity);
                Destroy(instance, durationTime);
                timeBtwSpawns = startTimeBtwSpawns;
            }
            else
            {
                timeBtwSpawns -= Time.deltaTime;
            }
        }
    }

    #endregion

    #region Death Fonctions

    private void TimeToDie()
    {
        isDead = true;
        GameManager.managerGame.DeadPlayer(playerID);
    }

    #endregion

}
