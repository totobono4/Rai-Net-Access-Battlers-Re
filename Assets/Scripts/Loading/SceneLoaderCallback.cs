using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderCallback : MonoBehaviour
{
    private bool isFisrtUpdate = true;

    private void Update() {
        if (!isFisrtUpdate) return;

        isFisrtUpdate = false;
        SceneLoader.LoaderCallback();
    }
}
