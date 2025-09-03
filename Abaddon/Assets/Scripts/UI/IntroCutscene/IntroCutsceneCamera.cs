using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneCamera : MonoBehaviour
{
    [SerializeField]
    private float endYPos;

    [SerializeField]
    private float speed = 0.5f;

    [SerializeField]
    private float runtime = 10f;

    [SerializeField]
    private string first_level_name = "Real Level copy";

    private float current_time = 0f;

    private float starting_y_pos;

    void Start()
    {
        starting_y_pos = transform.position.y;
    }

    void Update()
    {
        if (current_time > runtime)
        {
            SceneManager.LoadScene(first_level_name);
            return;
        }

        Vector3 new_position = new(
            transform.position.x,
            transform.position.y - speed * Time.deltaTime * 60f,
            transform.position.z
        );

        if (new_position.y < endYPos)
            new_position.y = starting_y_pos;

        transform.position = new_position;

        current_time += Time.deltaTime;
    }
}
