using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Team {
        Yellow,
        Blue
    };

    [SerializeField] Team team;
    [SerializeField] private Transform yellowLink;
    [SerializeField] private Transform yellowVirus;
    [SerializeField] private Transform blueLink;
    [SerializeField] private Transform blueVirus;

    private Transform link;
    private Transform virus;


    private List<Card> cards;

    private void Awake() {
        if (team == Team.Yellow) {
            link = yellowLink;
            virus = yellowVirus;
        }
        if (team == Team.Blue) {
            link = blueLink;
            virus = blueVirus;
        }

        List<Transform> playerCardsTransforms = new List<Transform> {
            Instantiate(link),
            Instantiate(link),
            Instantiate(link),
            Instantiate(link),
            Instantiate(virus),
            Instantiate(virus),
            Instantiate(virus),
            Instantiate(virus)
        };

        cards = new List<Card>();
        foreach (Transform playerCardTransform in playerCardsTransforms) cards.Add(playerCardTransform.GetComponent<Card>());

        for (int i = 0; i < cards.Count; i++) {
            int rand = Random.Range(0, 8);
            Card temp = cards[i];
            cards[i] = cards[rand];
            cards[rand] = temp;
        }
    }

    public List<Card> GetCards() { return cards; }
}
