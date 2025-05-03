using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    // Event that will be triggered when all puddles are cleaned
    public UnityEvent onAllPuddlesCleaned;

    // Event for broom task completion
    public UnityEvent onBroomTaskCompleted;

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

        // Initialize the events if null
        if (onAllPuddlesCleaned == null)
            onAllPuddlesCleaned = new UnityEvent();

        if (onBroomTaskCompleted == null)
            onBroomTaskCompleted = new UnityEvent();
    }

    void Start()
    {
        // Count all puddles in the scene
        GameObject[] puddles = GameObject.FindGameObjectsWithTag("Puddle");
        totalPuddles = puddles.Length;
    }

    // Call this method when a puddle is cleaned
    public void PuddleCleaned()
    {
        cleanedPuddles++;

        // Check if all puddles are cleaned
        if (cleanedPuddles >= totalPuddles && totalPuddles > 0)
        {
            onAllPuddlesCleaned.Invoke();
        }
    }

    // Check if all puddles are cleaned
    public bool AreAllPuddlesCleaned()
    {
        return cleanedPuddles >= totalPuddles && totalPuddles > 0;
    }

    // Called when the broom task is completed
    public void BroomTaskCompleted()
    {

        // Invoke the event
        if (onBroomTaskCompleted != null)
            onBroomTaskCompleted.Invoke();
    }

    // Reset the counter (useful for scene changes or restarting)
    public void ResetPuddleCounter()
    {
        cleanedPuddles = 0;
        GameObject[] puddles = GameObject.FindGameObjectsWithTag("Puddle");
        totalPuddles = puddles.Length;
    }
}