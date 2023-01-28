using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float WalkSpeed = 5f;
    [SerializeField] private float RunSpeed = 5f;


    [Space]
    [SerializeField] private float FallSpeedForce = 0.2f;
    [SerializeField] private float MaxFallSpeed = 10f;
   
    private float GroundRangeCheck = 1f;
    private float CurrentFallSpeed;

    [Space]
    [SerializeField] private float CrouchWalkSpeed = 3f;
    [SerializeField] private float CrouchingSpeed = 0.2f;
    [SerializeField] private float CrouchingLenght = 3f;

    [Space]
    [SerializeField] private float SensX = 200f;
    [SerializeField] private float SensY = 200f;

    private float XRotation = 0f;
    private bool isCrouching = false;

    private Camera Cam = null;
    private CharacterController Character = null;
    //private PhotonView view = null;

    private IEnumerator CrouchEnum = null;

   // public PhotonView GetView => view;
    public CharacterController GetCharacter => Character;

    public static PlayerController Instance = null;

    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        LockCursor(CursorLockMode.Locked, false);

        Character = GetComponent<CharacterController>();
        Cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        Move();
        Crouch();
        CameraMove();
    }

    private void Move()
    {
        float AxisX = Input.GetAxis("Horizontal");
        float AxisZ = Input.GetAxis("Vertical");

        Vector3 NewPosition = transform.right * AxisX + transform.forward * AxisZ;
        NewPosition.Normalize();

        if(isGrounded() == false)
        {
            Fall();
        }

        Character.Move(NewPosition * Time.deltaTime * CurrentSpeed());
    }

    private float CurrentSpeed()
    {
        if (isCrouching == true)
        {
            return CrouchWalkSpeed;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            return RunSpeed;
        }
        else
        {
            return WalkSpeed;
        }   
    }

    private void Fall()
    {
        CurrentFallSpeed += FallSpeedForce;

        if(CurrentFallSpeed > MaxFallSpeed)
        {
            CurrentFallSpeed = MaxFallSpeed;
        }

        Character.Move(new Vector3(0f, -CurrentFallSpeed * Time.deltaTime, 0f));

        if (isGrounded() == true)
        {
            CurrentFallSpeed = 0f;
        }
    }

    private bool isGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, GroundRangeCheck))
        {
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.gameObject.layer == 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.V))
        {
            if (isCrouching == false)
            {
                if (CrouchEnum != null)
                {
                    StopCoroutine(CrouchEnum);
                }

                CrouchEnum = CrouchCoroutine(.5f);

                StartCoroutine(CrouchEnum);
                isCrouching = true;
            }
        }
        else
        {
            if (isCrouching == true)
            {
                if (CrouchEnum != null)
                {
                    StopCoroutine(CrouchEnum);
                }

                CrouchEnum = CrouchCoroutine(1f);

                StartCoroutine(CrouchEnum);
            }

            isCrouching = false;
        }
    }

    private IEnumerator CrouchCoroutine(float YScale)
    {
        float TimeElpased = 0f;
        Vector3 newScale = new Vector3(transform.localScale.x, YScale, transform.localScale.z);

        while (TimeElpased < CrouchingLenght)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, TimeElpased / CrouchingLenght);
            TimeElpased += CrouchingSpeed;
            yield return null;
        }

        if (isCrouching)
        {
            GroundRangeCheck = GroundRangeCheck / 2;
        }
        else
        {
            GroundRangeCheck = 1f;
        }
    }

    private void CameraMove()
    {
        float MouseX = Input.GetAxis("Mouse X")  * SensX;
        float MouseY = Input.GetAxis("Mouse Y")  * SensY;

        XRotation -= MouseY;
        XRotation = Mathf.Clamp(XRotation, -90, 90);

        Cam.transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
        transform.Rotate(Vector3.up * MouseX);
    }

    public void LockCursor(CursorLockMode LockMode)
    {
        Cursor.lockState = LockMode;
    }

    public void LockCursor(CursorLockMode LockMode, bool isVisible)
    {
        Cursor.visible = isVisible;
        LockCursor(LockMode);
    }
}

[System.Serializable]
public struct AudioController 
{
    [SerializeField] private string Tag;
    public string GetTag => Tag;

    [SerializeField] AudioClip[] audioClips;
    public AudioClip[] GetAudioClips => audioClips;

}

