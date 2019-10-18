using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Code crée et géré par Corentin
public class PlayerEntity : MonoBehaviour
{
    // Player
    [Header("Player")]
    public int playerID = 0;
    
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
    private bool canThrow = false;
    private Vector2 throwDir;
    private float power = 0f;

    // Knockback
    private bool isKnocked = false;
    

    //Rigidbody
    [Header("Rigidbody")]
    public Rigidbody _rigidbody;

    // Debug
    [Header("Debug")]
    public bool _debugMode = false;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMove();
        UpdateModelOrient();
        UpdatePostion();
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
        GUILayout.Label(isHoldingItem ? "hold" : "empty");
        GUILayout.Label("power = " + power);
        GUILayout.EndVertical();
    }

    private void UpdatePostion()
    {
        //_rigidbody.velocity = new Vector3(Mathf.Abs(velocity.x * moveDir.x), 0, Mathf.Abs(velocity.y * moveDir.y));
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
                velocity = Vector2.zero;
            }
            else
            {
                velocity -= frictionToApply * frictionDir;
            }
        }
    }

    public void Move(Vector2 dir)
    {
        moveDir = dir;
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
        //pickedObject.GetComponent<PickupableObject>().SetPickable(false);
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
        GetComponent<Rigidbody>().mass -= pickedObject.GetComponent<Rigidbody>().mass;
        pickedObject = null;
        isHoldingItem = false;
        power = 0f;
    }

    #endregion

    #region Knockback Fonctions

    public void Knockback(Vector2 knockDir, float powerKnock)
    {
        // Récupère l'orientation de l'objet/canon ainsi que la puissance  afin d'envoyer le player dans le sens
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


    #endregion
}
