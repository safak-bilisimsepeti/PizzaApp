using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowLines : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int slices, int free)
    {
        float angle1 = -(slices * 15);
        float angle2 = -((slices + free) * 15);
        transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle1));
        transform.GetChild(1).rotation = Quaternion.Euler(new Vector3(0, 0, angle2));
    }
}
