using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMapPropagator : MonoBehaviour
{
    [SerializeField]
    private Tilemap[] obstacleMaps;
    private static EnemyMapPropagator main = null;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    public static Tilemap[] GetObstacleMaps()
    {
        return main.obstacleMaps;
    }
}
