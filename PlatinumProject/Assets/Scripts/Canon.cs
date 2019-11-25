using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

// Code créé et géré par Siméon
public class Canon : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 5f;
    public bool isRotating = false;
    private bool canEnter = true;

    [Header("Éjection")]
    public float knockPower = 10f;
    public GameObject pointToThrow;
    public GameObject smokeEffect;
    public float timeInsideCanon = 0f;
    public float timeToExpel = 3f;
    private bool isShooting = false;
    private Vector3 orientDir = Vector3.zero;

    [Header("Camera Shaker")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    [Header("Components")]
    private PlayerEntity playerCollisionned;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRotating)
        {
            UpdateRotate();
            RotateCanon();
        }

        if (isShooting)
        {
            SoundManager.managerSound.MakeCanonSound();
            CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
            animator.SetBool("isShooting", true);
            Instantiate(smokeEffect, pointToThrow.transform.position, Quaternion.identity);
            playerCollisionned.gameObject.SetActive(true);
            playerCollisionned.OutCanon();
            playerCollisionned.transform.position = pointToThrow.transform.position;
            orientDir = transform.forward;
            Vector2 orientDirCanon = new Vector2(orientDir.x, orientDir.z);
            playerCollisionned.Knockback(orientDirCanon, knockPower);
            isShooting = false;
            canEnter = true;
        }
    }

    private void UpdateRotate()
    {
        transform.Rotate(0, rotateSpeed, 0);
    }

    private void RotateCanon()
    {
        timeInsideCanon += Time.fixedDeltaTime;
        if (timeInsideCanon > timeToExpel)
        {
            ForcedEjection();
        }
    }

    public void ForcedEjection()
    {
        isShooting = true;
        isRotating = false;
        timeInsideCanon = 0f;
        playerCollisionned.OutCanon();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canEnter)
        {
            canEnter = false;
            animator.SetBool("isShooting", false);
            isRotating = true;
            playerCollisionned = collision.gameObject.GetComponent<PlayerEntity>();
            playerCollisionned.gameObject.SetActive(false);
            playerCollisionned.GoInsideCanon(this);
        }
    }
}
