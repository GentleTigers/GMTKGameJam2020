using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanStatus {
    Healthy  = 0,
    Infected = 1,
    Imune    = 2,
    Doctor   = 3,
    Dead     = 4
}

public enum InfectionStage {
    NotInfected = 0,
    NoSymptoms  = 1,
    Infectious  = 2,
    // TODO add more stages.
}

public enum CauseOfDeath {
    WATER
}

public class StatusChangedEventArgs : EventArgs {
    public HumanStatus NewStatus { get; }
    public HumanStatus OldStatus { get; }

    public StatusChangedEventArgs(HumanStatus oldStatus, HumanStatus newStatus) {
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public class StageChangedEventArgs : EventArgs {
    public InfectionStage OldStage { get; }
    public InfectionStage NewStage { get; }

    public StageChangedEventArgs(InfectionStage oldStage, InfectionStage newStage) {
        OldStage = oldStage;
        NewStage = newStage;
    }
}




public class Human : MonoBehaviour {

    private float speed = 2.5f;
    private Vector3 movement;
    private Vector3 lastMovement = new Vector3();
    [SerializeField] Vector2Int startDirection;
    [SerializeField] private Animator animator;
    public Animator Animator { get => animator != null ? animator : gameObject.GetComponent<Animator>(); set => animator = value; }

    [SerializeField] private HumanStatus status = HumanStatus.Healthy;
    public HumanStatus Status {
        get => status;
        set {
            HumanStatus oldStatus = status;
            status = value;
            if (status == HumanStatus.Infected) {
                stage = InfectionStage.NoSymptoms;
            } else {
                stage = InfectionStage.NotInfected;
            }
            OnStatusChanged(oldStatus, value);

        }
    }

    [SerializeField] private InfectionStage stage = InfectionStage.NotInfected;
    public InfectionStage Stage {
        get => Status == HumanStatus.Infected ? stage : InfectionStage.NotInfected;
        set {
            InfectionStage oldStage = stage;
            stage = value;
            degressionTimer = 0;
            OnStageChanged(oldStage, value);
        }
    }
    
    private float degressionTimer = 0;
    private float imuneToHealthyTimer = 0;

    public World CorrespondingWorld { get { return gameObject.GetComponentInParent<World>(); } }


    /* EVENTS */

    public event EventHandler<StatusChangedEventArgs> StatusChanged;
    public event EventHandler<StageChangedEventArgs> StageChanged;

    protected virtual void OnStatusChanged(HumanStatus oldStatus, HumanStatus newStatus) {
        StatusChanged?.Invoke(this, new StatusChangedEventArgs(oldStatus, newStatus));
    }

    protected virtual void OnStageChanged(InfectionStage oldStage, InfectionStage newStage) {
        StageChanged?.Invoke(this, new StageChangedEventArgs(oldStage, newStage));
    }



    void Start() {
        lastMovement = new Vector3(startDirection.x, startDirection.y);
        SetCorrectAnimation(this, EventArgs.Empty);
        StatusChanged += SetCorrectAnimation;
        StageChanged += SetCorrectAnimation;
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
        if (collision.gameObject.CompareTag("World")) {
            CollisionHumanLevel(this, collision.gameObject.GetComponent<World>());
        }
        /*EnvironmentObject environmentObject = collision.gameObject.GetComponent<EnvironmentObject>();
        if (environmentObject != null) {
            switch (environmentObject.type) {
                case EnvironmentType.Water:
                    CollisionHumanWater(this, environmentObject);
                    break;
            }
        }*/

    }


    private void CollisionInfectiousHealthy(Human infectious, Human healthy) {
        healthy.Status = HumanStatus.Infected;
    }

    private void CollisionInfectedDoctor(Human infected, Human doctor) {
        infected.Status = HumanStatus.Imune;
    }

    private void CollisionHumanLevel(Human human, World water) {
        human.Die(CauseOfDeath.WATER);
    }


    /* UPDATE (Movement and Status) */

    void Update() {
        if (!CorrespondingWorld.StartedPlaying) {
            return;
        }
        HandleMovementInput();

        switch (status) {
            case HumanStatus.Infected:
                degressionTimer += Time.deltaTime;
                if (degressionTimer >= CorrespondingWorld.DegressionTotalTime) {
                    AddInfectionStage();
                    degressionTimer = 0;
                }
                break;
            case HumanStatus.Imune:
                imuneToHealthyTimer += Time.deltaTime;
                if (imuneToHealthyTimer >= CorrespondingWorld.ImuneToHealthyTotalTime) {
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
        bool idle = movement.x == 0 && movement.y == 0;
        if (idle) {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lastMovement.y, lastMovement.x) * Mathf.Rad2Deg - 90);
        } else {
            transform.position += movement * speed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90);
            lastMovement = new Vector3(movement.x, movement.y);
        }
        Animator.SetFloat("Speed", movement.magnitude);
    }

    private void SetCorrectAnimation(object sender, EventArgs e) {
        Animator.SetInteger("InfectionStatus", (int)Status);
        Animator.SetInteger("InfectionStage", (int)Stage);

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
        if (Stage < InfectionStage.Infectious) { // TODO: if more stages add, put last stage here
            Stage += 1;
        }
    }

    /// <summary>
    /// Destroys the game object.
    /// </summary>
    private void Die(CauseOfDeath causeOfDeath) {
        if (causeOfDeath == CauseOfDeath.WATER) {
            AudioManager.instance.PlaySound("WaterSplash");
        }
        gameObject.SetActive(false);
        Status = HumanStatus.Dead;
    }
}

