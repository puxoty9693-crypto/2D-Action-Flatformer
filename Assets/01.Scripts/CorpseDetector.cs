using UnityEngine;

public class CorpseDetector : MonoBehaviour
{
    public CorpseObject nearCorpse {  get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CorpseObject corpse = collision.GetComponent<CorpseObject>();
        if(corpse != null) 
        {
            nearCorpse = corpse;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CorpseObject corpse = collision.GetComponent<CorpseObject>();
        if (corpse != null && corpse == nearCorpse) 
        {
            nearCorpse = null;
        }
    }






}
