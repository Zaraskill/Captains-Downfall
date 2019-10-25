using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Code crée et géré par Corentin
public class SoundManager : MonoBehaviour
{

    public static SoundManager managerSound;

    public AudioClip canonSound;
    public AudioClip wallSound;
    public AudioClip wallBreakSound;
    public AudioClip hitSound;
    public AudioClip deathSound;


    // Start is called before the first frame update
    void Awake()
    {
        managerSound = this;
        //if (managerSound != null)
        //{
        //    managerSound = this;
        //}
        //else
        //{
        //    Debug.LogError("Too many instances!");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MakeSound(AudioClip originalClip)
    {
        AudioSource.PlayClipAtPoint(originalClip, transform.position);
    }

    public void MakeCanonSound()
    {
        MakeSound(canonSound);
    }

    public void MakeWallHitSound()
    {
        MakeSound(wallSound);
    }

    public void MakeWallBreakSound()
    {
        MakeSound(wallBreakSound);
    }

    public void MakeHitSound()
    {
        MakeSound(hitSound);
    }

    public void MakeDeathSound()
    {
        MakeSound(deathSound);
    }
}
