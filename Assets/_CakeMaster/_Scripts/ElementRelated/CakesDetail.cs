using System.Collections.Generic;
using UnityEngine;

namespace _CakeMaster._Scripts.ElementRelated
{
    [CreateAssetMenu(menuName = "CakesDetail")]
    public class CakesDetail : ScriptableObject
    {
        public List<GameObject> cakes;
    }
}
