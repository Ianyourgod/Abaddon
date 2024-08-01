using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] ItemSpawner.TableTypes dropTable = ItemSpawner.TableTypes.Gnome;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        GameObject spawner = Instantiate((UnityEngine.GameObject)Resources.Load($"Prefabs/ItemDropSpawner"), transform.position, Quaternion.identity);
        spawner.GetComponent<ItemSpawner>().SpawnRandom(dropTable);
    }
}
