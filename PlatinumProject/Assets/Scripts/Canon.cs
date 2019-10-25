using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code créé et géré par Siméon
public class Canon : MonoBehaviour
{

    public GameObject pointToThrow;
    [Header("Rotation")]
    public float rotateSpeed = 5f;
    public bool isRotating = false;

    [Header("Éjection")]
    public float timeInsideCanon = 0f;
    public float timeToExpel = 3f;

    private Vector3 orientDir = Vector3.zero;
    public float knockPower = 10f;

    private bool isShooting = false;

    private PlayerEntity playerCollisionned;

    // Start is called before the first frame update
    void Start()
    {
        
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
            playerCollisionned.gameObject.SetActive(true);
            playerCollisionned.transform.position = pointToThrow.transform.position;
            orientDir = -transform.forward;
            Vector2 orientDirCanon = new Vector2(orientDir.x, orientDir.z);
            playerCollisionned.Knockback(orientDirCanon, knockPower);
            isShooting = false;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            isRotating = true;
            playerCollisionned = collision.gameObject.GetComponent<PlayerEntity>();
            playerCollisionned.gameObject.SetActive(false);
            playerCollisionned.GoInsideCanon(this);
        }
    }
}
