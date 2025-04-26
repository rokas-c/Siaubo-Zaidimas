using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroomJumpscareTrigger : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip jumpscareSound; // Single sound for the jumpscare
    [SerializeField] private float minDelayAfterBroomPlacement = 3f; // Minimum delay before jumpscare
    [SerializeField] private float maxDelayAfterBroomPlacement = 10f; // Maximum delay before jumpscare
    [SerializeField] private float volumeScale = 1f; // Volume multiplier for the sound

    [Header("Jumpscare Settings")]
    [SerializeField] private GameObject jumpscareModelPrefab; // Jumpscare character to spawn
    [SerializeField] private Transform jumpscareSpawnPosition; // Where to spawn the jumpscare model
    [SerializeField] private GameObject jumpscareColliderObject; // Collider to enable for jumpscare trigger
    
    private AudioSource audioSource;
    private bool jumpscareTriggered = false;
    
    void Start()
    {
        // Add audio source component if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Make sure jumpscare elements are hidden initially
        if (jumpscareModelPrefab != null)
        {
            jumpscareModelPrefab.SetActive(false);
        }
        
        if (jumpscareColliderObject != null)
        {
            jumpscareColliderObject.SetActive(false);
        }
    }
    
    // This public method will be called by BroomPlacementZone when the broom is placed
    public void OnBroomPlaced()
    {
        if (!jumpscareTriggered)
        {
            jumpscareTriggered = true;
            StartCoroutine(TriggerJumpscareSequence());
        }
    }
    
    private IEnumerator TriggerJumpscareSequence()
    {
        // Random delay before jumpscare
        float randomDelay = Random.Range(minDelayAfterBroomPlacement, maxDelayAfterBroomPlacement);
        yield return new WaitForSeconds(randomDelay);
        
        // Play jumpscare sound
        PlayJumpscareSound();
        
        // Additional delay after sound before jumpscare
        yield return new WaitForSeconds(1.5f);
        
        // Activate jumpscare model at spawn position
        if (jumpscareModelPrefab != null && jumpscareSpawnPosition != null)
        {
            jumpscareModelPrefab.SetActive(true);
            jumpscareModelPrefab.transform.position = jumpscareSpawnPosition.position;
            jumpscareModelPrefab.transform.rotation = jumpscareSpawnPosition.rotation;
        }
        
        // Activate jumpscare collider
        if (jumpscareColliderObject != null)
        {
            jumpscareColliderObject.SetActive(true);
        }
    }
    
    private void PlayJumpscareSound()
    {
        if (jumpscareSound != null && audioSource != null)
        {
            // Play the jumpscare sound
            audioSource.clip = jumpscareSound;
            audioSource.volume = volumeScale;
            audioSource.Play();
        }
    }
    
    // For debugging purposes
    public void ForceJumpscare()
    {
        if (!jumpscareTriggered)
        {
            jumpscareTriggered = true;
            StartCoroutine(TriggerJumpscareSequence());
        }
    }
}