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

    void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
}
