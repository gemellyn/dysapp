using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DotManager))]
public class DotPlayerController : MonoBehaviour
{
    private DotManager dotManager;
    private Transform currentDot = null;
    private Vector3 lastClickPos;
    public float TimeEndPath = 1.0f;
    private float timerEndPath = 0;
    public float DistMinPath = 0.5f;
    public Transform generateForLastDot = null;

    // Use this for initialization
    void Start()
    {
        dotManager = GetComponent<DotManager>();
    }

    void Update()
    {
        Vector3 clickPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);

        float distance = Vector3.Distance(lastClickPos, clickPos);
        lastClickPos = clickPos;

        timerEndPath -= Time.fixedDeltaTime;
        if (timerEndPath <= 0 && currentDot != null)
        {
            //Debug.Log("Fin trace");
            currentDot = null;
            dotManager.validateAllDots(false);
            dotManager.showAllPath(false);
            dotManager.showAllDots(false);
            dotManager.showDot(0, true);

        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(clickPos), Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {

                if ((currentDot == null && dotManager.Dots[0] == hit.collider.transform) ||
                    (currentDot != null && currentDot.GetComponent<Dot>().NextDot == hit.collider.transform) ||
                    (currentDot != null && currentDot.GetComponent<Dot>().NextDot != null && currentDot.GetComponent<Dot>().NextDot.GetComponent<Dot>().NextDot == hit.collider.transform))
                {
                    currentDot = hit.collider.transform;
                    currentDot.GetComponent<Dot>().setValidated(true);

                    timerEndPath = TimeEndPath;
                    if (currentDot.GetComponent<Dot>().NextDot != null)
                        timerEndPath *= Mathf.Max(1.0f, Vector3.Distance(currentDot.position, currentDot.GetComponent<Dot>().NextDot.position));
                       
                    //Debug.Log("coucou");
                    dotManager.showAllPath(false);
                    dotManager.showAllDots(false);
                    dotManager.showDot(hit.collider.transform, true, 4);
                    dotManager.showPath(hit.collider.transform, true, 3);

                }

                //la fin 
                if (currentDot != null && currentDot != generateForLastDot && currentDot.GetComponent<Dot>().NextDot == null)
                {
                    generateForLastDot = currentDot;
                    transform.GetComponent<DotAdderAuto>().generate();
                }
            }
        }
    }
     
    void FixedUpdate()
    {
       
       
    }
}
