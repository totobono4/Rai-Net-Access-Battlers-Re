using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsUI : MonoBehaviour
{
    [SerializeField] List<Button> tabButtons;
    [SerializeField] List<Transform> tabs;

    private void Awake() {
        for (int i = 0; i < tabButtons.Count; i++) {
            Transform tab = tabs[i];

            tabButtons[i].onClick.AddListener(() => {
                Show(tab);
            });
        }
    }

    private void Start() {
        Show(tabs[0]);
    }

    private void Show(Transform tab) {
        foreach (Transform _tab in tabs) Hide(_tab);
        tab.gameObject.SetActive(true);
    }

    private void Hide(Transform tab) {
        tab.gameObject.SetActive(false);
    }
}
