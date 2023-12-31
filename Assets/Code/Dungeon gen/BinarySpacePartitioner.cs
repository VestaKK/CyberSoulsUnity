using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioner
{
    RoomNode rootNode;

    public RoomNode RootNode { get => rootNode; }
    public BinarySpacePartitioner(int dungeonWidth, int dungeonLength)
    {
        this.rootNode = new RoomNode(
        new Vector2Int(0, 0),
        new Vector2Int(dungeonWidth, dungeonLength),
        null,
        0
        );
    }

    // Partition nodes iteratively
    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);

        int iterations = 0;
        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue();

            // if current node can be split, split into two
            if (currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2)
            {
                SplitSpace(currentNode, listToReturn, roomWidthMin, roomLengthMin, graph);
            }
        }

        return listToReturn;
    }

    // Partition a node
    private void SplitSpace(
        RoomNode currentNode,
        List<RoomNode> listToReturn,
        int roomWidthMin,
        int roomLengthMin,
        Queue<RoomNode> graph)
    {
        Line line = GetLineDividingSpace(
            currentNode.BottomLeftAreaCorner,
            currentNode.TopRightAreaCorner, 
            roomWidthMin, 
            roomLengthMin);

        RoomNode node1, node2;
        if (line.Orientation == Orientation.Horizontal)
        { // Partition nodes horizontally
            node1 = new RoomNode(
                currentNode.BottomLeftAreaCorner, 
                new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y), 
                currentNode, 
                currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(
                new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinates.y), 
                currentNode.TopRightAreaCorner, 
                currentNode, 
                currentNode.TreeLayerIndex + 1);
        }
        else
        { // Partition nodes vertically
            node1 = new RoomNode(
                currentNode.BottomLeftAreaCorner, 
                new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.y), 
                currentNode, 
                currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(
                new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorner.y), 
                currentNode.TopRightAreaCorner, 
                currentNode, 
                currentNode.TreeLayerIndex + 1);
        }


        listToReturn.Add(node1);
        graph.Enqueue(node1);
        listToReturn.Add(node2);
        graph.Enqueue(node2);
    }

    // Get a randomised line divinding a space
    private Line GetLineDividingSpace(
        Vector2Int bottomLeftAreaCorner,
        Vector2Int topRightAreaCorner,
        int roomWidthMin,
        int roomLengthMin)
    {
        Orientation orientation;

        // Checks orientation validity
        bool lengthStatus = (topRightAreaCorner.y - bottomLeftAreaCorner.y) >= 2 * roomLengthMin;
        bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= 2 * roomWidthMin;

        // Choose orientation given it's valid
        if (lengthStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }

        return new Line(
            orientation, 
            GetCoordinatesForOrientation(
                orientation, 
                bottomLeftAreaCorner, 
                topRightAreaCorner, 
                roomWidthMin, 
                roomLengthMin));
    }

    // Given an orientation, get a randomised coordinate for a line
    private Vector2Int GetCoordinatesForOrientation(
        Orientation orientation,
        Vector2Int bottomLeftAreaCorner, 
        Vector2Int topRightAreaCorner, 
        int roomWidthMin, 
        int roomLengthMin)
    {
        Vector2Int coordinates = Vector2Int.zero;
        if (orientation == Orientation.Horizontal)
        {
            coordinates = new Vector2Int(
                0, 
                Random.Range(
                    bottomLeftAreaCorner.y + roomLengthMin, 
                    topRightAreaCorner.y - roomLengthMin));
        }
        else
        {
            coordinates = new Vector2Int(
                Random.Range(
                    bottomLeftAreaCorner.x + roomWidthMin, 
                    topRightAreaCorner.x - roomWidthMin), 0);
        }

        return coordinates;
    }
}