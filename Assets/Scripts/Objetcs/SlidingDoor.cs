using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Slide Settings")]
    [Tooltip("How far (in Unity units) the door slides upward.")]
    public float slideDistance = 3f;

    [Tooltip("How many seconds the slide animation takes.")]
    public float slideDuration = 1.5f;

    [Tooltip("Animation curve for the slide (ease in/out by default).")]
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Audio (optional)")]
    [Tooltip("Optional sound to play when the door opens.")]
    public AudioClip openSound;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpen = false;
    private bool _isMoving = false;

    private AudioSource _audioSource;

    private void Awake()
    {
        _closedPosition = transform.position;
        _openPosition = _closedPosition + Vector3.up * slideDistance;

        if (openSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = openSound;
            _audioSource.playOnAwake = false;
        }
    }

    public void Open()
    {
        if (_isOpen || _isMoving) return;
        StartCoroutine(SlideRoutine(_closedPosition, _openPosition));
    }

    public void Close()
    {
        if (!_isOpen || _isMoving) return;
        StartCoroutine(SlideRoutine(_openPosition, _closedPosition));
    }

    private IEnumerator SlideRoutine(Vector3 from, Vector3 to)
    {
        _isMoving = true;

        if (_audioSource != null)
            _audioSource.Play();

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float curvedT = slideCurve.Evaluate(t);
            transform.position = Vector3.LerpUnclamped(from, to, curvedT);
            yield return null;
        }

        transform.position = to;

        _isOpen = (to == _openPosition);
        _isMoving = false;
    }

    
    // expose Open() to UnityEvents in the Inspector so you can also trigger it from Buttons, Triggers, etc. (?)

}
