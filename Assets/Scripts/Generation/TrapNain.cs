using UnityEngine;

public class TrapNain : MonoBehaviour
{
    private bool nainBecileHere = false;
    [SerializeField] private Animator animator;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Nain"))
        {
            Debug.Log("Detected Nain: " + collision.gameObject.name);
            if(collision.gameObject.GetComponent<CrazyNain>())
            {

                Debug.Log("Detected CrazyNain: " + collision.gameObject.name);
                if (collision.gameObject.GetComponent<CrazyNain>()._isDown == false)
                {
                    Debug.Log("Nain is not down: " + collision.gameObject.name);
                    if (collision.gameObject.GetComponent<CrazyNain>() is Nainbecile nainbecile)
                    {
                        Debug.Log("Hitting Nainbecile: " + collision.gameObject.name);
                        animator.SetTrigger("Active");
                        SoundManager.Instance.PlaySFX("NainChute");
                        nainbecile.Hit();
                        nainBecileHere = true;
                    }
                    else if (collision.gameObject.GetComponent<CrazyNain>() is NainNanere nainNanere && nainBecileHere)
                    {
                        Debug.Log("Hitting NainNanere: " + collision.gameObject.name);
                        nainNanere.transform.position = new Vector2(nainNanere.transform.position.x, nainNanere.transform.position.y - 1);
                        animator.SetTrigger("Active");
                        SoundManager.Instance.PlaySFX("NainChute");
                        nainNanere.Hit();
                    }
                }
            }
        }
    }
}
