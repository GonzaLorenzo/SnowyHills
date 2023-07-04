using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoBehaviour
{
    public GameObject player;

    void Awake()
    {
        transform.position = player.transform.position;
    }

    void Update()
    {
        transform.position = player.transform.position;
    }
}
