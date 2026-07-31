using JetBrains.Annotations;
using Unity.Burst;
using UnityEngine;

public class FormApplier
{
    private PlayerController controller;
    private PlayerHealth playerHealth;
    private GameObject currentVisual;
    private SPUM_Prefabs currentSpum;
    private Animator currentAnimator;

    public FormApplier(PlayerController controller, PlayerHealth playerHealth) 
    {
        this.controller = controller;
        this.playerHealth = playerHealth;
    }

    public void Apply(FormData newForm)
    {
        

        int presserveJC = controller.GetJumpBehaviour()?.CurrentJumpCount ?? 0;
        IJumpBehaviour newJumpBehaviour = new MultiJump(newForm.jumpPower, newForm.jumpCount);
        newJumpBehaviour.CurrentJumpCount = presserveJC;
        
        controller.SetJumpBehaviour(newJumpBehaviour);
        controller.SetMoveSpeed(newForm.moveSpeed);
        controller.SetSKillCooldown(newForm.skillCooldown);
        controller.SetAttackCooldown(newForm.attackCooldown);   
        controller.SetSkillAnimDuration(newForm.skillAnimDuration); 
        
        playerHealth.SetMaxHP(newForm.maxHP);


        

        if (currentVisual != null)
        {
            currentVisual.SetActive(false);
        }

        GameObject newVisual = FormPoolingManager.instance.GetVisual(newForm);
        if (newVisual != null)
        {
            newVisual.SetActive(true);
        }

        currentVisual = newVisual;

        currentSpum = FormPoolingManager.instance.GetSpumPrefab(newForm);
        currentAnimator = FormPoolingManager.instance.GetAnimator(newForm);

        string skilllProjId = !string.IsNullOrEmpty(newForm.skillProjectileId) ? newForm.skillProjectileId : newForm.projectileId;

        if (!string.IsNullOrEmpty(newForm.projectileId))
        {
            controller.SetAttackBehaviour(new RangedAttack(newForm.projectileId, LayerMask.GetMask("Enemy")));
        }

        


        else
        {

            WeaponHitbox hitbox = FormPoolingManager.instance.GetWeaponHitbox(newForm);

            if (hitbox != null)
            {
                controller.SetAttackBehaviour(new MeleeAttack(hitbox, newForm.attackDamage, newForm.attackDuration, controller));
            }
            else
            {
                controller.SetAttackBehaviour(null);
            }
        }


        IAttackBehaviour customSkill = newVisual != null ? newVisual.GetComponentInChildren<IAttackBehaviour>() : null;

        if(customSkill is IPlayerBoundSkill playerBound) 
        {
            playerBound.SetHealth(playerHealth);
        }


        if(customSkill is SwordShieldSkill swordShield) 
        {
            swordShield.SetAuraRenderer(controller.GetInvincibleAuraRenderer());
        }
        
        if(customSkill is AxeSkill axeSkill) 
        {
            WeaponHitbox axeHitbox = FormPoolingManager.instance.GetWeaponHitbox(newForm);
            axeSkill.SetContext(controller, axeHitbox, newForm.attackDamage, newForm.attackDuration, newForm.attackCooldown, newForm.maxHP);
            axeSkill.SetAuraRenderer(controller.GetInvincibleAuraRenderer());

        }

        if (customSkill != null)
        {
            controller.SetSkillBehaviour(customSkill);
        }
        else if (!string.IsNullOrEmpty(skilllProjId))
        {
            if (newForm.skillBurstCount > 1) 
            {
                controller.SetSkillBehaviour(new ArcherSkill(skilllProjId, LayerMask.GetMask("Enemy"), newForm.skillRange, newForm.skillBurstCount, newForm.skillBurstInterval, controller));
            }

            else 
            {
                
                controller.SetSkillBehaviour(new RangedAttack(skilllProjId, LayerMask.GetMask("Enemy")));

            }
        }

        else
        {
            WeaponHitbox hitbox = FormPoolingManager.instance.GetWeaponHitbox(newForm);
            controller.SetSkillBehaviour(hitbox != null ? new MeleeAttack(hitbox, newForm.skillDamage, newForm.skillDuration, controller) : null);
        }



    }
    public void PlayAnimation(PlayerState state, int index = 0) 
    {
        currentSpum?.PlayAnimation(state, index);
    }

    public void PlayCustomTrigger(string triggerName) 
    {
        if (string.IsNullOrEmpty(triggerName)) 
            return;
        currentAnimator?.SetTrigger(triggerName);
    }
  }
