using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWalls : MonoBehaviour
{
    public int startHealthPoints = 2;
    [SerializeField]
    private int currentHealthPoints;
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
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            currentHealthPoints -= 1;

            if (currentHealthPoints == 1)
            {
                myAnimator.SetTrigger("1HPLeft");
            }
            else if (currentHealthPoints == 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
