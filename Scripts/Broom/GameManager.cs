using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    
    // Event that will be triggered when all puddles are cleaned
    public UnityEvent onAllPuddlesCleaned;
    
    private int totalPuddles;
    private int cleanedPuddles = 0;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize the event if null
        if (onAllPuddlesCleaned == null)
            onAllPuddlesCleaned = new UnityEvent();
    }
    
    void Start()
    {
        // Count all puddles in the scene
        GameObject[] puddles = GameObject.FindGameObjectsWithTag("Puddle");
        totalPuddles = puddles.Length;
        Debug.Log("Total puddles to clean: " + totalPuddles);
    }
    
    // Call this method when a puddle is cleaned
    public void PuddleCleaned()
    {
        cleanedPuddles++;
        Debug.Log("Puddle cleaned! " + cleanedPuddles + "/" + totalPuddles);
        
        // Check if all puddles are cleaned
        if (cleanedPuddles >= totalPuddles && totalPuddles > 0)
        {
            Debug.Log("All puddles have been cleaned!");
            onAllPuddlesCleaned.Invoke();
        }
    }
    
    // Check if all puddles are cleaned
    public bool AreAllPuddlesCleaned()
    {
        return cleanedPuddles >= totalPuddles && totalPuddles > 0;
    }
    
    // Reset the counter (useful for scene changes or restarting)
    public void ResetPuddleCounter()
    {
        cleanedPuddles = 0;
        GameObject[] puddles = GameObject.FindGameObjectsWithTag("Puddle");
        totalPuddles = puddles.Length;
    }
}