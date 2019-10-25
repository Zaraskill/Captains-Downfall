using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Code crée et géré par Corentin
public class SoundManager : MonoBehaviour
{

    public static SoundManager managerSound;

    //public AudioClip canonSound;

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
        //MakeSound(canonSound);
    }
}
