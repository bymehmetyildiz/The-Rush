using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Character : MonoBehaviour
{
    enum Direction { Forward, Right, Left }

    //Referances
    private Animator animator;
    private Rigidbody rb;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask rightCorner;
    [SerializeField] private LayerMask leftCorner;   
    [SerializeField] private LayerMask walls;   
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTransposer transposer;

    //Movement
    private bool canMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDist;    
    [SerializeField] private float wallCheckDist;    
    [SerializeField] private Vector3 playerVector;


    //Rotation
    [SerializeField] Direction playerDirection = Direction.Forward;
    [SerializeField] Direction playerNextDirection = Direction.Forward;


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

        if (canMove && (IsOnGround() || IsOnRightCorner() || IsOnLeftCorner()))
            rb.velocity = playerVector;


    }

    //PlayerInputLogic
    private void PlayerInputLogic()
    {
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && IsOnRightCorner())
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

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && IsOnLeftCorner())
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
    }
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

    /*
    private void InputLogic()
    {
        if (!canMove)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (IsOnGround())
            {
                if (currentLane > 1 || isChangingLane)
                    return;

                StartCoroutine(ChangeLane(1));
            }
            else if (IsOnRightCorner())
            {
                if(!IsFacingRight())
                {
                    StartCoroutine(RotateSmoothlyTowards(true));
                }
            }


        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (IsOnGround())
            {
                if (currentLane < 1 || isChangingLane)
                    return;
                StartCoroutine(ChangeLane(-1));
            }
            else if(IsOnLeftCorner())
            {
                if (!IsFacingLeft())
                {
                    StartCoroutine(RotateSmoothlyTowards(false));
                }
            }
        }

    }

    private IEnumerator ChangeLane(int _targetLane)
    {
        targetLane += _targetLane;

        if (targetLane < 0)
            targetLane = 0;
        else if(targetLane > 2)
            targetLane = 2;

        if (targetLane < currentLane)
        {
            isChangingLane = true;
            currentLane = targetLane;
            animator.SetBool("ChangeLaneR", isChangingLane);
            if (!IsFacingRight() && !IsFacingLeft())
            {
                Vector3 targetPosition = new Vector3(transform.position.x - laneDistance, transform.position.y, transform.position.z);

                while (Vector3.Distance(new Vector3(targetPosition.x, 0, 0), new Vector3(transform.position.x, 0, 0)) > 0.05f)
                {
                    Vector3 newPos = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), laneChangeSpeed * Time.deltaTime);
                    transform.position = newPos;
                    yield return null;
                }
                transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
            }
            else if (IsFacingRight())
            {
                Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + laneDistance);

                while (Vector3.Distance(new Vector3(0, 0, transform.position.z), new Vector3(0, 0, targetPosition.z)) > 0.05f)
                {
                    Vector3 newPos = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, targetPosition.z), laneChangeSpeed * Time.deltaTime);
                    transform.position = newPos;
                    yield return null;
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, targetPosition.z);
            }
            yield return new WaitForSeconds(0.1f);
            isChangingLane = false;
            animator.SetBool("ChangeLaneR", isChangingLane);
            
        }
        else if(targetLane > currentLane)
        {
            isChangingLane = true;
            currentLane = targetLane;
            animator.SetBool("ChangeLaneL", isChangingLane);
            if (!IsFacingRight() && !IsFacingLeft())
            {
                Vector3 targetPosition = new Vector3(transform.position.x + laneDistance, transform.position.y, transform.position.z);
                while (Vector3.Distance(new Vector3(targetPosition.x, 0, 0), new Vector3(transform.position.x, 0, 0)) > 0.05f)
                {
                    Vector3 newPos = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), laneChangeSpeed * Time.deltaTime);
                    transform.position = newPos;
                    yield return null;
                }
                transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
            }
            else if (IsFacingLeft())
            {
                Vector3 targetPosition = new Vector3(transform.position.x , transform.position.y, transform.position.z - laneDistance);
                while (Vector3.Distance(new Vector3(0, 0, targetPosition.z), new Vector3(0, 0, transform.position.z)) > 0.05f)
                {
                    Vector3 newPos = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, targetPosition.z), laneChangeSpeed * Time.deltaTime);
                    transform.position = newPos;
                    yield return null;
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, targetPosition.z);

            }
            yield return new WaitForSeconds(0.1f);
            isChangingLane = false;
            animator.SetBool("ChangeLaneL", isChangingLane);
            
        }
        
    }

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
    */

    // Start Run
    public void StartRunning()
    {
        if (IsRotationCloseToZero() || !IsOnGround())
            return;

        StartCoroutine(TurnAndRun());
    }

    private IEnumerator TurnAndRun()
    {
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

    //Facing Direction
    private bool IsFacingRight()
    {
        return Vector3.Dot(transform.forward, Vector3.right) > 0.9f;
    }

    private bool IsFacingLeft()
    {
        return Vector3.Dot(transform.forward, Vector3.left) > 0.9f;
    }
    //End Facing Direction
}


