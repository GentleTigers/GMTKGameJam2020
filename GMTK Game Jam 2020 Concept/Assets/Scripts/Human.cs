using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanStatus {
    Healthy  = 0,
    Infected = 1,
    Imune    = 2,
    Doctor   = 3
}

public enum InfectionStage {
    NotInfected = 0,
    NoSymptoms  = 1,
    Infectious  = 2,
    // TODO add more stages.
}

public class Human : MonoBehaviour {


    private float speed = 2.5f;
    private Vector3 movement;
    public Animator animator;

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
            SetCorrectAnimation();
        }
    }

    [SerializeField] private InfectionStage stage = InfectionStage.NotInfected;
    public InfectionStage Stage {
        get => Status == HumanStatus.Infected ? stage : InfectionStage.NotInfected;
        set {
            stage = value;
            SetCorrectAnimation();
        }
    }
    private static float degressionTotalTime = 3f;
    private float degressionTimer = 0;

    private float imuneToHealthyTimer = 0;
    private static float imuneToHealthyTotalTime = 5f;



    void Start() {
        SetCorrectAnimation();
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


    /* UPDATE */

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
        HandleMovement();
        
    }

    private void HandleMovement() {
        transform.position += movement * speed * Time.fixedDeltaTime;
        // TODO rotation
        animator.SetFloat("Speed", movement.magnitude);
    }

    private void SetCorrectAnimation() {
        animator.SetInteger("InfectionStatus", (int)Status);
        animator.SetInteger("InfectionStage", (int)Stage);

        /*switch (status) {
            
            case HumanStatus.Healthy:
                animator.runtimeAnimatorController = Assets.instance.healthyAnimator;
                break;
            case HumanStatus.Infected:
                switch (Stage) {
                    case InfectionStage.NoSymptoms:
                        animator.runtimeAnimatorController = Assets.instance.infectedAnimator;
                        break;
                    case InfectionStage.Infectious:
                        animator.runtimeAnimatorController = Assets.instance.infectiousAnimator;
                        break;
                }
                break;
            case HumanStatus.Doctor:
                animator.runtimeAnimatorController = Assets.instance.healthyAnimator; // TODO
                break;
            case HumanStatus.Imune:
                animator.runtimeAnimatorController = Assets.instance.imuneAnimator;
                break;

        }*/
    }

    /// <summary>
    /// Adds one level of infection.
    /// </summary>
    public void AddInfectionStage() {
        Debug.Log("AddInfectionStage"); 
        if (Stage < InfectionStage.Infectious) { // TODO: if more stages add, put last stage here
            Stage += 1;
        }
    }

    /// <summary>
    /// Destroys the game object.
    /// </summary>
    private void Die() {
        GameObject.Destroy(gameObject);
    }
}
