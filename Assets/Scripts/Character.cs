using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Character : MonoBehaviour
{
    public enum Direction { Forward, Right, Left }

    //Referances
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider cc;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask rightCorner;
    [SerializeField] private LayerMask leftCorner;
    [SerializeField] private LayerMask walls;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTransposer transposer;

    //Movement
    private bool canMove;
    private bool isTurning; 
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDist;
    [SerializeField] private float wallCheckDist;
    [SerializeField] private Vector3 playerVector;


    //Rotation
    public Direction playerDirection = Direction.Forward;
    [SerializeField] Direction playerNextDirection = Direction.Forward;

    //Jumping
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpForce = 5f; // Adjust as needed

    //Crouching
    [SerializeField] private bool isRolling;
    [SerializeField] private float crouchTimer;
    [SerializeField] private float crouchDur;

    //Collision
    private bool isHit;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(0, 180, 0);
        canMove = false;
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace; // Set binding mode to World Space
        playerVector = new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime;
        cc = GetComponent<CapsuleCollider>();
        isHit = false;
    }


    void Update()
    {
        //InputLogic();  
        PlayerInputLogic();
    }

    private void FixedUpdate()
    {
        MovementLogic();
    }

    //Movement
    private void MovementLogic()
    {
        if (isHit)
            return;


        if (playerDirection == Direction.Forward)
        {
            playerVector = Vector3.forward * moveSpeed;
        }

        else if (playerDirection == Direction.Right)
        {
            playerVector = Vector3.right * moveSpeed;
        }

        else if (playerDirection == Direction.Left)
        {
            playerVector = Vector3.left * moveSpeed;
        }

        // Horizontal Movement of the Player
        if (canMove && IsOnGround())
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            animator.SetFloat("Input", horizontalInput);
            animator.SetBool("Strafe", horizontalInput != 0);



            switch (playerDirection)
            {
                case Direction.Forward:
                    playerVector.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
                    break;

                case Direction.Right:
                    playerVector.z = -Input.GetAxisRaw("Horizontal") * moveSpeed;
                    break;

                case Direction.Left:
                    playerVector.z = Input.GetAxisRaw("Horizontal") * moveSpeed;
                    break;
            }
        }
        else if (!IsOnGround())
            animator.SetBool("Strafe", false);


        if (canMove && (IsOnGround() || IsOnRightCorner() || IsOnLeftCorner()) && !isJumping)
            rb.velocity = playerVector;



    }


    //PlayerInputLogic
    private void PlayerInputLogic()
    {
        isGrounded = IsOnGround() || IsOnLeftCorner() || IsOnRightCorner();

        if (isHit)
            return;

        crouchTimer += Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && IsOnRightCorner() && !isRolling)
        {
            switch (playerDirection)
            {
                case Direction.Forward:
                    playerNextDirection = Direction.Right;
                    StartCoroutine(RotateSmoothlyTowards(true));
                    break;

                case Direction.Left:
                    playerNextDirection = Direction.Forward;
                    StartCoroutine(RotateSmoothlyTowards(true));
                    break;
            }
            playerDirection = playerNextDirection;
        }

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && IsOnLeftCorner() && !isRolling)
        {
            switch (playerDirection)
            {
                case Direction.Forward:
                    playerNextDirection = Direction.Left;
                    StartCoroutine(RotateSmoothlyTowards(false));
                    break;

                case Direction.Right:
                    playerNextDirection = Direction.Forward;
                    StartCoroutine(RotateSmoothlyTowards(false));
                    break;
            }
            playerDirection = playerNextDirection;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping && canMove && !isRolling)
        {
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (canMove && isGrounded && !isJumping && !isRolling)
            {
                crouchTimer = 0f;
                isRolling = true;
                animator.SetBool("Crouch", true);
                cc.center = Vector3.MoveTowards(cc.center, new Vector3(0, 0.45f, 0), 2f);
                cc.height = Mathf.MoveTowards(cc.height, 0.9f, 2f);


            }
        }
        else if (crouchTimer > crouchDur)
            Rise();
    }

    public void Rise()
    {
        isRolling = false;
        animator.SetBool("Crouch", false);
        cc.center = new Vector3(0, 0.9f, 0);
        cc.height = 1.8f;
    }



    // Jumping
    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
        StartCoroutine(Land());
    }

    private IEnumerator Land()
    {
        // Wait until the character leaves the ground
        while (IsOnGround() || IsOnLeftCorner() || IsOnRightCorner())
        {
            yield return null;
        }

        // Now wait until character returns to ground
        while (!(IsOnGround() || IsOnLeftCorner() || IsOnRightCorner()))
        {
            yield return null;
        }

        isJumping = false;
        animator.SetTrigger("Land");
    }

    //Rotation
    public IEnumerator RotateSmoothlyTowards(bool turnRight)
    {
        Quaternion startRotation = transform.rotation;
        float angle = turnRight ? 90f : -90f;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, angle, 0);

        float rotationSpeed = 360f; // degrees per second

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    // Start Run
    public void StartRunning()
    {
        if (IsRotationCloseToZero() || !IsOnGround())
            return;

        StartCoroutine(TurnAndRun());
    }

    private IEnumerator TurnAndRun()
    {
        if (!canMove && !isTurning)
        {
            isTurning = true;
            animator.SetBool("Turn180", true);
            // Rotate 180 degrees over 0.5 seconds
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, -180, 0);
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = endRotation;
            canMove = true;
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
            isTurning = false;
        }
    }

    private IEnumerator GetAngryAfterHit(float seconds)
    {
        if (isHit)
        {
            animator.SetBool("Hit", true);
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            yield return new WaitForSeconds(seconds);
            animator.SetBool("Hit", false);
            animator.SetBool("Turn180", true);
            // Rotate 180 degrees over 0.5 seconds
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, -180, 0);
            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = endRotation;
            animator.SetBool("Angry", true);
            isHit = false;
        }
    }

    private bool IsRotationCloseToZero(float threshold = 1f)
    {
        float yRotation = transform.eulerAngles.y;
        // Handles wrap-around (e.g., 359 degrees is close to 0)
        return Mathf.Abs(Mathf.DeltaAngle(yRotation, 0f)) < threshold;
    }
    //End Run

    // Ground Checks
    private bool IsOnGround()
    {
        // Check if the character is grounded using a raycast
        return Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDist, ground);
    }

    private bool IsOnRightCorner()
    {
        // Check if the character is grounded using a raycast
        return Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDist, rightCorner);
    }
    private bool IsOnLeftCorner()
    {
        // Check if the character is grounded using a raycast
        return Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDist, leftCorner);
    }

    // Wall Detection Method


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDist);
    }
    // End Ground Checks

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            canMove = false;
            isHit = true;
            StartCoroutine(GetAngryAfterHit(2.75f));
        }
    }

}


