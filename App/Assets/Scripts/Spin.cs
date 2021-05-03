using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public GameObject[] Rewards;
    public GameObject RewardPanel;
    public GameObject LosePanel;
    bool play = true;
    int slot = 0;
    int freeSlices = 0;
    float speed = 0;
    float quaternion = 0;
    int turn = 4;

    RotateDirection direction = RotateDirection.Stop;
    Vector2 origin = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        RefreshSpin(Random.Range(0, 5));
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == RotateDirection.Stop)
        {
            if (play)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (Vector2.Distance(Vector2.zero, pos) < 4.5f)
                    {
                        origin = pos;
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector2 last = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (origin != Vector2.zero && Vector2.Distance(origin, last) > 2)
                    {
                        float angle = Vector2.SignedAngle(origin, last);
                        direction = angle <= 0 ? RotateDirection.Clockwise : RotateDirection.CounterClockwise;
                        if (slot != 0)
                        {
                            if (direction == RotateDirection.Clockwise)
                            {
                                quaternion = (360 * turn) + (8 - slot * 45);
                            }
                            else if (direction == RotateDirection.CounterClockwise)
                            {
                                quaternion = (360 * turn) + ((slot) * 45);
                            }
                        }
                        else
                        {
                            quaternion = (360 * turn);
                        }
                        origin = Vector2.zero;
                        play = false;
                    }
                }
            }
        }
        else
        {
            float val = (direction == RotateDirection.Clockwise) ?
                (360 * turn) + (8 - slot * 45) : (360 * turn) + ((slot) * 45);
            speed = Mathf.Clamp(quaternion / val, 0.1f, 1) * 360 * Time.deltaTime * 2;
            transform.Rotate(new Vector3(0, 0, direction == RotateDirection.Clockwise ? -speed : speed));
            quaternion -= speed;
            if (quaternion <= 0)
            {
                if (slot != 0)
                {
                    GameObject w = Instantiate(Resources.Load<GameObject>("GiftPanel"));
                    w.GetComponent<Gift>().Init(freeSlices);
                }
                else
                {
                    StartCoroutine(Lose());
                }

                direction = RotateDirection.Stop;
            }
        }
    }

    public void RefreshSpin(int gainSlices = 0)
    {
        int[] slices = new int[] { 0, 1, 3, 5, 7};
        slot = slices[gainSlices];
        freeSlices = gainSlices;
        transform.rotation = Quaternion.identity;
        direction = RotateDirection.Stop;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount > 0)
                Destroy(transform.GetChild(i).GetChild(0).gameObject);
        }

        int reward = 0;

        for (int i = 0; i < transform.childCount; i++)
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
            obj.transform.SetParent(transform.GetChild(i));
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        play = true;
    }

    public IEnumerator Lose()
    {
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        yield return new WaitForSeconds(1f);
        GameObject black = Instantiate(Resources.Load<GameObject>("ToBlack"));
        black.transform.SetParent(canvas);
        black.transform.localPosition = Vector3.zero;
        black.transform.rotation = Quaternion.identity;
        black.transform.localScale = Vector3.one;
        black.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        black.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene("App");
    }
}

public enum RotateDirection
{
    Stop,
    Clockwise,
    CounterClockwise
}