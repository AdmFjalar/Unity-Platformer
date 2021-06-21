using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opossum : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject enemyDeath;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask targetLayers;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Flip();
        DamagePlayer();
    }

    /// <summary>
    /// Flips character when it reaches a wall.
    /// </summary>
    void Flip()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right * speed, 2, groundLayers))
        {
            speed *= -1;
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    /// <summary>
    /// Checks if the character has collided with the player and damages them if they have.
    /// </summary>
    void DamagePlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .5f, targetLayers);

        if (hits.Length > 0)
        {
            if (hits[hits.Length - 1].GetComponent<PlayerController>() != null)
            {
                hits[hits.Length - 1].GetComponent<PlayerController>().Damage();
            }
        }
    }

    /// <summary>
    /// Makes the character leap forward.
    /// </summary>
    public void Walk()
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }

    public void Die()
    {
        Instantiate(enemyDeath, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
