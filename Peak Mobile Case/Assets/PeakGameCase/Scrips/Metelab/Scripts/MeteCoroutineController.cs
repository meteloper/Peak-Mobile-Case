using System.Collections;using System.Collections.Generic;using UnityEngine;public class MeteCoroutineController{    private Coroutine _Coroutine;    private MonoBehaviour _MonoBehaviour;    public MeteCoroutineController(MonoBehaviour monoBehaviour)
    {
        _MonoBehaviour = monoBehaviour;
    }    public void StartCoroutine(IEnumerator enumarator)    {        if (_Coroutine != null)
            _MonoBehaviour.StopCoroutine(_Coroutine);        _Coroutine = _MonoBehaviour.StartCoroutine(enumarator);    }    public void StopCoroutine()    {        _MonoBehaviour.StopCoroutine(_Coroutine);        _Coroutine = null;    }}