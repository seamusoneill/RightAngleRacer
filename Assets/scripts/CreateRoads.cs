using UnityEngine;
using System.Collections;

//Script which takes the corners attached to this track and extrapolates stright line roads between them.

//Note for future development, red corners where we move in the direction of our turn are round, sharp turns where we don't change velocity until we button press are blue (triangles)m
// Possible third type where we have 0 velocity while in them are yellow rectangles and those allow us to change our vertical axis position. MAybe a timing challenge to speed you up if you time it correctly
// (possibly removed to allow 100% constant vert control)

public class CreateRoads : MonoBehaviour {

	public GameObject[] corner;
	CornerProperties[] corners;
	public GameObject road;
	public GameObject wall; 
	const float roadWidth = 15.0f;
	const float roadHeight = 0.01f;
	const float wallWidth = 1.0f;
	const float wallHeight = 1.0f;

	void Awake()
	{
	}

	//Move all this to awake? I think so. No rush.
	void Start (){
		
		//Create a road from each corner to the next
		for (int i = 0; i < corner.Length - 1; ++i)
		{
			//Set rotation
			Vector3 cornerToNextCorner = corner[i+1].transform.position - corner[i].transform.position;
			cornerToNextCorner.y = 0.0f;

			Quaternion roadRotation = Quaternion.LookRotation(cornerToNextCorner);

			//Set position using midpoint between corners
			Vector3 roadPosition = new Vector3 ((corner [i].transform.position.x + corner [i + 1].transform.position.x) / 2, 1, (corner [i].transform.position.z + corner [i + 1].transform.position.z) / 2);

			//Initialise road
			var newRoad = Instantiate(road, roadPosition, roadRotation) as GameObject;

			//Set scale of road
			float dist = Mathf.Sqrt(Mathf.Pow(corner[i].transform.localPosition.x - corner[i+1].transform.localPosition.x,2) + Mathf.Pow(corner[i].transform.localPosition.z - corner[i+1].transform.localPosition.z,2)); 
			newRoad.transform.localScale = new Vector3 (roadWidth, roadHeight, dist);

			/*
			Vector3 wallOffset = new Vector3 ((roadWidth + wallWidth) / 2 , 0, 0); //ERROR Have to rotate this

			Vector3 wallPosition = (roadPosition - wallOffset);
			var leftWall = Instantiate (wall, roadPosition - wallOffset+ (Vector3.up * wallHeight / 2), roadRotation) as GameObject;
			leftWall.transform.localScale = new Vector3 (wallWidth, wallHeight, dist - (roadWidth * 2)); 

			var rightWall = Instantiate (wall, roadPosition + wallOffset+ (Vector3.up * wallHeight / 2), roadRotation) as GameObject;
			rightWall.transform.localScale = new Vector3 (wallWidth, wallHeight, dist - (roadWidth * 2)); 
			*/
		}

		//Close off the last corner by attaching it to the first
		{
			//Set rotation
			Vector3 cornerToNextCorner = corner[0].transform.position - corner[corner.Length - 1].transform.position;
			cornerToNextCorner.y = 0.0f;

			Quaternion roadRotation = Quaternion.LookRotation(cornerToNextCorner);

			//Set position using midpoint between corners
			Vector3 roadPosition = new Vector3 ((corner [corner.Length - 1].transform.position.x + corner [0].transform.position.x) / 2, 1, (corner [corner.Length - 1].transform.position.z + corner [0].transform.position.z) / 2);

			//Initialise road
			var newRoad = Instantiate(road, roadPosition, roadRotation) as GameObject;

			//Set scale of road
			float dist = Mathf.Sqrt(Mathf.Pow(corner[corner.Length - 1].transform.localPosition.x - corner[0].transform.localPosition.x,2) + Mathf.Pow(corner[corner.Length - 1].transform.localPosition.z - corner[0].transform.localPosition.z,2)); 
			newRoad.transform.localScale = new Vector3 (roadWidth, roadHeight, dist);
		}

		//Step 4. Add walls to the track edges
		//Do this in the prefab if possible?
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
