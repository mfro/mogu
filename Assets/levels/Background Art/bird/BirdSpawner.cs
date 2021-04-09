using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject birdSpawn;

    [SerializeField] Vector2 speedRange;
    [SerializeField] Vector2 spawnDelayRange;

    private Flippable playerOrientation;
    void Start()
    {
        var player = FindObjectOfType<PlayerController>();

        playerOrientation = player.gameObject.GetComponent<Flippable>();

        StartCoroutine(SpawnBird());
    }

    private IEnumerator SpawnBird()
    {
        while(true)
        {
            float delay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);

            yield return new WaitForSeconds(delay);

            float speed = Random.Range(speedRange.x, speedRange.y);

            if (Random.Range(0f, 1f) < 0.5f)
            {
                speed *= -1;
            }

            Vector2 spawnPos;
            Quaternion rotation = Quaternion.Euler(0,0,0);

            float spawnHeight = Random.Range(-5.5f, 5.5f);

            if (playerOrientation.down.y != 0)
            {
                spawnPos = speed >= 0 ? new Vector2(-6, spawnHeight) : new Vector2(6, spawnHeight);
            }
            else
            {
                spawnPos = speed >= 0 ? new Vector2(spawnHeight, -6) : new Vector2(spawnHeight, 6);
                rotation = Quaternion.Euler(0, 0, speed >= 0 ? 90 : -90);
            }


            var bird = Instantiate(birdSpawn, (Vector2) Camera.main.transform.position + spawnPos, Quaternion.identity);

            BirdController controller = bird.GetComponent<BirdController>();

            controller.velocity = playerOrientation.down.y != 0 ? new Vector2(speed, 0) : new Vector2(0, speed);
        }
    }


    private void OnDestroy()
    {
        StopCoroutine(SpawnBird());
    }
}
