using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public enum TableTypes
    {
        Gnome,
        Pixie,
        Barrel,
        Pot
    }

    string drop;
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
        int random = Controller.main.rnd.Next(1, 101);
        switch (table)
        {
            case TableTypes.Gnome:
            case TableTypes.Pixie:
                if (random <= 20)
                {
                    drop = "minorpotion";
                } else if (random <= 36) {
                    drop = "1helmet";
                } else if (random <= 48) {
                    drop = "1pants";
                } else if (random <= 58) {
                    drop = "1chest";
                } else if (random <= 66) {
                    drop = "majorpotion";
                }
                break;
            default:
                drop = "minorpotion";
                break;
        }
        Instantiate((UnityEngine.Object) Resources.Load($"Prefabs/Equipment/{drop}"), transform.position, Quaternion.identity);
        Destroy(gameObject);
        return 1;
    }
}
