using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private float movementInputDirection; //A karakter irányát tárolja el

    private bool isFacingRight = true; //Eltárolja, hogy a karakter éppen a megfelelő irányba néz-e. Mivel a karakter a játék kezdetekor mindig a jó irányba néz, ezért "true" az alapértéke

    private Rigidbody2D rigidbody2D; //A karakter fizikai részére való hivatkozáshoz kell, tárolásra

    public float movementSpeed = 10.0f; //A karakter mozgási sebessége
    public float jumpForce = 16.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0) { Flip(); }
        else if (!isFacingRight && movementInputDirection > 0) { Flip(); }
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
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
    }

    private void ApplyMovement()
    {
        rigidbody2D.velocity = new Vector2(movementSpeed * movementInputDirection, rigidbody2D.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}