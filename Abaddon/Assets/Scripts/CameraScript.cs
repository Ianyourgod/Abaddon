using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] float defaultLerpSpeed = 0.1f;
    [SerializeField] public Transform defaultFollowTarget;
    private float currentLerpSpeed;
    private Transform currentTarget;
    private System.Action onComplete;

    void Start()
    {
        ResetTarget();
    }

    Vector2 CalcNextPosition()
    {
        // Calculate normalized time for sigmoid interpolation
        float t = currentLerpSpeed * Time.deltaTime;
        // Apply sigmoid function to get smooth S-curve
        float smoothT = t / (1 + t);

        return Vector3.Lerp(transform.position, currentTarget.position + new Vector3(0, 0, -10), smoothT);
    }

    void Update()
    {
        if (Controller.main == null) return;
        //print("panning to " + currentTarget.gameObject.name);

        Vector3 newPosition = CalcNextPosition();
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            onComplete?.Invoke();
            onComplete = null;
        }
        else
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }

    public void PanToPoint(Vector2 point)
    {
        defaultFollowTarget = Instantiate(new GameObject("Follow Target").transform);
        defaultFollowTarget.position = point;
    }

    public void ChangeTarget(Transform target, float lerpSpeed, System.Action onComplete)
    {
        currentTarget = target;
        currentLerpSpeed = lerpSpeed;
        this.onComplete = onComplete;
    }

    public void ResetTarget()
    {
        currentTarget = defaultFollowTarget;
        currentLerpSpeed = defaultLerpSpeed;
    }
}