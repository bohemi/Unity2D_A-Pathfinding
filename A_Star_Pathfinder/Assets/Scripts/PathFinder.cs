using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Transform findMe;
    [SerializeField] Transform finder;

    Grids grid;

    private void Start()
    {
        grid = GetComponent<Grids>();
    }

    private void Update()
    {
        FindPath(finder.position, findMe.position);
    }

    void FindPath(Vector2 start, Vector2 end)
    {
        Node startNode = grid.NodeFromWorldPoint(start);
        Node endNode = grid.NodeFromWorldPoint(end);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                {
                    if (openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];
                }
            }
            // when we reach here then we have the prefect node with the best possible path of previous iterations
            // and that node is currentNode. since we have traversed all the neighbours of currentNode in previous
            // iteration we mark this as traversed.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                ReconstructPath(startNode, endNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                // for gCost we can say currentNode contains its own gCost then currentNode will get the neighbour`s
                // gCost too so adding the previous gCost with the new gCost of the neighbour will give us the
                // neighbour`s cost from the starting node.
                int costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetDistance(neighbour,endNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    int GetDistance(Node current,Node neighbourNode)
    {
        int distanceX = Mathf.Abs(current.gridX - neighbourNode.gridX);
        int distanceY = Mathf.Abs(current.gridY - neighbourNode.gridY);

        if (distanceX > distanceY)
        { return 14 * distanceY + 10 * (distanceX - distanceY); }

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    void ReconstructPath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            // by getting parent of the currentNode we track the path to endNode and then reversing it.
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }

}
