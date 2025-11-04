using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{
    [CreateAssetMenu(menuName = "Procedural Generation Method/Simple Room Placement")]
    public class SimpleRoomPlacement : ProceduralGenerationMethod
    {
        [Header("Room Parameters")]
        [SerializeField] private int _maxRooms = 10;
        

        [Header("Room Size")]
        [SerializeField] private Vector2 sizeMin;
        [SerializeField] private Vector2 sizeMax;

        private List<RectInt> RoomList = new List<RectInt>();




        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            // Declare variables here
            // ........
            int _nbRooms = 0;
            RoomList = new List<RectInt>();
            for (int i = 0; i < _maxSteps; i++)
            {
                if (_nbRooms >= _maxRooms)
                {
                    break;
                }
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                int x = RandomService.Range(0, Grid.Width);
                int y = RandomService.Range(0, Grid.Lenght);

                float Width = RandomService.Range(sizeMin.x, sizeMax.x+1);
                float Length = RandomService.Range(sizeMin.y, sizeMax.y+1);

                // Your algorithm here
                RectInt room = new RectInt(x,y, (int)Width, (int)Length);
                if (CanPlaceRoom(room))
                {
                    PlaceRoom(room);
                    RoomList.Add(room);
                    _nbRooms++;
                }

                // Waiting between steps to see the result.
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken : cancellationToken);
            }
            CanPlaceRoads(RoomList);
            // Final ground building.
            BuildGround();
        }
        
        private void PlaceRoom(RectInt room)
        {
            for (int ix = room.xMin; ix < room.xMax; ix++)
            {
                for (int iy = room.yMin; iy < room.yMax; iy++)
                {
                    if(!Grid.TryGetCellByCoordinates(ix, iy, out Cell cell))
                        continue;
                    AddTileToCell(cell, ROOM_TILE_NAME,true);

                }
            }
        }

        private bool CanPlaceRoom(RectInt room)
        {
            for (int ix = room.xMin; ix < room.xMax; ix++)
            {
                for (int iy = room.yMin; iy < room.yMax; iy++)
                {
                    if (!Grid.TryGetCellByCoordinates(ix, iy, out Cell cell))
                        continue;

                    if (cell.ContainObject)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void CanPlaceRoads(List<RectInt> listRoom)
        {

            RectInt ClosestRoom = new RectInt();
            foreach (RectInt room in listRoom)
            {           
                float ClosestDistance = 0;
                foreach (RectInt room2 in listRoom)
                {
                    if (room != room2 && ClosestRoom != room2)
                    {
                        if(ClosestDistance > NormalizeRoomDistance(room, room2) || ClosestDistance == 0)
                        {
                            ClosestDistance = NormalizeRoomDistance(room, room2);
                            ClosestRoom = room2;
                        }
                    }
                }
                PlaceRoads(room, ClosestRoom);
                ClosestRoom = room;
            }
        }


        void PlaceRoads(RectInt room1, RectInt room2)
        {
            Debug.Log(room1.x + "," + room1.y + " / " + room2.x + "," + room2.y);
            Vector2Int start = new Vector2Int(
                Mathf.RoundToInt(room1.center.x),
                Mathf.RoundToInt(room1.center.y)
            );
            Vector2Int end = new Vector2Int(
                Mathf.RoundToInt(room2.center.x),
                Mathf.RoundToInt(room2.center.y)
            );

            // Algorithme de Bresenham pour une ligne discrète
            int dx = Mathf.Abs(end.x - start.x);
            int dy = Mathf.Abs(end.y - start.y);
            int sx = start.x < end.x ? 1 : -1;
            int sy = start.y < end.y ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (Grid.TryGetCellByCoordinates(start.x, start.y, out Cell cell))
                {
                    AddTileToCell(cell, CORRIDOR_TILE_NAME, true);
                }

                if (start.x == end.x && start.y == end.y) break;

                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; start.x += sx; }
                if (e2 < dx) { err += dx; start.y += sy; }
            }
        }

        float NormalizeRoomDistance(RectInt room1, RectInt room2)
        {
            return Mathf.Sqrt(Mathf.Pow((room2.x - room1.x), 2) + Mathf.Pow((room2.y - room1.y), 2));
        }


        private void BuildGround()
        {
            var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass");
            
            // Instantiate ground blocks
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int z = 0; z < Grid.Lenght; z++)
                {
                    if (!Grid.TryGetCellByCoordinates(x, z, out var chosenCell))
                    {
                        Debug.LogError($"Unable to get cell on coordinates : ({x}, {z})");
                        continue;
                    }
                    
                    GridGenerator.AddGridObjectToCell(chosenCell, groundTemplate, false);
                }
            }
        }
    }
}