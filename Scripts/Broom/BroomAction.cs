using UnityEngine;

public class BroomAction : MonoBehaviour
{
    public GameObject PickUpText;
    public GameObject BroomOnPlayer;
    public GameObject NotPunchedInText;
    
    private bool isPlayerNearby = false;
    
    void Start()
    {
        BroomOnPlayer.SetActive(false);
        PickUpText.SetActive(false);
        if (NotPunchedInText != null)
            NotPunchedInText.SetActive(false);
    }
    
    void Update()
    {
        if (isPlayerNearby)
        {
            // Check the static variable from PunchInAction
            if (PunchInAction.HasPunchedIn)
            {
                PickUpText.SetActive(true);
                if (NotPunchedInText != null)
                    NotPunchedInText.SetActive(false);
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    this.gameObject.SetActive(false);
                    BroomOnPlayer.SetActive(true);
                    PickUpText.SetActive(false);
                }
            }
            else
            {
                PickUpText.SetActive(false);
                if (NotPunchedInText != null)
                    NotPunchedInText.SetActive(true);
            }
        }
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
            if (NotPunchedInText != null)
                NotPunchedInText.SetActive(false);
        }
    }
}