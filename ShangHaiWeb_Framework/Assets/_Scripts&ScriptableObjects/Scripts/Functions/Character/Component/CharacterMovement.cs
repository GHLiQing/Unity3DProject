
using UnityEngine;
using System.Collections;
public class CharacterMovement : MonoBehaviour
{

    // Private variable
    private float gravity = 10; // Gravity of Character in World

    bool slideWhenOverSlopeLimit = false;

    float SlideSpeed = 10f;

    private RaycastHit hit;

    private Vector3 contactPoint;

    private float slideLimit;

    private float rayDistance;

    private bool grounded = false;

    private Vector3 moveDirection = Vector3.zero;

    public float moveSpeed = 1.5f;


    void Start()
    {
        CharacterController controller = GetComponent<CharacterController>();

        rayDistance = controller.height * .5f + controller.radius;//摄影机与障碍物碰撞拉近距离

        slideLimit = controller.slopeLimit - .1f;
    }

    void Update()
    {
        if (grounded)
        {
            bool sliding = false;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);

                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }

            if ((sliding && slideWhenOverSlopeLimit))
            {
                Vector3 hitNormal = hit.normal;

                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);

                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);

                moveDirection *= SlideSpeed;

                GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);
            }

            transform.TransformDirection(Vector3.forward);

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= moveSpeed;
        }

        transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

        //Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        //Move controller
        CharacterController controller = GetComponent<CharacterController>();

        if (controller.enabled)
        {
            CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
            grounded = (flags & CollisionFlags.Below) != 0;
        }

    }

    // Store point that we're in contact
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }
}