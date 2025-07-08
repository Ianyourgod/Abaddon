using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] ItemSpawner.TableTypes dropTable = ItemSpawner.TableTypes.Gnome;
    [SerializeField] GameObject forceDrop;

    public void Die()
    {
        GameObject spawner = Instantiate((GameObject)Resources.Load("Prefabs/Environment/ItemDropSpawner"), transform.position, Quaternion.identity);
        if (forceDrop != null)
        {
            spawner.GetComponent<ItemSpawner>().SpawnPath(forceDrop);
        }
        else
        {
            spawner.GetComponent<ItemSpawner>().SpawnRandom(dropTable);
        }
    }
}
