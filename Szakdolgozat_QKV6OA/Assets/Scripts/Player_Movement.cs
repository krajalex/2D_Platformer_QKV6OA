using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private float movementInputDirection = 0.0f; //A karakter irányát tárolja el
    private float movementInputVertical = 0.0f;

    private int amountOfJumpsLeft;

    private bool isFacingRight = true; //Eltárolja, hogy a karakter éppen a megfelelő irányba néz-e. Mivel a karakter a játék kezdetekor mindig a jó irányba néz, ezért "true" az alapértéke
    private bool isWalking = false;
    private bool isGrounded;
    private bool canJump;

    private Rigidbody2D rb; //A karakter fizikai részére való hivatkozáshoz kell, tárolásra
    private Animator animator;

    public int amountOfJumps = 2;

    public float movementSpeed = 10.0f; //A karakter mozgási sebessége
    public float jumpForce = 16.0f;
    public float groundCheckRadius;

    public Transform groundCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y == 0) 
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if (amountOfJumpsLeft == 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0) { Flip(); }
        else if (!isFacingRight && movementInputDirection > 0) { Flip(); }

        if (movementInputDirection != 0) { isWalking = true; }
        else { isWalking = false; }
    }

    private void UpdateAnimations()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal"); //Ha a játékos az A billentyűt tartja lenyomva, akkor ez -1 értéket, ha pedig a D billentyűt, akkor +1 értéket ad vissza

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}