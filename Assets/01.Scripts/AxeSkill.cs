using System.Collections;
using UnityEngine;

public class AxeSkill : MonoBehaviour, IAttackBehaviour, IPlayerBoundSkill
{
    [SerializeField] private float buffDuration = 10f;
    [SerializeField] private float cooldownTime = 15f;
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float attackSpeedMultiplier = 1.5f;
    [SerializeField] private float maxHpMultiplier = 1.5f;
    [SerializeField] private Color auraColor = new Color(1f, 0.4f, 0.1f, 1f);
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.7f;
    [SerializeField] private SpriteRenderer auraRenderer;

    private float lastUseTime = -999f;
    private Health health;
    private PlayerController controller;
    private WeaponHitbox hitbox;
    private int baseAttackDamage;
    private float baseAttackDuration;
    private float baseAttackCooldown;
    private int baseMaxpHP;
    private Coroutine buffRoutine;

    public bool IsBuffed { get; private set; }

    private void Awake()
    {
        ApplyAuraDefaults();
    }

    public void SetHealth(Health h) 
    {
        if (h != null)
            health = h;
    }

    public void SetAuraRenderer(SpriteRenderer renderer) 
    { 
        auraRenderer = renderer;
        ApplyAuraDefaults();
    }

    public void SetContext(PlayerController ctrl, WeaponHitbox hb, int attackDamage, float attackDuration, float attackCooldown, int maxHP) 
    {
        controller = ctrl;
        hitbox = hb;
        baseAttackDamage = attackDamage;
        baseAttackDuration = attackDuration;
        baseAttackCooldown = attackCooldown;
        baseMaxpHP = maxHP;
    }

    public void ApplyAuraDefaults() 
    {
        if (auraRenderer == null) return;
        auraRenderer.color = auraColor;
        auraRenderer.gameObject.SetActive(false);
    }

    public bool CanUse() 
    {
        return Time.time >= lastUseTime + cooldownTime;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction) 
    {
        if(!CanUse()) return;
        if (health == null || controller == null || hitbox == null) return;

        if (buffRoutine != null)
            StopCoroutine(buffRoutine);

        buffRoutine = StartCoroutine(BuffRoutine());
        lastUseTime = Time.time;
        
    }
    
    private IEnumerator BuffRoutine() 
    {
        IsBuffed = true;

        int buffDamage = Mathf.RoundToInt(baseAttackDamage*damageMultiplier);
        float buffCooldown = baseAttackCooldown / attackSpeedMultiplier;
        int buffedMaxHP = Mathf.RoundToInt(baseMaxpHP*maxHpMultiplier);

        controller.SetAttackBehaviour(new MeleeAttack(hitbox, buffDamage, baseAttackDuration, controller));
        controller.SetAttackCooldown(buffCooldown);
        health.SetMaxHP(buffedMaxHP);

        if (auraRenderer != null)
            auraRenderer.gameObject.SetActive(true);

        float elapsed = 0f;
        while(elapsed < buffDuration) 
        {
            if(auraRenderer != null) 
            {
                float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
                Color c = auraColor;
                c.a = alpha;
                auraRenderer.color = c;
            }
            elapsed += Time.deltaTime;
            yield return null;

        }

        controller.SetAttackBehaviour(new MeleeAttack(hitbox, baseAttackDamage, baseAttackDuration, controller));
        controller.SetAttackCooldown(baseAttackCooldown);
        health.SetMaxHP(baseMaxpHP);

        if (auraRenderer != null)
            auraRenderer.gameObject.SetActive(false);


        IsBuffed = false;

    }
}
