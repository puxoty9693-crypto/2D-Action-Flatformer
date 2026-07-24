using UnityEngine;

public class EnemyFacing : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;

    public void Face(float dirX)
    {
        if (visualRoot == null || Mathf.Abs(dirX) < 0.01f)
            return;

        Vector3 scale = visualRoot.localScale;
        float sign = dirX > 0 ? -1f : 1f;
        scale.x = sign * Mathf.Abs(scale.x);
        visualRoot.localScale = scale;
    }
}