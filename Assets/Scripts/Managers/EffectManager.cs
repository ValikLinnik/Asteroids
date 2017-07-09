using Injection;
using UnityEngine;

public class EffectManager : InjectorBase<EffectManager>
{
    #region SERIALIZE FIELDS

    [SerializeField]
    private EffectComponent _playerExplosion;

    [SerializeField]
    private EffectComponent _explosionAsteroid;

    [SerializeField]
    private EffectComponent _explosionEnemy;

    #endregion

    #region PUBLIC METHODS

    public EffectComponent GetPlayerExplosionEffect()
    {
        if (!_playerExplosion) return null;
        var temp = _playerExplosion.GetInstance();
        return temp;
    }

    public EffectComponent GetExplosionAsteroidEffect()
    {
        if (!_explosionAsteroid) return null;
        var temp = _explosionAsteroid.GetInstance();
        return temp;
    }

    public EffectComponent GetExplosionEnemyEffect()
    {
        if (!_explosionEnemy) return null;
        var temp = _explosionEnemy.GetInstance();
        return temp;
    }

    #endregion
}
