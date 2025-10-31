using UnityEngine;

[RequireComponent(typeof(Collider2D))] 
public class LevelExit : MonoBehaviour
{
    private bool _hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !_hasBeenTriggered)
        {
            _hasBeenTriggered = true; 

            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.HandleLevelComplete();
            }
           
        }
    }
}