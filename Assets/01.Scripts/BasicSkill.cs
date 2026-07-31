using UnityEngine;

public class BasicSkill : MonoBehaviour, IAttackBehaviour, IPlayerBoundSkill
{
    [SerializeField] private ParticleSystem effectPrefab;
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private bool attachToPlayer = true;

    [SerializeField] private float cooldownTime = 5f;
    [SerializeField] private int healAmount = 50;

    private float lastUseTime = -999f;
    private Health health;
    private ParticleSystem spawnedEffect;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (effectSpawnPoint == null)
            effectSpawnPoint = transform;

        if (attachToPlayer && effectPrefab != null) 
        {
            spawnedEffect = Instantiate(effectPrefab, effectSpawnPoint.position,Quaternion.identity, effectSpawnPoint);
            spawnedEffect.Stop();
        }



    }

    public void SetHealth(Health h) 
    {
        if (h != null)
            health = h;
        
    }


    public bool CanUse() 
    {
        return Time.time >= lastUseTime + cooldownTime;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction) 
    {
        if (!CanUse())  return;
        

        PlayEffect(spawnPos);
        Heal();

        lastUseTime = Time.time;
    }

    private void PlayEffect(Vector3 spawnPos) 
    {
        if (effectPrefab == null) return;

        if (attachToPlayer && spawnedEffect != null) 
        {
            spawnedEffect.transform.position = spawnPos;
            spawnedEffect.Play();
        }
        else 
        {
            ParticleSystem instance = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            instance.Play();
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }

    }

    private void Heal() 
    {
        if (health == null) return;

        int missing = health.GetMaxHP() - health.GetCurrentHP();
        if (missing <= 0) return;

        int actualHeal = Mathf.Min(healAmount, missing);

        health.TakeDamage(-actualHeal);
    }

    public float GetCooldownRemaining() 
    {
        return Mathf.Max(0f, (lastUseTime + cooldownTime) - Time.time);
    }

}
