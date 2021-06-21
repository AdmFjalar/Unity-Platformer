using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private float timer;
    [SerializeField] private float timeLimit = 10f;

    public float speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeLimit)
        {
            Destroy(gameObject);
        }

        rb2d.velocity = (Vector2.right * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<Frog>() != null)
            {
                FindObjectOfType<PlayerController>().score += 30;
                collision.gameObject.GetComponent<Frog>().Die();
            }
            else
            {
                FindObjectOfType<PlayerController>().score += 20;
                collision.gameObject.GetComponent<Opossum>().Die();
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<Frog>() != null)
            {
                FindObjectOfType<PlayerController>().score += 30;
                collision.gameObject.GetComponent<Frog>().Die();
            }
            else
            {
                FindObjectOfType<PlayerController>().score += 20;
                collision.gameObject.GetComponent<Opossum>().Die();
            }
        }

        Destroy(gameObject);
    }
}
