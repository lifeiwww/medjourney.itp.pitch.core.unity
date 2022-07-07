using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer.Utilities;
using Serilog;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UISequenceBar : MonoBehaviour
{
    [SerializeField] private GameObject SequenceElement;
    [SerializeField] private GameObject SequenceHolder;
    [SerializeField] private TextMeshProUGUI currentSequenceNum;

    private List<int> _currentNumList = new List<int>();
    private List<GameObject> _sequenceElements = new List<GameObject>();

    private void OnEnable()
    {
        //UpdateNumbers();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F1))
        //    SetSequence(new List<int> { 1, 2, 3, 1, 4 });

        //if (Input.GetKeyDown(KeyCode.F2))
        //    ProgressSequence();

        //if (Input.GetKeyDown(KeyCode.F4))
        //    Reset();
    }

    // set the sequence number formatting
    void UpdateNumbers()
    {
        for (int i = 0; i < _sequenceElements.Count; i++)
        {
            var num = _currentNumList[i]+1;
            _sequenceElements[i].GetComponentInChildren<TextMeshProUGUI>().text = num.ToString("D2");
        }
    }

    public void SetSequence(List<int> sequence)
    {
        ResetSequence();
        _currentNumList = sequence;
        for (int i = 0; i < sequence.Count; i++) 
            AddSequenceElement(sequence[i]);
        
        UpdateNumbers();

        // fill the target number up next
        ProgressSequence();
    }

    public void ResetSequence()
    {
        currentSequenceNum.text = "";
        _sequenceElements.Clear();
        foreach (Transform obj in SequenceHolder.transform)
            Destroy(obj.gameObject);
    }

    void AddSequenceElement(int numInSequence)
    {
        numInSequence += 1;
        var obj = Instantiate(SequenceElement, SequenceHolder.transform);
        _sequenceElements.Add(obj);
    }

    public void ProgressSequence()
    {
        if (!_sequenceElements.Any())
        {
            currentSequenceNum.text = "*";
            return;
        }


        var first = _sequenceElements.First();
        currentSequenceNum.text = first.GetComponentInChildren<TextMeshProUGUI>().text;
        _sequenceElements.Remove(first);
        Destroy(first);
    }
}
