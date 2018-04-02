using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *	Author: Tyler Arseneault
 *	Lab 3: A* Path Finding
 *	Node
 *	Stores information regarding a node connected by edges
 */
public class Node
{
    public Vector3 position { get; set; } //position of the node
    public string name { get; set; } //name of the node (used to compare, or name the gameobject)
    public List<Edge> edges { get; set; } //a list of edges connected to the node
    public Node from; //used for path finding. The lowest "cost" node used get to the current node
    public string fromEdge; //name of the edge used to get to the current node
    public float cost { get; set; } //cost of the path to get to this node

    /*
     *  Node
     *  Constructor
     *  Param: string name, floats x coordinate, y coordinate, z coordinate
     */
    public Node(string name, float x, float y, float z)
    {
        this.name = name;
        this.position = new Vector3(x, y, z);
        this.edges = new List<Edge>();
        this.cost = Mathf.Infinity; //start with the highest possible cost
    }
}