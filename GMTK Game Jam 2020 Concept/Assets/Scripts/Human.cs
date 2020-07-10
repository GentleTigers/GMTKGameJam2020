using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanStatus {
    Healthy,
    Infected,
    Infectious,
    Imune,
    Doctor
}

public class Human : MonoBehaviour {


    private float speed = 2.5f;

    private Vector3 movement;

    [SerializeField] private HumanStatus status = HumanStatus.Healthy;
    public HumanStatus Status { get => status; set {
            status = value;
            SetCorrectSprite();
        }
    }

    

    private float infectionTimer = 5f;


    // Start is called before the first frame update
    void Start() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Human otherHuman = collision.gameObject.GetComponent<Human>();
        if (otherHuman != null) {
            if (otherHuman.status == HumanStatus.Healthy) {
                otherHuman.Status = HumanStatus.Infected;
            }
        }
        
    }

    
    void Update() {
        HandleMovementInput();
        if (status == HumanStatus.Infected) {
            infectionTimer -= Time.deltaTime;
            if (infectionTimer <= 0) {
                Status = HumanStatus.Infectious;
            }
        }
    }

    private void HandleMovementInput() {
        if (status == HumanStatus.Infectious) {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement.Normalize();
        }
    }

    private void FixedUpdate() {
        // TODO rotation
        transform.position += movement * speed * Time.deltaTime;
    }

    private void SetCorrectSprite() {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        switch (status) {
            
            // TODO set sprite
            case HumanStatus.Healthy:
                spriteRenderer.sprite = Assets.instance.healthySprite;
                break;
            case HumanStatus.Infected:
                spriteRenderer.sprite = Assets.instance.infectedSprite;
                break;
            case HumanStatus.Infectious:
                spriteRenderer.sprite = Assets.instance.infectiousSprite;
                break;
            default:
                break;
        }
    }
}
