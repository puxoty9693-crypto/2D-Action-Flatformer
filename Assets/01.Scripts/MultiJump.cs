using UnityEngine;

public class MultiJump : IJumpBehaviour
{
    private float jumpPower;
    private int maxJumpCount;
    private int currentJumpCount;

    public int CurrentJumpCount
    {
        get => currentJumpCount;
        set => currentJumpCount = Mathf.Clamp(value, 0, maxJumpCount);
    }

    public MultiJump(float  jumpPower, int maxJumpCount)
    {
        this.jumpPower = jumpPower;
        this.maxJumpCount = Mathf.Max(1, maxJumpCount);
        this.currentJumpCount = 0;
       
    }
    public void Jump(Rigidbody2D rb, bool isGround) 
    {

        if (isGround) 
        {
            currentJumpCount = 0;
        }

        if (currentJumpCount < maxJumpCount) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            currentJumpCount++;
        }
    }
}
