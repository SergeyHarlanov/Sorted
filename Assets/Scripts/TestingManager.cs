using System;
using UnityEngine;

public class TestingManager : MonoBehaviour
{
    public static TestingManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool IsOneSpawn = false;
}