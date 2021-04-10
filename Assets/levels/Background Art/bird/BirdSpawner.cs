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
        while (true)
        {
            float delay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);

            yield return new WaitForSeconds(delay);
            print($"wait {delay}");

            if (!Physics.IsEnabled) continue;

            float speed = Random.Range(speedRange.x, speedRange.y);

            if (Random.Range(0f, 1f) < 0.5f)
            {
                speed *= -1;
            }

            var orientation = Quaternion.FromToRotation(Vector2.down, playerOrientation.down);
            var spawnHeight = Random.Range(-5.5f, 5.5f);
            var spawnPos = new Vector2(speed >= 0 ? -7 : 7, spawnHeight);

            spawnPos = orientation * spawnPos;

            var bird = Instantiate(birdSpawn, (Vector2)Camera.main.transform.position + spawnPos, Quaternion.identity);

            var controller = bird.GetComponent<BirdController>();
            var renderer = bird.GetComponent<SpriteRenderer>();

            renderer.flipX = speed >= 0;
            bird.transform.localRotation = orientation;
            controller.velocity = playerOrientation.down.y != 0 ? new Vector2(speed, 0) : new Vector2(0, speed);
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(SpawnBird());
    }
}
