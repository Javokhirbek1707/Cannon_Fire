using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    int _score;
    int _aliveMen = 11;
    bool _gameOver;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }

    public void AddScore(int value)
    {
        _score += value;
        UIManager.Instance.UpdateScore(_score);
    }

    public void OnManKilled()
    {
        _aliveMen--;

        if (_aliveMen <= 0)
        {
            _gameOver = true;
            UIManager.Instance.ShowWin();
            UIManager.Instance.RestartText();
        }
    }

    public void OnShotsEmpty()
    {
        if (_aliveMen > 0)
        {
            _gameOver = true;
            UIManager.Instance.ShowLose();
            UIManager.Instance.RestartText();
        }
    }

    public void UnlockTrajectory()
    {
        FindObjectOfType<Player>().UnlockTrajectory();
    }
}