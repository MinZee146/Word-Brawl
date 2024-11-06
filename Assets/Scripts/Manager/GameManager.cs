public class GameManager : SingletonPersistent<GameManager>
{
    private bool _isGameOver;
    public bool IsGameOver => _isGameOver;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameDictionary.Instance.Initialize();
        AudioManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        PowerUpsManager.Instance.Initialize();
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
            _isGameOver = true;
            UIManager.Instance.ToggleGameOverScreen();
        }
    }
}
