using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;

    [SerializeField] private GameObject firstPerson;
    [SerializeField] private GameObject thirdPerson;

    [SerializeField] private float turnSmoothTime;

    private Animator animator;
    private Vector3 moveVector = Vector3.zero;
    private Vector3 direction;
    private Vector3 moveDirection;
    private Vector3 move;

    private float turnSmoothVelocity;
    private float targetAngle;
    private float angle;
    private float vertical;
    private float horizontal;

    private bool isFirstPerson = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Variables Initialization
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0f, vertical);

        //Ground behaviour mechanic
        GroundBehaviourMechanic();

        //Check user actions with camera
        CheckCamera();
    }
    private void PlayerMove(float speed)
    {
        if (isFirstPerson)
        {
            move = transform.right * horizontal + transform.forward * vertical;

            controller.Move(move * speed * Time.deltaTime);
        }
        else if (!isFirstPerson)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }
    private void GroundBehaviourMechanic()
    {
        StartCoroutine(CheckJump(10, 5));


        //sneak mechanic algorithm - test code (can be deleted and overrited)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("Crouch");
            controller.height = 0.35f;
            controller.center = new Vector3(0, 0.175f, 0);
            if (direction.magnitude >= 0.1f)
            {
                PlayerMove(0);
                animator.SetTrigger("Crouch");
                PlayerMove(2);
                StartCoroutine(StartActionAfterTime("Sneak", 1));

                controller.height = 0.35f;
                controller.center = new Vector3(0, 0.175f, 0);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            PlayerMove(0);
            animator.SetTrigger("CrouchToStand");

            controller.height = 0.35f;
            controller.center = new Vector3(0, 0.175f, 0);
        }


        //simple movements
        else if (direction.magnitude >= 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                StartActions("Run", 12);
            }
            else
            {
                StartActions("Walk", 5);
            }
        }

        else animator.SetTrigger("Stand");
        controller.height = 0.5f;
        controller.center = new Vector3(0, 0.25f, 0);
    }
    IEnumerator CheckJump(float gravity, float jumpSpeed)
    {
        if (controller.isGrounded && Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger("Jump"); 
            moveVector.y = jumpSpeed;
        }

        moveVector.y -= gravity * Time.deltaTime;
        controller.Move(moveVector * Time.deltaTime);

        yield return null;
    }

    private void CheckCamera()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (isFirstPerson)
            {
                firstPerson.SetActive(false);
                thirdPerson.SetActive(true);

                turnSmoothTime = 0.1f;

                isFirstPerson = false;
            }
            else if (!isFirstPerson)
            {
                thirdPerson.SetActive(false);
                firstPerson.SetActive(true);

                turnSmoothTime = 0.4f;

                isFirstPerson = true;
            }
        }
    }

    private void StartActions([Optional] string trigger, [Optional] float speed)
    {
        animator.SetTrigger(trigger);
        PlayerMove(speed);
    }

    IEnumerator StartActionAfterTime(string trigger, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetTrigger(trigger);
    }
}