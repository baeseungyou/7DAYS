using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSwing : MonoBehaviour, IWeaponSwing
{
    public Transform axeTransform;

    public float swingAngle = 30f;
    public float swingTime = 0.2f;
    public Vector3 swingAxis = new Vector3(0f, 0f, -1f);

    public AudioClip swingSound;
    private AudioSource audioSource;
    public bool _hasWeapon;
    public bool hasWeapon  
    {
        get { return _hasWeapon; }
        set { _hasWeapon = value; }
    }

    private bool isSwinging = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasWeapon && Input.GetMouseButtonDown(0) && !isSwinging)
        {
            StartCoroutine(Swing());
        }
    }
    
    IEnumerator Swing()
    {
        isSwinging = true;

        if (swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swingSound);
        }

        // 시작 각도 (로컬 회전값 기준)
        Quaternion startRot = axeTransform.localRotation;

        // 목표 각도 (Z축으로 -swingAngle만큼 회전)
        Quaternion peakRot = startRot * Quaternion.Euler(0f, 0f, -swingAngle);

        // 스윙 (내려가기)
        float timer = 0f;
        while (timer < swingTime)
        {
            axeTransform.localRotation = Quaternion.Slerp(startRot, peakRot, timer / swingTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // 스윙 (돌아오기)
        timer = 0f;
        while (timer < swingTime)
        {
            axeTransform.localRotation = Quaternion.Slerp(peakRot, startRot, timer / swingTime);
            timer += Time.deltaTime;
            yield return null;
        }

        axeTransform.localRotation = startRot;
        isSwinging = false;
    }
}
