using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Mathf;

public class Player_Move : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCoolDown;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        anim.SetBool("walk", horizontalInput != 0);
        anim.SetBool("grounded", isgrounded());

        if (wallJumpCoolDown > 0.02f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (onWall() && !isgrounded())
            {
                body.gravityScale = 1; // Prevents player from sticking to walls
            }
            else
            {
                body.gravityScale = 5;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isgrounded()) // Fixed: Use GetKeyDown & isgrounded()
            {
                Jump();
            }
        }
        else
        {
            wallJumpCoolDown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (isgrounded()) {

            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower); // Fixed: Use velocity
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isgrounded())
        {
            if (horizontalInput == 0)
            {
                body.linearVelocity = new Vector2(-Abs(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                body.linearVelocity = new Vector2(-Abs(transform.localScale.x) * 3, 6);
            wallJumpCoolDown = 0;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // No changes needed
    }

    private bool isgrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
