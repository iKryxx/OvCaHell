using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Vector2 input;
    public float speed;

    [HideInInspector] float m = 1;
    [HideInInspector] private float CurrentSpeed;
    [HideInInspector] bool shouldBeFlipped;

    /*[HideInInspector]*/ public bool triggerInput;
    public GameObject holdingItem;

    public static PlayerMovement instance;

    public float currentSpeed { get { return CurrentSpeed; } }
    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y/10000);
        manageMovingAnimation();

    }

    private void manageMovingAnimation()
    {
        if (input.x < 0)
            m = -1;
        else if (input.x > 0)
            m = 1;

        CurrentSpeed = rb.velocity.sqrMagnitude * m;
        if (CurrentSpeed != 0)
            AnimationManager.instance.playWalkingAnimation();
        else
            AnimationManager.instance.stopWalkingAnimation();
        if (holdingItem.activeSelf)
        {
            shouldBeFlipped = !(270 < holdingItem.transform.parent.eulerAngles.z || holdingItem.transform.parent.eulerAngles.z < 90);
            if(!shouldBeFlipped)
                holdingItem.transform.localRotation = Quaternion.Euler(180, 0, -45);
            else
                holdingItem.transform.localRotation = Quaternion.Euler(0, 0, -45);


        }
        else
        {
            if (CurrentSpeed < 0) shouldBeFlipped = true;
            if (currentSpeed > 0) shouldBeFlipped = false;
        }

        foreach (Transform child in transform.Find("Body"))
        {
            if (child.name == "Feet")
                child.GetChild(0).GetComponent<SpriteRenderer>().flipX = shouldBeFlipped;
            else
                child.GetComponent<SpriteRenderer>().flipX = shouldBeFlipped;
        }
        
    }

    void FixedUpdate() {
        rb.velocity = input * speed; 
    }
}
