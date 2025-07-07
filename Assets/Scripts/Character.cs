using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Referances
    private Animator animator;
    private Rigidbody rb;

    //Movement
    private bool canMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDist;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(0, 180, 0);
        canMove = false;
    }


    void Update()
    {
        if (canMove && IsGrounded())
            rb.velocity = new Vector3(transform.forward.x * moveSpeed, rb.velocity.y, transform.forward.z * moveSpeed);

    }

    public void StartRunning()
    {
        if (IsRotationCloseToZero())
            return;

        StartCoroutine(TurnAndRun());
    }

    private IEnumerator TurnAndRun()
    {
        animator.SetBool("Turn", true);
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
    }

    private bool IsRotationCloseToZero(float threshold = 1f)
    {
        float yRotation = transform.eulerAngles.y;
        // Handles wrap-around (e.g., 359 degrees is close to 0)
        return Mathf.Abs(Mathf.DeltaAngle(yRotation, 0f)) < threshold;
    }

    private bool IsGrounded()
    {
        // Check if the character is grounded using a raycast
        return Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDist);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDist);
    }
}


