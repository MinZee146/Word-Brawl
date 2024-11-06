public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameDictionary.Instance.Initialize();
        AudioManager.Instance.Initialize();
        PowerupsManager.Instance.InitializePowerUps();
        NameRegister.Instance.Initialize();
    }

    public void NewGame()
    {
        GameFlowManager.Instance.StartGame();
    }

    public void CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();

        if (Board.Instance.FoundWords.Count == 0)
        {
            UIManager.Instance.ToggleGameOverScreen();
        }
    }
}
