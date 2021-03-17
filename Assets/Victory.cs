using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    private float? enterTime = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        enterTime = Time.time;

        while (Time.time - enterTime < 1f)
            await Task.Yield();

        if (enterTime != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enterTime = null;
    }
}
