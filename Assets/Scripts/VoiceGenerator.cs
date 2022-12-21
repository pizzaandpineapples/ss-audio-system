using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VoiceGenerator : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip[] DialogueSoundClips;
    [Range(1, 5)]
    public int FrequencyLevel = 2;
    [Range(-3, 3)]
    public float MinPitch = 0.5f;
    [Range(-3, 3)]
    public float MaxPitch = 3f;
    public float DialogueSpeed = 0.04f;
    public bool StopAudioSource;
    public bool MakeDialoguePredictable;

    public string InputDialogue;
    public TextMeshProUGUI DialogueText;

    private void Awake()
    {
        _audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(IterateThroughDialogue(InputDialogue));
        }
    }

    public IEnumerator IterateThroughDialogue(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        DialogueText.text = line;
        DialogueText.maxVisibleCharacters = 0;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            PlayDialogueSound(DialogueText.maxVisibleCharacters, DialogueText.text[DialogueText.maxVisibleCharacters]);
            DialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(DialogueSpeed);
        }
    }

    public void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        if (currentDisplayedCharacterCount % FrequencyLevel == 0)
        {
            if (StopAudioSource)
            {
                _audioSource.Stop();
            }
            AudioClip soundClip = null;
            // create predictable audio from hashing
            if (MakeDialoguePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                // sound clip
                int predictableIndex = hashCode % DialogueSoundClips.Length;
                soundClip = DialogueSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int)(MinPitch * 100);
                int maxPitchInt = (int)(MaxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0, so if there is no range then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    _audioSource.pitch = predictablePitch;
                }
                else
                {
                    _audioSource.pitch = MinPitch;
                }
            }
            // otherwise, randomize the audio
            else
            {
                // sound clip
                int randomIndex = Random.Range(0, DialogueSoundClips.Length);
                soundClip = DialogueSoundClips[randomIndex];
                // pitch
                _audioSource.pitch = Random.Range(MinPitch, MaxPitch);
            }

            // play sound
            _audioSource.PlayOneShot(soundClip);
        }
    }
}
