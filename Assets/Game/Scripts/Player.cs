using Cinemachine;
using UnityEngine;

namespace Horror.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam3;
        [SerializeField] private CinemachineVirtualCamera vCam1;
        [SerializeField] private CharacterController character;
        [SerializeField] private float moveSpeedFoward;
        [SerializeField] private float moveSpeedBack;
        [SerializeField] private float moveSpeedSides;
        [SerializeField] private float groundOffset;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float sensitivityMouseX = 500f;
        [SerializeField] private float sensitivityMouseY = 5f;
        [SerializeField] private Vector2 mouseYSensitivity;
        [SerializeField] private Animator animator;


        private CinemachineComposer camComposer;
        private float hzInput, vInput;
        private Vector3 dir;
        private Vector3 spherePos;
        private Vector3 velocity;

        void Start()
        {
            character = GetComponent<CharacterController>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            Cursor.lockState = CursorLockMode.Locked;
            camComposer = vCam3.GetCinemachineComponent<CinemachineComposer>();

        }

        void Update()
        {
            GetDirectionAndMove();
            Gravity();


            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");


            camComposer.m_ScreenY = Mathf.Clamp(camComposer.m_ScreenY + mouseY * sensitivityMouseY * Time.deltaTime, mouseYSensitivity.x, mouseYSensitivity.y);
            transform.Rotate(Vector3.up, mouseX * sensitivityMouseX * Time.deltaTime, Space.World);

            animator.SetFloat("hInput", hzInput);
            animator.SetFloat("vInput", vInput);

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("shoot");
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                animator.SetTrigger("reload");
            }

        }

        private void GetDirectionAndMove()
        {
            hzInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");

            float speed = 0;
            if (vInput > 0)
            {
                speed = moveSpeedFoward;
            }
            else
            {
                speed = moveSpeedBack;
            }
            if (hzInput != 0)
            {
                speed = moveSpeedSides;
            }

            dir = transform.forward * vInput + transform.right * hzInput;
            character.Move(dir * speed * Time.deltaTime);
        }

        private bool IsGround()
        {
            spherePos = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
            if (Physics.CheckSphere(spherePos, character.radius - 0.05f, groundMask)) return true;
            return false;
        }

        private void Gravity()
        {
            if (!IsGround()) velocity.y += gravity * Time.deltaTime;
            else if (velocity.y < 0) velocity.y = -2;

            character.Move(velocity * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spherePos, character.radius - 0.05f);
        }



        
    }
}
