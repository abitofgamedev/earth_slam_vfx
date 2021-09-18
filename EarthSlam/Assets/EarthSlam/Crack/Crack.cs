using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crack : MonoBehaviour
{
    public int Length;
    public int BlendShapeCount;
    public List<Transform> CornerPoints;
    [SerializeField] SkinnedMeshRenderer _Crack;
    [SerializeField] SkinnedMeshRenderer _CrackMask;

    public float GetBlendShape(int index)
    {
        return 100-_Crack.GetBlendShapeWeight(index);
    }

    public void SetBlendShape(int index, float value)
    {
        _Crack.SetBlendShapeWeight(index, 100 - value);
        _CrackMask.SetBlendShapeWeight(index, 100 - value);
    }

    //[SerializeField] float _OpenValue;
    //[SerializeField] float _Speed;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        for (int i = 1; i < _BlendShapeCount - 1; i++)
    //        {
    //            _Crack.SetBlendShapeWeight(i, 100);
    //            _CrackMask.SetBlendShapeWeight(i, 100);
    //        }
    //        StopAllCoroutines();
    //        StartCoroutine(Coroutine_OpenUp());
    //    }
    //}

    //IEnumerator Coroutine_OpenUp()
    //{
    //    for (int i = 1; i < BlendShapeCount-1; i++) {
    //        float lerp = 0;
    //        while (lerp < 1)
    //        {
    //            _Crack.SetBlendShapeWeight(i, 100 - lerp * _OpenValue);
    //            _CrackMask.SetBlendShapeWeight(i, 100 -  lerp * _OpenValue);
    //            lerp += Time.deltaTime* _Speed;
    //            yield return null;
    //        }
    //        _Crack.SetBlendShapeWeight(i, 100 - _OpenValue);
    //        _CrackMask.SetBlendShapeWeight(i, 100 - _OpenValue);
    //    }
    //}
}
