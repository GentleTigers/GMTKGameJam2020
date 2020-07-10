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


    
    void Start() {

    }


    /* COLLISIONS */

    private void OnCollisionEnter2D(Collision2D collision) {
        Human otherHuman = collision.gameObject.GetComponent<Human>();
        if (otherHuman != null) {
            switch (otherHuman.status) {
                case HumanStatus.Healthy:
                    switch (this.Status) {
                        case HumanStatus.Infectious:
                            CollisionInfectiousHealthy(this, otherHuman);
                            break;
                    }
                    break;
                case HumanStatus.Doctor:
                    switch (this.Status) {
                        case HumanStatus.Infectious:
                            CollisionInfectiousDoctor(this, otherHuman);
                            break;
                        case HumanStatus.Infected:
                            CollisionInfectedDoctor(this, otherHuman);
                            break;
                    }
                    break;
            }
        }
        EnvironmentObject environmentObject = collision.gameObject.GetComponent<EnvironmentObject>();
        if (environmentObject != null) {
            switch (environmentObject.type) {
                case EnvironmentType.Water:
                    CollisionHumanWater(this, environmentObject);
                    break;
            }
        }
    }


    private void CollisionInfectiousHealthy(Human infectious, Human healthy) {
        healthy.Status = HumanStatus.Infected;
    }

    private void CollisionInfectedDoctor(Human infected, Human doctor) {
        infected.Status = HumanStatus.Imune;
    }

    private void CollisionInfectiousDoctor(Human infectious, Human doctor) {
        infectious.Status = HumanStatus.Imune;
    }

    private void CollisionHumanWater(Human human, EnvironmentObject water) {
        human.Die();
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
        } else {
            movement.x = 0;
            movement.y = 0;
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
            case HumanStatus.Doctor:
                spriteRenderer.sprite = Assets.instance.doctorSprite;
                break;
            case HumanStatus.Imune:
                spriteRenderer.sprite = Assets.instance.imuneSprite;
                break;
            default:
                break;
        }
    }


    private void Die() {
        GameObject.Destroy(gameObject);
    }
}
