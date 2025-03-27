using System;
using System.Collections;
using System.Collections.Generic;
using _CakeMaster._Scripts.ControllerRelated;
using _CakeMaster._Scripts.GameplayRelated;
using UnityEngine;
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
        RotateCakeToAlign(count);
        return count;
    }
    void RotateCakeToAlign(int count) => transform.localEulerAngles = new Vector3(0, -count*30, 0);

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
        RotateCakeToAlign(activatedSlices.Count);
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

    public void AnimateCakeOnSorted()
    {
        StartCoroutine(MoveAlongCurve(transform, transform.position, UIController.instance.cakeIconWorldPos, 0.35f));
    }
    
    IEnumerator MoveAlongCurve(Transform obj, Vector3 start, Vector3 end, float duration)
    {
        // Midpoint for curve bending sideways along X axis
        Vector3 control = (start + end) / 2 + Vector3.right * 2f; // Right or left depending on direction

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 position = Mathf.Pow(1 - t, 2) * start +
                               2 * (1 - t) * t * control +
                               Mathf.Pow(t, 2) * end;

            obj.position = position;
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = end;
    }

}
