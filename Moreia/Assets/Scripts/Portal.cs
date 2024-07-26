using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] string SceneTarget;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PortalTravel()
    {
        Debug.Log("portal traveled through");
        SceneManager.LoadSceneAsync(SceneTarget);
    }
}
