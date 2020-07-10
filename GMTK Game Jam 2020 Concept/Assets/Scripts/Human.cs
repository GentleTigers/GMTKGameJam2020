using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanStatus {
    Healthy,
    Infected,
    Imune,
    Doctor
}

public enum InfectionStage {
    NotInfected,
    NoSymptoms,
    Infectious,
    // TODO add more stages.
}

public class Human : MonoBehaviour {


    private float speed = 2.5f;
    private Vector3 movement;

    [SerializeField] private HumanStatus status = HumanStatus.Healthy;
    public HumanStatus Status {
        get => status;
        set {
            status = value;
            if (status == HumanStatus.Infected) {
                stage = InfectionStage.NoSymptoms;
            } else {
                stage = InfectionStage.NotInfected;
            }
            SetCorrectSprite();
        }
    }

    [SerializeField] private InfectionStage stage = InfectionStage.NotInfected;
    public InfectionStage Stage {
        get => Status == HumanStatus.Infected ? stage : InfectionStage.NotInfected;
        set {
            stage = value;
            SetCorrectSprite();
        }
    }
    private static float degressionTotalTime = 10f;
    private float degressionTimer = 0;

    private float imuneToHealthyTimer = 0;
    private static float imuneToHealthyTotalTime = 10f;
    private bool ForeverImune { get { return imuneToHealthyTotalTime < 0; } }


    
    void Start() {

    }


    /* COLLISIONS */

    private void OnCollisionEnter2D(Collision2D collision) {
        Human otherHuman = collision.gameObject.GetComponent<Human>();
        if (otherHuman != null) {
            switch (otherHuman.status) {
                case HumanStatus.Healthy:
                    if (this.Stage >= InfectionStage.Infectious) {
                        CollisionInfectiousHealthy(this, otherHuman);
                    }
                    break;
                case HumanStatus.Doctor:
                    if (this.Stage >= InfectionStage.NoSymptoms) {
                        CollisionInfectedDoctor(this, otherHuman);
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

    private void CollisionHumanWater(Human human, EnvironmentObject water) {
        human.Die();
    }


    void Update() {
        HandleMovementInput();

        switch (status) {
            case HumanStatus.Infected:
                degressionTimer += Time.deltaTime;
                if (degressionTimer >= degressionTotalTime) {
                    AddInfectionStage();
                    degressionTimer = 0;
                }
                break;
            case HumanStatus.Imune:
                imuneToHealthyTimer += Time.deltaTime;
                if (imuneToHealthyTimer >= imuneToHealthyTotalTime) {
                    Status = HumanStatus.Healthy;
                    imuneToHealthyTimer = 0;
                }
                break;
        }
        
    }

    private void HandleMovementInput() {
        if (Stage >= InfectionStage.Infectious) {
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
                switch (Stage) { // TODO: change color for each stage.
                    case InfectionStage.NoSymptoms:
                        spriteRenderer.sprite = Assets.instance.infectedSprite;
                        break;
                    case InfectionStage.Infectious:
                        spriteRenderer.sprite = Assets.instance.infectiousSprite;
                        break;
                }
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

    /// <summary>
    /// Adds one level of infection.
    /// </summary>
    public void AddInfectionStage() {
        Debug.Log("AddInfectionStage");
        Stage += 1;
    }

    /// <summary>
    /// Destroys the game object.
    /// </summary>
    private void Die() {
        GameObject.Destroy(gameObject);
    }
}
