using UnityEngine;

[CreateAssetMenu(fileName = "FormData", menuName = "Devour Vessel/Form Data")]
public class FormData : ScriptableObject
{
    public string formName;

    //스탯
    public float moveSpeed;
    public float jumpPower;
    public int maxHP;
    public int jumpCount;
    

    //외형
    public RuntimeAnimatorController animatorController;
    public GameObject formPrefab;

    //근접 공격
    public string weaponRange;
    public int attackDamage;
    public float attackDuration; //히트박스 온 시간
    public float attackCooldown;


    //원거리 공격
    public string projectileId;
    public string attackTriggerOverride;


    //스킬
    public string skillTriggerOverride;
    public float skillCooldown = 10f;
    public int skillDamage;
    public float skillDuration;
    public string skillProjectileId;
    public float skillAnimDuration = 0.4f;

    //연사 스킬
    public float skillRange = 15f;
    public int skillBurstCount = 1;
    public float skillBurstInterval = 0.15f;


}
