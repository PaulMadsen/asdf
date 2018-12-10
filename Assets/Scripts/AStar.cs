using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS4470Puzzle8;
using System;
using UnityEngine.UI;

public class AStar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //strait line distance between two vectors;
    public static float Heuristic(Vec3 a, Vec3 b)
    {
        return Mathf.Sqrt(
            Mathf.Pow(a.theVector.x - b.theVector.x, 2) +
            Mathf.Pow(b.theVector.y - b.theVector.y, 2) +
            Mathf.Pow(a.theVector.z - b.theVector.z, 2) );
    }
    static public List<Vec3> GetNeihbors(Vec3 node)
    {
        List<Vec3> retVal = new List<Vec3>();
        Vector3 floor = new Vector3(0, -1, 0);
        Vector3 chest = new Vector3(0, 1, 0);

        Vector3 west = new Vector3(node.theVector.x - 1, node.theVector.y, node.theVector.z);
        if (World.GetBlock(west) == 0)//knees
            if (World.GetBlock(west + floor) != 0) 
                if (World.GetBlock(west + chest) == 0)
                    retVal.Add(new Vec3(west));
        Vector3 east = new Vector3(node.theVector.x + 1, node.theVector.y, node.theVector.z);
        if (World.GetBlock(east) == 0)
            if (World.GetBlock(east + floor) != 0)
                if (World.GetBlock(east + chest) == 0)
                    retVal.Add(new Vec3(east));
        Vector3 north = new Vector3(node.theVector.x, node.theVector.y, node.theVector.z+1);
        if (World.GetBlock(north) == 0)
            if (World.GetBlock(north + floor) != 0)
                if (World.GetBlock(north + chest) == 0)
                    retVal.Add(new Vec3(north));
        Vector3 south = new Vector3(node.theVector.x, node.theVector.y, node.theVector.z-1);
        if (World.GetBlock(south) == 0)
            if (World.GetBlock(south + floor) != 0)
                if (World.GetBlock(south + chest) == 0)
                    retVal.Add(new Vec3(south));
        return retVal;

    }
    public static List<Vector3> ReconstructPath(Dictionary<Vec3, Vec3> cameFrom, Vec3 current)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(new Vector3(current.theVector.x, current.theVector.y, current.theVector.z));
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(new Vector3(current.theVector.x, current.theVector.y, current.theVector.z));
        }
        return path;
    }
    public static List<Vector3> A_Star(Vector3 _start, Vector3 _end, Canvas debugCanvas = null)
    {        
        //wrappers
        Vec3 start = new Vec3(_start, 0);        
        Vec3 end = new Vec3(_end);        

        HashSet<Vec3> closedSet = new HashSet<Vec3>();
        PriorityQueue<Vec3> openSet = new PriorityQueue<Vec3>();
        openSet.Enqueue(start);
        Dictionary<Vec3, Vec3> cameFrom = new Dictionary<Vec3, Vec3>();
        Dictionary<Vec3, int> gScore = new Dictionary<Vec3, int>();

        gScore.Add(start, 0);
        //Dictionary<Vec3, int> fScore = new Dictionary<Vec3, int>();
        //fScore.Add(start, (int)Heuristic(start, end));

        Vec3 current;
        int emergencyStopCount = 0;
        int stopCondition = 1000;
        while (openSet.Count() != 0 && emergencyStopCount++ < stopCondition)
        {
            current = openSet.Dequeue();
            /*GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.GetComponent<MeshRenderer>().material.color = Color.red;            
            go.transform.position = current.theVector + new Vector3(0.5f, 0.5f, 0.5f);
            go.transform.localScale *= .4f;
            
            Destroy(go, 2);*/

            if (current.theVector == end.theVector)
                return ReconstructPath(cameFrom, current); 

            closedSet.Add(current);
            List<Vec3> neighbors = GetNeihbors(current);
            foreach (Vec3 neighbor in neighbors)
            {                
                if (closedSet.Contains(neighbor)) continue;
                float tentative_gScore = gScore[current] + 1; //uniform cost
                bool doEnqueue = false;
                if (!openSet.Contains(neighbor))
                    doEnqueue = true;
                else if (tentative_gScore >= gScore[neighbor]) continue;

                if (!cameFrom.ContainsKey(neighbor))
                    cameFrom.Add(neighbor, current);
                gScore[neighbor] = (int)tentative_gScore;        
                neighbor.value = gScore[neighbor] + (int)Heuristic(neighbor, end);
                if (doEnqueue) //enqueue after the value is set so the underlying heap sort can work off of the value
                    openSet.Enqueue(neighbor);
            }

        }        
        return null;    
    }

}

/// <summary>
/// Wrapper for Vector3 that can use IComparable
/// </summary>
public class Vec3 : IComparable<Vec3>, IEquatable<Vec3>
{
    public Vector3 theVector;
    public float value = 0;
    public Vec3(Vector3 vec, float value = int.MaxValue)
    {
        theVector = new Vector3(vec.x, vec.y, vec.z);
        this.value = value;
    }
    public int CompareTo(Vec3 other)
    {
        return (int)(this.value - other.value);
    }

    public bool Equals(Vec3 other)
    {
        //return this.theVector == other.theVector;
        return this.theVector.x == other.theVector.x &&
            this.theVector.y == other.theVector.y &&
            this.theVector.z == other.theVector.z;
    }
    public override int GetHashCode()
    {        
        int sofar = 0;
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
                sofar += (int)((theVector.x + 1) * 7 + (theVector.y + 1) * 11 + (theVector.z + 1) * 13);
        return sofar;
    }
}
