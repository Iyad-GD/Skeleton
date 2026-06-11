using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Core minigame logic. Attach to a manager GameObject inside your minigame Canvas.
/// </summary>
public class ArrowMinigame : MonoBehaviour
{
    [Header("Sequence Settings")]
    [Tooltip("How many arrows in the sequence.")]
    public int sequenceLength = 4;
    [Tooltip("Time the player has to input the full sequence.")]
    public float timeLimit = 5f;

    [Header("Arrow Sprites")]
    public Sprite arrowUp;
    public Sprite arrowDown;
    public Sprite arrowLeft;
    public Sprite arrowRight;

    [Header("UI References")]
    [Tooltip("Parent transform where arrow Image slots live (horizontal layout group recommended).")]
    public Transform arrowSequenceContainer;
    [Tooltip("Prefab: a single UI Image used to display one arrow. Just an Image component on a GameObject.")]
    public GameObject arrowSlotPrefab;
    [Tooltip("The timer fill image (Image type: Filled, fill method: Horizontal).")]
    public Image timerBar;
    [Tooltip("Shown on success.")]
    public GameObject successPanel;
    [Tooltip("Shown on failure.")]
    public GameObject failPanel;
    [Tooltip("The root panel of the whole minigame UI.")]
    public GameObject minigamePanel;

    [Header("Feedback Colors")]
    public Color defaultColor   = Color.white;
    public Color correctColor   = Color.green;
    public Color wrongColor     = Color.red;

    // The generated sequence
    private List<KeyCode> _sequence = new List<KeyCode>();
    private List<Image>   _slots    = new List<Image>();
    private int  _currentIndex = 0;
    private bool _active = false;
    private float _timeRemaining;

    // Who to notify on win
    private SlidingDoor _linkedDoor;

    // Key → sprite lookup
    private Dictionary<KeyCode, Sprite> _spriteMap;

    private void Awake()
    {
        _spriteMap = new Dictionary<KeyCode, Sprite>
        {
            { KeyCode.UpArrow,    arrowUp    },
            { KeyCode.DownArrow,  arrowDown  },
            { KeyCode.LeftArrow,  arrowLeft  },
            { KeyCode.RightArrow, arrowRight }
        };
    }

    /// <summary>
    /// Called by the trigger to open the minigame.
    /// </summary>
    public void StartMinigame(SlidingDoor door)
    {
        _linkedDoor = door;
        GenerateSequence();
        BuildSlots();

        successPanel.SetActive(false);
        failPanel.SetActive(false);
        minigamePanel.SetActive(true);

        _currentIndex  = 0;
        _timeRemaining = timeLimit;
        _active        = true;

        // Pause the game while minigame is open
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!_active) return;

        // Timer — use unscaled because timeScale is 0
        _timeRemaining -= Time.unscaledDeltaTime;
        if (timerBar != null)
            timerBar.fillAmount = _timeRemaining / timeLimit;

        if (_timeRemaining <= 0f)
        {
            StartCoroutine(EndMinigame(false));
            return;
        }

        // Read input
        KeyCode[] keys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        foreach (KeyCode key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                HandleInput(key);
                break;
            }
        }
    }

    private void HandleInput(KeyCode key)
    {
        if (key == _sequence[_currentIndex])
        {
            // Correct
            _slots[_currentIndex].color = correctColor;
            _currentIndex++;

            if (_currentIndex >= _sequence.Count)
                StartCoroutine(EndMinigame(true));
        }
        else
        {
            // Wrong — flash all red then end
            StartCoroutine(FlashWrong());
        }
    }

    private IEnumerator FlashWrong()
    {
        _active = false;
        foreach (var slot in _slots)
            slot.color = wrongColor;

        yield return new WaitForSecondsRealtime(0.6f);
        StartCoroutine(EndMinigame(false));
    }

    private IEnumerator EndMinigame(bool won)
    {
        _active = false;

        if (won)
        {
            successPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(1.2f);
            _linkedDoor?.Open();
        }
        else
        {
            failPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(1.2f);
        }

        CloseMinigame();
    }

    private void CloseMinigame()
    {
        Time.timeScale = 1f;
        minigamePanel.SetActive(false);

        // Clean up slots for next run
        foreach (Transform child in arrowSequenceContainer)
            Destroy(child.gameObject);
        _slots.Clear();
        _sequence.Clear();
    }

    private void GenerateSequence()
    {
        KeyCode[] options = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        _sequence.Clear();
        for (int i = 0; i < sequenceLength; i++)
            _sequence.Add(options[Random.Range(0, options.Length)]);
    }

    private void BuildSlots()
    {
        foreach (Transform child in arrowSequenceContainer)
            Destroy(child.gameObject);
        _slots.Clear();

        foreach (KeyCode key in _sequence)
        {
            GameObject slot = Instantiate(arrowSlotPrefab, arrowSequenceContainer);
            Image img = slot.GetComponent<Image>();
            img.sprite = _spriteMap[key];
            img.color  = defaultColor;
            _slots.Add(img);
        }
    }
}
