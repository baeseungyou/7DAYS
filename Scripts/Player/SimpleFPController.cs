using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFPController : MonoBehaviour
{
    [Header("MOUSE LOOK")]
    public Vector2 mouseSensitivity = new Vector2(80, 80);
    public Vector2 verticalLookLimit = new Vector2(-85, 85);

    private float xRot;
    private Camera cam;

    [Header("MOVEMENT")]
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public float jumpForce = 150;
    private float speed = 2;
    private bool isGrounded = true;

    [Header("CONTROLS")]
    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode strafeLeft = KeyCode.A;
    public KeyCode strafeRight = KeyCode.D;
    public KeyCode run = KeyCode.LeftShift;
    public KeyCode jump = KeyCode.Space;

    [Header("SIGHT")]
    public bool sight = true;
    public GameObject sightPrefab;

    private Rigidbody rb;

    public bool hideCursor = false;

    [Header("ATTACK")]
    public float attackRange = 2f;

    [Header("SPEED")]
    public float maxStamina = 5f;
    public float staminaDecreaseRate = 1f; // 달릴 때마다 스피드 5중 1초에 1씩 깎임
    public float staminaRecoveryRate = 0.5f; // 안 달릴 때 초당 0.5씩 회복

    //private float currentStamina;
    private bool isRunningAllowed = true;

    [Header("SPEED UI")]
    public Slider staminaBarFill;

    [Header("CONT")]
    public KeyCode cont = KeyCode.LeftControl;
    public float standingCamHeight = 1.8f;
    public float crouchingCamHeight = 1f;
    public float crouchTransitionSpeed = 5f;

    private bool isCrouching = false;
    private float originalCamY;

    [Header("JUMP")]
    public float jumpCooldown = 0.5f;
    private float jumpTimer = 0f;


    private void OnEnable()
    {
        Cursor.visible = !hideCursor;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();

        Cursor.visible = !hideCursor;

        //currentStamina = maxStamina;

        if (sight)
        {
            GameObject sightObj = Instantiate(sightPrefab);
            sightObj.transform.SetParent(transform.parent);
        }

        originalCamY = cam.transform.localPosition.y;
    }

    void Update()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        CameraLook();
        PlayerMove();
        PlayerAttack();
        PlayerDown();

        if (staminaBarFill != null)
        {
            staminaBarFill.value = PlayerSpeedBar.Instance.currentStamina / PlayerSpeedBar.Instance.maxStamina;
        }

    }

    void CameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity.x * 10;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity.y * 10;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, verticalLookLimit.x, verticalLookLimit.y);
        cam.transform.localEulerAngles = new Vector3(xRot, 0, 0);

        transform.Rotate(Vector3.up * mouseX);
    }

    private bool isJumping = false;
    void PlayerMove()
    {
        bool isTryingToRun = Input.GetKey(run);
        float stamina = PlayerSpeedBar.Instance.currentStamina;
        //float staminaThreshold = 0;
        // PlayerSpeedBar.Instance.currentStamina 기준으로 판단
        //bool canRun = isTryingToRun && PlayerSpeedBar.Instance.currentStamina > staminaThreshold;


        if (isTryingToRun && stamina > 0f)
        {
            speed = runSpeed;
            PlayerSpeedBar.Instance.DecreaseStamina(Time.deltaTime * staminaDecreaseRate);
        }
        else if (!isTryingToRun)
        {
            speed = walkSpeed;
            PlayerSpeedBar.Instance.RecoverStamina(Time.deltaTime * staminaRecoveryRate);
        }

        else
        {
            speed = walkSpeed;

        }

        if (staminaBarFill != null)
        {
            // UI는 PlayerSpeedBar의 상태를 보여주도록!
            staminaBarFill.value = PlayerSpeedBar.Instance.currentStamina / PlayerSpeedBar.Instance.maxStamina;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f + 0.01f);

        if (Input.GetKeyDown(jump) && !isJumping)
        {
            Debug.Log("점프!");
            rb.velocity = new Vector3(rb.velocity.x, 5f, rb.velocity.z);
            isJumping = true; // 점프 중으로 설정
        }

        if (Input.GetKey(forward))
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        if (Input.GetKey(backward))
            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
        if (Input.GetKey(strafeLeft))
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.Self);
        if (Input.GetKey(strafeRight))
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
    }


    void PlayerAttack()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // 중앙 기준
            if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
            {
                ZombieAI zombie = hit.collider.GetComponentInParent<ZombieAI>();
                if (zombie != null)
                {
                    WeaponManager weaponManager = GetComponent<WeaponManager>();
                    float weaponDamage = 0;


                    if (weaponManager != null && weaponManager.GetEquippedWeapon() != null)
                    {
                        Weapon equippedWeapon = weaponManager.GetEquippedWeapon().GetComponent<Weapon>();
                        if (equippedWeapon != null)
                        {
                            weaponDamage = equippedWeapon.damage;
                        }
                    }

                    // 데미지 전달
                    zombie.TakeDamage((int)weaponDamage);
                    Debug.Log($"공격 성공 - {weaponDamage} 데미지!");
                }
                else
                {
                    Debug.Log("공격 - 다른 오브젝트 맞음: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("공격했지만 아무것도 맞지 않음");
            }
        }
    }

    void PlayerDown()
    {
        if (Input.GetKeyDown(cont))
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(cont))
        {
            isCrouching = false;
        }

        float targetHeight = isCrouching ? originalCamY - 0.5f : originalCamY; // 0.5f는 "내려가는 정도"
        Vector3 camPos = cam.transform.localPosition;

        camPos.y = Mathf.Lerp(camPos.y, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        cam.transform.localPosition = camPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 접촉 면의 법선이 위쪽(0,1,0)에 가까우면 바닥
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 45f)
            {
                isJumping = false; // 다시 점프 가능
                break;
            }
        }
    }
}