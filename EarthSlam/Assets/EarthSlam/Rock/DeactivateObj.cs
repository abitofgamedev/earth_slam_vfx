using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObj : MonoBehaviour
{
    private void AnimatorCallback_Deactivate()
    {
        gameObject.SetActive(false);
    }
}
