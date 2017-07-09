using System;
using Game.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Injection;

public class MenuManager : InjectorBase<MenuManager> 
{
    #region INJECTED FIELDS

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Text _score;

    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private GameObject _mainMenuWrapper;

    [SerializeField]
    private GameObject _blackScreen;

    [SerializeField]
    private GameObject _introWrapper;

    [SerializeField]
    private InputField _inputField;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        bool condition = _dataStorageManager.IsNameEntered;
        if(_introWrapper) _introWrapper.SetActive(!condition);
        if(_mainMenuWrapper) _mainMenuWrapper.SetActive(condition);
        if(condition) SetScoreValue();
        if(_blackScreen) _blackScreen.gameObject.SetActive(false);

        _startButton.onClick.AddListener(LoadGameScene);
        if(_inputField) _inputField.onEndEdit.AddListener(OnEnterNameButtonPressed);
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(LoadGameScene);
        if(_inputField) _inputField.onEndEdit.RemoveListener(OnEnterNameButtonPressed);
    }

    #endregion

    #region PRIVATE METHODS

    private void SetScoreValue()
    {
        if (!_score) throw new NullReferenceException("Score lbl is null.");
        int score = _dataStorageManager.GetBestScore();
        _score.text = score == 0 ? _dataStorageManager.GameConfig.DefaultScore : score.ToString();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(_dataStorageManager.GameConfig.GameSceneIndex);
    }

    private void OnEnterNameButtonPressed(string text)
    {
        if(_dataStorageManager) _dataStorageManager.PlayerName = text;
        SetScoreValue();
        if(_introWrapper) _introWrapper.SetActive(false);
        if(_mainMenuWrapper) _mainMenuWrapper.SetActive(true);
    }

    #endregion
}
