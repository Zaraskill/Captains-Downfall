using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    // Move
    [Header("Move")]
    [Range(0f, 100f)]  public float acceleration = 20f;
    public float moveSpeedMax = 10f;
    private Vector2 moveDir;
    private Vector2 velocity = Vector2.zero;

    // Frictions
    [Header("Friction")]
    [Range(0f, 100f)] public float friction;
    [Range(0f, 100f)] public float turnFriction;

    private Vector2 orientDir = Vector2.right;
    public GameObject modelObj;
    public GameObject modelObj2;

    //Rigidbody
    [Header("Rigidbody")]
    public Rigidbody rigidbody;

    // Debug
    [Header("Debug")]
    public bool _debugMode = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMove();
        UpdateModelOrient();

        Vector3 movePosition = transform.position;
        movePosition.x += velocity.x * Time.fixedDeltaTime;
        movePosition.z += velocity.y * Time.fixedDeltaTime;
        transform.position = movePosition;
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
        GUILayout.EndVertical();
    }

    private void UpdateModelOrient()
    {
        float angle =  -Vector2.SignedAngle(Vector2.right, orientDir);
        Vector3 eulerAngles = modelObj.transform.eulerAngles;
        eulerAngles.y = angle;
        modelObj.transform.eulerAngles = eulerAngles;
        modelObj2.transform.eulerAngles = eulerAngles;
    }

    public void Move(Vector2 dir)
    {
        moveDir = dir;
    }

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
}
