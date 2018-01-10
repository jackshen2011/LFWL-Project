using UnityEngine;
using System.Collections;

class ParticleControl : UnModalUIBase {

    private ParticleSystem[] particleSystems = new ParticleSystem[0];
    bool bIsInitial = false;

    void Start()
    {
        //Debug.Log("Unity:"+"particleSystems start1");
        if (particleSystems.Length == 0)
        {
            bIsInitial = true;
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            //Debug.Log("Unity:"+"particleSystems start2:" + particleSystems.Length.ToString());
        }

    }
    public void Initial()
    {
        //Debug.Log("Unity:"+"particleSystems Initial");
        if (particleSystems.Length==0)
        {
            bIsInitial = true;
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            //Debug.Log("Unity:"+"particleSystems Initial:"+ particleSystems.Length.ToString());
        }
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i] != null)
            {
                particleSystems[i].Play();
            }
        }
    }
    void Update()
    {
        if (!bIsInitial)
        {
            return;
        }
        bool allStopped = true;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i]!=null)
            {
                if (!particleSystems[i].isStopped)
                {
                    allStopped = false;
                }
            }
        }
        if (allStopped)
            DestroyThis();
    }
}
