using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Game Manager is NULL!");
            return _instance;
        }
    }

    private int _score;
    private int _aliveMen = 11;
    private bool _isGameOver = false;

    void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void AddScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }

    public int GetScore()
    {
        return _score;
    }

    public void OnManKilled()
    {
        _aliveMen--;
        if (_aliveMen <= 0)
        {
            _isGameOver = true;
            UIManager.Instance.ShowWin();
            UIManager.Instance.RestartText();
        }
    }


    public void OnShotsEmpty()
    {
        if (_aliveMen > 0)
        {
            _isGameOver = true;
            UIManager.Instance.ShowLose();
            UIManager.Instance.RestartText();
        }
    }
}