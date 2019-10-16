using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool canPick = false;
    public GameObject pickedObject;
    public GameObject pointToHold;
    private Vector3 positionHolded;

    // Throw
    [Header("Throw")]
    private bool canThrow = false;
    private Vector3 throwDir;
    private float power = 0f;
    public float powerMax = 10f;

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
        positionHolded = pointToHold.transform.position;
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
        GUILayout.Label("Pos spawn = " + positionHolded);
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
            if (velocity.sqrMagnitude > moveSpeedMax * moveSpeedMax)
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



    #endregion

    #region Throw Fonctions



    #endregion

}
