using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : MonoBehaviour
{
    public float second = 1;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, second);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
