using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager managerCamera;

    //Movement
    private Transform targetPosition;
    private Vector3 basePosition;
    private bool onMovement = false;
    private float timer;
    public float timeTravel = 5f;


    private Animator animator;


    void Awake()
    {
        if (managerCamera != null)
        {

        }
        managerCamera = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (onMovement)
        {
            timer += Time.unscaledDeltaTime / timeTravel;
            transform.position = Vector3.Lerp(basePosition, targetPosition.position, timer);
        }
    }

    public void GoToPlayer(GameObject player)
    {
        onMovement = true;
        targetPosition = player.transform;
        animator.enabled = false;
    }

    public void ResetRound()
    {
        timer = 0f;
        onMovement = false;
        targetPosition = null;
        transform.position = basePosition;
        animator.enabled = true;
    }
}
