using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private bool invincible = false;
    private float fallY = -100;
    private GameManager gameManager;
    [SerializeField] private Animator anim;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 150f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource interactionSource;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip extraLifeSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private Color collideColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Image hearts;
    [SerializeField] private Sprite[] heartSprites;
    [SerializeField] private TextMeshProUGUI pointText;

    public int score;
    public int lives = 6;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = "SCORE: " + score;

        if (lives >= 0)
        {
            hearts.sprite = heartSprites[6 - lives];
        }

        if (lives <= 0 && fallY - transform.position.y > 20)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        if (lives > 0 && !gameManager.won)
        {
            Shoot();
            Move();
            Animate();
        }
    }

    /// <summary>
    /// Spawns bullets in the players looking direction when pressing the fire button.
    /// </summary>
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!GetComponent<SpriteRenderer>().flipX)
            {
                Instantiate(bulletPrefab, transform.position + new Vector3(1.5f, 0, 0), Quaternion.identity);
            }
            else
            {
                GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(-1.5f, 0, 0), Quaternion.identity);
                bullet.GetComponent<Bullet>().speed *= -1;
            }
        }
    }

    /// <summary>
    /// Move and jump controls for player.
    /// </summary>
    void Move()
    {
        if (!Physics2D.Raycast(transform.position, Vector2.right * Input.GetAxis("Horizontal"), GetComponent<Collider2D>().bounds.size.x / 2 + 0.03f, targetLayers) &&
            !Physics2D.Raycast(transform.position + new Vector3(0, (GetComponent<Collider2D>().bounds.size.y / 2) * 0.95f, 0), Vector2.right * Input.GetAxis("Horizontal"), GetComponent<Collider2D>().bounds.size.x / 2 + 0.03f, targetLayers) &&
            !Physics2D.Raycast(transform.position - new Vector3(0, (GetComponent<Collider2D>().bounds.size.y / 2) * 0.95f, 0), Vector2.right * Input.GetAxis("Horizontal"), GetComponent<Collider2D>().bounds.size.x / 2 + 0.03f, targetLayers)) //Checks if the player is able to run and isn't about to run up against a wall (This is to prevent bug where player tries to clip through wall and flies back and forth)
        {
            transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Physics2D.Raycast(transform.position, -Vector2.up, GetComponent<Collider2D>().bounds.size.y / 2 + 0.1f -GetComponent<Collider2D>().offset.y, targetLayers)) //Checks if the player is standing on ground to avoid jumping in the air.
        {
            mainSource.PlayOneShot(jumpSound);
            rb2d.AddForce(Vector2.up * jumpForce);
        }
    }

    /// <summary>
    /// Controls animator.
    /// </summary>
    void Animate()
    {
        if (rb2d.velocity.y == 0)
        {
            anim.SetFloat("yDelta", 0);
            anim.SetBool("SameY", true);
        }
        else
        {
            anim.SetBool("SameY", false);

            if (rb2d.velocity.y > 0)
            {
                anim.SetFloat("yDelta", 1);
            }
            else
            {
                anim.SetFloat("yDelta", -1);
            }
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            anim.SetBool("Running", false);
        }
        else
        {
            anim.SetBool("Running", true);
        }

        if (Input.GetAxis("Horizontal") > 0 && GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetAxis("Horizontal") < 0 && !GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    /// <summary>
    /// Deals damage to player and calls following methods.
    /// </summary>
    public void Damage()
    {
        if (!invincible && !gameManager.won)
        {
            mainSource.PlayOneShot(damageSound);
            lives--;

            if (lives > 0)
            {
                StartCoroutine(Flasher());
            }

            else
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Starts the player death sequence.
    /// </summary>
    void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        anim.SetBool("Dying", true);
        StartCoroutine(DeathFall());
        fallY = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Point")
        {
            score += 50;
            Destroy(collision.gameObject);
            interactionSource.PlayOneShot(pickupSound);
        }
        else if (collision.gameObject.tag == "ExtraLife")
        {
            if (lives < 6)
            {
                lives++;
                Destroy(collision.gameObject);
                interactionSource.PlayOneShot(extraLifeSound);
            }
        }
        else if (collision.gameObject.tag == "Goal")
        {
            gameManager.Win();
        }
    }

    IEnumerator Flasher()
    {
        invincible = true;
        for (int i = 0; i < 5; i++)
        {
            GetComponent<SpriteRenderer>().material.color = collideColor;
            yield return new WaitForSeconds(.2f);
            GetComponent<SpriteRenderer>().material.color = defaultColor;
            yield return new WaitForSeconds(.2f);
        }
        invincible = false;
        StopAllCoroutines();
    }

    IEnumerator DeathFall()
    {
        yield return new WaitForSeconds(.5f);

        GetComponent<Rigidbody2D>().velocity = Vector2.up * 5;
        GetComponent<Rigidbody2D>().gravityScale = 1;

        deathMenu.SetActive(true);

        StopAllCoroutines();
    }
}
