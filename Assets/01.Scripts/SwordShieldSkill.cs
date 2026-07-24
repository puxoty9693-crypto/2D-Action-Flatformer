using System.Collections;
using UnityEngine;

public class SwordShieldSkill : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] private float invincibleDuration = 2f;
    [SerializeField] private float cooldownTime = 8f;

    [SerializeField] private Color invincibleColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private bool blink = true;
    [SerializeField] private float blinkInterval = 0.1f;

    private float lastUseTime = -999f;
    private SpriteRenderer sr;
    private PlayerHealth playerHealth;
    private Color originalColor;
    private Coroutine invincibleRoutine;
    public bool IsInvincible { get; private set; }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
        originalColor = sr.color;

    }

    public bool CanUse() 
    {
        return Time.time >= lastUseTime + cooldownTime;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction) 
    {
        if(!CanUse()) return;

        if(invincibleRoutine != null)
            StopCoroutine(invincibleRoutine);

        invincibleRoutine = StartCoroutine(InvincibleRoutine());
        lastUseTime = Time.time;
    }

    private IEnumerator InvincibleRoutine() 
    {
        IsInvincible = true;

        float elapsed = 0f;
        bool toggle = false;

        while(elapsed < invincibleDuration) 
        {
            if (blink) 
            {
                toggle = !toggle;
                sr.color = toggle ? invincibleColor : originalColor;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }
            else 
            {
                sr.color = invincibleColor;
                yield return null;
                elapsed += Time.deltaTime;
            }
        }

        sr.color = invincibleColor;
        elapsed += Time.deltaTime;
    }
}
