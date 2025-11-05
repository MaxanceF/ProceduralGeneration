using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;


[CreateAssetMenu(menuName = "Procedural Generation Method/BSP")]
public class BSPScript : ProceduralGenerationMethod
{
    [SerializeField] private Vector2Int sizeMin;
    [SerializeField] private Vector2Int sizeMax;

    [Header("Debug")]
    public List<BSPNode> Tree;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        Tree = new List<BSPNode>();
        Debug.Log("Test Algo");
        RectInt allGrid = new RectInt(0, 0, Grid.Width, Grid.Lenght);
        BSPNode root = new BSPNode(allGrid);
        int NbOfCuts = 3;        
        BSPNode ParentChilds = root;
        List<BSPNode> ChildToCut = new List<BSPNode>();
        ChildToCut.Add(root);
        Tree.Add(root);
        for (int i = 0; i< NbOfCuts; i++)
        {
            List<BSPNode> newChildren = new List<BSPNode>();
            foreach (BSPNode child in ChildToCut)
            {
                CreateChildOfNode(child);
                newChildren.Add(child._child1);
                newChildren.Add(child._child2);
                
                
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }
            ChildToCut.Clear();
            ChildToCut.AddRange(newChildren);
            //if (i + 1 >= NbOfCuts)
            //{
            //    foreach (BSPNode child in ChildToCut)
            //    {
            //        //int Width = RandomService.Range(sizeMin.x, sizeMax.x + 1);
            //        //int Length = RandomService.Range(sizeMin.y, sizeMax.y + 1);
            //        //PlaceARoomInThisArea(child._bounds, Width, Length);
            //        PlaceRoom(child._bounds);
            //    }
            //}
        }
        BuildGround();
    }

    void CreateChildOfNode(BSPNode node)
    {
        int FirstChildWidth = 5;
        int FirstChildLength = 5;
        if (node._bounds.width > 5)
        {
            FirstChildWidth = RandomService.Range(5, node._bounds.width);
        }
        if(node._bounds.height > 5)
        {
            FirstChildLength = RandomService.Range(5, node._bounds.height);
        }
        int SecondChildWidth = node._bounds.width - FirstChildWidth;
        int SecondChildLength = node._bounds.height - FirstChildLength;

        int RandomCut1 = RandomService.Range(0, 2);
        int RandomCut2 = RandomService.Range(0, 2);
        BSPNode child1;
        BSPNode child2;
        if (RandomCut1 == 0)
        {
            child1 = new BSPNode(HorizontalCutGrid(node._bounds.width, FirstChildLength, node._bounds.x, node._bounds.y));           
        }
        else
        {
            child1 = new BSPNode(VerticalCutGrid(FirstChildWidth, node._bounds.height, node._bounds.x , node._bounds.y));
        }

        if (RandomCut2 == 0)
        {
            child2 = new BSPNode(HorizontalCutGrid(node._bounds.width, SecondChildLength, node._bounds.x, node._bounds.y + FirstChildLength));
        }
        else
        {
            child2 = new BSPNode(VerticalCutGrid(SecondChildWidth, node._bounds.height, node._bounds.x + FirstChildWidth, node._bounds.y));          
        }
        node._child1 = child1;
        Tree.Add(child1);
        node._child2 = child2;
        Tree.Add(child2);
        int Width = RandomService.Range(sizeMin.x, sizeMax.x + 1);
        int Length = RandomService.Range(sizeMin.y, sizeMax.y + 1);
        PlaceARoomInThisArea(child1._bounds, Width, Length);
         Width = RandomService.Range(sizeMin.x, sizeMax.x + 1);
         Length = RandomService.Range(sizeMin.y, sizeMax.y + 1);
        PlaceARoomInThisArea(child2._bounds, Width, Length);
        //PlaceRoom(child1._bounds);
        //PlaceRoom(child2._bounds);
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
        string Color = null;
        switch (RandomColor)
        {
            case 0:
                Color = "Room";
                break;
            case 1:
                Color = "Corridor";
                break;
            case 2:
                Color = "Grass";
                break;
            case 3:
                Color = "Water";
                break;
            case 4:
                Color = "Rock";
                break;
            case 5:
                Color = "Sand";
                break;
        }
        for (int ix = room.xMin; ix < room.xMax; ix++)
        {
            for (int iy = room.yMin; iy < room.yMax; iy++)
            {
                if (!Grid.TryGetCellByCoordinates(ix, iy, out Cell cell))
                    continue;
                AddTileToCell(cell, Color, true);
            }
        }
        Debug.Log("Room Placed in " + room + "with the color: " + Color);
    }

    void PlaceARoomInThisArea(RectInt area, int Width, int Heigth)
    {
        int x = RandomService.Range(area.x, area.width);
        int y = RandomService.Range(area.y, area.height);

        RectInt room = new RectInt(x, y, (int)Width, (int)Heigth);
        PlaceRoom(room);
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

[Serializable]
public class BSPNode
{
    public RectInt _bounds;
    public BSPNode _child1, _child2;

    public BSPNode(RectInt bounds)
    {
        _bounds = bounds;
    }
}