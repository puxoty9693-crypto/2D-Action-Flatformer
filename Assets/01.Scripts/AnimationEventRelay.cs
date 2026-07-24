using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    public void OnAttackHitFrame() 
    {
        enemyAI?.OnAttackHitFrame();
    }
}
