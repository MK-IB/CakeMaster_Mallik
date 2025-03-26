using System;
using System.Collections;
using System.Collections.Generic;
using _CakeMaster._Scripts.GameplayRelated;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CakeElement : MonoBehaviour
{
    [SerializeField] private List<GameObject> slices;
    public CakeColors cakeColor; 
    void Awake()
    {
        foreach (var slice in slices)
            slice.SetActive(false);
        string objectName = transform.name;
        string cleanName = objectName.Replace("(Clone)", "").Trim();
        cakeColor = (CakeColors)Enum.Parse(typeof(CakeColors), cleanName);
    }

    public int ActivateSlices()
    {
        int count = Random.Range(1, slices.Count-1);
        Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            slices[i].SetActive(true);
        }
        return count;
    }
}
