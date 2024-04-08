using Cinemachine;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Horror.Player
{
    public class Player : MonoBehaviour
    {
        [Header("Public Variables")]
        [HideInInspector] public bool die;

        [Header("Set Configs")]
        [SerializeField] private CinemachineVirtualCamera vCam3;
        [SerializeField] private CinemachineVirtualCamera vCam1;
        [SerializeField] private CharacterController character;
        [SerializeField] private Transform aimRectTransform;
        [SerializeField] private Transform firstAimRectTransform;
        [SerializeField] private LayerMask hitLayer;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float gravity = -1;
        [SerializeField] private Light Lantern;
        [SerializeField] private TextMeshProUGUI textAmmunition;

        [Header("Set UI")]
        [SerializeField] private Image batteryFill;
        [SerializeField] private Image lifeFill;
        [SerializeField] private Image currentMagazineAmmunitionFill;
        [SerializeField] private Image currentAmmunitionFill;

        [Header("Player Stats")]
        [SerializeField] private int maxLife;
        [Range(0, 10)]
        [SerializeField] private int Initialbattery;
        [SerializeField] private int damagePistol;
        [SerializeField] private int damageKnife;
        [SerializeField] private int magazineAmmunition;
        [SerializeField] private int ammunition;
        [SerializeField] private float moveSpeedFoward;
        [SerializeField] private float moveSpeedBack;
        [SerializeField] private float moveSpeedSides;
        [SerializeField] private float groundOffset;
        [SerializeField] private Vector2 mouseYSensitivity;
        [SerializeField] private float sensitivityMouseX = 500f;
        [SerializeField] private float sensitivityMouseY = 5f;
        [SerializeField] private float fireSpeed = 0.5f;
        [SerializeField] private float reloadSpeed = 2.7f;


        [Header("Player Parts")]
        [SerializeField] private GameObject[] partsDesativar;


        private Coroutine coroutineTakeDamage;
        private CinemachineComposer camComposer;
        private Animator animator;
        private float hzInput, vInput;
        [SerializeField] private float currentLife;
        private Vector3 dir;
        private Vector3 spherePos;
        private Vector3 velocity;
        private bool value;
        private bool isFirstCam;
        private float currentGravity;
        private bool isReloadFinish;
        private bool isShotFinish;
        private bool lanternOn = true;
        private int currentMagazineAmmunition;
        private float battery;


        private bool key;
        public InteractiveScript OBJ { get; set; }


        private bool isPaused;

        public float Battery
        {
            get => battery;

            set
            {
                battery = value;

                if (battery > 10)
                {
                    battery = 10;
                }
                else if (battery < 0)
                {
                    battery = 0;
                }

            }
        }

        public bool IsPaused { get => isPaused; set => isPaused = value; }

        void Start()
        {
            character = GetComponent<CharacterController>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            Cursor.lockState = CursorLockMode.Locked;
            camComposer = vCam3.GetCinemachineComponent<CinemachineComposer>();
            currentMagazineAmmunition = Mathf.Clamp(ammunition, 0, magazineAmmunition);
            ammunition -= currentMagazineAmmunition;
            currentLife = maxLife;
            currentGravity = gravity;
            isReloadFinish = true;
            isShotFinish = true;
            Battery = Initialbattery;

        }

        void Update()
        {
            if (IsPaused) return;

            if (die) return;
            SetUI();
            OnOffLantern();
            GetDirectionAndMove();
            Gravity();
            SetRotation();
            SetAnimations();
            GetObj();

        }

        private void GetObj()
        {
            if (OBJ != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (OBJ.NameObj == "key")
                    {
                        key = true;
                        Destroy(OBJ.gameObject);
                    }

                    if (OBJ.NameObj == "battery")
                    {
                        Battery += 10;
                        Destroy(OBJ.gameObject);
                    }

                    if (OBJ.NameObj == "door")
                    {
                        OBJ.OpenDoor();
                    }

                }
            }
        }

        private void SetUI()
        {
            textAmmunition.text = $"{currentMagazineAmmunition}/{ammunition}";
            if (lanternOn)
            {
                Battery -= (Time.deltaTime / 60);
                batteryFill.fillAmount = Battery / 10;

                if (Battery < 1)
                {
                    Lantern.intensity = Battery;
                }
                else
                {
                    Lantern.intensity = 1;
                }
            }
        }

        private void OnOffLantern()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                lanternOn = !lanternOn;
                Lantern.gameObject.SetActive(lanternOn);
            }
        }

        public void TakeDamage(int Damage)
        {
            currentLife -= Damage;
            if (coroutineTakeDamage == null)
            {
                coroutineTakeDamage = StartCoroutine(TakeDamage());
            }
        }

        public IEnumerator TakeDamage()
        {
            lifeFill.fillAmount = (currentLife / maxLife);

            if (currentLife <= 0)
            {
                currentLife = 0;
                die = true;
                animator.SetTrigger("die");
                GetComponent<Collider>().enabled = false;
                yield break;
            }

            animator.SetTrigger("takeDamage");
            yield return new WaitForSeconds(0.2f);
            coroutineTakeDamage = null;
        }

        #region Private Methods
        private void ChangeCam(bool firstCam)
        {
            isFirstCam = firstCam;

            if (firstCam)
            {

                aimRectTransform.gameObject.SetActive(false);
                firstAimRectTransform.gameObject.SetActive(true);
                vCam3.gameObject.SetActive(false);
                vCam1.gameObject.SetActive(true);

                value = false;
                Invoke(nameof(PartsEnable), 1f);
            }
            else
            {
                firstAimRectTransform.gameObject.SetActive(false);
                aimRectTransform.gameObject.SetActive(true);
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
        private void Reload()
        {
            //todo fazer barulo de sem bala
            if (ammunition <= 0) return;
            if (!isReloadFinish || !isShotFinish) return;

            isReloadFinish = false;
            animator.SetTrigger("reload");
            currentMagazineAmmunition = Mathf.Clamp(ammunition, 0, magazineAmmunition);
            ammunition -= currentMagazineAmmunition;
            Invoke(nameof(ReloadFinish), reloadSpeed);
        }

        private void ReloadFinish()
        {
            isReloadFinish = true;
        }


        private void FireShot()
        {
            if (!isReloadFinish || !isShotFinish) return;


            if (currentMagazineAmmunition <= 0)
            {
                Reload();
            }
            else
            {
                isShotFinish = false;
                currentMagazineAmmunition--;
                animator.SetTrigger("shoot");

                Vector3 targetAim = isFirstCam ? firstAimRectTransform.position : aimRectTransform.position;

                Vector2 imageScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, targetAim);

                Ray ray = Camera.main.ScreenPointToRay(imageScreenPosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
                {
                    Debug.Log("Raycast hit object: " + hit.collider.gameObject.name);

                    Vector3 hitPoint = hit.point;

                    Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();

                    enemy.TakeDamage(damagePistol);
                    Debug.Log("Hit point in world space: " + hitPoint);

                }

                Invoke(nameof(ShotFinish), fireSpeed);
            }
        }

        private void ShotFinish()
        {
            isShotFinish = true;
        }


        private void SetAnimations()
        {

            animator.SetFloat("hInput", hzInput);
            animator.SetFloat("vInput", vInput);

            if (Input.GetMouseButtonDown(0))
            {
                FireShot();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
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
            else if (velocity.y < 0)
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
