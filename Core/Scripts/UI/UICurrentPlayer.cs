using System.Collections;
using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.API;
using dreamcube.unity.Core.Scripts.Stores;
using TMPro;
using UniRx;
using UnityEngine;

public class UICurrentPlayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentPlayerNameTextBox;

    private void Awake()
    {
        if (currentPlayerNameTextBox == null)
            currentPlayerNameTextBox = gameObject.GetComponent<TextMeshProUGUI>();
        
        GameRoundDataStore.CurrentPlayerName.Subscribe(x =>
        {
            if (currentPlayerNameTextBox != null)
                currentPlayerNameTextBox.text = GameRoundDataStore.CurrentPlayerName.Value;
        });
    }
}
