using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinGame : MonoBehaviour
{
    public AudioClip tick;
    public GameObject[] Rewards;
    int[] slots = new int[] { 0, 1, 3, 5, 7 };
    int freeSlices = 0;
    float speed = 0;
    bool stop = false;
    float target = 0;
    float timeScale = 0;
    float tickTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (speed > 0)
        {
            transform.GetChild(0).GetChild(0).Rotate(new Vector3(0, 0, speed * timeScale));

            if (stop)
            {
                speed = Mathf.Clamp(speed - (0.02f * timeScale), 1f, 6);
                
                if (speed <= 1.05f && Quaternion.Angle(transform.GetChild(0).GetChild(0).rotation, Quaternion.Euler(new Vector3(0, 0, target))) < 5)
                {
                    target = 0;
                    speed = 0;
                    if (freeSlices > 0)
                    {
                        GameObject w = Instantiate(Resources.Load<GameObject>("GiftPanel"));
                        w.GetComponent<Gift>().Init(freeSlices);
                    }
                    else
                    {
                        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
                        GameObject w = Instantiate(Resources.Load<GameObject>("LosePanel"));
                        w.transform.SetParent(canvas);
                        w.transform.localPosition = Vector3.zero;
                        w.transform.rotation = Quaternion.identity;
                        w.transform.localScale = Vector3.one;
                        w.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                        w.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                        Destroy(w, 4);
                        StartCoroutine(Lose());
                    }
                }
            }

            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0)
            {
                tickTimer = 0.75f / speed;
                GetComponent<AudioSource>().pitch = (2f - tickTimer);
                GetComponent<AudioSource>().PlayOneShot(tick, 0.2f);
            }
        }
    }

    void Init()
    {
        tickTimer = 0;
        timeScale = Time.deltaTime * 20;
        int reward = 0;

        for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
        {
            GameObject obj;
            if (i % 2 == 0)
            {
                obj = Instantiate(Rewards[0]);
            }
            else
            {
                reward++;
                obj = Instantiate(Rewards[reward]);
            }
            obj.transform.SetParent(transform.GetChild(0).GetChild(0).GetChild(i));
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        GameToken token = Local.user.gameData[0].UseGameToken(GameStats.spinTokens, true);
        freeSlices = token.level;

        UnityEngine.UI.Text text1 = transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        text1.text = Language.Translate(text1.text);
        UnityEngine.UI.Text text2 = transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        text2.text = Language.Translate(text2.text);
    }

    public void StartSpin()
    {
        speed = 6;
        StopSpin();
    }

    public void StopSpin()
    {
        target = (slots[freeSlices] * 45);
        stop = true;
    }

    IEnumerator Lose()
    {
        yield return new WaitForSeconds(3);
        GetComponent<Animator>().Play("Out");
        Destroy(gameObject, 1);
    }
}
