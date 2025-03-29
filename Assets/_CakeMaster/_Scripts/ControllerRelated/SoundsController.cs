using UnityEngine;

namespace _CakeMaster._Scripts.ControllerRelated
{
    public class SoundsController : MonoBehaviour
    {
        public static SoundsController instance;
        private AudioSource audioSource;
        public AudioClip tap, cakeFormed, cakeMoving; 

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayClip(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
