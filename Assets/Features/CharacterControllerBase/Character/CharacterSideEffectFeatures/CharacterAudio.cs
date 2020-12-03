using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections.Generic;

[Serializable]
[RequireComponent(typeof(FirstPersonCharacterController), typeof(AudioSource))]
public class CharacterAudio : MonoBehaviour {

    public AudioClip[] footsteps;
    public AudioClip jump;
    public AudioClip land;

    [SerializeField]
    private GameObject characterSignalsInterfaceTarget;
    private ICharacterSignals characterSignals;
  
    private AudioSource audioSource;

    private void Awake() {
        characterSignals = characterSignalsInterfaceTarget.GetComponent<FirstPersonCharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        characterSignals.stepped
            .SelectRandom(footsteps)
            .Subscribe(clip => audioSource.PlayOneShot(clip))
            .AddTo(this);

        characterSignals.jumped
            .Subscribe(_ => audioSource.PlayOneShot(jump))
            .AddTo(this);

        characterSignals.landed
            .Subscribe(_ => audioSource.PlayOneShot(land))
            .AddTo(this);
    }
}