using UnityEngine;

public class ZoomOuter : MonoBehaviour
{
    public Material reveal_mat;
    public float duration = 2f;
    public float initial_wait = 1f;

    private float wait_timer = 0f;
    private float timer = 0f;

    void Start()
    {
        reveal_mat.SetFloat("_HalfSize", 0f); // start fully black
    }

    void Update()
    {
        wait_timer += Time.deltaTime;
        if (wait_timer < initial_wait || timer >= duration)
            return;

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);

        float size = Mathf.SmoothStep(0f, 0.5f, t);
        reveal_mat.SetFloat("_HalfSize", size);
    }
}
