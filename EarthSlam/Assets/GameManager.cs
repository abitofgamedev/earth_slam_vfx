using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _GroundShockwavePrefab;
    List<GameObject> _queue;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                GameObject shockwave = Instantiate(_GroundShockwavePrefab, hit.point, Quaternion.identity);
                StartCoroutine(Coroutine_RemoveObj(1, shockwave));
                shockwave.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3000 + _queue.Count;
            }
        }
    }

    IEnumerator Coroutine_RemoveObj(float time,GameObject obj)
    {
        if (_queue == null)
        {
            _queue = new List<GameObject>();
        }
        _queue.Add(obj);
        yield return new WaitForSeconds(time);
        _queue.Remove(obj);
        Destroy(obj);
    }
}
