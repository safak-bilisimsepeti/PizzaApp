﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBTeamEntry : MonoBehaviour
{
    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        Camera.main.GetComponent<UIController>().SelectTeamRewards(index);
    }
}