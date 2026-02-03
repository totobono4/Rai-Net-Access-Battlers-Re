using System;
using UnityEngine;

public class RaiNetGameCleaner : GameCleaner
{
    [SerializeField] CardsReadyUI cardsReadyUI;
    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] PauseUI pauseUI;
    [SerializeField] PauseTabUI pauseTabUI;
    [SerializeField] PlayerInfosUI playerInfosUI;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        gameOverUI.OnClean += GameOverUI_OnClean;
        pauseTabUI.OnClean += PauseTabUI_OnClean;
    }

    private void GameOverUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    private void PauseTabUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    public override void Clean() {
        base.Clean();

        gameOverUI.OnClean -= GameOverUI_OnClean;
        pauseTabUI.OnClean -= PauseTabUI_OnClean;

        GameManager.Instance.Clean();
        cardsReadyUI.Clean();
        gameOverUI.Clean();
        pauseUI.Clean();
        playerInfosUI.Clean();
    }
}
