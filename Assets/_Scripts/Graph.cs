using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

/*
 *	Author: Tyler Arseneault
 *	Lab 3: A* Path Finding
 *	Graph
 *	Takes in an XML file containing information regarding the nodes, their edges, and
 *	a path. It will place the nodes and edges into their proper positions.
 */
public class Graph : MonoBehaviour {

	public TextAsset xml; //xml file to be parsed
	public List<Node> nodes; //List of the nodes in the graph
	public List<Edge> edges; //list of the edges in the graph
	public Path path; //the path containing information regarding the starting and ending nodes
	public List<Node> shortestPath; //eventually contains the shortest possible path from the start to end

	/*
	 *	Initializes the variables, and sets up the graph as needed
	 */
	void Start () {
		nodes = new List<Node>();
		edges = new List<Edge>();
		string data = xml.text; //get the text of the xml file
		Parser(data); //parse the text
		
		//Place the nodes in their positions
		//Set up the start and end nodes in the path object
		foreach(Node node in nodes){
			PlaceNode(node);
			if(node.name == path.start){
				path.startNode = node;
			}else if(node.name == path.end){
				path.endNode = node;
			}
		}

		//Place the edges where they need to be
		//Add the edges to the correct nodes
		foreach(Edge edge in edges){
			PlaceEdge(edge);
			foreach(Node node in nodes){
				if(node.name == edge.start){
					node.edges.Add(edge);
					edge.startNode = node;
				}else if(node.name == edge.end){
					node.edges.Add(edge);
					edge.endNode = node;
				}
			}
		}

		//Calculate the shortest path
		shortestPath = FindPath();

		//Run through the shortest path, and change the colour of the nodes and edges used to traverse it
		foreach(Node node in shortestPath){
			string nodeName = node.name;
			string edgeName = node.fromEdge;
			GameObject sphere = GameObject.Find(nodeName);
			GameObject cylinder = GameObject.Find(edgeName);
			sphere.GetComponent<Renderer>().material.color = Color.blue;
			if(!node.Equals(path.startNode))
				cylinder.GetComponent<Renderer>().material.color = Color.blue;
		}
	}
	
	/*
	 *	Parser
	 *	Param: string data
	 *	Returns: N/A
	 *	Parses the xml data, creating Objects for them
	 */
	void Parser(string data){
		XmlDocument xmlDoc = new XmlDocument(); 
		xmlDoc.LoadXml(data);

		//For each Node node inside of the Graph node, parse its paramesters, and use
		//them to create a new Node Object
		foreach(XmlElement element in xmlDoc.SelectNodes("Graph/Node")){
			string name = element.GetAttribute("Name");
			float x = float.Parse(element.GetAttribute("X"));
			float y = float.Parse(element.GetAttribute("Y"));
			float z = float.Parse(element.GetAttribute("Z"));

			nodes.Add(new Node(name, x, y, z));
		}

		//For each Edge node inside of the Graph node, parse its paramesters, and use
		//them to create a new Edge Object
		foreach(XmlElement element in xmlDoc.SelectNodes("Graph/Edge")){
			string start = element.GetAttribute("Start");
			string end = element.GetAttribute("End");
			float cost = float.Parse(element.GetAttribute("Cost"));

			edges.Add(new Edge(cost, start, end));
		}

		//For each Path node inside of the Graph node, parse its paramesters, and use
		//them to create a new Path Object
		foreach(XmlElement element in xmlDoc.SelectNodes("Graph/Path")){
			string s = element.GetAttribute("Start");
			string e = element.GetAttribute("End");

			path = new Path(s, e);
		}
	}

	/*
	 *	PlaceNode
	 *	Param: Node node
	 *	Return: N/A
	 *	Creates a sphere GameObject for the node, names it, and then places it in the scene using
	 *	the parameters of node
	 */
	void PlaceNode(Node node){
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = node.position;
		sphere.name = node.name;
		
		sphere.GetComponent<Renderer>().material.color = Color.red; //make it red
	}

	/*
	 *	PlaceEdge
	 *	Param: Edge edge
	 *	Return: N/A
	 *	Creates a cylinder GameObject for the edge, names it, and then places it in the scene using
	 *	the parameters of edge
	 */
	void PlaceEdge(Edge edge){
		Vector3 start = GameObject.Find(edge.start).transform.position; //position of the start node
		Vector3 end = GameObject.Find(edge.end).transform.position; //position of the end node

		GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cylinder.transform.position = (end - start)/2.0f + start;

		// Scale the cylinder so that it stretches across from the starting node to the ending node
		// Also make it appear skinny, so as not to hide the node
		Vector3 scale = transform.localScale;
		scale.y =(end - start).magnitude/2.0f;
		scale.x = 0.25f;
		scale.z = 0.25f;
		cylinder.transform.localScale = scale; //set the cylinder's scale

		cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start); //rotate the cylinder to face the nodes
		cylinder.GetComponent<Renderer>().material.color = Color.red; //make it red

		cylinder.name = edge.name;
	}

	/*
	 *	CalculateHeuristic
	 *	Param: Vector3 curr, Vector3 goal
	 *	Return: float distance
	 *	Calculates the magnitude of the distance between the current node and the goal node
	 */
	float CalculateHeuristic(Vector3 curr, Vector3 goal){
		return Mathf.Abs(curr.magnitude - goal.magnitude);
	}

	/*
	 *	FindPath
	 *	Param: N/A
	 *	Return: List<Node> thePath
	 *	Calculates the shortest path from the starting node to the ending node in the path object
	 */
	List<Node> FindPath(){
		Node start = path.startNode; //starting node
		Node goal = path.endNode; //ending node
		
		List<Node> visited = new List<Node>(); //nodes that have been visited once before
		List<Node> pq = new List<Node>(); //priority queue
		List<Node> thePath = new List<Node>(); //the shortest path
		Node current = null; //the current node we are at
		Node backNode = null;
		float dist = 0f; //the cost of the path
		pq.Add(start); //start by addign the first node to the priority queue

		// we are currently at the starting node
		current = start;
		current.cost = 0f; // we have a cost of 0 to get from the start to the start

		while(pq.Count != 0){
			// we need to check the first element of the priority queue
			current = pq[0];

			// for every edge connected to the current node, we need to find the best path
			foreach(Edge edge in current.edges){
				// the next node to be checked
				Node next = edge.startNode;

				// if there is no where to go from the current node, then go back to the last one
				// make sure to mark the dead end as visited
				if(next.Equals(current)){
					visited.Add(next);
					next = edge.endNode;
				}

				// if we have visited the next node, then we don't want to revisit
				if(visited.Contains(next)){
					continue;
				}

				// calculate the distance from the current node to the final node
				dist = current.cost + edge.cost + CalculateHeuristic(current.position, goal.position);

				// if the distance is greater then the cost of the next node, then move on
				// otherwise, set the cost of the next node, and set the from node to the current node
				// so that we can backtrack later
				// add the next node to the priority queue
				if(dist >= next.cost){
					continue;
				}else{
					next.cost = dist;
					next.from = current;
					next.fromEdge = edge.name;
					pq.Add(next);
				}
			}

			// add the current node to the visited list
			visited.Add(current);

			// remove the current node from the priority queue
			pq.Remove(current);
		}
		
		// start from the goal
		// work backwards from it to add the trailing nodes to the shortest path
		backNode = goal;
		while(backNode != start){
			thePath.Add(backNode);
			backNode = backNode.from;
		}

		// add the starting node to the path as well
		thePath.Add(start);
		

		return thePath;
	}
}
