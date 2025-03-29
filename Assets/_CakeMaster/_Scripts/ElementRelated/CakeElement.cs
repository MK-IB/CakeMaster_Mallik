using System;
using System.Collections;
using System.Collections.Generic;
using _CakeMaster._Scripts.ControllerRelated;
using _CakeMaster._Scripts.GameplayRelated;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class CakeElement : MonoBehaviour
{
    [SerializeField] private List<GameObject> slices;
    [SerializeField] private ParticleSystem selectionFx;
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
        StartCoroutine(RotateCakeToAlign(count));
        return count;
    }

    public void SetFxColor(Color color)
    {
        // ParticleSystem.MainModule mainModule = selectionFx.main;
        // mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
        selectionFx.startColor = color;
    }
    IEnumerator RotateCakeToAlign(int count)
    {
        yield return new WaitForSeconds(0.35f);
        transform.DORotate(new Vector3(0, -count * 30, 0), 0.35f);
    }

    public int GetEmptySpaces()
    {
        return slices.Count - activatedSlices.Count;
    }

    public int GetActivatedSlices()
    {
        return activatedSlices.Count;
    }

    public void AddSlices(int num, Vector3 position)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject toActivate = slices[activatedSlices.Count + i];
            toActivate.SetActive(true);
            toActivate.transform.DORotate(Vector3.zero, 0.35f).From();
            toActivate.transform.DOMove(position, 0.35f).From();
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
        StartCoroutine(RotateCakeToAlign(activatedSlices.Count));
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
        if (activatedSlices.Count <= 0) return;
        activatedSlices[activatedSlices.Count - 1].SetActive(false);
        UpdateActivatedSlices();
    }

    public void AnimateCakeOnSorted()
    {
        StartCoroutine(MoveAlongCurve(transform, transform.position, UIController.instance.cakeIconWorldPos, 0.35f));
    }

    public void SelectionFx() => selectionFx.Play();
    
    IEnumerator MoveAlongCurve(Transform obj, Vector3 start, Vector3 end, float duration)
    {
        // Midpoint for curve bending sideways along X axis
        Vector3 direction = (Random.value > 0.5f) ? Vector3.right : Vector3.left;
        Vector3 control = (start + end) / 2 + direction * 2f;


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
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.35f);
        MainController.instance.SetActionType(GameState.Refilling);
    }

}
