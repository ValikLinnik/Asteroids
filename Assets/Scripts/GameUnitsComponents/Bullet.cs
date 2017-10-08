using System;
using Injection;
using UnityEngine;

public class Bullet : InjectorBase<Bullet>, IDisposable 
{
    #region INJECTED FIELDS

    [Inject]
    private DataStorageManager _dataStorageManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField, Range(0, 10)]
    private float _speed = 1;

    [SerializeField, Range(0, 3)]
    private float _lifeTime = 1;

    #endregion

    #region PRIVATE FIELDS

    private QuietComponent<Rigidbody> _rigidbody;

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
        if(!_rigidbody) _rigidbody = new QuietComponent<Rigidbody>(this);  
        _rigidbody.Component.AddForce(transform.up * _speed, ForceMode.Impulse);
        StopAllCoroutines();
        this.WaitAndDo(_lifeTime, Dispose);
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
