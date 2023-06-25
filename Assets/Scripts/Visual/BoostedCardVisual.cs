using UnityEngine;

public class BoostedCardVisual : MonoBehaviour
{
    [SerializeField] private OnlineCard onlineCard;
    [SerializeField] private Transform lineBoostVisual;

    private void Start() {
        onlineCard.OnBoostChanged += LineBoostUpdated;
    }

    private void LineBoostUpdated(object sender,OnlineCard.BoostedChangedArgs e) {
        Hide();
        if (e.onlineCard == onlineCard && e.boosted) Show();
    }

    private void Show() {
        lineBoostVisual.gameObject.SetActive(true);
    }

    private void Hide() {
        lineBoostVisual.gameObject.SetActive(false);
    }
}
