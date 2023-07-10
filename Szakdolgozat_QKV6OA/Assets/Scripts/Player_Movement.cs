using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private float movementInputDirection = 0.0f; //A karakter irányát tárolja el
    private float movementInputVertical = 0.0f;

    private int amountOfJumpsLeft = 2;
    private int facingDirection = 1; //Ha az értéke -1, akkor balra néz a karakter, ha pedig +1, akkor jobbra

    private bool isFacingRight = true; //Eltárolja, hogy a karakter éppen a megfelelő irányba néz-e. Mivel a karakter a játék kezdetekor mindig a jó irányba néz, ezért "true" az alapértéke
    private bool isWalking = false;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private bool canJump = false;

    private Rigidbody2D rb; //A karakter fizikai részére való hivatkozáshoz kell, tárolásra
    private Animator animator;

    public int amountOfJumps = 2;

    public float movementSpeed = 10.0f; //A karakter mozgási sebessége
    public float jumpForce = 16.0f;
    public float groundCheckRadius = 3.0f;
    public float wallCheckDistance = 0.36f;
    public float wallSlideSpeed = 2.0f;
    public float movementForceInAir = 25.0f;
    public float airDragMultiplier = 0.95f; //Légellenállás megvalósítása, amikor a karakter esik lefelé, és nem kap mellette semmilyen egyéb input-ot.
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;

    public Vector2 wallHopDirection; //Meghatározza, hogy melyik irányba ugrik a karakter a falakról, ezzel lehet módosítani, hogy mennyire legyen meredek a falról való elugrás
    public Vector2 wallJumpDirection;


    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize(); //1-re állítja mind a két koordinátát
        wallJumpDirection.Normalize(); //1-re állítja mind a két koordinátát
        //Azért kell Normalize-olni, hogy amikor erőt adunk hozzá, akkor mindig ugyanannyit adjunk hozzá
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if ((isGrounded && rb.velocity.y == 0) || isWallSliding)
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
        animator.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal"); //Ha a játékos az A billentyűt tartja lenyomva, akkor ez -1 értéket, ha pedig a D billentyűt, akkor +1 értéket ad vissza

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }

        else if (isWallSliding && movementInputDirection == 0 && canJump) //Wall hop
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }

        else if((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump) //Wall jump
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private void ApplyMovement()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }

        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd);

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }

        else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1; //A +1-et -1-re, a -1-et +1-re változtatja, amikor megfordítjuk a karaktert
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}