using UnityEngine;

public abstract class Interactible : MonoBehaviour , IInteractible
{
    protected bool IsInteractible = false;

    public virtual void Interaction()
    {
        print("nul");
    }
    
}
