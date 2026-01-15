public enum FoodEffect
{
    None,
    ReduceSpeed,
    LaggyTouch,
}

public enum GrowthStage
{
    Small,
    Medium,
    Large,
    Kraken,
}

public enum GameStates
{
    MainMenu,// game not running, and will reset everytime the game starts
    GamePause, // game is paused/ game not running
    InGame,   // game resumed/ running
    GameOver,   // game over screen, game not running
    InfoPanel  // game paused, info panel
}

public enum SoundType
{
    BGM1,
    BGM2,
    Reeling,
    Bite,
    Eat,
    Button,
    Move,
    Dead,
}