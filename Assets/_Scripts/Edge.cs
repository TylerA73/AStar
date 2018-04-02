using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *	Author: Tyler Arseneault
 *	Lab 3: A* Path Finding
 *	Edge
 *	Stores all of the information regarding an edge connecting two nodes
 */
public class Edge{
    public float cost { get; set; } //cost of that edge (used in cost calculation)
    public string start { get; set; } //the name of the node where the edge begins
    public string end { get; set; } //the name of the node where the edge ends
    public string name { get; set; } //name of the edge
    public Node startNode { get; set; } 
    public Node endNode { get; set; }

    /*
     *  Edge
     *  Constructor
     *  Param: float cost, strings start, end
     */
    public Edge(float cost, string start, string end){
        this.cost = cost; //set the cost to traverse this edge
        this.start = start; //name of the starting node connected by this edge
        this.end = end; //name of the ending node connected by this edge
        this.name = start + end; //combine the nodes connected to form the name
        this.startNode = null;  //set the starting node to null first
        this.endNode = null;    //set the ending node to null first
    }
}
