using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const string PARAM_MOVE = "1_Move";
    private const string PARAM_ATTACK = "2_Attack";
    private const string PARAM_DAMAGED = "3_Damaged";
    private const string PARAM_DEATH = "4_Death";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool isMoving) => animator?.SetBool(PARAM_MOVE, isMoving);
    public void PlayAttack() => animator?.SetTrigger(PARAM_ATTACK);
    public void PlayDamage() => animator?.SetTrigger(PARAM_DAMAGED);
    public void PlayDeath() => animator?.SetTrigger(PARAM_DEATH);
}