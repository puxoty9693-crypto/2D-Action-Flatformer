using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void OnAttackHitFrame() 
    {
        controller?.OnAttackHitFrame();
    }

}
