using UnityEngine;

public class NetworkSceneLoaderCallback : MonoBehaviour {
    private bool isFisrtUpdate = true;

    private void Update() {
        if (!isFisrtUpdate) return;

        isFisrtUpdate = false;
        NetworkSceneLoader.LoaderCallback();
    }
}
