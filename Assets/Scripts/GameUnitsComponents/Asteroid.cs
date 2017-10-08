﻿using System;
using Game.Data;
using Injection;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class Asteroid : InjectorBase<Asteroid>, IDisposable
{
    #region EVENTS

    public event Action<Asteroid> OnAsteroidShot;

    private void OnAsteroidShotHandler()
    {
        if (OnAsteroidShot != null) OnAsteroidShot(this);
    }

    public event Action<Asteroid> OnAsteroidCrash;

    private void OnAsteroidCrashHandler()
    {
        if (OnAsteroidCrash != null) OnAsteroidCrash(this);
    }

    #endregion

    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private SoundManager _soundManager;

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private int _scoreValue;

    #endregion

    #region PUBLIC PROERTIES

    public int Score
    {
        get
        {
            return _scoreValue;
        }
    }

    #endregion

    #region PRIVATE PROPERTIES

    private Vector3 RandomDirection
    {
        get
        {
            var x = UnityRandom.Range(-10f, 10f);
            x = x == 0 ? -10 : x;
            var y = UnityRandom.Range(-10f, 10f);
            y = y == 0 ? 10 : y;

            return (new Vector3(x, y)).normalized;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private float _speed = 1;
    private QuietComponent<Rigidbody> _rigidbody;

    #endregion

    #region PUBLIC METHODS

    public void Initialize()
    {
        if(!_rigidbody) _rigidbody = new QuietComponent<Rigidbody>(this);
        _speed = UnityRandom.Range(.1f, 3f);
        var dir = RandomDirection;
        _rigidbody.Component.velocity = dir * _speed;
    }

    #endregion

    #region UNITY EVENTS

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_dataStorageManager.GameConfig.LaserTag))
        {
            OnAsteroidShotHandler();
        }

        if(other.CompareTag(_dataStorageManager.GameConfig.PlayerTag))
        {
            OnAsteroidCrashHandler();
        }
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        if(_rigidbody && _rigidbody.Component) _rigidbody.Component.velocity = Vector3.zero;
        this.PutInPool();
    }

    #endregion
}
