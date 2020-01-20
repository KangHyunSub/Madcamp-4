using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed;
    public float jumpForce;

    Transform cameraTransform;
    Rigidbody mRigidBody;


    float verticalLookRotation;
    public float verticalLookRotationClamp;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    bool grounded;
    public LayerMask groundedMask;

    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        groundedMask = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalLookRotationClamp, verticalLookRotationClamp);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDirection * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        if(Input.GetButtonDown("Jump") && grounded)
        {
            mRigidBody.AddForce(transform.up * jumpForce);
            grounded = false;
        }

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 1 + 0.1f, groundedMask))
        {
            grounded = true;
        }
    }

    private void FixedUpdate()
    {
        mRigidBody.MovePosition(mRigidBody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
