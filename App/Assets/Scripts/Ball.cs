using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject kickFX;
    public GameObject headFX;
    public AudioClip kick;
    public AudioClip hit;
    public bool controlled = false;
    bool goal = false;
    bool head = false;
    bool hold = false;
    
    Vector3 alpha = Vector3.zero;
    Vector3 target = Vector3.zero;
    Vector3 kickPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hold)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0.01f, 0.1f, 0.16f), Time.deltaTime * 4);
        }
    }

    public void Kick(Vector3 Alpha, Vector3 Target)
    {
        Debug.DrawLine(transform.position, Alpha, Color.blue, 60);
        Debug.DrawLine(transform.position, Target, Color.yellow, 60);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 1000, Color.red, 60);

        GameObject fx = Instantiate(kickFX);
        fx.transform.position = transform.position;
        Destroy(fx, 1);

        //alpha = Alpha;
        //target = Target;

        //Vector3 vector = new Vector3(target.x / 1.75f, target.y / 7, target.y).normalized * 150;

        kickPos = transform.position;

        Vector3 vector = ray.direction * 200;
        GetComponent<Rigidbody>().AddForce(vector, ForceMode.Impulse);

        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        GetComponent<AudioSource>().PlayOneShot(kick);

        controlled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        controlled = false;

        if (collision.collider.gameObject.tag == "GoalNet")
        {
            GetComponent<Rigidbody>().velocity *= 0.25f;
            //Time.timeScale = 0.25f;
        }
        else
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
            GetComponent<AudioSource>().PlayOneShot(hit);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        controlled = false;

        if (collider.gameObject.tag == "GoalTrigger" && !goal)
        {
            goal = true;
            int score = CalculateScore();
            Camera.main.GetComponent<PlayerController>().Goal(score);
        }
        else if (collider.gameObject.tag == "BullHead" && !head && !goal)
        {
            head = true;
            GameObject fx = Instantiate(headFX);
            fx.transform.position = collider.transform.position;
            Destroy(fx, 1);
            collider.transform.parent.GetComponent<BullHead>().Hit();
        }
        else if (collider.gameObject.tag == "GoalKeeper" && !hold)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(collider.transform.parent.GetComponent<GoalKeeper>().hand);
            Camera.main.GetComponent<AudioSource>().PlayOneShot(Camera.main.GetComponent<PlayerController>().lostFX);
            hold = true;
        }
    }

    int CalculateScore()
    {
        int score = 0;

        if (goal)
        {
            float x = Mathf.Abs(Camera.main.GetComponent<PlayerController>().goalKeeper.transform.GetChild(7).position.x - transform.position.x);
            Difficulty difficulty = Camera.main.GetComponent<PlayerController>().token.difficulty;
            bool movingHead = Camera.main.GetComponent<PlayerController>().bullHead.transform.GetChild(0).GetComponent<Animator>().GetBool("Move");

            if (head && x < 15)
            {
                score = GameStats.soccerScores[4, (int)difficulty];
            }
            else if (head && movingHead)
            {
                score = GameStats.soccerScores[3, (int)difficulty];
            }
            else if (head)
            {
                score = GameStats.soccerScores[2, (int)difficulty];
            }
            else
            {
                score = GameStats.soccerScores[1, (int)difficulty];
            }
        }

        return score;
    }
}
