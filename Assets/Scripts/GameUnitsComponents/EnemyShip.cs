using System;
using Game.Data;
using Injection;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class EnemyShip : InjectorBase<EnemyShip>, IDisposable
{
    #region INJECTED FIELDS

    [Inject]
    private SoundManager _soundManager;

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region EVENTS

    public event Action<EnemyShip> OnEnemyShipShot;

    private void OnEnemyShipShotHandler()
    {
        if (OnEnemyShipShot != null) OnEnemyShipShot(this);
    }

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField, Range(1, 10)]
    private float _speed = 1;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField, Range(.1f,3f)]
    private float _shootDelay = .3f;

    #endregion

    #region PUBLIC PROPERTIES

    public PlayerSpaceShip Player
    {
        get;
        set;
    }

    #endregion

    #region PRIVATE FIELDS

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
    private float _angleOffSet = 270f;

    #endregion

    #region PUBLIC METHODS

    public void Initialize()
    {
        if (!_rigidbody) return;
        _rigidbody.velocity = RandomDirection * _speed;
        StopAllCoroutines();
        this.WaitAndDo( _shootDelay, Shot);
    }

    #endregion

    #region PRIVATE METHODS

    private void Shot()
    {
        if (!_bulletPrefab)
            return;
        var temp = _bulletPrefab.GetInstance();
        temp.transform.position = _spawnPoint.position;
        temp.transform.rotation = _spawnPoint.rotation;
        temp.Initialize();
        if (_soundManager) _soundManager.EnableWeaponEnemy(true);
        this.WaitAndDo(_shootDelay, Shot);
    }

    #endregion

    #region UNITY EVENTS

    private void LateUpdate()
    {
        if(!Player) return;
        var target = Quaternion.LookRotation(Player.transform.position - transform.position);
        var rotation = new Vector3(0, 0, target.eulerAngles.x - _angleOffSet);
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_dataStorageManager.GameConfig.LaserTag))
        {
            OnEnemyShipShotHandler();
        }
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        StopAllCoroutines();
        if(_rigidbody) _rigidbody.velocity = Vector3.zero;
        this.PutInPool();
    }

    #endregion
}
