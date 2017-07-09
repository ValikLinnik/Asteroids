using System;
using System.Collections.Generic;
using Injection;
using UnityEngine;

public class AsteroidsManager : InjectorBase<AsteroidsManager>
{
    #region EVENTS

    public event Action<int> OnShotAsteroid;

    private void OnShotAsteroidHandler(int points)
    {
        if (OnShotAsteroid != null)
            OnShotAsteroid(points);
    }

    #endregion

    #region INJECTED FIELDS

    [Inject]
    private InputManager _inputManager;

    [Inject]
    private SoundManager _soundManager;

    [Inject]
    private EffectManager _effectManager;

    [Inject]
    private GameManager _gameManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Asteroid[] _asteroidsPrefabs;

    [SerializeField,Range(1,10)]
    private int _startAsteroidsQuantity = 3;

    [SerializeField,Range(1,30)]
    private int _maxAsteroidQuantity = 20;

    [SerializeField]
    private EnemyShip _enemyShipPrefab;

    #endregion

    #region PRIVATE FIELDS

    private Vector3 _bottomLeft;
    private Vector3 _upRight;
    private List<Asteroid> _asteroids = new List<Asteroid>();
    private EnemyShip _enemyShip;
    private float _offScreenOffSet = 1f;

    public Vector3 RandomPositionQutOfScreen
    {
        get
        {
            var pos = RandomPositionInScreen;

            var random = UnityEngine.Random.Range(0,3);
            var randomAx = UnityEngine.Random.Range(0,2) == 0;

            if(random == 0 || random == 2) pos.x = randomAx ? _bottomLeft.x - _offScreenOffSet : _upRight.x + _offScreenOffSet;
            if(random == 1 || random == 2) pos.y = randomAx ? _bottomLeft.y - _offScreenOffSet : _upRight.y + _offScreenOffSet;

            return pos;   
        }
    }

    private Vector3 RandomPositionInScreen
    {
        get
        {
            return new Vector3(RandomX, RandomY);
        }
    }

    private float RandomX
    {
        get
        {
            var val = UnityEngine.Random.Range(_bottomLeft.x, _upRight.x);
            return val;
        }
    }

    private float RandomY
    {
        get
        {
            var val = UnityEngine.Random.Range(_bottomLeft.y, _upRight.y);
            return val;
        }
    }

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if (_inputManager)
        {
            _bottomLeft = _inputManager.BottomLeft;
            _upRight = _inputManager.UpRight;
        }
    }

    #endregion

    #region PUBLIC METHODS

    public void StartLevel()
    {
        GetAsteroids(_startAsteroidsQuantity);
    }

    public void EndLevel()
    {
        foreach (var item in _asteroids)
        {
            if(item) item.Dispose();
        }

        _asteroids.Clear();
        if(_enemyShip) _enemyShip.Dispose();
        _enemyShip = null;
    }

    #endregion

    #region PRIVATE METHODS

    private void GetAsteroids(int quantity, bool inScreen = true)
    {
        for (int i = 0; i < quantity; i++)
        {
            _asteroids.Add(GetRandomAsteroid(inScreen));
        } 
    }

    private Asteroid GetRandomAsteroid(bool inScreen = true)
    {
        if (_asteroidsPrefabs.IsNullOrEmpty()) return null;
        var prefab = _asteroidsPrefabs.GetRandomItem();
        if (!prefab) return null;
        var temp = prefab.GetInstance();
        temp.transform.position = inScreen ? RandomPositionInScreen : RandomPositionQutOfScreen;
        temp.Initialize();
        temp.OnAsteroidShot += OnAsteroidShotHandler;
        temp.OnAsteroidCrash += OnAsteroidCrash;
        return temp;
    }

    private void OnAsteroidCrash (Asteroid asteroid)
    {
        DestroyAsteroid(asteroid);
    }

    private void OnAsteroidShotHandler (Asteroid asteroid)
    {
        OnShotAsteroidHandler(asteroid.Score);
        DestroyAsteroid(asteroid);
    }

    private void DestroyAsteroid(Asteroid asteroid)
    {
        if(_soundManager) _soundManager.EnableExplosionAsteroid(true);
        if(_effectManager)
        {
            var effect = _effectManager.GetExplosionAsteroidEffect();
            effect.transform.position = asteroid.transform.position;
        }

        asteroid.OnAsteroidShot -= OnAsteroidShotHandler;
        asteroid.OnAsteroidCrash -= OnAsteroidCrash;

        asteroid.Dispose();
        _asteroids.Remove(asteroid);
        var addValue = UnityEngine.Random.Range(0,4);
        if(_asteroids.Count + addValue <= _maxAsteroidQuantity) GetAsteroids(addValue, false);
        if(_asteroids.Count == 0) GetAsteroids(4, false);

        if(_enemyShip == null && IsAddEnemyShip) AddEnemyShip();
    }

    private void AddEnemyShip()
    {
        if(!_enemyShipPrefab) return;

        _enemyShip = _enemyShipPrefab.GetInstance();

        if(!_enemyShip) return;

        _enemyShip.Player = _gameManager.PlayerSpaceShip;
        _enemyShip.Initialize();
        _enemyShip.transform.position = RandomPositionQutOfScreen;
        _enemyShip.OnEnemyShipShot += OnEnemyShipShot;
    }

    void OnEnemyShipShot (EnemyShip ship)
    {
        ship.OnEnemyShipShot -= OnEnemyShipShot;

        if(_effectManager)
        {
            var effect = _effectManager.GetExplosionEnemyEffect();
            effect.transform.position = ship.gameObject.transform.position;
        }

        _enemyShip = null;
        if(_soundManager) _soundManager.EnableExplosionEnemy(true);
        ship.Dispose();
    }

    private bool IsAddEnemyShip
    {
        get
        {
            return UnityEngine.Random.Range(0,5) == 0;
        }
    }

    #endregion
}
