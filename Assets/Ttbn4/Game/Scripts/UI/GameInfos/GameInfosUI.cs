using TMPro;
using UnityEngine;

namespace Ttbn4.Game.UI {
    public class GameInfosUI : MonoBehaviour {
        [SerializeField] TextMeshProUGUI versionText;
        [SerializeField] TextMeshProUGUI companyText;

        private void Awake() {
            versionText.text = Application.productName + " v" + Application.version;
            companyText.text = Application.companyName;
        }
    }
}