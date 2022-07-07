using TMPro;
using UnityEngine;

#pragma warning disable 0649
#pragma warning disable 0414

public class LeaderboardListObject : MonoBehaviour
{
    [SerializeField] private int characterLimit;
    [SerializeField] private GameObject currentInOrder;
    [SerializeField] private GameObject disabledInOrder;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI[] order;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI scoreDiff;

    public void setStandings(int o)
    {
        for (var i = 0; i < order.Length; i++) order[i].text = o.ToString();
    }

    public void setName(string l)
    {
        if (l == null)
            return;

        var textToShow = l;
        if (l.Length >= characterLimit) textToShow = l.Substring(0, characterLimit);
        label.text = textToShow;
    }

    public void setScore(int s)
    {
        score.text = string.Format("{0:n0}", s);
    }

    public void setScoreDiff(int d)
    {
        if (d < 0)
            scoreDiff.text = "";
        else
            scoreDiff.text = "+" + d;
    }

    public void isCurrentPlayer(bool isCurrent)
    {
        if (isCurrent)
        {
            //some graphic change
            group.alpha = 1f;
            currentInOrder.SetActive(true);
            disabledInOrder.SetActive(false);
            scoreDiff.gameObject.SetActive(true);
        }
        else
        {
            //remove the graphic
            group.alpha = 0.6f;
            currentInOrder.SetActive(false);
            disabledInOrder.SetActive(true);
            scoreDiff.gameObject.SetActive(false);
        }
    }
}