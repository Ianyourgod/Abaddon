using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public enum TableTypes
    {
        Goblin,
        Pixie,
        Barrel,
        Pot
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int SpawnRandom(TableTypes table)
    {
        switch (table)
        {
            case TableTypes.Goblin:
                print("yippee");
                Instantiate((UnityEngine.Object) Resources.Load($"Prefabs/Equipment/{Controller.main.rnd.Next(2, 6)}"), transform.position, Quaternion.identity);
                break;
            default:
                break;
        }
        Destroy(gameObject);
        return 1;
    }
}
