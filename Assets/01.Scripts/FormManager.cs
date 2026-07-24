using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class FormManager : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private FormData defaultForm;
    [SerializeField] private CorpseDetector corpseDetector;
   


    private FormData currentForm;
    private FormApplier applier;

    private FormData[] formSlots = new  FormData[2];
    private int currentSlotIndex = 0;



    private PlayerState lastState = PlayerState.IDLE;
    private int lastindex = 0;


    private void Awake()
    {
        applier = new FormApplier(controller, playerHealth);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (defaultForm != null) 
        {
            formSlots[0] = defaultForm;
            currentSlotIndex = 0;
            ChangeForm(defaultForm);
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) 
        {
            TryAbsortForm();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame) 
        {
            CycleSlot();
        }
    }

    private void TryAbsortForm() 
    {
        CorpseObject nearbyCorpse = corpseDetector.nearCorpse;

        if (nearbyCorpse == null || nearbyCorpse.formData == null)
            return;

        if (currentForm == nearbyCorpse.formData)
            return;

        formSlots[1] = nearbyCorpse.formData;
        currentSlotIndex = 1;
        ChangeForm(nearbyCorpse.formData);

    }

    private void CycleSlot()
    {
        int otherIndex = currentSlotIndex == 0 ? 1 : 0;

        if (formSlots[otherIndex] == null)
            return;

        currentSlotIndex = otherIndex;
        ChangeForm(formSlots[currentSlotIndex]);
            
    }



    public void ChangeForm(FormData newForm) 
    {
        if (newForm == null)
            return;
            

        currentForm = newForm;
        applier.Apply(newForm);

        applier.PlayAnimation(lastState, lastindex);
        
    }
    
    public void PlaySkill() 
    {
        if(currentForm == null || string.IsNullOrEmpty(currentForm.skillTriggerOverride))
            return;
        applier.PlayCustomTrigger(currentForm.skillTriggerOverride);
    }


    public void PlayAnimation(PlayerState state, int index = 0)
    {
        lastState = state;
        lastindex = index;

        if (state == PlayerState.ATTACK && currentForm != null && !string.IsNullOrEmpty(currentForm.attackTriggerOverride))
        {
            applier.PlayCustomTrigger(currentForm.attackTriggerOverride);
        }

        else
        {
            applier.PlayAnimation(state, index);
        }
    }


    public FormData GetCurrentForm() => currentForm;
    
}
