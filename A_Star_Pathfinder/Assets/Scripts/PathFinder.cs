using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _searcher;
    [SerializeField] bool _isStart; // dont move the player until path si found

    [SerializeField] float _speed;
    [SerializeField] int _currentPathNode; // starts from runnerpath 0 to end. it stores the current runnerpath node.
    List<Node> _runnerPath; // stores all the nodes that have been found by the FindPath function.
    Grids _grid;

    private void Start()
    {
        _isStart = false;
        _runnerPath = new List<Node>();
        _grid = GetComponent<Grids>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _runnerPath.Clear();
            _currentPathNode = 0;

            _target.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            FindPath(_searcher.position, _target.position);
            _isStart = true;
            
            print("Steps to goal = " + _runnerPath.Count);
        }

        if (_isStart)
            MovePlayer(_runnerPath, _searcher, ref _currentPathNode, _speed);
    }

    void MovePlayer(List<Node> paths, Transform searcher, ref int pathNumber, float speed)
    {
        if (paths.Count == pathNumber)
        {
            paths.Clear();
            pathNumber = 0;
            _isStart = false;
            return;
        }

        Vector2 moveHere = Vector2.MoveTowards(searcher.position, paths[pathNumber].WorldPosition, speed * Time.deltaTime);

        searcher.position = new Vector2(moveHere.x, moveHere.y);

        if (searcher.position == new Vector3(paths[pathNumber].WorldPosition.x, paths[pathNumber].WorldPosition.y))
        {
            if (paths.Count > pathNumber)
            {
                pathNumber++;
            }
        }
    }

    void FindPath(Vector2 start, Vector2 end)
    {
        Stopwatch tookTime = new Stopwatch();
        tookTime.Start();

        Node startNode = _grid.NodeFromWorldPoint(start);
        Node endNode = _grid.NodeFromWorldPoint(end);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            // ************************* Optimize This *********************************

            // an expensive part of algorithm.
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                {
                    if (openSet[i].HCost < currentNode.HCost)
                        currentNode = openSet[i];
                }
            }

            // when we reach here then we have the node with the best possible path of previous iterations
            // and that node is currentNode. since we have traversed all the neighbours of currentNode in previous
            // iteration we mark this as traversed.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                tookTime.Stop();
                print("Found in " + tookTime.ElapsedMilliseconds + " ms");
                ReconstructPath(startNode, endNode);
                return;
            }

            foreach (Node neighbour in _grid.GetNeighbours(currentNode))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    continue;

                // for gCost we can say currentNode contains its own gCost then currentNode will get the neighbour`s
                // gCost too so adding the previous gCost with the new gCost of the neighbour will give us the
                // neighbour`s cost from the starting node.
                // * we can also say that the neighbour is +10/14 than the currentNode which gives us gCost.
                int costToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                if (costToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = costToNeighbour;
                    neighbour.HCost = GetDistance(neighbour,endNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    int GetDistance(Node current,Node neighbourNode)
    {
        int distanceX = Mathf.Abs(current.GridX - neighbourNode.GridX);
        int distanceY = Mathf.Abs(current.GridY - neighbourNode.GridY);

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
            _runnerPath.Add(currentNode);
            // by getting parent of the currentNode we track the path to endNode and then reverse it.
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        _runnerPath.Reverse();
        _grid.path = path;
    }
}
