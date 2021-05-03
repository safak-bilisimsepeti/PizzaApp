using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool multiplayer = false;
    public GameToken token;
    public float cameraSpeed = 10;
    public GameObject ballPrefab;
    public AudioClip goalFX;
    public AudioClip lostFX;
    public AudioClip whistle;
    public GoalKeeper goalKeeper;
    public BullHead bullHead;
    public SoccerMannequin mannequin;

    public bool play = false;

    Ball ball;
    Vector3 origin = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //multiplayer = Local.buffer == null ? false : (bool)Local.buffer;
        GetComponent<UIController>().RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!ball)
            return;

        if (ball.controlled)
        {
            Vector3 target = ball.transform.position + new Vector3(0, 0, -10);
            float speed = Mathf.Clamp(Vector3.Distance(transform.parent.position, target) * Time.deltaTime, 0.1f, cameraSpeed * Time.deltaTime);
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, speed * cameraSpeed);
        }
        else
        {
            Vector3 target = origin;
            float speed = Mathf.Clamp(Vector3.Distance(transform.parent.position, target) * Time.deltaTime, 0.1f, cameraSpeed * Time.deltaTime);
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, speed * cameraSpeed);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(transform.position, ray.direction * 1000, Color.red);
    }

    public void Kick(LineRenderer line)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < line.positionCount; i++)
        {
            Vector3 point = Camera.main.WorldToScreenPoint(line.GetPosition(i));
            points.Add(point);
        }

        Vector3 first = points[0];
        Vector3 target = points[points.Count - 1];
        Vector3 alpha = points[0];

        foreach(Vector3 dot in points)
        {
            alpha = Mathf.Abs(first.x - dot.x) > Mathf.Abs(first.x - alpha.x) ? dot : alpha;
        }

        target -= first;
        alpha -= first;

        ball.GetComponent<Ball>().Kick(alpha, target);
        goalKeeper.HoldTheBall(ball.gameObject);
        play = false;
        StartCoroutine(DelayToRefresh());
    }

    IEnumerator DelayToRefresh()
    {
        yield return new WaitForSeconds(3f);
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        GameObject t = Instantiate(Resources.Load<GameObject>("Transition"));
        t.transform.SetParent(canvas);
        t.transform.localPosition = Vector3.zero;
        t.transform.localRotation = Quaternion.identity;
        t.transform.localScale = Vector3.one;
        t.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        t.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Destroy(t, 1.5f);
        yield return new WaitForSeconds(0.5f);
        Refresh();
    }

    public void Refresh()
    {
        token = Local.user.gameData[multiplayer ? 2 : 1].UseGameToken(GameStats.soccerTokens, true);

        if (token != null)
        {
            Camera.main.transform.parent.position = 
                new Vector3(Random.Range(-30 * (int)token.difficulty, 30 * (int)token.difficulty), 1.25f, Random.Range(-75, -75 + (-25 * (int)token.difficulty)));
            var lookPos = GameObject.FindGameObjectWithTag("Goal").transform.position - transform.position;
            lookPos.y = 0;
            Camera.main.transform.parent.rotation = Quaternion.LookRotation(lookPos);
            origin = transform.parent.position;

            if (ball != null)
                Destroy(ball.gameObject);
            goalKeeper.GetComponent<Animator>().SetBool("Right", false);
            goalKeeper.GetComponent<Animator>().SetBool("Left", false);

            ball = Instantiate(ballPrefab).GetComponent<Ball>();
            Vector3 ballPos = new Vector3(Camera.main.transform.position.x, 1.25f, Camera.main.transform.position.z + 20);
            ball.transform.position = ballPos;
            GameObject.FindGameObjectWithTag("GoalNet").GetComponent<Cloth>().capsuleColliders = new CapsuleCollider[] { ball.GetComponent<CapsuleCollider>() };

            bullHead.gameObject.SetActive(token.level != 0);
            bullHead.transform.position = new Vector3(Random.Range(-25, 25), Random.Range(10, 35), 100);
            bullHead.Init(token.level > 1);

            mannequin.gameObject.SetActive((int)token.difficulty > 1);
            mannequin.transform.position = new Vector3(Random.Range(-25, 25), 0, 0);
            mannequin.Init((int)token.difficulty > 2);

            GetComponent<AudioSource>().PlayOneShot(whistle, 0.25f);

            play = true;
        }
        else
        {
            Debug.Log("TOKEN_ERR");
        }

        GetComponent<UIController>().RefreshUI();
    }

    public void Goal(int score)
    {
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        GameObject w = Instantiate(Resources.Load<GameObject>("GoalText"));
        w.transform.SetParent(canvas);
        w.transform.localPosition = Vector3.zero;
        w.transform.rotation = Quaternion.identity;
        w.transform.localScale = Vector3.one;
        //w.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        //w.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Destroy(w, 1);

        if (score > 0)
        {
            Local.user.gameData[multiplayer ? 2 : 1].points += score;
            w.GetComponent<UnityEngine.UI.Text>().text += "\n" + "+" + score.ToString();
        }

        GetComponent<AudioSource>().PlayOneShot(goalFX, 2);
    }

    public void SetGameMode(bool Multiplayer)
    {
        multiplayer = Multiplayer;
    }
}
