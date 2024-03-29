﻿using System.Collections;
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

        SpawnBirds();
    }

    private async void SpawnBirds()
    {
        while (this != null)
        {
            float speed = Random.Range(speedRange.x, speedRange.y);

            if (Random.Range(0f, 1f) < 0.5f)
            {
                speed *= -1;
            }

            var orientation = Quaternion.FromToRotation(Vector2.down, playerOrientation.down);
            var spawnHeight = Random.Range(-5.5f, 5.5f);
            var spawnPos = new Vector2(speed >= 0 ? -7 : 7, spawnHeight);
            var velocity = new Vector2(speed, 0);

            spawnPos = orientation * spawnPos;
            velocity = orientation * velocity;

            var bird = Instantiate(birdSpawn, (Vector2)Camera.main.transform.position + spawnPos, Quaternion.identity);

            var controller = bird.GetComponent<BirdController>();
            var renderer = bird.GetComponent<SpriteRenderer>();

            renderer.flipX = speed >= 0;
            bird.transform.localRotation = orientation;
            controller.velocity = velocity;

            float delay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
            await Util.Seconds(delay, true);
        }
    }
}
