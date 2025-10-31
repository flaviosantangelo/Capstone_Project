using UnityEngine;


public class UniqueID : MonoBehaviour
{
    [SerializeField] private string _id;

    [ContextMenu("Genera ID Unico")]
    private void GenerateGuid()
    {
        _id = System.Guid.NewGuid().ToString();
    }

    void Start()
    {
        if (PlayerDataManager._instance == null) return;
        if (PlayerDataManager._instance.IsItemCollected(_id))
        {
            Destroy(gameObject);
        }
    }
    
    public string GetID()
    {
        return _id;
    }
}