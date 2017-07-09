using System;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

public class TextColoraizer : MonoBehaviour 
{
    #region SERIALIZE FIELDS

    [SerializeField, Range(0,1f), Tooltip("Change color speed")]
    private float _speed = 1;

    #endregion

    #region PRIVATE FIELDS

    private Text _text;
    private Text Text
    {
        get
        {
            if (!_text) _text = GetComponent<Text>();
            return _text;
        }
    }

    private Color RandomColor
    {
        get
        {
            return new Color(UnityRandom.Range(0, 1f),UnityRandom.Range(0, 1f),UnityRandom.Range(0, 1f));   
        }
    }

    private Color _newColor;
    private float _timeToNext;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        _timeToNext = Time.time + _speed;
        _newColor = RandomColor;
    }

    private void LateUpdate()
    {
        if(!Text) throw new NullReferenceException("Text is null.");

        Text.color = Color.Lerp(Text.color, _newColor, Time.deltaTime * _speed);

        if(Time.time < _timeToNext) return;

        _timeToNext = Time.time + _speed;
        _newColor = RandomColor;
    }

    #endregion
}
