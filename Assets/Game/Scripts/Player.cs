using Cinemachine;
using System.Threading;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Horror.Player
{
    public class Player : MonoBehaviour
    {
        [Header("Set Configs")]
        [SerializeField] private CinemachineVirtualCamera vCam3;
        [SerializeField] private CinemachineVirtualCamera vCam1;
        [SerializeField] private CharacterController character;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float gravity = -1;

        [Header("Player Stats")]
        [SerializeField] private int maxLife;
        [SerializeField] private int magazineAmmunition;
        [SerializeField] private int initAmmunition;
        [SerializeField] private float moveSpeedFoward;
        [SerializeField] private float moveSpeedBack;
        [SerializeField] private float moveSpeedSides;
        [SerializeField] private float groundOffset;
        [SerializeField] private Vector2 mouseYSensitivity;
        [SerializeField] private float sensitivityMouseX = 500f;
        [SerializeField] private float sensitivityMouseY = 5f;




        [Header("Player Parts")]
        [SerializeField] private GameObject[] partsDesativar;

        private CinemachineComposer camComposer;
        private Animator animator;
        private float hzInput, vInput;
        private int currentLife;
        private bool die;
        private Vector3 dir;
        private Vector3 spherePos;
        private Vector3 velocity;
        private bool value;
        private bool isFirstCam;
        float currentGravity;

        void Start()
        {
            character = GetComponent<CharacterController>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            Cursor.lockState = CursorLockMode.Locked;
            camComposer = vCam3.GetCinemachineComponent<CinemachineComposer>();
            currentLife = maxLife;
            currentGravity = gravity;

        }

        void Update()
        {
            if (die) return;

            GetDirectionAndMove();
            Gravity();
            SetRotation();
            SetAnimations();
        }

        public void TakeDamage(int Damage)
        {
            currentLife -= Damage;

            if (currentLife < 0)
            {
                die = true;
                currentLife = 0;
            }

        }

        #region Private Methods
        private void ChangeCam(bool firstCam)
        {
            isFirstCam = firstCam;

            if (firstCam)
            {
                vCam3.gameObject.SetActive(false);
                vCam1.gameObject.SetActive(true);

                value = false;
                Invoke(nameof(PartsEnable), 1f);
            }
            else
            {
                vCam3.gameObject.SetActive(true);
                vCam1.gameObject.SetActive(false);
                value = true;
                Invoke(nameof(PartsEnable), 0.7f);
            }

        }

        private void PartsEnable()
        {
            for (int i = 0; i < partsDesativar.Length; i++)
            {
                partsDesativar[i].gameObject.SetActive(value);
            }
        }

        private void FireShot()
        {

        }
        private void SetAnimations()
        {

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

        private void SetRotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            camComposer.m_ScreenY = Mathf.Clamp(camComposer.m_ScreenY + mouseY * sensitivityMouseY * Time.deltaTime, mouseYSensitivity.x, mouseYSensitivity.y);
            transform.Rotate(Vector3.up, mouseX * sensitivityMouseX * Time.deltaTime, Space.World);
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
            if (!IsGround()) velocity.y += currentGravity * Time.deltaTime;
            else if(velocity.y < 0) 
            {
                currentGravity = gravity;
                velocity.y = -2;
            }

            character.Move(velocity * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spherePos, character.radius - 0.05f);
        }

        #endregion

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("FirstCam"))
            {
                if (isFirstCam) return;
                
                ChangeCam(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("FirstCam"))
            {
                ChangeCam(false);
            }
        }

    }
}
