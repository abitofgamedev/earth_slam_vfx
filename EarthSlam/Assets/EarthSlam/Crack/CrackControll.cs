using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CrackControll : MonoBehaviour
{
    [SerializeField] Transform _Container;
    [SerializeField] Crack _CrackPrefab;//
    [SerializeField] float _OpenValue;//
    [SerializeField] float _Speed;//
    [SerializeField] ParticleSystem _Slam;
    [SerializeField] float _CloseTime;

    [Range(0f,1f)]
    [SerializeField] float _SideCrackChance;
    [SerializeField] Vector2Int _SideCrackRange;
    List<Crack> _cracks;//
    List<CrackControll> crackControlls;
    [SerializeField] Animator _RockEmerge;
    [SerializeField] ParticleSystem _SmallPuffsPrefab;

    [SerializeField] CinemachineImpulseSource _CinemachineImpulseSource;
    [SerializeField] float _ImpulseForceSlam;
    [SerializeField] float _ImpulseForceRock;

    private float _range;

    public void Open(float range, bool isSideCrack = false)
    {
        for (int i = 0; i < _Container.childCount; i++)
        {
            Destroy(_Container.GetChild(i).gameObject);
        }
        _range = range;
        StopAllCoroutines();
        StartCoroutine(Coroutine_CrackOpen(isSideCrack));
    }
    
    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Coroutine_CrackClose());
    }

    IEnumerator Coroutine_CrackOpen(bool isSideCrack)
    {

        _RockEmerge.gameObject.SetActive(false);
        if (!isSideCrack)
        {
            _CinemachineImpulseSource.GenerateImpulseAt(transform.position, Vector3.forward * _ImpulseForceSlam);
            _Slam.Play();
        }

        _cracks = new List<Crack>();
        crackControlls = new List<CrackControll>();

        int range = Mathf.RoundToInt(_range);
        Vector3 startPoint = transform.position;

        for (int i = 0; i < range; i += _CrackPrefab.Length)
        {
            Crack crack = Instantiate(_CrackPrefab, _Container);
            crack.transform.position = startPoint;
            crack.transform.forward = transform.forward;
            startPoint += transform.forward * _CrackPrefab.Length * transform.localScale.z;
            for(int j = 0; j < crack.BlendShapeCount; j++)
            {
                crack.SetBlendShape(j, 0);
            }
            _cracks.Add(crack);
        }

        int rangeIndex = 0;

        for (int i = 0; i < _cracks.Count; i++)
        {
            for (int j = 1; j < _cracks[i].BlendShapeCount; j++) 
            {
                if (rangeIndex == range-1)
                {
                    break;
                }
                rangeIndex++;

                if (!isSideCrack)
                {
                    InstantiateSideCrack(i, j);
                    InstantiatePuff(i, j);
                }


                float lerp = 0;
                while (lerp < 1)
                {
                    _cracks[i].SetBlendShape(j, _OpenValue * lerp);
                    if (j == _cracks[i].BlendShapeCount - 1 && i < _cracks.Count - 1)
                    {
                        _cracks[i+1].SetBlendShape(0, _OpenValue * lerp);
                    }
                    lerp += Time.deltaTime * _Speed;
                    yield return null;
                }
                _cracks[i].SetBlendShape(j, _OpenValue);
                if (j == _cracks[i].BlendShapeCount - 1 && i < _cracks.Count - 1)
                {
                    _cracks[i + 1].SetBlendShape(0, _OpenValue);
                }
            }
        }
        if (!isSideCrack)
        {
            InstantiateRock();
            yield return new WaitForSeconds(_CloseTime);

            Close();
        }
    }

    private void InstantiateRock()
    {
        Debug.Log("Testt");
        _RockEmerge.gameObject.SetActive(true);
        Vector3 position = transform.position + transform.forward * _range * transform.localScale.z;
        _RockEmerge.transform.position = position;
        _RockEmerge.transform.forward = transform.forward;
        _RockEmerge.SetTrigger("Appear");
        _CinemachineImpulseSource.GenerateImpulseAt(position,Vector3.forward*_ImpulseForceRock);
    }

    private void InstantiatePuff(int i, int j)
    {
        Transform puffPoint = _cracks[i].CornerPoints[j];
        ParticleSystem puff = Instantiate(_SmallPuffsPrefab, puffPoint.position, Quaternion.identity);
        puff.transform.forward = Vector3.up;
    }

    private void InstantiateSideCrack(int i, int j)
    {
        float chance = Random.Range(0f, 1f);
        if (chance > _SideCrackChance)
        {
            Transform point = _cracks[i].CornerPoints[j];
            CrackControll crackControll = Instantiate(this, _Container);
            crackControll.transform.position = point.position;
            crackControll.transform.forward = Quaternion.Euler(0, Random.Range(-45, 45), 0) * point.forward;
            crackControll._OpenValue = Random.Range(_OpenValue / 2f, _OpenValue);
            crackControll.Open(Random.Range(_SideCrackRange.x, _SideCrackRange.y),true);
            crackControlls.Add(crackControll);
        }
    }

    IEnumerator Coroutine_CrackClose()
    {
        _RockEmerge.SetTrigger("Disappear");
        foreach (var i in crackControlls)
        {
            i.Close();
        }
        float lerp = 0;
        while (lerp < 1)
        {
            float value = Mathf.Lerp(_OpenValue, 0, lerp);
            for (int i = 0; i < _cracks.Count; i++)
            {
                for (int j = 0; j < _cracks[i].BlendShapeCount; j++) 
                {
                    _cracks[i].SetBlendShape(j, _cracks[i].GetBlendShape(j)*(1-lerp));
                }
            }
            lerp += Time.deltaTime;
            yield return null;
        }
        for(int i = 0; i < _cracks.Count; i++)
        {
            Destroy(_cracks[i].gameObject);
        }
        for(int i = 0; i < crackControlls.Count; i++)
        {
            Destroy(crackControlls[i].gameObject);
        }
    }
}
