using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointsScript : MonoBehaviour
{

    public GameObject textmesghpro_objective;

    //text components
    TextMeshProUGUI text;

    void Start()
    {
        text = textmesghpro_objective.GetComponent<TextMeshProUGUI>();
    }

    public void setPoints(int points)
    {
        string s = "Points: " + points.ToString();
        text.SetText(s);
    }
}
