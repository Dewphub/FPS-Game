using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class skipCutscene : MonoBehaviour
{
    [SerializeField] GameObject sceneLoader;
    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetButtonDown("Continue"))
            sceneLoader.SetActive(true);
    }
}
