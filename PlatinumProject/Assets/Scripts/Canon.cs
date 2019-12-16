using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    //Vibration
    [Header("Vibration Manette")]
    public int motorIndex;
    public float motorLevel;
    public float duration;

    public List<Sprite> UIPlayerInCanon; 

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRotating)
        {
            UpdateRotate();
            RotateCanon();
        }
        else
        {
            Color c = GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color;
            c.a = 0;
            GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = c;
        }
    }

    private void UpdateRotate()
    {
        transform.GetChild(0).Rotate(0, rotateSpeed, 0);
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
        isRotating = false;
        timeInsideCanon = 0f;
        playerCollisionned.OutCanon();
        SoundManager.managerSound.MakeCanonSound();
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        animator.SetBool("isShooting", true);
        GameObject _instance = Instantiate(smokeEffect, pointToThrow.transform.position, Quaternion.identity);
        Destroy(_instance, 2f);
        playerCollisionned.gameObject.SetActive(true);
        playerCollisionned.OutCanon();
        playerCollisionned.transform.position = pointToThrow.transform.position;
        orientDir = transform.GetChild(0).transform.forward;
        Vector2 orientDirCanon = new Vector2(orientDir.x, orientDir.z);
        playerCollisionned.Knockback(orientDirCanon, knockPower);
        playerCollisionned.controller.mainPlayer.SetVibration(motorIndex, motorLevel, duration);
        isShooting = false;
        canEnter = true;
        playerCollisionned = null;
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
            DisplayCanonUI();
        }
    }

    private void DisplayCanonUI()
    {
        if (playerCollisionned != null)
        {
            for(int i = 0; i < UIPlayerInCanon.Count; i++)
            {
                if (playerCollisionned.playerID == i)
                {
                    GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().sprite = UIPlayerInCanon[i];
                }
            }
            if(playerCollisionned.teamID == 1)
            {
                GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = Color.blue;
            }
            else if (playerCollisionned.teamID == 2)
            {
                GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = Color.red;
            }
            else if (playerCollisionned.teamID == 0)
            {
                switch(playerCollisionned.playerID)
                {
                    case 0:
                        GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = Color.blue;
                        break;
                    case 1:
                        GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = Color.red;
                        break;
                    case 2:
                        GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = Color.green;
                        break;
                    case 3:
                        GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = Color.yellow;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
