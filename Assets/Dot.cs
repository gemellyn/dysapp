using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

    public Transform NextDot { get; set; }
    public Transform PreviousDot { get; set; }
    public Material MatToValidate;
    public Material MatValidated;
    private bool Validated;

    // Use this for initialization
    void Start () {
        setValidated(false);
        Validated = false;
    }

    public void setValidated(bool validated)
    {
        if (validated)
        {
            if (!this.Validated)
            {
                GetComponentInChildren<Animator>().SetTrigger("Validate");
                GetComponentInChildren<SpriteRenderer>().material = MatValidated;
            }
        }
        else
        {
            if(this.Validated)
                GetComponentInChildren<SpriteRenderer>().material = MatToValidate;
        }
        this.Validated = validated; 
    }
}
