using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionData : MonoBehaviour
{
    [field: SerializeField] public string sceneName { private set; get; }
    [field: SerializeField] public Vector2 spawnPoint { private set; get; }
}
