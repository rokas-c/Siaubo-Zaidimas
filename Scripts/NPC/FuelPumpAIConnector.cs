using UnityEngine;

public class FuelPumpAIConnector : MonoBehaviour
{
    [Header("References")]
    public FuelPumpInteraction fuelPump; // Reference to your existing fuel pump script
    public AI aiController; // Reference to your existing AI script

    [Header("Debug")]
    public bool aiWasActivated = false;

    private void Start()
    {
        // Check if references are assigned
        if (fuelPump == null)
        {
            Debug.LogError("Fuel Pump reference not assigned in FuelPumpAIConnector!");
        }

        if (aiController == null)
        {
            Debug.LogError("AI Controller reference not assigned in FuelPumpAIConnector!");
        }

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
        // If AI hasn't been activated yet, check the fuel pump
        if (!aiWasActivated && fuelPump != null && aiController != null)
        {
            CheckFuelPumpAndActivateAI();
        }
    }

    private void CheckFuelPumpAndActivateAI()
    {
        // Check if the fuel pump has been used
        if (fuelPump.HasBeenUsed())
        {
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

            Debug.Log("AI activated by fuel pump interaction!");
            aiWasActivated = true;
        }
    }
}