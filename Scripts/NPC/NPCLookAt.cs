using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCLookAt : MonoBehaviour
{
    public Transform objectTolookAt;
    public float headWeight;
    public float bodyWeight;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtPosition(objectTolookAt.position);
        animator.SetLookAtWeight(1, bodyWeight, headWeight);
    }
}