using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gift : MonoBehaviour
{
    public AudioClip claim;
    public AudioClip gain;
    int freeSlices = 0;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.UI.Text text = transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        text.text = Language.Translate(text.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int FreeSlices)
    {
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        freeSlices = FreeSlices;
        transform.SetParent(canvas);
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        GetComponent<RectTransform>().offsetMax = Vector2.zero;

        transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "x" + freeSlices.ToString();
    }

    public void ClaimGift()
    {
        transform.GetChild(0).GetChild(2).GetComponent<UnityEngine.UI.Button>().interactable = false;

        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();

        GameObject Gift = transform.GetChild(0).GetChild(3).gameObject;

        GameObject FreePizzaPanel;

        if (SceneManager.GetActiveScene().name == "App")
        {
            FreePizzaPanel = canvas.GetComponent<PageController>().GetPage("FreePizza").gameObject;
        }
        else
        {
            FreePizzaPanel = Instantiate(Resources.Load<GameObject>("FreePizza"));
            FreePizzaPanel.transform.SetParent(canvas);
            FreePizzaPanel.transform.localPosition = Vector3.zero;
            FreePizzaPanel.transform.rotation = Quaternion.identity;
            FreePizzaPanel.transform.localScale = Vector3.one;
            FreePizzaPanel.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            FreePizzaPanel.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }

        Gift.transform.SetParent(canvas);
        transform.GetComponent<Animator>().Play("Out");

        GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneObject");
        foreach (GameObject obj in objs)
        {
            obj.GetComponent<Animator>().Play("Out");
            Destroy(obj, 3);
        }

        Gift.GetComponent<Animator>().SetBool("Fly", true);
        Destroy(Gift, 3);

        GetComponent<AudioSource>().PlayOneShot(claim);

        StartCoroutine(FX(FreePizzaPanel));
    }

    IEnumerator FX(GameObject Panel)
    {
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        yield return new WaitForSeconds(2.75f);

        GameObject shine = Instantiate(Resources.Load<GameObject>("SmallShine"));
        shine.transform.SetParent(Panel.transform.GetChild(0));
        shine.transform.localPosition = Vector3.zero;
        shine.transform.rotation = Quaternion.identity;
        shine.transform.localScale = Vector3.one;
        Destroy(shine, 0.5f);

        GetComponent<AudioSource>().PlayOneShot(gain, 1);

        yield return new WaitForSeconds(0.25f);

        GameObject lines = Instantiate(Resources.Load<GameObject>("GlowLines"));
        lines.transform.SetParent(Panel.transform.GetChild(0));
        lines.transform.localPosition = Vector3.zero;
        lines.transform.rotation = Quaternion.identity;
        lines.transform.localScale = Vector3.one;
        lines.GetComponent<GlowLines>().Init(Local.user.freeSlices, freeSlices);
        Destroy(lines, 5);

        Panel.GetComponent<FreePizzaPanel>().Quantity.text = (Local.user.freeSlices + freeSlices).ToString() + " / 24";
        UnityEngine.UI.Image image = Panel.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>();

        bool old = false;
        for (int i = 0; i < 5; i++)
        {
            if (old)
            {
                image.fillAmount = Local.user.freeSlices / 24.0f;
                old = false;
            }
            else
            {
                image.fillAmount = (Local.user.freeSlices + freeSlices) / 24.0f;
                old = true;
            }
            
            yield return new WaitForSeconds(0.25f);
        }

        Local.user.freeSlices += freeSlices;
        Panel.GetComponent<FreePizzaPanel>().Refresh();

        if (SceneManager.GetActiveScene().name != "App")
        {
            yield return new WaitForSeconds(1f);
            Panel.GetComponent<Animator>().Play("Out");
            Destroy(Panel, 1);
        }
        
        Destroy(gameObject);
    }
}
