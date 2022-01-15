using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
    [HideInInspector] public int moveSpeed;
    public int duckSpeed;
    public int runSpeed;
    public int jumpHeight;
    public int maxFallVelocity;
    bool isFalling;
    bool fallDamageApplied;
    [HideInInspector] public bool movementRestricted;

    public LayerMask groundMask;
    public float groundDistance;
    [HideInInspector] public int totalJumps;
    [HideInInspector] public bool canDoubleJump;

    [HideInInspector] public float gravity = -9.81f;
    bool isGrounded;
    [HideInInspector] public Vector3 velocity;
    CharacterController controller;
    PlayerCombat playerCombat;
    bool isDucked;

    float slopeJumpTimer;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCombat.playerDied || playerCombat.gameSettings.levelCleared)
            movementRestricted = true;

        if (!movementRestricted)
        {
            if ((Input.GetButton("Duck") && isGrounded) || Physics.Raycast(transform.position, Vector3.up, 1))
                isDucked = true;
            else
                isDucked = false;

            if (isDucked)
            {
                controller.height = 1;
                GetComponent<CapsuleCollider>().height = 1;
                moveSpeed = duckSpeed;
            }
            else
            {
                controller.height = Mathf.Lerp(controller.height, 2, 15 * Time.deltaTime);
                GetComponent<CapsuleCollider>().height = Mathf.Lerp(controller.height, 2, 15 * Time.deltaTime);
                moveSpeed = runSpeed;
            }

            isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                if (isFalling && !fallDamageApplied)
                {
                    fallDamageApplied = true;
                    playerCombat.playerHealth -= Mathf.RoundToInt(playerCombat.fallDmgMultiplier * -velocity.y);
                }
                totalJumps = 0;
                slopeJumpTimer = 0;
                velocity.y = -2;
            }

            if (!isGrounded && velocity.y < maxFallVelocity)
            {
                isFalling = true;
                fallDamageApplied = false;
            }
            else
            {
                isFalling = false;
            }

            float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

            Vector3 move = transform.right * x + transform.forward * z;

            if (Input.GetButtonDown("Jump") && isGrounded && !isDucked)
            {
                totalJumps++;
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }

            if (Input.GetButtonDown("Jump") && totalJumps == 1 && !isGrounded && !isDucked && canDoubleJump)
            {
                totalJumps++;
                velocity.y = Mathf.Sqrt((jumpHeight - 2) * -2 * gravity);
            }

            if (!isGrounded && totalJumps != 1)
            {
                slopeJumpTimer += Time.deltaTime;
                if (slopeJumpTimer < 0.2f && Input.GetButtonDown("Jump") && !isDucked)
                {
                    totalJumps++;
                    velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }
            }

            velocity.y += 2 * gravity * Time.deltaTime;
            controller.Move(move);
            controller.Move(velocity * Time.deltaTime);

            animator.SetInteger("SelectedWeapon", playerCombat.selectedWeapon);

            if ((x > 0) || (x < 0) || (z > 0) || (z < 0))
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }
}
