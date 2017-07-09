using UnityEngine;

public class RandomObjectRotator : MonoBehaviour 
{
    #region SERIALIZE FIELDS

    [SerializeField, Range(1, 10)]
    private float _speed = 1;

    #endregion

    #region PRIVATE FIELDS

    private Vector3 _direction;

    #endregion

    #region PRIVATE METHODS

    private void Start()
    {
        _direction = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
    }

    #endregion

    #region UNITY EVENTS

    private void LateUpdate()
    {
        transform.Rotate(_direction.normalized * Time.deltaTime * _speed);
    }

    #endregion
}
