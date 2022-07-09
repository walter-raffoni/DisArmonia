using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int roomID = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (roomID > 0)
            {
                RoomsManager.instance.ChangeLevel(roomID);
                GameManager.instance.currentLevel = roomID;
                player.checkpointObject = this;
            }
        }
    }
}