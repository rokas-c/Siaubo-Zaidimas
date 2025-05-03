using UnityEngine;
using System.Collections;

public class FuelPumpAIConnector : MonoBehaviour
{
    [Header("References")]
    public FuelPumpInteraction fuelPump; // Reference to your existing fuel pump script
    public AI aiController; // Reference to your existing AI script

    [Header("Settings")]
    public float activationDelay = 5f; // Delay in seconds before AI activates

    [Header("Debug")]
    public bool aiWasActivated = false;
    private bool isDelayStarted = false;

    private void Start()
    {
        // Initially disable the AI's NavMeshAgent
        if (aiController != null)
        {
            var navMeshAgent = aiController.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }

            // Disable the AI script but not the GameObject
            aiController.enabled = false;
        }
    }

    private void Update()
    {
        // If AI hasn't been activated yet and delay hasn't started, check the fuel pump
        if (!aiWasActivated && !isDelayStarted && fuelPump != null && aiController != null)
        {
            CheckFuelPumpAndStartDelay();
        }
    }

    private void CheckFuelPumpAndStartDelay()
    {
        // Check if the fuel pump has been used
        if (fuelPump.HasBeenUsed())
        {
            // Start the delay coroutine
            StartCoroutine(DelayedActivation());
            isDelayStarted = true;
        }
    }

    private IEnumerator DelayedActivation()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(activationDelay);

        // Enable the AI script
        aiController.enabled = true;

        // Enable the NavMeshAgent
        var navMeshAgent = aiController.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }

        // Get the Animator component to trigger animation if needed
        var animator = aiController.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsMoving", false);
        }

        aiWasActivated = true;
    }
}