using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

// Code créé et géré par Siméon
public class BreakableWalls : MonoBehaviour
{
    [Header("Health Points")]
    public int startHealthPoints = 2;
    [SerializeField]
    private int currentHealthPoints;

    [Header("Particules")]
    public GameObject destructionEffect;

    [Header("Camera Shaker")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    [Header("Components")]
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        currentHealthPoints = startHealthPoints;
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealthPoints == 0)
        {
            SoundManager.managerSound.MakeWallBreakSound();
            CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
            GameObject _instance = Instantiate(destructionEffect, transform.position, Quaternion.identity);
            Destroy(_instance, 2f);
            this.gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        currentHealthPoints--;
    }

    public void Rebuilt()
    {
        currentHealthPoints = startHealthPoints;
    }

}
