using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack
}

public class Enemy : MonoBehaviour, IPoolable
{
    protected EnemyDetector detector;
    protected EnemyMover mover;
    protected EnemyFacing facing;
    protected EnemyAnimator anim;

    protected EnemyState currentState = EnemyState.Idle;
    protected EnemyState previousState = EnemyState.Idle;
    protected Transform player;

    protected virtual void Awake()
    {
        detector = GetComponent<EnemyDetector>();
        mover = GetComponent<EnemyMover>();
        facing = GetComponent<EnemyFacing>();
        anim = GetComponent<EnemyAnimator>();
    }

    public virtual void OnSpawn()
    {
        currentState = EnemyState.Idle;
        previousState = EnemyState.Idle;
        player = null;
        mover.Stop();
        anim?.SetMoving(false);
    }

    public virtual void OnDespawn()
    {
        mover.Stop();
        player = null;
        currentState = EnemyState.Idle;
    }

    protected virtual void Update()
    {
        currentState = detector.Detect();
        player = detector.DetectedPlayer;

        if (currentState != previousState)
        {
            OnStateChanged(previousState, currentState);
            previousState = currentState;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                OnIdle();
                break;
            case EnemyState.Chase:
                OnChase();
                break;
            case EnemyState.Attack:
                OnAttack();
                break;
        }
    }

    protected virtual void OnStateChanged(EnemyState from, EnemyState to)
    {
        anim?.SetMoving(to == EnemyState.Chase);
        if (to == EnemyState.Attack)
            anim?.PlayAttack();
    }

    protected virtual void OnIdle()
    {
        mover.Stop();
    }

    protected virtual void OnChase()
    {
        if (player == null) return;
        mover.MoveTowards(player.position);
        facing?.Face(mover.GetDirX(player.position));
    }

    protected virtual void OnAttack()
    {
        mover.Stop();
        if (player != null)
            facing?.Face(mover.GetDirX(player.position));
    }

    protected void PlayAttackAnimation() 
    {
        anim?.PlayAttack();
    }

    public void PlayDamage() => anim?.PlayDamage();
    public void PlayDeath() => anim?.PlayDeath();
}