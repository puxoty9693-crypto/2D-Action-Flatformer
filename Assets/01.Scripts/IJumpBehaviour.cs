using UnityEngine;

public interface IJumpBehaviour
{ 
    void Jump(Rigidbody2D rb, bool isGround);
    int CurrentJumpCount { get; set; }

}
