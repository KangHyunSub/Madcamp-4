using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed;
    public float jumpForce;
    public int me;
    public GameManager gameManager;

    Rigidbody mRigidBody;


    float verticalLookRotation;
    public float verticalLookRotationClamp;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    bool grounded;
    public LayerMask groundedMask;
    public Transform cameraTransform;

    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        groundedMask = 1 << LayerMask.NameToLayer("Ground");
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            me = (me == 1) ? 0 : 1;
        }

        if (me == 1)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);
            verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalLookRotationClamp, verticalLookRotationClamp);
            cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

            Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 targetMoveAmount = moveDirection * walkSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

            if (Input.GetButtonDown("Jump") && grounded)
            {
                mRigidBody.AddForce(transform.up * jumpForce);
                grounded = false;
            }

            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1 + 0.1f, groundedMask))
            {
                grounded = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (me == 1)
        {
            mRigidBody.MovePosition(mRigidBody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            gameManager.EndGame(true);
        }
    }

}
