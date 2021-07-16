using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static Transform Player
    {
        get
        {
            return instance.player;
        }
    }
    public Transform player;
    public GameObject bloodParticlePrefab;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("<color=ffcccc>WARN</color>: There are more than one GameManager running in same scene");
        }

        instance = this;
    }

    private void Start()
    {
        //PoolManager.CreatePool<BloodParticle>(bloodParticlePrefab, transform, 10);
    }
}