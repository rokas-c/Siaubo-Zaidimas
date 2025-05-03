using UnityEngine;

public class BroomAction : MonoBehaviour
{
    public GameObject PickUpText;
    public GameObject PutDownText;
    public GameObject BroomOnPlayer;
    public GameObject BroomPlacementLocation;

    private bool isPlayerNearby = false;
    private bool isBroomPickedUp = false;

    void Start()
    {
        BroomOnPlayer.SetActive(false);
        PickUpText.SetActive(false);
        if (PutDownText != null)
            PutDownText.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            bool canPutDown = false;

            // Check if all puddles are cleaned
            if (GameManager.Instance != null)
            {
                canPutDown = GameManager.Instance.AreAllPuddlesCleaned();
            }

            // Check the static variable from PunchInAction
            if (PunchInAction.HasPunchedIn)
            {
                if (!isBroomPickedUp)
                {
                    // Show pick up text if broom isn't picked up
                    PickUpText.SetActive(true);
                    if (PutDownText != null)
                        PutDownText.SetActive(false);

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        PickUpBroom();
                    }
                }
                else if (canPutDown) // Only allow putting down if all puddles are cleaned OR in debug mode
                {
                    // Show put down text if all puddles are cleaned
                    PickUpText.SetActive(false);
                    if (PutDownText != null)
                        PutDownText.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        PutDownBroom();
                    }
                }
                else
                {
                    // Broom is picked up but can't be put down yet
                    PickUpText.SetActive(false);
                    if (PutDownText != null)
                        PutDownText.SetActive(false);
                }
            }
            else
            {
                PickUpText.SetActive(false);
                if (PutDownText != null)
                    PutDownText.SetActive(false);
            }
        }
    }

    private void PickUpBroom()
    {
        this.gameObject.SetActive(false);
        BroomOnPlayer.SetActive(true);
        PickUpText.SetActive(false);
        isBroomPickedUp = true;
    }

    private void PutDownBroom()
    {

        // Place the broom at the designated location if specified
        if (BroomPlacementLocation != null)
        {
            transform.position = BroomPlacementLocation.transform.position;
            transform.rotation = BroomPlacementLocation.transform.rotation;
        }

        this.gameObject.SetActive(true);
        BroomOnPlayer.SetActive(false);
        if (PutDownText != null)
            PutDownText.SetActive(false);
        isBroomPickedUp = false;

        // Disable this script and the collider so the broom can't be picked up again
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;

        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            PickUpText.SetActive(false);
            if (PutDownText != null)
                PutDownText.SetActive(false);
        }
    }
}