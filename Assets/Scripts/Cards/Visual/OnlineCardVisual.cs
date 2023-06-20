using UnityEngine;

public class OnlineCardVisual : CardVisual
{
    [SerializeField] private OnlineCard onlineCard;
    [SerializeField] Transform show, hide;

    private void Awake() {
        onlineCard.OnStateChanged += StateChanged;
    }

    private void StateChanged(object sender, OnlineCard.StateChangedArgs e) {
        switch(e.state) {
            case OnlineCard.State.Unrevealed: Hide(); break;
            case OnlineCard.State.Captured:
            case OnlineCard.State.Revealed: Show(); break;
        }
    }

    private void Show() {
        show.gameObject.SetActive(true);
        hide.gameObject.SetActive(false);
    }

    private void Hide() {
        hide.gameObject.SetActive(true);
        show.gameObject.SetActive(false);
    }
}
