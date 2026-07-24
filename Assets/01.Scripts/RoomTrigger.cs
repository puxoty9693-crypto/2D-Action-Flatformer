using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private CameraMove cameraMove;
    [SerializeField] private Vector2 roomMin;
    [SerializeField] private Vector2 roomMax;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private EnemySpawner enemySpawner;

 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        cameraMove.SetRoomBounds(roomMin, roomMax);
        EnemySpawner spawner = GetComponent<EnemySpawner>();
        enemySpawner?.SpawnEnemies();

    }


}
