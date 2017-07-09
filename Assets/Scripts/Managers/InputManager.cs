using System;
using Injection;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : InjectorBase<InputManager>
{
    #region EVENTS AND HANDLERS

    public event Action<DirectionEnum> Rotate;

    private void RotateHandler(DirectionEnum direction)
    {
        if (Rotate != null) Rotate(direction);
    }

    public event Action MoveForward;

    private void MoveForwardHandler()
    {
        if (MoveForward != null) MoveForward();
    }

    public event Action MoveForwardEnd;

    private void MoveForwardEndHandler()
    {
        if (MoveForwardEnd != null) MoveForwardEnd();
    }

    public event Action Fire;

    private void FireHandler()
    {
        if (Fire != null) Fire();
    }

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Camera _sceneCamera;

    #endregion

    #region PUBLIC PROPERTIES

    public Vector2 BottomLeft
    {
        get;
        set;
    }

    public Vector2 UpRight
    {
        get;
        set;
    }

    #endregion

    #region UNITY EVENTS

    protected override void Awake()
    {
        base.Awake();
        CalculateScreenBorders();
    }

    private void LateUpdate()
    {
        if(Input.GetKey(KeyCode.RightArrow)) RotateHandler(DirectionEnum.Right);
        if(Input.GetKey(KeyCode.D)) RotateHandler(DirectionEnum.Right);
        if(Input.GetKey(KeyCode.LeftArrow)) RotateHandler(DirectionEnum.Left);
        if(Input.GetKey(KeyCode.A)) RotateHandler(DirectionEnum.Left);
        if(Input.GetKey(KeyCode.UpArrow)) MoveForwardHandler();
        if(Input.GetKey(KeyCode.W)) MoveForwardHandler();
        if(Input.GetKeyUp(KeyCode.UpArrow)) MoveForwardEndHandler();
        if(Input.GetKeyUp(KeyCode.W)) MoveForwardEndHandler();
        if(Input.GetKeyDown(KeyCode.Space)) FireHandler();
        if(Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject()) FireHandler();
    }

    #endregion

    #region PRIVATE METHODS

    private void CalculateScreenBorders()
    {
        if (!_sceneCamera) throw new NullReferenceException("_sceneCamera is null");
        BottomLeft = _sceneCamera.ScreenToWorldPoint(Vector3.zero);  
        UpRight = _sceneCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));  
    }

    #endregion
}
