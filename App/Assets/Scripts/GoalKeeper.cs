using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : MonoBehaviour
{
    public Transform hand;
    GameObject ball = null;
    float jumpHeight = 10;
    bool jump = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (jump)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, jumpHeight, 75), Time.deltaTime * (jumpHeight * 4));
            if (Vector3.Distance(transform.position, new Vector3(0, jumpHeight, 75)) < 0.1f)
            {
                jump = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, 75), Time.deltaTime * (jumpHeight * 3));
        }
    }

    public void HoldTheBall(GameObject Ball)
    {
        ball = Ball;
        StartCoroutine(Hold());
    }

    IEnumerator Hold()
    {
        float chance = 1.0f / ((int)Camera.main.GetComponent<PlayerController>().token.difficulty + 1);
        bool hold = Random.Range(0.0f, chance) < 0.2f;
        yield return new WaitForSeconds(hold ? 0.0f : 0.2f);
        string anim = (transform.position.x < ball.transform.position.x) ? "Right" : "Left";
        GetComponent<Animator>().SetBool(anim, true);
        yield return new WaitForSeconds(0.4f);
        jumpHeight = Mathf.Clamp(ball.transform.position.y, 0, 20);
        jump = true;
    }
}
