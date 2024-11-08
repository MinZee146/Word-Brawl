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
        NameRegister.Instance.Initialize();
    }

    public void NewGame()
    {
        GameFlowManager.Instance.StartGame();
        PowerUpsManager.Instance.Initialize();
    }

    public void Replay()
    {
        NewGame();
        Board.Instance.NewGame();
        UIManager.Instance.ToggleGameOverPanel();
    }

    public void CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();
        _isGameOver = Board.Instance.FoundWords.Keys.Count == 0;
    }
}
