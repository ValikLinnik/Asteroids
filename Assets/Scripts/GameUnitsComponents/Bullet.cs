using System;
using Game.Data;
using Injection;
using UnityEngine;

public class Bullet : InjectorBase<Bullet>, IDisposable 
{
    [Inject]
    private DataStorageManager _dataStorageManager;

    #region SERIALIZE FIELDS

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField, Range(0, 10)]
    private float _speed = 1;

    [SerializeField, Range(0, 3)]
    private float _lifeTime = 1;

    #endregion

    #region UNITY EVENTS

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_dataStorageManager.GameConfig.AsteroidTag))
        {
            Dispose();
        }
    }

    #endregion

    #region PUBLIC METHODS

    public void Initialize()
    {
        if (!_rigidbody) throw new NullReferenceException("_bulletRigidbody is null");
        _rigidbody.AddForce(transform.up * _speed, ForceMode.Impulse);
        StopAllCoroutines();
        this.WaitAndDo(_lifeTime, Dispose);
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        if(_rigidbody) _rigidbody.velocity = Vector3.zero;
        this.PutInPool();
    }

    #endregion
}
