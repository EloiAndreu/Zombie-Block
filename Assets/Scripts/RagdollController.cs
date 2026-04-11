using UnityEngine;

public class RagdollController : MonoBehaviour
{
    Rigidbody[] rbs;
    Animator anim;

    void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        anim = GetComponent<Animator>();

        SetRagdoll(false);
    }

    public void SetRagdoll(bool state)
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = !state;
        }

        anim.enabled = !state;
    }
}