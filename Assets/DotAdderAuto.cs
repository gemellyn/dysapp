using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DotManager))]
public class DotAdderAuto : MonoBehaviour {

    private DotManager dotManager;

    public int NbDots = 30;
    public int Seed = 1;
    

    //Params de generation en cours
    private float lastAngle;
    private float nextAngle;
    private float sommeAngle;
    private float sensVarAngle;
    private float speedVarAngle;
    private float distance;

    // Use this for initialization
    void Start () {
        dotManager = GetComponent<DotManager>();
        generate();
    }

    public void generate()
    {
        StartCoroutine("generateCo");
    }

    private void generateOnePath(int seed)
    {
        this.lastAngle = 0.0f;
        this.nextAngle = 0.0f;
        this.sommeAngle = 0.0f;
        this.sensVarAngle = 1.0f;
        this.speedVarAngle = 2.0f;
        this.distance = 1.3f;

        dotManager.cleanDots();

        Vector3 spawnPos = new Vector3(0.1f, 0.5f, 10);
        spawnPos = Camera.main.ViewportToWorldPoint(spawnPos);
        dotManager.addDot(spawnPos);

       // Random.InitState(seed);
        Debug.Log("Geenerate for seed " + seed);
        for (int i = 0; i < NbDots; i++)
            spawnNextPoint();
        
    }

    public IEnumerator generateCo()
    {
        yield return new WaitForSeconds(0.3f);
        //Random.seed = 1;
        int c = 0;
        bool inViewPort = false;
        for (; c < 200 && !inViewPort; c++)
        {
            Seed = 779910;// Random.Range(0, 999999);
            generateOnePath(Seed);
            
            //On teste le tracé
            Vector3 bary = dotManager.getBarycenter();
            Camera.main.transform.position = new Vector3(bary.x, bary.y, Camera.main.transform.position.z);
            inViewPort = true;
            if (dotManager.isOutOfViewPort())
            {
                inViewPort = false;
                Debug.Log("Out of viewport");

                //Pour tourner autour du barycentre, on prépare
                foreach (Transform tDot in dotManager.Dots)
                    tDot.Translate(dotManager.transform.position - bary);
                dotManager.transform.position = bary;

                //On teste 8 angles (rot de 45 deg)
                int r = 0;
                for (; r < 15 && !inViewPort; r++)
                {
                    dotManager.transform.Rotate(0, 0, 45);
                    if (!dotManager.isOutOfViewPort())
                        inViewPort = true;
                }
                dotManager.UpdateSplineAllDots();

                Debug.Log("Nb Rotate :" + r);
                if (inViewPort)
                    Debug.Log("Rotate Worked !");
                    
            }
        }

        Debug.Log("En " + c + " coups");

        dotManager.validateAllDots(false);
        dotManager.showAllPath(false);
        dotManager.showAllDots(false);
        dotManager.showDot(0, true);

        yield return null;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2")) 
        {

            dotManager.transform.Rotate(0, 0, 90);
        }
    }

    Vector3 spawnNextPoint()
    {

        Vector3 dir;
        Vector3 lastPoint = dotManager.Dots[dotManager.Dots.Count - 1].position;
        Vector3 lastDir;
        if (dotManager.Dots.Count >= 2)
        { 
            //Dernière direction{
            
            Vector3 A = dotManager.Dots[dotManager.Dots.Count - 2].position;
            dir = (lastPoint - A).normalized;
            lastDir = dir;
            dir = Quaternion.AngleAxis(nextAngle, Vector3.forward) * dir;
            
        }
        else
        {
            dir = Vector3.right;
            lastDir = dir;
            dir = Quaternion.AngleAxis(nextAngle, Vector3.forward) * dir;
        }

        Vector3 spawnPos = lastPoint + dir * distance;

        //On applique les forces du bord
       /* Vector3 fBord = new Vector3(0.5f,0.5f,10) - Camera.main.WorldToViewportPoint(spawnPos);
        fBord.x = Mathf.Sign(fBord.x) * (Mathf.Exp(1.0f * Mathf.Abs(fBord.x)) - 1);
        fBord.y = Mathf.Sign(fBord.y) * (Mathf.Exp(1.0f * Mathf.Abs(fBord.y)) - 1);
        if (Mathf.Abs(fBord.x) > Mathf.Abs(fBord.y))
            spawnPos.x += fBord.x;
        else
            spawnPos.y += fBord.y;

        if (dotManager.Dots.Count >= 2)
        {
            Debug.Log("From " + nextAngle);
            nextAngle = Vector3.Angle(lastDir, (spawnPos - lastPoint).normalized);
            speedVarAngle = Mathf.Abs(nextAngle - lastAngle);
            sensVarAngle = Mathf.Sign(nextAngle - lastAngle);

            Debug.Log(nextAngle);
        }*/
           

        //spawnPos = (spawnPos - lastPoint).normalized * distance + lastPoint;


        dotManager.addDot(spawnPos);
        sommeAngle += Mathf.Abs(nextAngle);
        
       // Debug.Log(nextAngle + " " + sommeAngle);
        //Si plus de 180 ou changement de signe de l'angle
        if (sommeAngle >= 180 || lastAngle * nextAngle < 0)
        {
            if (sommeAngle >= 180)
                sensVarAngle = -sensVarAngle;
            if (Random.Range(0.0f, 1.0f) > 0.5f)
                nextAngle = -nextAngle;
            speedVarAngle = Random.Range(1.0f, +5.0f);
            //Debug.Log("New speed " + speedVarAngle);
            sommeAngle = 0.0f;
        }
            
        /*else if (sommeAngle >= 360 && sensVarAngle < 0)
        {
            sensVarAngle = -sensVarAngle;
            speedVarAngle = Random.Range(2.0f, +10.0f);
            Debug.Log("New speed " + speedVarAngle);
        } */   
        
        {
            lastAngle = nextAngle;
            nextAngle += sensVarAngle * speedVarAngle;
            speedVarAngle += Random.Range(0.0f, 0.1f);
            //distance += Random.Range(-0.1f, 0.1f);
        }

        nextAngle = Mathf.Sign(nextAngle) *  Mathf.Min(40, Mathf.Abs(nextAngle));

        /*if (sommeAngle > 360)
            sommeAngle -= 360;
        if (sommeAngle < 0)
            sommeAngle += 360;*/

        return spawnPos;
    }
    
   
}
