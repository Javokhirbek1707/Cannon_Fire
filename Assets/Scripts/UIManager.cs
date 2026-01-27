using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("UI Manager is NULL!");
            return _instance;
        }
    }

    [SerializeField]
    private TextMeshProUGUI _info;

    [SerializeField] 
    private TextMeshProUGUI _scoreText;
    [SerializeField] 
    private TextMeshProUGUI _shotsText;

    [SerializeField] 
    private TextMeshProUGUI _winPanel;
    [SerializeField] 
    private TextMeshProUGUI _losePanel;
    [SerializeField]
    private TextMeshProUGUI _restart;

    void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        UpdateScore(0);
        StartCoroutine(InfoText());
    }

    IEnumerator InfoText()
    {
        yield return new WaitForSeconds(6f);
        _info.gameObject.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (_scoreText != null)
            _scoreText.text = "Score: " + score;
    }

    public void UpdateShots(int shots)
    {
        if (_shotsText != null)
            _shotsText.text = "Shots: " + shots;
    }


    public void ShowWin()
    {
        _winPanel.gameObject.SetActive(true);
    }

    public void ShowLose()
    {
        _losePanel.gameObject.SetActive(true);
    }
    public void RestartText()
    {
        _restart.gameObject.SetActive(true);
    }
}