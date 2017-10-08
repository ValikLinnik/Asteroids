using System;
using Injection;
using UnityEngine;

public class ScreenBorderComponent : InjectorBase<ScreenBorderComponent>
{
    #region INJECTED FIELDS

    [Inject]
    private InputManager _inputManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private MeshRenderer _modelRendelrer;

    #endregion

    #region PROPERTIES

    private Vector2 BottomLeft
    {
        get;
        set;
    }

    private Vector2 UpRight
    {
        get;
        set;
    }

    #endregion

    #region FIELDS

    private Vector2 _bottomLeft;
    private Vector2 _upRight;
    private Vector3 _modelSize;
    private QuietComponent<Rigidbody> _rigidbody;


    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        SetSize();  
        _rigidbody = new QuietComponent<Rigidbody>(this);
    }

    #endregion

    #region UNITY EVENTS

    private void FixedUpdate()
    {
        CheckMovingHandler();
    }

    #endregion

    #region PRIVATE METHODS

    private void CheckMovingHandler()
    {
        if (!_rigidbody.Component) throw new NullReferenceException("rigidbody is null");
        if (_rigidbody.Component.IsSleeping()) return;

        var pos = transform.position;

        if (pos.x > _upRight.x && _rigidbody.Component.velocity.x > 0 || pos.x < _bottomLeft.x && _rigidbody.Component.velocity.x < 0)
        {
            pos.x = -pos.x;
            transform.position = pos;
        }

        if (pos.y > _upRight.y && _rigidbody.Component.velocity.y > 0 || pos.y < _bottomLeft.y && _rigidbody.Component.velocity.y < 0)
        {
            pos.y = -pos.y;
            transform.position = pos;
        }
    }

    private void SetSize()
    {
        if (!_inputManager) throw new NullReferenceException("_inputManager is null");

        BottomLeft = _inputManager.BottomLeft;
        UpRight = _inputManager.UpRight;

        if (!_modelRendelrer) throw new NullReferenceException("_modelRendelrer is null");

        _modelSize = _modelRendelrer.bounds.extents;
        _bottomLeft = new Vector2(BottomLeft.x - _modelSize.x, BottomLeft.y - _modelSize.y);
        _upRight = new Vector2(UpRight.x + _modelSize.x, UpRight.y + _modelSize.y);
    }

    #endregion
}
