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
        int dropID;
        int random = Controller.main.rnd.Next(1, 101);
        switch (table)
        {
            case TableTypes.Goblin:
                switch (Controller.main.rnd.Next())
                break;
            default:
                break;
        }
        Instantiate((UnityEngine.Object) Resources.Load($"Prefabs/Equipment/{dropID}"), transform.position, Quaternion.identity);
        Destroy(gameObject);
        return 1;
    }
}
