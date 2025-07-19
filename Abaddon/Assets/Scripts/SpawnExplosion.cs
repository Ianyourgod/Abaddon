using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExplosion : MonoBehaviour
{
    [SerializeField]
    GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update() { }
}
