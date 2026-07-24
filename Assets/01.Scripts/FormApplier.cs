using JetBrains.Annotations;
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
            controller.SetAttackBehaviour(new RangedAttack(newForm.projectileId, newForm.attackDamage, LayerMask.GetMask("Enemy")));
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

        if (!string.IsNullOrEmpty(skilllProjId)) 
        {
            controller.SetSkillBehaviour(new RangedAttack(skilllProjId, newForm.skillDamage, LayerMask.GetMask("Enemy")));

        }

        else 
        {
            WeaponHitbox hitbox = FormPoolingManager.instance.GetWeaponHitbox(newForm);
            if (hitbox != null)
                controller.SetSkillBehaviour(new MeleeAttack(hitbox, newForm.skillDamage, newForm.skillDuration, controller));
            else
                controller.SetSkillBehaviour(null);
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
