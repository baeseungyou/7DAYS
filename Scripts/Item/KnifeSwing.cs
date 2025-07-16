using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSwing : MonoBehaviour, IWeaponSwing
{
    public Transform axeTransform;

    public float swingAngle = 40f;
    public float swingTime = 0.2f;
    //public Vector3 swingAxis = new Vector3(-1f, 0f, 0f);
    public AudioClip swingSound;
    private AudioSource audioSource;
    private bool _hasWeapon;
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

        Vector3 cameraForwardLocal = axeTransform.parent.InverseTransformDirection(Camera.main.transform.forward);

        Vector3 startPos = axeTransform.localPosition;
        Vector3 peakPos = startPos + cameraForwardLocal * 0.2f;

        float timer = 0f;
        while (timer < swingTime)
        {
            axeTransform.localPosition = Vector3.Lerp(startPos, peakPos, timer / swingTime);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < swingTime)
        {
            axeTransform.localPosition = Vector3.Lerp(peakPos, startPos, timer / swingTime);
            timer += Time.deltaTime;
            yield return null;
        }

        axeTransform.localPosition = startPos;
        isSwinging = false;
    }
}
