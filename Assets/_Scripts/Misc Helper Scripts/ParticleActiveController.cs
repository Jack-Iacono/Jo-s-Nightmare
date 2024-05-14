using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleActiveController : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}
