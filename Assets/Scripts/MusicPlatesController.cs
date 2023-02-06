using System;
using System.Collections;
using UnityEngine;

using Unity.Netcode;
using Unity.Collections;

public class MusicPlatesController : NetworkBehaviour {

    [SerializeField] private PuzzleSlab plate1;
    [SerializeField] private PuzzleSlab plate2;
    [SerializeField] private PuzzleSlab plate3;
    [SerializeField] private PuzzleSlab plate4;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip noteClip;
    [SerializeField] private AudioClip incorrectClip;

    [SerializeField] private NetworkVariable<FixedString64Bytes> currentSequence = new(value: "", writePerm: NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<FixedString64Bytes> playerSequence = new(value: "", writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<State> state = new(value: State.Idle, writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<int> success = new(value: 0, writePerm: NetworkVariableWritePermission.Server);




    private IEnumerator PlayPlateNote(char note) {
        switch(note) {
            default:
                yield return PlayPlateNote(1);
                break;
            case '2':
                yield return PlayPlateNote(2);
                break;
            case '3':
                yield return PlayPlateNote(3);
                break;
            case '4':
                yield return PlayPlateNote(4);
                break;
        }
    }

    private IEnumerator PlayPlateNote(int slabIndex) {

        if (state.Value == State.Completed)
            yield break;


        PuzzleSlab slab;
        Color litColor;
        switch(slabIndex) {
            default:
                slab = plate1;
                litColor = Color.red;
                break;
            case 2:
                slab = plate2;
                litColor = Color.green;
                break;
            case 3:
                slab = plate3;
                litColor = Color.blue;
                break;
            case 4:
                slab = plate4;
                litColor = Color.yellow;
                break;
        }

        // Play sound
        slab.audioSource.Play();

        slab.renderer.material.color = litColor;

        yield return new WaitForSeconds(0.5f);

        slab.renderer.material.color = Color.white;
    }


    private void PlateStep(int index) {

        if (state.Value == State.Idle) {
            state.Value = State.SequenceDisplay;
            return;
        }

        if (state.Value != State.InputNeeded)
            return;


        StartCoroutine(PlayPlateNote(index));


        if (IsServer) {
            playerSequence.Value += index.ToString();
            Debug.Log("Player sequence: " + index.ToString());
        }



        if (playerSequence.Value == currentSequence.Value) {

            // Correct full sequence

            if (IsServer)
                success.Value++;

            if (success.Value >= 6) {
                plate1.renderer.material.color = Color.green;
                plate2.renderer.material.color = Color.green;
                plate3.renderer.material.color = Color.green;
                plate4.renderer.material.color = Color.green;

                audioSource.PlayOneShot(correctClip);

                if (IsServer)
                    state.Value = State.Completed;
            } else {
                if (IsServer)
                    state.Value = State.SequenceDisplay;
            }

        } else if (currentSequence.Value[playerSequence.Value.Length - 1] == playerSequence.Value[playerSequence.Value.Length - 1]) {

            // Correct Plate

        } else {

            // Incorrect Plate

            audioSource.PlayOneShot(incorrectClip);

            if (IsServer) {
                playerSequence.Value = "";
                currentSequence.Value = "";
            }

            if (IsServer)
                state.Value = State.SequenceDisplay;
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

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
    }

    private void Start() {
        
        plate1.audioSource.pitch = 0.7f;
        plate2.audioSource.pitch = 0.9f;
        plate3.audioSource.pitch = 1.1f;
        plate4.audioSource.pitch = 1.3f;
        plate1.audioSource.clip = noteClip;
        plate2.audioSource.clip = noteClip;
        plate3.audioSource.clip = noteClip;
        plate4.audioSource.clip = noteClip;
    }

    private float _displayDelayTimer = 1f;
    private int sequenceIndex = -1;
    private void Update() {

        if (state.Value != State.SequenceDisplay)
            return;


        _displayDelayTimer = Mathf.MoveTowards(_displayDelayTimer, 0f, Time.deltaTime);

        if (_displayDelayTimer == 0f) {
            _displayDelayTimer = 1f;
            sequenceIndex++;

            if (IsServer && sequenceIndex == 0) {
                GenerateSequence();
            }

            if (sequenceIndex < currentSequence.Value.Length) {
                StartCoroutine(PlayPlateNote(Convert.ToChar(currentSequence.Value[sequenceIndex])));
            } else {
                sequenceIndex = -1;

                if (IsServer)
                    state.Value = State.InputNeeded;

            }
        }
    }

    private void GenerateSequence() {
        playerSequence.Value = "";
        currentSequence.Value = "";
        for (int i = 0; i < success.Value + 1; i++) {
            int sequenceNote = UnityEngine.Random.Range(1, 5);
            currentSequence.Value += sequenceNote.ToString();
        }
    }

    private enum State {
        Idle,
        SequenceDisplay,
        InputNeeded,
        Completed
    }
}
