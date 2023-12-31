using UnityEngine;

using System.Collections.Generic;
public class RoomNode : Node
{
    public Vector2 SpawnPoint { get; set; }
    public Vector2 ExitPoint { get; set; }

    public Vector2 MiddlePoint { get; set; }

    public List<(CorridorNode,RoomNode)> ConnectedNodes { get; set; }
    public List<Door> Doors { get; set; }
    public List<Prop> Props { get; set; }

    // public List<Entity> Entities { get; set; }

    public RoomType Type { get; set; }

    // Enemy stuff
    public int EnemyCount { get; set; }
    public float _roomDifficulty = 0f;

    private DungeonController _currentDungeon;

    public RoomNode(
        Vector2Int bottomLeftAreaCorner, 
        Vector2Int topRightAreaCorner, 
        Node parentNode, 
        int index) : base(parentNode)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);

        this.TreeLayerIndex = index;

        ConnectedNodes = new List<(CorridorNode, RoomNode)>();
        Props = new List<Prop>();
        Doors = new List<Door>();
    }

    public void SpawnEnemies<T>(Spawner<T> spawner) where T : Enemy {
        int spawns = 0;
        int targetSpawns = spawner.GetSpawnCount();
        int maxDepth = spawner.maxSpawnCheckCount;
        Vector3 base_spawn = new Vector3(MiddlePoint.x, 0, MiddlePoint.y);
        Debug.Log("Spawning enemies for " + this);
        while(spawns++ < targetSpawns) {
            int rand = Random.Range(0, spawner.Prefabs.Count);
            T prefab = spawner.Prefabs[rand];

            bool safeSpawn;
            Vector3 spawnPoint = GetSafeSpawn(base_spawn, spawner.GetController(prefab), maxDepth, out safeSpawn);
            if(safeSpawn) {
                Enemy e = spawner.SpawnEntity(DungeonController.transform, spawnPoint, Quaternion.identity, prefab);
                e.CurrentDungeon = DungeonController;

                // maintain room relationship
                e.CurrentRoom = this;
            }
            // Entities.Add(e);
        }
        Debug.Log("Finished Spawning");
        // return Entities;
    }

    public bool EntityInBounds(Entity e) {
        Vector2 vec2d = new Vector2(e.Position.x, e.Position.z);

        return SpawnBounds.Contains(vec2d, true);
    }

    private Vector3 GetSafeSpawn(Vector3 base_spawn, CharacterController prefab, int maxDepth, out bool isSafe) {
        Vector3 randSpawn = GetRandSpawn(base_spawn, prefab);
        Collider[] collided = Physics.OverlapSphere(randSpawn, prefab.radius);
        int depthCount = 0;
        while(collided.Length != 0) {
            randSpawn = GetRandSpawn(base_spawn, prefab);
            collided = Physics.OverlapSphere(randSpawn, prefab.radius); //Physics.OverlapCapsule(randSPawn);//
            depthCount++;
            if(depthCount >= maxDepth) {
                isSafe = false;
                Debug.Log(randSpawn + " was not safe for prefab: " + prefab);
                return Vector3.zero;
            }

        }
        isSafe = true;
        return randSpawn;
    }

    private Vector3 GetRandSpawn(Vector3 base_spawn, CharacterController prefab) {
        float randX = Random.Range(-SpawnBounds.width / 2, SpawnBounds.width / 2);
        float randZ = Random.Range(-SpawnBounds.height / 2, SpawnBounds.height / 2);
        Debug.Log("Spawning at: " + (prefab.center.y + prefab.height + 0.25f));
        Debug.Log(prefab);
        Debug.Log("Height: " + prefab.height);
        Debug.Log("Center: " + prefab.center.y);
        return new Vector3(base_spawn.x + randX, prefab.center.y + 0.25f, base_spawn.z + randZ); // raise it off the floor
    }

    public int Width { get => (int) (TopRightAreaCorner.x - BottomLeftAreaCorner.x); }
    public int Length { get => (int) (TopRightAreaCorner.y - BottomLeftAreaCorner.y); }

    public DungeonController DungeonController { get => _currentDungeon; set => _currentDungeon = value; }
}

public enum RoomType 
{
    Crossway, // TODO, maybe deprecate
    Hallway,
    BeegRoom,
    StorageRoom,
    LabRoom,
    ComputerRoom,
    Library,
    ChallengeRoom, // TODO
    SpawnRoom,
    ExitRoom
}