using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlatesController : MonoBehaviour {

    [SerializeField] private PuzzleSlab plate1;
    [SerializeField] private PuzzleSlab plate2;
    [SerializeField] private PuzzleSlab plate3;
    [SerializeField] private PuzzleSlab plate4;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip noteClip;
    [SerializeField] private AudioClip incorrectClip;

    [SerializeField] private string currentSequence = "";
    [SerializeField] private string playerSequence = "";

    private int success = 0;

    private bool started = false;

    private static bool _completed = false;

    private bool sequenceStarted = false;

    

    public static bool completed {
        get {
            return _completed;
        }
        private set {
            _completed = value;
            GameController.instance.CompletePuzzles();
        }
    }


    private void OnEnable() {
        plate1.onPlayerStep += (player) => PlateStep(1);
        plate2.onPlayerStep += (player) => PlateStep(2);
        plate3.onPlayerStep += (player) => PlateStep(3);
        plate4.onPlayerStep += (player) => PlateStep(4);
    }


    private void OnDisable() {
        plate1.onPlayerStep -= (player) => PlateStep(1);
        plate2.onPlayerStep -= (player) => PlateStep(2);
        plate3.onPlayerStep -= (player) => PlateStep(3);
        plate4.onPlayerStep -= (player) => PlateStep(4);
    }


    private void Start() {
        
        plate1.audioSource.pitch = 0.8f;
        plate2.audioSource.pitch = 0.9f;
        plate3.audioSource.pitch = 1.1f;
        plate4.audioSource.pitch = 1.2f;
        plate1.audioSource.clip = noteClip;
        plate2.audioSource.clip = noteClip;
        plate3.audioSource.clip = noteClip;
        plate4.audioSource.clip = noteClip;
    }


    private void PlateStep(int index) {

        if (completed || sequenceStarted)
            return;

        if (!started) {
            StartCoroutine(GenerateSequence());
            started = true;
        } else {
            playerSequence += index.ToString();
            StartCoroutine(PlayPlateNote(index));
            CheckSequence();
        }
    }

    private IEnumerator GenerateSequence() {

        sequenceStarted = true;
        currentSequence = "";
        playerSequence = "";

        yield return new WaitForSeconds(2);


        while (currentSequence.Length < success + 1) {
            int newEntry = Random.Range(1, 5);
            StartCoroutine(PlayPlateNote(newEntry));
            currentSequence += newEntry.ToString();

            yield return new WaitForSeconds(1);
        }

        sequenceStarted = false;
        
    }

    private IEnumerator PlayPlateNote(int slabIndex) {
        PuzzleSlab slab;
        switch(slabIndex) {
            case 1:
                slab = plate1;
                break;
            case 2:
                slab = plate2;
                break;
            case 3:
                slab = plate3;
                break;
            case 4:
                slab = plate4;
                break;
            default:
                yield break;
        }

        Color litColor;
        switch(slabIndex) {
            case 1:
                litColor = Color.red;
                break;
            case 2:
                litColor = Color.green;
                break;
            case 3:
                litColor = Color.blue;
                break;
            case 4:
                litColor = Color.yellow;
                break;
            default:
                yield break;
        }


        // Play sound
        slab.audioSource.Play();
        slab.renderer.material.color = litColor;

        yield return new WaitForSeconds(0.5f);

        slab.renderer.material.color = Color.white;
    }


    private void CheckSequence() {

        if (sequenceStarted)
            return;

        if (currentSequence.Substring(0, playerSequence.Length) != playerSequence) {
            audioSource.PlayOneShot(incorrectClip);
            StopCoroutine(GenerateSequence());
            StartCoroutine(GenerateSequence());
            return;
        }

        if (playerSequence == currentSequence) {
            // Correct sequence
            
            success++;
            if (success >= 6) {
                StopAllCoroutines();
                plate1.renderer.material.color = Color.green;
                plate2.renderer.material.color = Color.green;
                plate3.renderer.material.color = Color.green;
                plate4.renderer.material.color = Color.green;
                audioSource.PlayOneShot(correctClip);
                completed = true;
            } else {
                StopCoroutine(GenerateSequence());
                StartCoroutine(GenerateSequence());
            }

        }
    }
}
