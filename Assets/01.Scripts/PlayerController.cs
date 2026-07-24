using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private Collider2D col;

    [SerializeField] float moveSpeed;
    [SerializeField] float defaultJumpPower;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform visualRoot;
    [SerializeField] private FormManager formManager;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackAnimDuration = 0.25f;
    [SerializeField] private float attackHitDelay = 0.15f;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float skillCooldown = 10f;
    [SerializeField] private float skillAnimDuration = 0.4f;
    [SerializeField] private float skillHitDelay = 0.2f;
    


    public void SetAttackCooldown(float cooldown) => attackCooldown = cooldown;
    public void SetSKillCooldown(float cooldown) => skillCooldown = cooldown;
    public void SetSkillAnimDuration(float duration) => skillAnimDuration = duration;

    private Dictionary<FormData, float> lastSkillTimeByForm = new Dictionary<FormData, float>();

    private bool wasMoving = false;
    private float lastAttackTime = -999f;
    
    private bool isAttacking = false;
    private bool isUsingSkill = false;
    


    private Coroutine skillHitCoroutine;
    private Coroutine attackHitCoroutine;


        

    private float dir;
    private bool isGround;

    private IJumpBehaviour jumpBehaviour;
    private IAttackBehaviour attackBehaviour;
    private IAttackBehaviour skillBehaviour;
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        jumpBehaviour = new MultiJump(defaultJumpPower, 1);
        
       
    }

    private float GetLastSkillTime(FormData form) => lastSkillTimeByForm.TryGetValue(form, out float t) ? t : -999f;

   

    // Update is called once per frame
    void Update()
    {
        dir = 0;
        if (Keyboard.current.aKey.isPressed)
            dir += -1;
        if (Keyboard.current.dKey.isPressed)
            dir += 1;

        // 그라운드 체크
        GroundCheck();

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            jumpBehaviour?.Jump(rb, isGround);
        }

        FormData currentForm = formManager?.GetCurrentForm();
        float lastSkillTime = GetLastSkillTime(currentForm);

        if (Mouse.current.leftButton.wasPressedThisFrame && !isUsingSkill && Time.time - lastAttackTime >= attackCooldown)
        {
           
            formManager?.PlayAnimation(PlayerState.ATTACK, 0);
            lastAttackTime = Time.time;
            isAttacking = true;

            if (attackHitCoroutine != null)
                StopCoroutine(attackHitCoroutine);
            attackHitCoroutine = StartCoroutine(AttackHitFrameRoutine());
            
        }

        if(Keyboard.current.rKey.wasPressedThisFrame && !isUsingSkill &&!isAttacking && Time.time - lastSkillTime >= skillCooldown) 
        {
            formManager?.PlaySkill();
            lastSkillTimeByForm[currentForm] = Time.time;
            isUsingSkill = true;

            if(skillHitCoroutine != null)
                StopCoroutine(skillHitCoroutine);
            skillHitCoroutine = StartCoroutine(SkillHitFrameRoutine());
        }



        if (isAttacking && Time.time - lastAttackTime >= attackAnimDuration) 
        {
            isAttacking = false;
            formManager?.PlayAnimation(dir != 0 ? PlayerState.MOVE : PlayerState.IDLE, 0);
            wasMoving = dir != 0;

        }

        if(isUsingSkill && Time.time - GetLastSkillTime(currentForm) >= skillAnimDuration) 
        {
            isUsingSkill = false;
            formManager?.PlayAnimation(dir != 0 ? PlayerState.MOVE : PlayerState.IDLE, 0);
            wasMoving = dir != 0;
        }


        


        if( dir != 0 && visualRoot != null) 
        {
            float sign = dir > 0 ? -1f : 1f;
            Vector3 scale = visualRoot.localScale;
            scale.x = sign * Mathf.Abs(scale.x);
            visualRoot.localScale = scale;
        }


        bool isMoving = dir != 0;

        if (!isAttacking && isMoving != wasMoving)
        {
            formManager?.PlayAnimation(isMoving ? PlayerState.MOVE : PlayerState.IDLE, 0);
            wasMoving = isMoving;
            
        }

    }

    public void OnAttackHitFrame() 
    {
        
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        attackBehaviour?.Attack(spawnPos, GetFacingDirection());
    }

    public void OnSkillHitFrame() 
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        skillBehaviour?.Attack(spawnPos, GetFacingDirection());
    }



    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.15f, Vector2.down, 0.4f, groundLayer);

        isGround = hit.collider != null && rb.linearVelocity.y <= 0.05f;

    }

    private Vector2 GetFacingDirection() 
    {
        if(visualRoot == null) 
        {
            return Vector2.right;
        }
        return visualRoot.localScale.x < 0 ? Vector2.right : Vector2.left;
    }

    



    public void SetJumpBehaviour(IJumpBehaviour newBehavior) => jumpBehaviour = newBehavior;
    public IJumpBehaviour GetJumpBehaviour() => jumpBehaviour;
    public void SetAttackBehaviour(IAttackBehaviour newBehavior) => attackBehaviour = newBehavior;

    public void SetSkillBehaviour(IAttackBehaviour newBehaviour) => skillBehaviour = newBehaviour;


    public void SetMoveSpeed(float speed) => moveSpeed = speed;

    private IEnumerator AttackHitFrameRoutine() 
    {

        
        yield return new WaitForSeconds(attackHitDelay);
        
        OnAttackHitFrame();
    }

    private IEnumerator SkillHitFrameRoutine() 
    {
        yield return new WaitForSeconds(skillHitDelay);
        OnSkillHitFrame();
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position - new Vector3(0, 0.4f, 0), 0.15f);
    }
}


