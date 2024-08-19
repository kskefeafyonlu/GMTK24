using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    public GameObject UpgradeMenu;
    public List<UpgradeUI> UpgradesUI = new List<UpgradeUI>();
    public int AttributePoints = 10;
    public TextMeshProUGUI AttributePointsText;

    private void Start()
    {
        UpdateUI();
        foreach (var upgrade in UpgradesUI)
        {
            upgrade.Points = upgrade.MinPoints;
        }
    }

    public void AddPoint(int index)
    {
        if (AttributePoints > 0 && UpgradesUI[index].Points < UpgradesUI[index].MaxPoints)
        {
            UpgradesUI[index].AddPoint();
            AttributePoints--;
            UpdateUI();
        }
    }

    public void RemovePoint(int index)
    {
        if (UpgradesUI[index].Points > UpgradesUI[index].MinPoints)
        {
            UpgradesUI[index].RemovePoint();
            AttributePoints++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        AttributePointsText.text = AttributePoints.ToString();
        foreach (var upgrade in UpgradesUI)
        {
            upgrade.UpdatePointsText();
        }
    }

    public void ToggleUpgradeMenu()
    {
        UpgradeMenu.SetActive(!UpgradeMenu.activeSelf);
    }
}

[System.Serializable]
public class UpgradeUI
{
    public TextMeshProUGUI PointsText;
    public int MaxPoints;
    public int Points;
    public int MinPoints;

    public void AddPoint()
    {
        if (Points < MaxPoints)
        {
            Points++;
            UpdatePointsText();
        }
        else
        {
            Debug.Log("Max points reached");
        }
    }

    public void RemovePoint()
    {
        if (Points > MinPoints)
        {
            Points--;
            UpdatePointsText();
        }
        else
        {
            Debug.Log("Min points reached");
        }
    }

    public void UpdatePointsText()
    {
        if (Points == MaxPoints)
        {
            PointsText.text = "Max";
        }
        else if (Points == MinPoints)
        {
            PointsText.text = "Min";
        }
        else
        {
            PointsText.text = Points.ToString();
        }
    }
}