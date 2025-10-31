using System; 

public static class EnemyAlertManager
{
    public static event Action _onPlayerSpotted;
    public static event Action _onEnemySearchFinished;
    private static int _searchingEnemyCount = 0; 

    public static void Alert_PlayerSpotted()
    {
        if (_searchingEnemyCount == 0)
        {
            _onPlayerSpotted?.Invoke(); 
        }
        _searchingEnemyCount++;
    }
    
    public static void Alert_SearchFinished()
    {
        _searchingEnemyCount--;
        if (_searchingEnemyCount <= 0)
        {
            _searchingEnemyCount = 0; 
            _onEnemySearchFinished?.Invoke(); 
        }
    }
}