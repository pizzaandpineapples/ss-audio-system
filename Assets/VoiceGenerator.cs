using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VoiceGenerator : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] dialogueSoundClips;
    [Range(1, 5)]
    public int frequencyLevel = 2;
    [Range(-3, 3)]
    public float minPitch = 0.5f;
    [Range(-3, 3)]
    public float maxPitch = 3f;
    public float dialogueSpeed = 0.04f;
    public bool stopAudioSource;
    public bool makePredictable;

    public string inputDialogue;
    public TextMeshProUGUI dialogueText;

    private void Awake()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(IterateThroughDialogue(inputDialogue));
        }
    }

    public IEnumerator IterateThroughDialogue(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(dialogueSpeed);
        }
    }

    public void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            AudioClip soundClip = null;
            // create predictable audio from hashing
            if (makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                // sound clip
                int predictableIndex = hashCode % dialogueSoundClips.Length;
                soundClip = dialogueSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0, so if there is no range then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            // otherwise, randomize the audio
            else
            {
                // sound clip
                int randomIndex = Random.Range(0, dialogueSoundClips.Length);
                soundClip = dialogueSoundClips[randomIndex];
                // pitch
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }

            // play sound
            audioSource.PlayOneShot(soundClip);
        }
    }
}
