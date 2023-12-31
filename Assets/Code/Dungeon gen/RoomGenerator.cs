using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxIterations;
    private DungeonController _currentDungeon;

    public RoomGenerator(DungeonController currentDungeon, int maxIterations)
    {
        this.maxIterations = maxIterations;
        this._currentDungeon = currentDungeon;
    }

    // Given a space, generate a room under the parameter constraints (bottomCorner Modifier, topCornerModifier & offset)
    public List<RoomNode> GenerateRoomsInGivenSpaces(
        List<Node> roomSpaces, 
        float bottomCornerModifier, 
        float topCornerModifier, 
        int offset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();

        foreach (var space in roomSpaces)
        {
            // Get a random coordinate for a room within the space
            Vector2Int newBottomLeft = 
                StructureHelper.GenerateBottomLeftCornerBetween(
                    space.BottomLeftAreaCorner, 
                    space.TopRightAreaCorner, 
                    bottomCornerModifier, 
                    offset);
            Vector2Int newTopRight = 
                StructureHelper.GenerateTopRightCornerBetween(
                    space.BottomLeftAreaCorner, 
                    space.TopRightAreaCorner, 
                    topCornerModifier, 
                    offset);

            space.BottomLeftAreaCorner = newBottomLeft;
            space.TopRightAreaCorner = newTopRight;
            space.BottomRightAreaCorner = new Vector2Int(newTopRight.x, newBottomLeft.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeft.x, newTopRight.y);
            
            RoomNode room = (RoomNode) space;
            room.GenerateWalls();
            room.MiddlePoint = StructureHelper.CalculateMiddlePoint(
                        room.BottomLeftAreaCorner, 
                        room.TopRightAreaCorner);
            room.DungeonController = _currentDungeon;
            listToReturn.Add(room);
        }

        return listToReturn;
    }
}