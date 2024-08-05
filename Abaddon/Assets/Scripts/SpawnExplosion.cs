using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Instantiate((UnityEngine.GameObject)Resources.Load("Prefabs/Explosion"), transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}