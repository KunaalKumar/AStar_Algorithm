using System;
using System.Collections.Generic;

public class Program
{
    public class Node {
        public String name { get; }
        public int xCord { get; }
        public int yCord { get; }
        public Node[] links;
        private int linkCount = 0;

        public Node(String name, int xCord, int yCord, int linkNumber) {
            this.name = name;
            this.xCord = xCord;
            this.yCord = yCord;
            links = new Node[linkNumber];
        }

//        Link to given node
        public void linkTo(Node nodeToLink) {
            if (isLinked(nodeToLink)) {

            }
            else {
                links[linkCount] = nodeToLink;
                linkCount++;
                nodeToLink.linkTo(this);
            }
        }

//      Check if given Node is linked
        public bool isLinked(Node nodeToTest) {
            for (int i = 0; i < linkCount; i++) {
                if (links[i] == nodeToTest) {
                    return true;
                }
            }
            return false;
        }
    }

    public class AStarNodes {
        public float costSoFar { get; }
        public float heuristic { get; }
        public float tec => costSoFar + heuristic;
        public AStarNodes cameFrom { get;  }
        public Node nodeVal { get; }

        public AStarNodes(Node currentNode, Node endNode, float costSoFar = 0, AStarNodes cameFrom = null) {
            this.costSoFar = costSoFar;
            heuristic = GetDistance(currentNode, endNode);
            nodeVal = currentNode;
            this.cameFrom = cameFrom;
        }

//        Get distance between two points
        private static float GetDistance(Node node1, Node node2) {
            return (float) Math.Sqrt(Math.Pow((node2.xCord - node1.xCord), 2) + Math.Pow((node2.yCord - node1.yCord), 2));
        }
    }

    private static Node[] nodeLists = new Node[16];

    public static void Main(String[] args) {
        InitializeNodes();
        Console.WriteLine("Enter start point (as a capital letter)");
        Node startNode = GetNodeFromName(Convert.ToString(Console.ReadLine()));
        Console.WriteLine("Enter end point (as a capital letter)");
        Node endNode = GetNodeFromName(Convert.ToString(Console.ReadLine()));
        Console.WriteLine("The most optimal route is " + EvaluateNode(startNode, endNode));
    }

    private static String EvaluateNode(Node startNode, Node endNode) {
        List<AStarNodes> open = new List<AStarNodes>();
        List<AStarNodes> closed = new List<AStarNodes>();
        Node currNode = startNode;
        float costSoFar;
        AStarNodes currANode = new AStarNodes(currNode, endNode);
        open.Add(currANode);
        for(int i = 0; i < open.Count; i++ ) {
            currNode = GetLowestTecNode(open);
            currANode = GetLowestTecAStarNode(open);
            open.Remove(currANode);
            costSoFar = currANode.costSoFar;
            if (currNode.name.Equals(endNode.name)) {
                break;
            }
            for (int j = 0; j < currNode.links.Length; j++) {
                AStarNodes temp = new AStarNodes(currNode.links[j], endNode, costSoFar + GetDistance(currNode, currNode.links[j]),
                    currANode);

                if (AnotherExists(temp, open) != -1) {
                    if (temp.costSoFar < open[AnotherExists(temp, open)].costSoFar) {
                        open.RemoveAt(AnotherExists(temp, open));
                        open.Add(temp);
                    }
                }
                if (AnotherExists(temp, closed) != -1) {
                        if (temp.costSoFar < closed[AnotherExists(temp, closed)].costSoFar) {
                            closed.RemoveAt(AnotherExists(temp, closed));
                            open.Add(temp);
                    }
                }
                else {
                    open.Add(temp);
                }
            }
            closed.Add(open[i]);
        }

        Stack<String> path = new Stack<string>();

        while (currANode.cameFrom != null) {
            path.Push(currANode.nodeVal.name);
            currANode = currANode.cameFrom;
        }
        path.Push(currANode.nodeVal.name);

        String finalPath = "";
        foreach (String letter in path) {
            finalPath += letter + ">";
        }
        return finalPath.Substring(0, finalPath.Length - 1);
    }

//    Check is another node of the same name exists in the given list
    private static int AnotherExists(AStarNodes temp, List<AStarNodes> open) {
        for (int i = 0; i < open.Count; i++) {
            if (temp.nodeVal.name.Equals(open[i].nodeVal.name)) {
                return i;
            }
        }
        return -1;
    }

//    Get the Node with the lowest total estimated cost
    private static Node GetLowestTecNode(List<AStarNodes> open) {
        float lowest = float.NaN;
        Node smallestNode = null;
        for (int i = 0; i < open.Count; i++) {
            if (float.IsNaN(lowest)) {
                lowest = open[i].tec;
                smallestNode = open[i].nodeVal;
            }
            else if (open[i].tec < lowest) {
                lowest = open[i].tec;
                smallestNode = open[i].nodeVal;
            }
        }
        return smallestNode;
    }

//    Get the lowest AStar Node with the lowest total estimetad cost
    private static AStarNodes GetLowestTecAStarNode(List<AStarNodes> open) {
        float lowest = float.NaN;
        AStarNodes smallestNode = null;
        for (int i = 0; i < open.Count; i++) {
            if (float.IsNaN(lowest)) {
                lowest = open[i].tec;
                smallestNode = open[i];
            }
            else if (open[i].tec < lowest) {
                lowest = open[i].tec;
                smallestNode = open[i];
            }
        }
        return smallestNode;
    }

