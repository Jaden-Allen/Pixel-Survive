using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerSpeeds speeds = new PlayerSpeeds(new Speed(4.317f), new Speed(5.6f));
    public CharacterController controller;
    public Camera cam;
    public PlayerHands hands;
    public Animator anim;
    public Player player;

    public PlayerMovementSettings settings;

    public float fov = 90f;
    public float aimModifier = 0f;

    public float jumpHeight = 1.25f;

    public float gravity = -19.62f;
    public float terminalVelocity = -78.4f;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.1f;

    Vector3 rawMovement = Vector3.zero;
    Vector3 smoothedMovement = Vector3.zero;
    Vector3 airMovement = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    float speed = 0;
    float x = 0;
    float z = 0;
    float smoothedX = 0;
    float smoothedZ = 0;

    bool isSprinting = false;
    bool isMoving = false;
    bool isCrouching;
    

    private void Update()
    {
        

        CalculateInputs();
        Sprint();
        CalculateSpeed();
        Gravity();
        Jump();
        Crouch();
        ApplyMovement();
    }
    void CalculateInputs()
    {
        
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        if (player.manager.uiManager.uiOpen)
        {
            x = 0;
            z = 0;
        }
        smoothedX = Mathf.Lerp(smoothedX, x, 9f * Time.deltaTime);
        smoothedZ = Mathf.Lerp(smoothedZ, z, 9f * Time.deltaTime);

        smoothedX = Mathf.Clamp(smoothedX, -1f, 1f);
        smoothedZ = Mathf.Clamp(smoothedZ, -1f, 1f);

        smoothedMovement = transform.right * smoothedX + transform.forward * smoothedZ;
        airMovement = Vector3.Lerp(airMovement, smoothedMovement, 3f * Time.deltaTime);
        rawMovement = transform.right * x + transform.forward * z;
        isMoving = rawMovement != Vector3.zero;

        hands.HandMovement(isMoving, isSprinting, IsGrounded());
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isSprinting", isSprinting);
        //anim.SetBool("isCrouching", isCrouching);


    }
    void Crouch()
    {
        if (settings.toggleCrouch)
        {
            if (Input.GetButtonDown("Crouch"))
            {
                isCrouching = !isCrouching;
            }
        }
        else
        {
            if (Input.GetButton("Crouch"))
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }
        
        if (isCrouching)
        {
            CrouchCheck();
        }
    }
    void CrouchCheck()
    {

    }
    void Sprint()
    {
        if (settings.toggleSprint)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                isSprinting = !isSprinting;
            }
        }
        else
        {
            if (Input.GetButton("Sprint") && isMoving)
            {
                isSprinting = true;
            }
            if (!isMoving || z <= 0)
            {
                isSprinting = false;
            }
        }
    }
    void CalculateSpeed()
    {
        switch (isSprinting)
        {
            case true:
                speed = Mathf.Lerp(speed, speeds.sprintSpeeds.forwardSpeed, 20f * Time.deltaTime);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov - aimModifier + 20f, 20f * Time.deltaTime);
                break;
            case false:
                speed = Mathf.Lerp(speed, speeds.walkSpeeds.forwardSpeed, 20f * Time.deltaTime);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov - aimModifier, 20f * Time.deltaTime);
                break;
        }
    }
    void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2;
        }
    }
    void Jump()
    {
        if (Input.GetButton("Jump") && IsGrounded())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    void ApplyMovement()
    {
        // Movement
        if (!IsGrounded())
        {
            controller.Move(airMovement * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(smoothedMovement * speed * Time.deltaTime);
        }
        // Gravity
        velocity.y = Mathf.Clamp(velocity.y, terminalVelocity, 1000f);
        controller.Move(velocity * Time.deltaTime);
    }
}
[System.Serializable]
public class PlayerSpeeds
{
    public Speed walkSpeeds;
    public Speed sprintSpeeds;

    public PlayerSpeeds(Speed walkSpeeds, Speed sprintSpeeds)
    {
        this.walkSpeeds = walkSpeeds;
        this.sprintSpeeds = sprintSpeeds;
    }
}
[System.Serializable]
public class Speed
{
    public float forwardSpeed;
    public float backwardSpeed;
    public float leftSpeed;
    public float rightSpeed;

    public Speed(float forwardSpeed, float backwardSpeed, float leftSpeed, float rightSpeed)
    {
        this.forwardSpeed = forwardSpeed;
        this.backwardSpeed = backwardSpeed;
        this.leftSpeed = leftSpeed;
        this.rightSpeed = rightSpeed;
    }
    public Speed(float allSpeeds)
    {
        this.forwardSpeed = allSpeeds;
        this.backwardSpeed = allSpeeds;
        this.leftSpeed = allSpeeds;
        this.rightSpeed = allSpeeds;
    }
}
[System.Serializable]
public class PlayerMovementSettings
{
    public bool toggleCrouch;
    public bool toggleSprint;
}