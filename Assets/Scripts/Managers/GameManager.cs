using System;
using Game.Data;
using Injection;
using UnityEngine;

public class GameManager : InjectorBase<GameManager> 
{
    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private InputManager _inputManager;

    [Inject]
    private GameUIManager _gameUIManager;

    [Inject]
    private SoundManager _soundManager;

    [Inject]
    private AsteroidsManager _asteroidsManager;

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private PlayerSpaceShip _playerPrefab;

    [SerializeField,Range(1f,5f)]
    private float _timeToDelayAddPlayer = 2f;

    #endregion

    #region PRIVATE FIELDS

    private PlayerSpaceShip _player;
    private int _playerLife;
    private int _playerScore;

    #endregion

    #region PUBLIC PROPERTIES

    public PlayerSpaceShip PlayerSpaceShip
    {
        get
        {
            return _player;
        }
    }

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        StartLevel();
        if(_stateManager) _stateManager.OnStateChanged += OnStateChanged;
        if(_gameUIManager) _gameUIManager.OnRestart += StartLevel;
        if(_asteroidsManager) _asteroidsManager.OnShotAsteroid += OnAsteroidShotHandler;
    }

    private void OnDestroy()
    {
        if(_stateManager) _stateManager.OnStateChanged -= OnStateChanged;
        if(_gameUIManager) _gameUIManager.OnRestart -= StartLevel;
        if(_asteroidsManager) _asteroidsManager.OnShotAsteroid -= OnAsteroidShotHandler;
        if(_soundManager) _soundManager.SetGameState();
        Context.Instance.Dispose();
    }

    #endregion

    #region PRIVATE METHODS

    private void StartLevel()
    {
        AddPlayer();
        _playerScore = 0;
        _playerLife = _dataStorageManager.GameConfig.LifeStartValue;

        if (_gameUIManager) 
        {
            _gameUIManager.Life = _dataStorageManager.GameConfig.LifeStartValue.ToString();
            _gameUIManager.Score = _playerScore.ToString();
            _gameUIManager.EnebleNewHightScoreLbl = false;
        }

        if (_stateManager) _stateManager.SetState(GameState.Play);
        if(_asteroidsManager) _asteroidsManager.StartLevel();
        if(_soundManager) _soundManager.SetGameState();
    }

    private void EndLevel()
    {
        if (_stateManager) _stateManager.SetState(GameState.GameOver);
        if(_asteroidsManager) _asteroidsManager.EndLevel();
        SetResult();
    }

    private void OnPlayerShot()
    {
        _playerLife--;
        if (_gameUIManager) _gameUIManager.Life = _playerLife.ToString();
        RemovePlayer();

        if(_playerLife <= 0) EndLevel();
        else
        { 
            StopAllCoroutines();
            this.WaitAndDo(_timeToDelayAddPlayer, AddPlayer);
        }
    }

    private void AddPlayer()
    {
        if (!_playerPrefab) throw new NullReferenceException("_playerPrefab is null");
        _player = _playerPrefab.GetInstance();
        if (!_player) throw new NullReferenceException("_player is null");
        _player.Initialize();
        _player.transform.position = Vector3.zero;
        _player.transform.rotation = Quaternion.identity;
        _player.OnPlayerShot += OnPlayerShot;
    }

    private void RemovePlayer()
    {
        _player.OnPlayerShot -= OnPlayerShot;
        _player.Dispose();
        _player.PutInPool();
    }

    private void OnStateChanged(GameState current, GameState previous)
    {
        if (_soundManager)
        {
            if (current == GameState.Pause || current == GameState.GameOver) _soundManager.SetPauseState();
            else _soundManager.SetGameState();
        }
    }

    private void OnAsteroidShotHandler (int points)
    {
        _playerScore += points;
        if (_gameUIManager) _gameUIManager.Score = _playerScore.ToString();
    }

    private void SetResult()
    {
        int best = _dataStorageManager.GetBestScore();
        if(best >= _playerScore) return;
        _dataStorageManager.SetBestScore(_playerScore);
        if(_gameUIManager) _gameUIManager.EnebleNewHightScoreLbl = true;
    }

    #endregion
}
