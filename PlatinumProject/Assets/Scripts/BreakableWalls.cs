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

    [Header("Components")]
    private Animator myAnimator;

    public GameObject destructionEffect;

    // Start is called before the first frame update
    void Start()
    {
        currentHealthPoints = startHealthPoints;
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealthPoints == 1)
        {
            myAnimator.SetTrigger("1HPLeft");
        }
        else if (currentHealthPoints == 0)
        {
            SoundManager.managerSound.MakeWallBreakSound();
            CameraShaker.Instance.ShakeOnce(1f, 1f, 0.1f, 1f);
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        currentHealthPoints--;
    }

}
