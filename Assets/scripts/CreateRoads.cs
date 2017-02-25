using UnityEngine;
using System.Collections;
using System.Collections.Generic;//TODO remove this when i no longer need lists.

//Script which takes the corners attached to this track and extrapolates straight line roads between them.

//Note for future development, red corners where we move in the direction of our turn are round, sharp turns where we don't change velocity until we button press are blue (triangles)m
// Possible third type where we have 0 howrizontal while in them are yellow rectangles and those allow us to change our vertical axis position. Maybe a timing challenge to speed you up if you time it correctly
// (possibly removed to allow 100% constant horizonal control)

public class CreateRoads : MonoBehaviour {
	
	public GameObject[] corner;
	MeshFilter[] cornerMeshFilters;
	public GameObject road;
	public GameObject wall; 
	const float roadWidth = 15f;
	const float roadHeight = 0.01f;
	const float wallWidth = 1.0f;
	const float wallHeight = 2.0f;

	List<Vector3> draws = new List<Vector3> ();
	void Awake()
	{
	}
		
	struct Road{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 direction;
		public Vector3 perpendicularUnitDirection;
		public float length; 
		public Vector3 rightPointEdge1;
		public Vector3 rightPointEdge2;
		public Vector3 leftPointEdge1;
		public Vector3 leftPointEdge2;
		public Vector3 insideCorner;
		public Vector3 outsideCorner;
		public Vector3 leftIntersection1; 
		public Vector3 leftIntersection2;
		public Vector3 rightIntersection1;
		public Vector3 rightIntersection2;
		public Vector3 cornerEdge1; 
		public Vector3 cornerEdge2;
	}

	//Move all this to awake? I think so. No rush. (this would mean initialising cars with start)
	void Start ()
	{
		Road[] roads;
		roads = new Road[corner.Length]; // Create as many roads as corners. 
		cornerMeshFilters = GetComponentsInChildren<MeshFilter>();

		for (int i = 0; i < corner.Length; i++) {
			
			Vector3 corner1pos = corner [i].transform.position;
			Vector3 corner2pos;
			if (i != corner.Length - 1)
				corner2pos = corner [i + 1].transform.position;
			else
				corner2pos = corner [0].transform.position;

			roads[i].direction = corner2pos - corner1pos;
			roads[i].direction.y = 0.0f;

			roads[i].position = new Vector3 ((corner2pos.x + corner1pos.x) / 2,1,(corner2pos.z + corner1pos.z) / 2);// + ((roadWidth) * road1.perpendicularUnitDirection);

			roads [i].rotation = Quaternion.LookRotation (roads[i].direction);
		
			//Calculate edges of each road
			roads[i].perpendicularUnitDirection = new Vector3 (roads[i].direction.z, 0, -roads[i].direction.x).normalized;

			roads [i].rightPointEdge1 = corner1pos + ((roadWidth / 2) * roads[i].perpendicularUnitDirection);
			roads [i].rightPointEdge2 = corner2pos + ((roadWidth / 2) * roads[i].perpendicularUnitDirection);

			roads [i].leftPointEdge1 = corner1pos - ((roadWidth / 2) * roads[i].perpendicularUnitDirection);
			roads [i].leftPointEdge2 = corner2pos - ((roadWidth / 2) * roads[i].perpendicularUnitDirection);
		}

		for (int i = 0; i < roads.Length; i++) {

			Road road1 = roads [i];
			Road road2;
			if (i != roads.Length - 1)
				road2 = roads [i + 1];
			else
				road2 = roads [0];

			//Get the shortest distance bewtween the corner intersections. 
			road1.leftIntersection1 = GetIntersection (road1.leftPointEdge1, road1.leftPointEdge2, road2.leftPointEdge1, road2.leftPointEdge2);
			road1.rightIntersection1 = GetIntersection (road1.rightPointEdge1, road1.rightPointEdge2, road2.rightPointEdge1, road2.rightPointEdge2);

			if (Vector3.Distance(road1.leftIntersection1, road1.leftPointEdge1) > Vector3.Distance(road1.rightIntersection1, road1.rightPointEdge1))
			{
				road1.outsideCorner = road1.leftIntersection1;
				road1.insideCorner = road1.rightIntersection1;
				road1.leftIntersection1 = road1.rightIntersection1 - ((roadWidth) * road1.perpendicularUnitDirection);
				road1.cornerEdge1 = road1.leftIntersection1;
				road2.rightIntersection2 = road1.rightIntersection1;
				road2.leftIntersection2 = road2.rightIntersection2 - ((roadWidth) * road2.perpendicularUnitDirection);
				road1.cornerEdge2 = road2.leftIntersection2;
			}
			else{
				road1.outsideCorner = road1.rightIntersection1;
				road1.insideCorner = road1.leftIntersection1;

				road1.rightIntersection1 = road1.leftIntersection1 + ((roadWidth) * road1.perpendicularUnitDirection);
				road1.cornerEdge1 = road1.rightIntersection1;

				road2.leftIntersection2 = road1.leftIntersection1;
				road2.rightIntersection2 = road2.leftIntersection2 + ((roadWidth) * road2.perpendicularUnitDirection);
				road1.cornerEdge2 = road2.rightIntersection2;
			}

			roads [i] = road1;
			if (i != roads.Length - 1)
				roads [i + 1] = road2;
			else
				roads [0] = road2;
		}

		for (int i = 0; i < roads.Length; i++) {

			Road road1 = roads [i];
			Road road2;
			if (i != roads.Length - 1)
				road2 = roads [i + 1];
			else
				road2 = roads [0];
			
			Vector3 corner1pos = corner [i].transform.position;
			Vector3 corner2pos;
			if (i != corner.Length - 1)
				corner2pos = corner [i + 1].transform.position;
			else
				corner2pos = corner [0].transform.position;
			
			Vector3[] intersections1 = { road1.leftIntersection1,  road1.rightIntersection1}; //No longer really necessary but leave it in in case tracks get weird. 
			Vector3[] intersections2 = { road1.leftIntersection2  ,road1.rightIntersection2};
			Vector3[] insideEdge = GetShortestDistance (intersections1, intersections2);
	
			road1.length = Vector3.Distance(insideEdge[0],insideEdge[1]);

			//Get midpoint;
			road1.position = new Vector3 ((insideEdge [0].x + insideEdge [1].x) / 2, 1, (insideEdge [0].z + insideEdge [1].z) / 2) + ((roadWidth/2) * road1.perpendicularUnitDirection);

			var newRoad = Instantiate (road, road1.position, road1.rotation);
			newRoad.transform.localScale = new Vector3 (roadWidth, roadHeight, road1.length);

			//Instantiate corners
			Mesh m = new Mesh();
			m.name = ("Corner" + i + "Mesh");;
			m.vertices = new Vector3[] {road1.cornerEdge1- corner[i].transform.position,road1.cornerEdge2- corner[i].transform.position,road1.insideCorner- corner[i].transform.position,road1.outsideCorner- corner[i].transform.position};
			m.uv = new Vector2[] {new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};	
			m.triangles = new int[]{ 0, 2 , 1, 1,2,0,0,3,1,1,3,0};
			m.RecalculateNormals();
			corner [i].GetComponent<MeshFilter>().mesh = m;

			//Instantiate Walls
			Vector3 wallHeightOffset = new Vector3 (0, wallHeight/2,0);
			var wall1 = Instantiate(wall, wallHeightOffset + road1.position + roadWidth/2 * road1.perpendicularUnitDirection,road1.rotation);
			wall1.transform.localScale = new Vector3 (wallWidth, wallHeight, road1.length);

			var wall2 = Instantiate(wall,  wallHeightOffset + road1.position - roadWidth/2 * road1.perpendicularUnitDirection,road1.rotation);
			wall2.transform.localScale = new Vector3 (wallWidth, wallHeight, road1.length);

			var wall3 = Instantiate(wall,  wallHeightOffset + (road1.cornerEdge1 + road1.outsideCorner)/2 ,road1.rotation);
			if (road1.cornerEdge1 != road1.insideCorner)
				wall3.transform.localScale = new Vector3 (wallWidth, wallHeight,   Vector3.Distance(road1.outsideCorner, road1.cornerEdge1));
			else
				wall3.transform.localScale = new Vector3 (wallWidth, wallHeight,  Vector3.Distance(road1.outsideCorner , road1.cornerEdge2));

			var wall4 = Instantiate(wall,  wallHeightOffset + (road1.cornerEdge2 + road1.outsideCorner)/2 ,road2.rotation);
			if (road1.cornerEdge1 != road1.insideCorner)
				wall4.transform.localScale = new Vector3 (wallWidth, wallHeight,   Vector3.Distance(road1.outsideCorner, road1.cornerEdge1));
			else
				wall4.transform.localScale = new Vector3 (wallWidth, wallHeight,  Vector3.Distance(road1.outsideCorner , road1.cornerEdge2));
			//var wall4 = Instantiate(wall, road1.position - roadWidth/2 * road1.perpendicularUnitDirection,road1.rotation);
			//wall2.transform.localScale = new Vector3 (wallWidth, wallHeight, road1.length);
		}
	}
		
