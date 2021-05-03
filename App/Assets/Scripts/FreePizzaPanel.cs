using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePizzaPanel : MonoBehaviour
{
    public UnityEngine.UI.Image progressImage;
    public UnityEngine.UI.Text Quantity;

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        progressImage.fillAmount = Local.user.freeSlices / 24f;
        Quantity.text = Local.user.freeSlices.ToString() + " / 24";
        UnityEngine.UI.Text text = transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
        text.text = Language.Translate(text.text);
    }
}
