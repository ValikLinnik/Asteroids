using UnityEngine;

public class EffectComponent : MonoBehaviour 
{
    #region SERIALIZE FIELDS

    [SerializeField, Range(1, 5)]
    private float _lifeTime = 2;

    #endregion

    #region PRIVATE METHODS

    private void OnEnable()
    {
        StopAllCoroutines();
        this.WaitAndDo(_lifeTime, () => this.PutInPool());
    }

    #endregion
}