	Vector3 GetIntersection(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4) // Gets the x and z intersection, currently ignores y axis. 
	{
		Vector3 point;
		float magnitude = ((point1.x - point2.x) * (point3.z - point4.z) - (point1.z - point2.z) * (point3.x - point4.x));
		point.x = ((point1.x * point2.z - point1.z * point2.x) * (point3.x - point4.x) - (point1.x - point2.x) * (point3.x * point4.z - point3.z * point4.x)) / magnitude;
		point.y = 1;
		point.z = ((point1.x * point2.z - point1.z * point2.x) * (point3.z - point4.z) - (point1.z - point2.z) * (point3.x * point4.z - point3.z * point4.x)) / magnitude;
		return point;
	}
		
	Vector3[] GetShortestDistance(Vector3[] points1, Vector3[] points2)//Compares all the points in points 1 to all the points in points 2 and returns the two points with the shortest distance
	{
		Vector3[] shortestDistance = { points1 [0], points2 [0] };
		float distance = Vector3.Distance (points1 [0], points2 [0]);

		for (int i = 0; i < points1.Length; i++) {
			for (int j = 0; j < points2.Length; j++) {
				if (distance > Vector3.Distance (points1 [i], points2 [j])) {
					distance = Vector3.Distance (points1 [i], points2 [j]);
					shortestDistance [0] = points1 [i];
					shortestDistance [1] = points2 [j];
				}
			}
		}
		return shortestDistance;
	}

	void Update()
	{
	///	for (int i = 0; i < draws.Count; i++) {
	//		if (i % 2 == 0) {
	//			Debug.DrawLine(draws[i],draws[i+1]);
	//		}
	//	}
	}

	void OnDrawGizmos() //Method to draw what the track is going to look like defined by the corners in editor mode.
	{
		for (int i = 0; i < corner.Length - 1; ++i) {
			if (corner[i] != null) {
				{
					Gizmos.DrawSphere (corner [i].transform.position, 1.0f);
					Gizmos.DrawLine (corner [i].transform.position, corner [i + 1].transform.position);
				}
			}
		}

		Gizmos.DrawSphere(corner [corner.Length-1].transform.position,1.0f);
		Gizmos.DrawLine (corner [corner.Length-1].transform.position, corner [0].transform.position);
	}
}