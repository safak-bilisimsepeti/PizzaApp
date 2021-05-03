using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerMannequin : MonoBehaviour
{
    bool move = false;
    bool left = false;
    Vector3 origin = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            if (left)
            {
                if (transform.position.x - origin.x < -20)
                {
                    left = false;
                }
                else
                {
                    transform.Translate(new Vector3(Time.deltaTime * -10, 0, 0));
                }
            }
            else
            {
                if (transform.position.x - origin.x > 20)
                {
                    left = true;
                }
                else
                {
                    transform.Translate(new Vector3(Time.deltaTime * 10, 0, 0));
                }
            }
        }
    }

    public void Init(bool Move = false)
    {
        origin = transform.position;
        move = Move;
    }
}
