using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using VTools.Grid;


[CreateAssetMenu(menuName = "Procedural Generation Method/BSP")]
public class BSPScript : ProceduralGenerationMethod
{
    [SerializeField] private Vector2Int sizeMin;
    [SerializeField] private Vector2Int sizeMax;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        Debug.Log("Test Algo");
        RectInt allGrid = new RectInt(0, 0, Grid.Width, Grid.Lenght);
        BSPNode root = new BSPNode(allGrid);
        root._position = new Vector2Int(0, 0);
        int NbOfCuts = 2;        
        BSPNode ParentChilds = root;
        List<BSPNode> ChildToCut = new List<BSPNode>();
        ChildToCut.Add(root);
        for (int i = 0; i< NbOfCuts; i++)
        {
            List<BSPNode> newChildren = new List<BSPNode>();
            foreach (BSPNode child in ChildToCut)
            {
                CreateChildOfNode(child);
                newChildren.Add(child._child1);
                newChildren.Add(child._child2);
                //int Width = RandomService.Range(sizeMin.x, sizeMax.x + 1);
                //int Length = RandomService.Range(sizeMin.y, sizeMax.y + 1);
                //PlaceARoomInThisArea(child._bounds, Width, Length);
                PlaceRoom(child._bounds);
            }
            ChildToCut.Clear();
            ChildToCut.AddRange(newChildren);
            //if(i+1 >= NbOfCuts)
            //{
            //    foreach (BSPNode child in ChildToCut)
            //    {
            //        int Width = RandomService.Range(sizeMin.x, sizeMax.x + 1);
            //        int Length = RandomService.Range(sizeMin.y, sizeMax.y + 1);
            //        PlaceARoomInThisArea(child._bounds, Width, Length);
            //    }
            //}
        }

        Debug.Log("root child 1 bounds = " + root._child1._bounds);
        Debug.Log("root child 2 bounds = " + root._child2._bounds);

        Debug.Log("C1 child 1 bounds = " + root._child1._child1._bounds);
        Debug.Log("C1 child 2 bounds = " + root._child1._child2._bounds);

        Debug.Log("C2 child 1 bounds = " + root._child2._child1._bounds);
        Debug.Log("C2 child 2 bounds = " + root._child2._child2._bounds);
    }

    void CreateChildOfNode(BSPNode node)
    {
        int FirstChildWidth = RandomService.Range(0, node._bounds.width);
        int FirstChildLength = RandomService.Range(0, node._bounds.height);
        int SecondChildWidth = node._bounds.width - FirstChildWidth;
        int SecondChildLength = node._bounds.height - FirstChildLength;

        int RandomCut1 = RandomService.Range(0, 2);
        int RandomCut2 = RandomService.Range(0, 2);
        if(RandomCut1 == 0)
        {
            BSPNode child1 = new BSPNode(HorizontalCutGrid(FirstChildWidth, FirstChildLength, node._position.x, node._position.y));
            child1._position.x = child1._bounds.x;
            child1._position.y = child1._bounds.y;
            node._child1 = child1;
        }
        else
        {
            BSPNode child1 = new BSPNode(VerticalCutGrid(FirstChildWidth, FirstChildLength, node._position.x , node._position.y));
            child1._position.x = child1._bounds.x;
            child1._position.y = child1._bounds.y;
            node._child1 = child1;
        }

        if (RandomCut2 == 0)
        {
            BSPNode child2 = new BSPNode(HorizontalCutGrid(SecondChildWidth, SecondChildLength, node._position.x, node._position.y + FirstChildLength));
            child2._position.x = child2._bounds.x;
            child2._position.y = child2._bounds.y;
            node._child2 = child2;
        }
        else
        {
            BSPNode child2 = new BSPNode(VerticalCutGrid(SecondChildWidth, SecondChildLength, node._position.x + FirstChildWidth, node._position.y));
            child2._position.x = child2._bounds.x;
            child2._position.y = child2._bounds.y;
            node._child2 = child2;
        }
    }

    RectInt HorizontalCutGrid(int ChildWidth, int ChildLength, int XStartPos, int YStartPos) //Y
    {
        RectInt NewChildGrid = new RectInt(XStartPos, YStartPos, ChildWidth, ChildLength);
        return NewChildGrid;
    }

    RectInt VerticalCutGrid(int ChildWidth, int ChildLength, int XStartPos, int YStartPos) //X
    {
        RectInt NewChildGrid = new RectInt(XStartPos, YStartPos, ChildWidth, ChildLength);
        return NewChildGrid;
    }


    private void PlaceRoom(RectInt room)
    {
        int RandomColor = RandomService.Range(0, 6);
        for (int ix = room.xMin; ix < room.xMax; ix++)
        {
            for (int iy = room.yMin; iy < room.yMax; iy++)
            {
                if (!Grid.TryGetCellByCoordinates(ix, iy, out Cell cell))
                    continue;
                
                switch (RandomColor)
                {
                    case 0:
                        AddTileToCell(cell, ROOM_TILE_NAME, true);
                        break;
                    case 1:
                        AddTileToCell(cell, CORRIDOR_TILE_NAME, true);
                        break;
                    case 2:
                        AddTileToCell(cell, GRASS_TILE_NAME, true);
                        break;
                    case 3:
                        AddTileToCell(cell, WATER_TILE_NAME, true);
                        break;
                    case 4:
                        AddTileToCell(cell, ROCK_TILE_NAME, true);
                        break;
                    case 5:
                        AddTileToCell(cell, SAND_TILE_NAME, true);
                        break;
                }
                

            }
        }
    }

    void PlaceARoomInThisArea(RectInt area, int Width, int Heigth)
    {
        int x = RandomService.Range(area.x, area.width);
        int y = RandomService.Range(area.y, area.height);

        RectInt room = new RectInt(x, y, (int)Width, (int)Heigth);
        PlaceRoom(room);
    }
}


public class BSPNode
{
    public RectInt _bounds;
    public BSPNode _child1, _child2;
    public Vector2Int _position;

    public BSPNode(RectInt bounds)
    {
        _bounds = bounds;
    }
}