using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    public Transform DetectedPlayer { get; private set; }
    public float DetectRange => detectRange;
    public float AttackRange => attackRange;

    public EnemyState Detect()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (hit != null)
        {
            DetectedPlayer = hit.transform;
            float dist = Vector2.Distance(transform.position, DetectedPlayer.position);
            return dist <= attackRange ? EnemyState.Attack : EnemyState.Chase;
        }

        DetectedPlayer = null;
        return EnemyState.Idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}