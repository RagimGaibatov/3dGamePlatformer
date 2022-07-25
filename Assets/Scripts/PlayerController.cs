using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float jumpHorizontalSpeed;
    [SerializeField] private float jumpButtonGracePeriod;

    [SerializeField] private Transform cameraTransform;

    private Animator _animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundTime;
    private float? jumpButtonPressedTime;

    private bool isJumping;
    private bool isGrounded;


    private void Awake(){
        _animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }

    void Update(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            inputMagnitude /= 2;
        }

        _animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) *
                            movementDirection;
        movementDirection.Normalize();

        float gravity = Physics.gravity.y * gravityMultiplier;
        
        if (isJumping && ySpeed > 0 && Input.GetButton("Jump") == false){
            gravity *= 2f;
        }

        ySpeed += gravity * Time.deltaTime;


        if (characterController.isGrounded){
            lastGroundTime = Time.time;
        }

        if (Input.GetButtonDown("Jump")){
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundTime <= jumpButtonGracePeriod){
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            _animator.SetBool("IsGrounded", true);
            isGrounded = true;
            _animator.SetBool("IsJumping", false);
            isJumping = false;
            _animator.SetBool("IsFalling", false);
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod){
                ySpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
                _animator.SetBool("IsJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundTime = null;
            }
        }
        else{
            characterController.stepOffset = 0f;
            _animator.SetBool("IsGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -3){
                _animator.SetBool("IsFalling", true);
            }
        }


        if (movementDirection != Vector3.zero){
            _animator.SetBool("IsMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
        }
        else{
            _animator.SetBool("IsMoving", false);
        }

        if (isGrounded == false){
            Vector3 velocity = movementDirection * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void OnAnimatorMove(){
        if (isGrounded){
            Vector3 velocity = _animator.deltaPosition;
            velocity.y = ySpeed * Time.deltaTime;

            characterController.Move(velocity);
        }
    }

    private void OnApplicationFocus(bool hasFocus){
        if (hasFocus){
            Cursor.lockState = CursorLockMode.Locked;
        }
        else{
            Cursor.lockState = CursorLockMode.None;
        }
    }
}