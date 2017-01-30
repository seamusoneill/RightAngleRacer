using UnityEngine;
using System.Collections;

public class CornerProperties: MonoBehaviour {

    public int roadWidth; //The width of the road at the ccorner point
    public Rect areaOfEffect; //The rectangle which slows the player down on entrance
	public GameObject nextCorner; //The next corner in the road //TODO not have to assign these manually
    private LineRenderer lineRenderer;
    // Use this for initialization
    void Start()
    {
        CreateRoad();
		lineRenderer = new LineRenderer();
    }

    void CreateRoad()
    {
     //   Debug.DrawRay(transform.position, nextCorner.transform.position, new Color(30,100,30));
       // Gizmos.DrawLine(transform.position, nextCorner.transform.position);
    }

	//TODO during corner placement i want a line drawn from one corener to it's nextCorner

    // Update is called once per frame
    void Update()
    {
    }
}