    //        Get distance between two points
    private static float GetDistance(Node node1, Node node2) {
        return (float) Math.Sqrt(Math.Pow((node2.xCord - node1.xCord), 2) + Math.Pow((node2.yCord - node1.yCord), 2));
    }

//    Return node corresponding  with the given name
    private static Node GetNodeFromName(String nodeName) {
        for (int i = 0; i < nodeLists.Length; i++) {
            if (nodeLists[i].name.Equals(nodeName)) {
                return nodeLists[i];
            }
        }
        return null;
    }

    private static void InitializeNodes() {
//        Add the new Nodes to the list
        nodeLists[0] = new Node("A", -19, 11, 2);
        nodeLists[1] = new Node("B", -13, 13, 2);
        nodeLists[2] = new Node("C", 4, 14, 3);
        nodeLists[3] = new Node("D", -4, 12, 3);
        nodeLists[4] = new Node("E", -8, 3, 7);
        nodeLists[5] = new Node("F", -18, 1, 2);
        nodeLists[6] = new Node("G", -12, -8, 3);
        nodeLists[7] = new Node("H", 12, -9, 3);
        nodeLists[8] = new Node("I", -18, -11, 2);
        nodeLists[9] = new Node("J", -4, -11, 5);
        nodeLists[10] = new Node("K", -12, -14, 3);
        nodeLists[11] = new Node("L", 2, -18, 3);
        nodeLists[12] = new Node("M", 18, -13, 3);
        nodeLists[13] = new Node("N", 4, -9, 3);
        nodeLists[14] = new Node("O", 22, 11, 2);
        nodeLists[15] = new Node("P", 18, 3, 4);

//        Link the new Nodes
        nodeLists[0].linkTo(nodeLists[1]);
        nodeLists[0].linkTo(nodeLists[4]);
        nodeLists[1].linkTo(nodeLists[3]);
        nodeLists[2].linkTo(nodeLists[3]);
        nodeLists[2].linkTo(nodeLists[4]);
        nodeLists[2].linkTo(nodeLists[15]);
        nodeLists[3].linkTo(nodeLists[4]);
        nodeLists[4].linkTo(nodeLists[6]);
        nodeLists[4].linkTo(nodeLists[9]);
        nodeLists[4].linkTo(nodeLists[13]);
        nodeLists[4].linkTo(nodeLists[7]);
        nodeLists[5].linkTo(nodeLists[6]);
        nodeLists[5].linkTo(nodeLists[8]);
        nodeLists[6].linkTo(nodeLists[9]);
        nodeLists[7].linkTo(nodeLists[13]);
        nodeLists[7].linkTo(nodeLists[15]);
        nodeLists[8].linkTo(nodeLists[10]);
        nodeLists[9].linkTo(nodeLists[13]);
        nodeLists[9].linkTo(nodeLists[10]);
        nodeLists[9].linkTo(nodeLists[11]);
        nodeLists[10].linkTo(nodeLists[11]);
        nodeLists[11].linkTo(nodeLists[12]);
        nodeLists[12].linkTo(nodeLists[15]);
        nodeLists[12].linkTo(nodeLists[14]);
        nodeLists[14].linkTo(nodeLists[15]);

    }
}
