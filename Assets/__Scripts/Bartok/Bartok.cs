using System.Collections.Generic;
using UnityEngine;

public class Bartok : MonoBehaviour
{
    static public Bartok S;
    [Header("Set in I")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    [Header("Set D")]
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;
    private void Awake()
    {
        S = this;
    }
    private void Start()
    {
        deck = GetComponent<Deck>();// Получить компонент Deck
        deck.InitDeck(deckXML.text);// Передать ему DeckXML
        Deck.Shuffle(ref deck.cards);// Перетасовать колоду
    }
}

