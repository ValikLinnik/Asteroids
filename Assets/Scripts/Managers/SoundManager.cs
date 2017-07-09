using System;
using Injection;
using UnityEngine;
using Game.Data;
using UnityEngine.Audio;

public class SoundManager : InjectorBase<SoundManager>
{
    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private AudioSource _backgroudMusic;

    [SerializeField]
    private AudioSource _engineSound;

    [SerializeField]
    private AudioSource _weaponPlayer;

    [SerializeField]
    private AudioSource _explosionPlayer;

    [SerializeField]
    private AudioSource _explosionAsteroid;

    [SerializeField]
    private AudioSource _explosionEnemy;

    [SerializeField]
    private AudioSource _weaponEnemy;

    [SerializeField]
    private AudioMixerSnapshot _gameState;

    [SerializeField]
    private AudioMixerSnapshot _pauseState;

    #endregion

    #region PRIVATE FIELDS

    private static SoundManager _instance;

    #endregion

    #region UNITY EVENTS

    protected override void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            _instance.InjectManager();
            Destroy(gameObject);
            return;
        }
            
        base.Awake();
        DontDestroyOnLoad(this);
        EnableBackgroundMusic(true);
    }

    #endregion

    #region PUBLIC METHODS

    public void SetGameState()
    {
        if(_gameState) _gameState.TransitionTo(.01f);
    }

    public void SetPauseState()
    {
        if(_pauseState) _pauseState.TransitionTo(.01f);
    }

    public void EnableBackgroundMusic(bool play)
    {
        if (!_backgroudMusic) throw new Exception("_backgroudMusic not assigned");

        if (play)
        {
            if (!_backgroudMusic.loop) _backgroudMusic.loop = true;
            _backgroudMusic.Play();
        }
        else _backgroudMusic.Pause();
    }

    public void EnableEngineSound(bool play)
    {
        PlaySound(_engineSound, play);
    }

    public void EnablePlayerWeaponSound(bool play)
    {
        PlaySound(_weaponPlayer, play);
    }

    public void EnablePlayerExplosionSound(bool play)
    {
        PlaySound(_explosionPlayer, play);
    }

    public void EnableExplosionAsteroid(bool play)
    {
        PlaySound(_explosionAsteroid, play);
    }

    public void EnableExplosionEnemy(bool play)
    {
        PlaySound(_explosionEnemy, play);
    }

    public void EnableWeaponEnemy(bool play)
    {
        PlaySound(_weaponEnemy, play);
    }

    public void DisableSounds()
    {
        EnableEngineSound(false);
    }

    #endregion

    #region PRIVATE METHODS

    private void InjectManager()
    {
        base.Awake();
    }

    private void PlaySound(AudioSource source, bool play)
    {
        if (!source) throw new NullReferenceException("source is null");
        if (play) source.Play();
        else source.Stop();
    }

    #endregion
}
