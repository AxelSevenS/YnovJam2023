using UnityEngine;

using Unity.Netcode;

public class SlabsController : NetworkBehaviour {

    [SerializeField] private PuzzleSlab player1Slab1;
    [SerializeField] private PuzzleSlab player1Slab2;
    [SerializeField] private PuzzleSlab player1Slab3;
    [SerializeField] private PuzzleSlab player2Slab1;
    [SerializeField] private PuzzleSlab player2Slab2;
    [SerializeField] private PuzzleSlab player2Slab3;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip incorrectClip;

    private int progress = -1;


    private static bool _completed = false;

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
        player1Slab1.onPlayerStep += Player1Slab1Step;
        player1Slab2.onPlayerStep += Player1Slab2Step;
        player1Slab3.onPlayerStep += Player1Slab3Step;
        player2Slab1.onPlayerStep += Player2Slab1Step;
        player2Slab2.onPlayerStep += Player2Slab2Step;
        player2Slab3.onPlayerStep += Player2Slab3Step;
    }

    private void OnDisable() {
        player1Slab1.onPlayerStep -= Player1Slab1Step;
        player1Slab2.onPlayerStep -= Player1Slab2Step;
        player1Slab3.onPlayerStep -= Player1Slab3Step;
        player2Slab1.onPlayerStep -= Player2Slab1Step;
        player2Slab2.onPlayerStep -= Player2Slab2Step;
        player2Slab3.onPlayerStep -= Player2Slab3Step;
    }

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
    }

    private void Start() {
        SetProgress(0);
    }


    private void Player1Slab1Step(Player player) {
        if (progress == 2) {
            SetProgress(3);
        } else if (progress < 3) {
            SetProgress(0);
        }
    }

    private void Player1Slab2Step(Player player) {
        if (progress == 0) {
            SetProgress(1);
        } else if (progress < 1) {
            SetProgress(0);
        }
    }

    private void Player1Slab3Step(Player player) {
        if (progress == 4) {
            SetProgress(5);
        } else if (progress < 5) {
            SetProgress(0);
        }
    }


    private void Player2Slab1Step(Player player) {
        if (progress == 5) {
            SetProgress(6);
        } else if (progress < 6) {
            SetProgress(0);
        }
    }

    private void Player2Slab2Step(Player player) {
        if (progress == 3) {
            SetProgress(4);
        } else if (progress < 4) {
            SetProgress(0);
        }
    }
    private void Player2Slab3Step(Player player) {
        if (progress == 1) {
            SetProgress(2);
        } else if (progress < 2) {
            SetProgress(0);
        }
    }

    private void SetProgress(int progress) {
        switch(progress) {
            case 0:
                player1Slab1.renderer.material.color = Color.white;
                player1Slab2.renderer.material.color = Color.green;
                player1Slab3.renderer.material.color = Color.white;
                player2Slab1.renderer.material.color = Color.white;
                player2Slab2.renderer.material.color = Color.white;
                player2Slab3.renderer.material.color = Color.white;
                break;
            case 1:
                player1Slab1.renderer.material.color = Color.white;
                player1Slab2.renderer.material.color = Color.black;
                player1Slab3.renderer.material.color = Color.white;
                player2Slab1.renderer.material.color = Color.white;
                player2Slab2.renderer.material.color = Color.red;
                player2Slab3.renderer.material.color = Color.white;
                break;
            case 2:
                player1Slab1.renderer.material.color = Color.white;
                player1Slab2.renderer.material.color = Color.black;
                player1Slab3.renderer.material.color = Color.white;
                player2Slab1.renderer.material.color = Color.white;
                player2Slab2.renderer.material.color = Color.white;
                player2Slab3.renderer.material.color = Color.black;
                break;
            case 3:
                player1Slab1.renderer.material.color = Color.black;
                player1Slab2.renderer.material.color = Color.black;
                player1Slab3.renderer.material.color = Color.white;
                player2Slab1.renderer.material.color = Color.white;
                player2Slab2.renderer.material.color = Color.white;
                player2Slab3.renderer.material.color = Color.black;
                break;
            case 4:
                player1Slab1.renderer.material.color = Color.black;
                player1Slab2.renderer.material.color = Color.black;
                player1Slab3.renderer.material.color = Color.white;
                player2Slab1.renderer.material.color = Color.white;
                player2Slab2.renderer.material.color = Color.black;
                player2Slab3.renderer.material.color = Color.black;
                break;
            case 5:
                player1Slab1.renderer.material.color = Color.black;
                player1Slab2.renderer.material.color = Color.black;
                player1Slab3.renderer.material.color = Color.black;
                player2Slab1.renderer.material.color = Color.white;
                player2Slab2.renderer.material.color = Color.black;
                player2Slab3.renderer.material.color = Color.black;
                break;
            case 6:
                player1Slab1.renderer.material.color = Color.green;
                player1Slab2.renderer.material.color = Color.green;
                player1Slab3.renderer.material.color = Color.green;
                player2Slab1.renderer.material.color = Color.green;
                player2Slab2.renderer.material.color = Color.green;
                player2Slab3.renderer.material.color = Color.green;
                completed = true;
                break;
        }

        if (this.progress < progress && progress != 0) {
            audioSource.clip = correctClip;
            audioSource.Play();
        } else if (this.progress > progress) {
            audioSource.clip = incorrectClip;
            audioSource.Play();
        }

        this.progress = progress;
    }
}
