using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *	Author: Tyler Arseneault
 *	Lab 3: A* Path Finding
 *	Path
 *	Stores information regarding the starting node and ending node
 *	This is used to determine a path.
 */
public class Path{

	public string start { get; set; }
	public string end { get; set; }
	public Node startNode { get; set; }
	public Node endNode { get; set; }

	/*
	 *	Path
	 *	Constructor
	 *	Param: string starting node, string ending node
	 */
	public Path(string s, string e){
		this.start = s;
		this.end = e;
	}

}
