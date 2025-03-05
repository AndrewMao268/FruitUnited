using QuikGraph.Algorithms;
using Unity.VisualScripting;
using UnityEngine;

abstract public class BackgroundParallax : MonoBehaviour
{
    public GameObject cam;

    private Vector3 startPos;
    public float interval = 20.0f;
    public float parallaxEffect = 1.0f;
    
    [HideInInspector] public int id;
    private int idOffset = 100;

    protected void OwnStart()
    {
        startPos = transform.position;
        if (id == 0)
        {
            for (int i = -idOffset; i < idOffset + 1; i++)
            {
                if (i == 0) continue;

                GameObject newObj = Instantiate(gameObject);
                newObj.GetComponent<BackgroundParallax>().id = i;
            }
        }
    }

    abstract public void SetRandomFactor();
    abstract public float GetRandomFactor();
   
    void FixedUpdate()
    {
        float parallax = (cam.transform.position.x - cam.GetComponent<CameraMan>().initialX) * parallaxEffect;
        float position = startPos.x + interval * id + GetRandomFactor();
        transform.position = new Vector3(position + parallax, transform.position.y, transform.position.z);
    }
}
