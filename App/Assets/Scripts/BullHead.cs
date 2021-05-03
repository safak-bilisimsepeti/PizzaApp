using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullHead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(bool move)
    {
        transform.GetChild(0).GetComponent<Animator>().SetBool("Move", move);
        transform.GetChild(0).GetComponent<Animator>().speed = 1;
    }

    public void Hit()
    {
        transform.GetChild(0).GetComponent<Animator>().speed = 0;
        GetComponent<AudioSource>().Play();
    }
}
