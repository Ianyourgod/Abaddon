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
        Vase
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

    public void SpawnRandom(TableTypes table)
    {
        int random = Controller.main.rnd.Next(1, 101);
        switch (table)
        {
            case TableTypes.Gnome:
                if (random <= 15)
                {
                    drop = "minorpotion";
                }
                else if (random <= 27)
                {
                    drop = "0helmet";
                }
                else if (random <= 39)
                {
                    drop = "0pants";
                }
                else if (random <= 49)
                {
                    drop = "majorpotion";
                }
                else if (random <= 58)
                {
                    drop = "0chest";
                }
                else if (random <= 64)
                {
                    drop = "1helmet";
                }
                else if (random <= 70)
                {
                    drop = "1pants";
                }
                else if (random <= 75)
                {
                    drop = "1chest";
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
                break;
            case TableTypes.Pixie:
                if (random <= 50)
                {
                    drop = "minorpotion";
                }
                else if (random <= 80)
                {
                    drop = "majorpotion";
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
                break;
            case TableTypes.Barrel:
            case TableTypes.Vase:
                if (random <= 26)
                {
                    drop = "minorpotion";
                }
                else if (random <= 46)
                {
                    drop = "majorpotion";
                }
                else if (random <= 60)
                {
                    drop = "0sword";
                }
                else if (random <= 66)
                {
                    drop = "1sword";
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
                break;
            default:
                drop = "minorpotion";
                break;
        }
        print(random);
        print(drop);
        Instantiate((UnityEngine.Object)Resources.Load($"Prefabs/Equipment/{drop}"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void SpawnPath(string path)
    {
        Instantiate((UnityEngine.Object)Resources.Load(path), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}