using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class SwordShieldSkill : MonoBehaviour, IAttackBehaviour, IPlayerBoundSkill
{
    [SerializeField] private float guardDuration = 2f;
    [SerializeField] private float cooldownTime = 8f;
    [SerializeField] private Color auraColor = new Color(0.4f, 0.8f, 1f, 0.6f);
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.7f;
    [SerializeField] private SpriteRenderer auraRenderer;

    private float lastUseTime = -999f;
    private Health health;
    private Coroutine guardRoutine;

    public bool IsGuarding { get; private set; }
    private void Awake()
    {
        ApplyAuraDefaults();
    }

    public void SetHealth(Health h) 
    {
        if(h != null) 
            health = h;
    }

    public void SetAuraRenderer(SpriteRenderer renderer)
    {
        auraRenderer = renderer;
        ApplyAuraDefaults();
    }

    private void ApplyAuraDefaults() 
    {
        if(auraRenderer == null) return;
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
        if (health == null) return;


        if (guardRoutine != null)
            StopCoroutine(guardRoutine);

        guardRoutine = StartCoroutine(GuardRoutine());
        lastUseTime = Time.time;
    }




    private IEnumerator GuardRoutine()
    {
       
        IsGuarding = true;

        if (auraRenderer != null)
            auraRenderer.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < guardDuration)
        {
            int missing = health.GetMaxHP() - health.GetCurrentHP();
            if (missing > 0)
                health.TakeDamage(-missing);
           


            if (auraRenderer != null)
            {
                float t = (Mathf.Sin(Time.time*pulseSpeed) + 1f) * 0.5f;
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
                Color c = auraColor;
                c.a = alpha;
                auraRenderer.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (auraRenderer != null)
            auraRenderer.gameObject.SetActive(false);
        
        IsGuarding = false;
       
    }

    
}