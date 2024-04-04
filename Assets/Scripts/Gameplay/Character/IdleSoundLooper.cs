using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

/// <summary>
/// Loops a audio clip with a delay
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class IdleSoundLooper : MonoBehaviour {

    public AudioClip[] audioClips; // Array of audio clips to loop through
        public float minDelayBetweenClips = 1f; // Minimum delay between each clip
    public float maxDelayBetweenClips = 2f; // Maximum delay between each clip
    [SerializeField] string target = "name of target"; // name of the tag on the entity you want to search for
    [SerializeField] float range = 3; // the range of detection
    private AudioSource audioSource;
    private int currentIndex = 0;
    public bool loopAudio = true;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Check if audio clips are available
        if (audioClips.Length > 0)
        {
            StartCoroutine(LoopAudioClips());
        }
        else
        {
            Debug.LogWarning("No audio clips assigned. AudioLooper will not run.");
        }
    }
    IEnumerator LoopAudioClips()
    {
        while (loopAudio)
        {
            // Play the next audio clip
            currentIndex = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[currentIndex];
            audioSource.Play();
            SearchForTargets();

            float randomDelay = Random.Range(minDelayBetweenClips, maxDelayBetweenClips);
            yield return new WaitForSeconds(audioSource.clip.length + randomDelay);
        }
    }
    private void SearchForTargets()
    {
        var hits = Physics.OverlapSphere(transform.position, range);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.CompareTag(target))
                {
                    // does the hit target have the ability to hear?
                    CharacterSenses targetSenses;
                    if(hit.TryGetComponent<CharacterSenses>(out targetSenses)){
                        targetSenses.Hear(this.gameObject);
                    }
                }
            }
        }
    }
}
