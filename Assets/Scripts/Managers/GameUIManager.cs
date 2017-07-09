using System;
using Game.Data;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : InjectorBase<GameUIManager> 
{
    #region EVENTS

    public event Action OnRestart;

    private void OnRestartHandler()
    {
        if (OnRestart != null)
            OnRestart();
    }

    #endregion

    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _lifeText;

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private Button _resumeButton;

    [SerializeField]
    private Button _restartButton;

    [SerializeField]
    private Text _gameOverScore;

    [SerializeField]
    private GameObject _newHightScore;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if(_backButton) _backButton.onClick.AddListener(OnBackButtonClick); 
        if(_backButton) _pauseButton.onClick.AddListener(OnPauseButtonClick);
        if(_resumeButton) _resumeButton.onClick.AddListener(OnResumeButtonClick);
        if(_restartButton) _restartButton.onClick.AddListener(OnRestartButtonClick);
    }

    private void OnDestroy()
    {
        if(_backButton) _backButton.onClick.RemoveListener(OnBackButtonClick);
        if(_backButton) _pauseButton.onClick.RemoveListener(OnPauseButtonClick);
        if(_resumeButton) _resumeButton.onClick.RemoveListener(OnResumeButtonClick);
        if(_restartButton) _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        Time.timeScale = 1;
    }

    #endregion

    #region PUBLIC PROPERTIES

    public string Life
    {
        set
        {
            if (_lifeText) _lifeText.text = value;
        }
    }

    public string Score
    {
        set
        {
            if(_scoreText) _scoreText.text = value;
            if(_gameOverScore) _gameOverScore.text = value;
        }
    }

    public bool EnebleNewHightScoreLbl
    {
        set
        {
            if(_newHightScore) _newHightScore.SetActive(value);
        }
    }

    #endregion

    #region PRIVATE METHODS

    private void OnRestartButtonClick()
    {
        OnRestartHandler();
    }

    private void OnBackButtonClick()
    {
        SceneManager.LoadScene(_dataStorageManager.GameConfig.MenuSceneIndex);
    }

    private void OnPauseButtonClick()
    {
        Time.timeScale = 0;
        if(_stateManager) _stateManager.SetState(GameState.Pause);
    }

    private void OnResumeButtonClick()
    {
        Time.timeScale = 1;
        if(_stateManager) _stateManager.SetState(GameState.Resume);
    }

    #endregion
}
