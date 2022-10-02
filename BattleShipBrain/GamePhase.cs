namespace BattleShipBrain
{
    public enum GamePhase
    {
        Initialized = 0,
        ShipPlacement = 1,
        Started = 2,
        ResumedFromDb = 3,
        ResumedFromLocal = 4,
        Finished = 5
    }
}