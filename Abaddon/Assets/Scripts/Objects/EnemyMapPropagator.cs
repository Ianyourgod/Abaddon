using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMapPropagator : MonoBehaviour
{
    [SerializeField]
    private Tilemap[] obstacleMaps;
    private static EnemyMapPropagator main;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    public static Tilemap[] getObstacleMaps()
    {
        return main.obstacleMaps;
    }
}
