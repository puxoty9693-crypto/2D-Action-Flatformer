using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class FormPoolingManager : MonoBehaviour
{
    public static FormPoolingManager instance;

    [SerializeField] private Transform visualRoot;
    [SerializeField] private FormData[] allForms;
    

    private Dictionary<FormData, GameObject> pool = new Dictionary<FormData, GameObject>();
    private Dictionary<FormData, WeaponHitbox> weaponHitboxes = new Dictionary<FormData, WeaponHitbox> ();
    private Dictionary<FormData, SPUM_Prefabs> spumComponents = new Dictionary<FormData, SPUM_Prefabs> ();
    private Dictionary<FormData, Animator> animators = new Dictionary<FormData, Animator> ();

    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject); return;
        }
        BuildPool();
    }

    private Transform FindDeep(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }

            Transform result = FindDeep(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }



    private void BuildPool()
    {
        foreach (var form in allForms) 
        { 
            if (form == null || form.formPrefab == null) continue;
            if (pool.ContainsKey(form)) continue;

            GameObject instance = Instantiate(form.formPrefab, visualRoot);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            SPUM_Prefabs spum = instance.GetComponent< SPUM_Prefabs>();
            if(spum != null) 
            {
                spum.OverrideControllerInit();
                
                Animator anim = instance.GetComponentInChildren<Animator>();
                if (anim != null) 
                {
                    animators[form] = anim;
                }
                spumComponents[form] = spum;

            }

            if(!string.IsNullOrEmpty(form.weaponRange))
            {
                Transform weapon = FindDeep(instance.transform,form.weaponRange);
                
                if (weapon != null) 
                {
                    if(weapon.GetComponent<Collider2D>() == null) 
                    {
                        BoxCollider2D col = weapon.gameObject.AddComponent<BoxCollider2D>();
                        col.isTrigger = true;
                    }
                    
                    WeaponHitbox hitbox = weapon.gameObject.AddComponent<WeaponHitbox>();

                    weaponHitboxes[form] = hitbox;
                }
                else 
                {

                }
            }

            instance.SetActive(false);
            pool[form] = instance;

        }

    }

    public Animator GetAnimator(FormData form)
    {
        animators.TryGetValue(form, out Animator anim);
        return anim;
    }




    public GameObject GetVisual(FormData form)
    {
        if (form == null) return null;

        if (pool.TryGetValue(form, out GameObject visual))
        {
            return visual;

        }

        return null;

    } 

    public WeaponHitbox GetWeaponHitbox(FormData form) 
    {
        if(form == null) 
            return null;
        weaponHitboxes.TryGetValue(form, out WeaponHitbox hitbox);
        return hitbox;

    }   

    public SPUM_Prefabs GetSpumPrefab(FormData form) 
    {
        if(form==null) return null;
        spumComponents.TryGetValue(form, out SPUM_Prefabs spum);
        return spum;
    }

}




