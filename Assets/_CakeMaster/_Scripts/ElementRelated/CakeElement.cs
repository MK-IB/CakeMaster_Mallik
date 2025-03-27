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
    private List<GameObject> activatedSlices = new List<GameObject>();
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
        //Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            slices[i].SetActive(true);
            activatedSlices.Add(slices[i]);
        }
        transform.localEulerAngles = new Vector3(0, -count*30, 0);
        return count;
    }

    public int GetEmptySpaces()
    {
        return slices.Count - activatedSlices.Count;
    }

    public int GetActivatedSlices()
    {
        return activatedSlices.Count;
    }

    public void AddSlices(int num)
    {
        for (int i = 0; i < num; i++)
        {
            slices[activatedSlices.Count + i].SetActive(true);
        }
        UpdateActivatedSlices();
    }

    void UpdateActivatedSlices()
    {
        activatedSlices = new List<GameObject>();
        for (int i = 0; i < slices.Count; i++)
        {
            if(slices[i].activeSelf)
                activatedSlices.Add(slices[i]);
        }
    }

    public void DeactivateSlices(int num)
    {
        for (int i = num-1; i >= 0; i--)
        {
            activatedSlices[i].SetActive(false);
        }
        UpdateActivatedSlices();
    }

    public void DeactivateOneSlice()
    {
        activatedSlices[activatedSlices.Count - 1].SetActive(false);
        UpdateActivatedSlices();
    }
    
    

    public IEnumerator MoveSlicesToTarget(int requiredSlices, Transform cake)
    {
        CakeElement cakeElement = cake.GetComponent<CakeElement>();
        int activatedSlicesCount = cakeElement.GetActivatedSlices() - 1;
        if(requiredSlices <= activatedSlices.Count)
            for (int i = 0; i < activatedSlices.Count; i++)
            {
                Transform slice = activatedSlices[i].transform;
                slice.SetParent(cake);
                slice.localPosition = Vector3.zero;
                slice.localEulerAngles = Vector3.up * (30 + activatedSlicesCount * 60);
            }

        yield return null;
    }
}
