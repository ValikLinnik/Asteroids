using System;
using Game.Data;
using Injection;
using UnityEngine;

public class PlayerSpaceShip : InjectorBase<PlayerSpaceShip>, IDisposable
{
    #region INJECTED FIELDS

    [Inject]
    private InputManager _inputManager;

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private SoundManager _soundManager;

    [Inject]
    private EffectManager _effectManager;

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region EVENTS

    public event Action OnPlayerShot;

    private void OnPlayerShotHandler()
    {
        if (OnPlayerShot != null) OnPlayerShot();
    }

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    protected Rigidbody _rigidbody;

    [SerializeField, Range(1f, 100f)]
    private float _rotationSpeed = 6f;

    [SerializeField, Range(1f, 20f)]
    private float _flySpeed = 6f;

    [SerializeField]
    private GameObject _enginesPlayerEffect;

    [SerializeField]
    private Animator _shipAnimator;

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private Bullet _bullet;

    #endregion

    #region PRIVATE FIELDS

    private bool _isEngineEnabled;

    #endregion

    #region UNITY EVENTS

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_dataStorageManager.GameConfig.AsteroidTag)) 
        {
            OnPlayerShotEffects();
            OnPlayerShotHandler();
        }
    }

    private void OnDestroy()
    {
        if(_stateManager) _stateManager.OnStateChanged -= OnStateChanged;
        InputUnSubcribe();
    }

    #endregion

    #region PUBLIC METHODS

    public void Initialize()
    {
        if(_shipAnimator) _shipAnimator.Play("Blink");
        InputSubcribe();
        if(_stateManager) _stateManager.OnStateChanged += OnStateChanged;
    }

    public void Dispose()
    {
        if(_rigidbody) _rigidbody.velocity = Vector3.zero;
        MoveForwardEnd();
        OnDestroy();
    }

    #endregion

    #region PRIVATE METHODS

    private void OnPlayerShotEffects()
    {
        if(_soundManager) 
        {
            _soundManager.EnablePlayerExplosionSound(true);
            _soundManager.EnableEngineSound(false);
        }

        if(_effectManager) 
        {
            var effect = _effectManager.GetPlayerExplosionEffect();
            effect.transform.position = transform.position;
        }
    }

    private void InputSubcribe()
    {
        if(!_inputManager) throw new NullReferenceException("_inputManager is null");

        _inputManager.Rotate += Rotate; 
        _inputManager.MoveForward += MoveForward;
        _inputManager.MoveForwardEnd += MoveForwardEnd;
        _inputManager.Fire += Fire;
    }

    private void InputUnSubcribe()
    {
        if(!_inputManager) return;

        _inputManager.Rotate -= Rotate; 
        _inputManager.MoveForward -= MoveForward;
        _inputManager.MoveForwardEnd -= MoveForwardEnd;
        _inputManager.Fire -= Fire;
    }

    private void Rotate(DirectionEnum direction)
    {
        transform.Rotate(Vector3.forward * (int)direction * _rotationSpeed * Time.deltaTime);
    }

    private void MoveForward()
    {
        if(_stateManager.CurrentState == GameState.Pause || _stateManager.CurrentState == GameState.GameOver) return;

        if(!_rigidbody) throw new NullReferenceException("rigidbody is null");
        _rigidbody.AddForce(transform.up * Time.deltaTime * _flySpeed, ForceMode.Impulse);
        if(_enginesPlayerEffect) _enginesPlayerEffect.SetActive(true);
        if(_soundManager && !_isEngineEnabled) 
        {
            _soundManager.EnableEngineSound(true);
            _isEngineEnabled = true;
        }
    }

    void MoveForwardEnd()
    {
        if(_stateManager.CurrentState == GameState.Pause || _stateManager.CurrentState == GameState.GameOver) return;
        if(_enginesPlayerEffect) _enginesPlayerEffect.SetActive(false);
        if(_soundManager) _soundManager.EnableEngineSound(false);
        _isEngineEnabled = false;
    }

    private void Fire ()
    {
        if(_stateManager.CurrentState == GameState.Pause || _stateManager.CurrentState == GameState.GameOver) return;

        if(!_bullet) throw new NullReferenceException("_bullet is null");
        if(!_spawnPoint) throw new NullReferenceException("_spawn point is null");
        var temp = _bullet.GetInstance();
        if(!temp) throw new NullReferenceException("temp bullet is null");
        temp.transform.position = _spawnPoint.position;
        temp.transform.rotation = _spawnPoint.rotation;
        temp.Initialize();
        if (_soundManager) _soundManager.EnablePlayerWeaponSound(true);
    }

    private void OnStateChanged (GameState current, GameState previous)
    {
        if(current == GameState.Resume)  MoveForwardEnd();
    }

    #endregion
}
