// small static holder to pass data between MenuScene and GameScene
public static class GameSession
{
    public static int selectedLevel = 1;
    // When true, MenuManager will show the levels list on the next menu load
    public static bool showLevelsOnMenuLoad = false;
}
