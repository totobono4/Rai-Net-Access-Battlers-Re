using System;
using Ttbn4.Game.UI;
using UnityEngine;

namespace Ttbn4.Game.Clean {
    public abstract class GameCleaner : MonoBehaviour {
        public static GameCleaner Instance { get; private set; }

        [SerializeField] PauseUI pauseUI;
        [SerializeField] PauseTabUI pauseTabUI;

        protected void Awake() {
            if (Instance != null) {
                Debug.LogError("GameCleaner has multiple instances");
                return;
            }
            Instance = this;
        }

        protected virtual void Start() {
            pauseTabUI.OnClean += PauseTabUI_OnClean;
        }

        protected virtual void PauseTabUI_OnClean(object sender, EventArgs e) {
            Clean();
        }

        protected virtual void Clean() {
            pauseTabUI.OnClean -= PauseTabUI_OnClean;
            pauseTabUI.Clean();
        }
    }
}