using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private int _startingShots = 20;
    [SerializeField] private int _scorePerMan = 50;

    private int _shotsLeft;
    private int _menLeft;
    private int _score;
    private bool _gameOver;

    public bool GameOver => _gameOver;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        _menLeft = FindObjectsByType<ManScript>(FindObjectsSortMode.None).Length;

        _shotsLeft = _startingShots;
        UIManager.Instance.UpdateShots(_shotsLeft);
        UIManager.Instance.UpdateScore(_score);
    }

    private void Update()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if(Input.GetKeyDown(KeyCode.Escape))
            QuitApplication();

    }

    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Application Quit requested");
    }

    public void OnShotUsed()
    {
        if (_gameOver) return;

        _shotsLeft--;
        UIManager.Instance.UpdateShots(_shotsLeft);

        if (_shotsLeft <= 0 && _menLeft > 0)
            Lose();
    }

    public void OnManDestroyed(ManScript man)
    {
        if (_gameOver) return;

        _menLeft--;
        _score += _scorePerMan;

        UIManager.Instance.UpdateScore(_score);

        if (_menLeft <= 0)
            Win();
    }

    private void Win()
    {
        _gameOver = true;
        UIManager.Instance.ShowWin();
        UIManager.Instance.RestartText();
    }

    private void Lose()
    {
        _gameOver = true;
        UIManager.Instance.ShowLose();
        UIManager.Instance.RestartText();
    }

    public void OnManDestroyed()
    {
        OnManDestroyed(null);
    }
}
