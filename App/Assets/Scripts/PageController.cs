using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PageController : MonoBehaviour
{
    public Page[] pages;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RectTransform GetPage(string name)
    {
        return (from page in pages where page.name == name select page.rect).First();
    }

    public void ShowPage(string name)
    {
        foreach (Page page in pages)
        {
            page.rect.gameObject.SetActive(page.name == name);
        }
    }
}

[System.Serializable]
public class Page
{
    public string name;
    public RectTransform rect;
}