using UnityEngine;
using System.Collections;

public class NextCorner : MonoBehaviour {

	GameObject m_nextCorner;

	void SetNextCorner(GameObject nextCorner)
	{
		m_nextCorner = nextCorner;
	}

	GameObject GetNextCorner()
	{
		return m_nextCorner;
	}
}
