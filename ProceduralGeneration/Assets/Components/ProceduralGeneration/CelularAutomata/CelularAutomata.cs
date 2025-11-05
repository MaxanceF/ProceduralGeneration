using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTools.Grid;

[CreateAssetMenu(menuName = "Procedural Generation Method/CelularAutomata")]

public class CelularAutomata : ProceduralGenerationMethod
{
    [SerializeField] public List<List<int>> GridList;
    [SerializeField] private List<List<int>> StepList;
    [SerializeField] int NoiseDensity = 0;
    [SerializeField] int NbOfNearbyGrassNeeded = 4;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        StepList = new List<List<int>>();
        GridList = new List<List<int>>();
        SetRandomGrid(NoiseDensity);
        
        for (int i = 0; i < _maxSteps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GridList = StepList;
            BuildMapList(GridList);
            UpdateCells(StepList);
            // Waiting between steps to see the result.
            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }
    }

    void SetRandomGrid(int noiseDensity) //0 = ground, 1 = water
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            StepList.Add(new List<int>());
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if(noiseDensity == 100)
                {
                    StepList[i].Add(0);
                }
                else
                {
                    int CellType = RandomService.Range(0, 100);
                    if(CellType < noiseDensity)
                    {
                        StepList[i].Add(0);
                    }
                    else
                    {
                        StepList[i].Add(1);
                    }
                }
                
                
               
            }
        }
        
    }

    void BuildMapList(List<List<int>> mapList)
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                switch (mapList[i][j])
                {
                    case 0:
                        if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                            continue;
                        AddTileToCell(cell, "Grass", true);
                        break;
                    case 1:
                        if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell2))
                            continue;
                        AddTileToCell(cell2, "Water", true);
                        break;
                    default:
                        if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell3))
                            continue;
                        AddTileToCell(cell3, "Room", true);
                        break;
                }
            }
        }
    }

    void UpdateCells(List<List<int>> steplist)
    {
        
        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if (IsCorner(i, j))
                {
                    steplist[i][j] = 1;
                    if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                        continue;
                    AddTileToCell(cell, "Water", true);
                }
                else
                {
                    if (IsBecomingGrass(i, j, NbOfNearbyGrassNeeded))
                    {
                        if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                            continue;
                        AddTileToCell(cell, "Grass", true);
                    }
                    else
                    {
                        if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                            continue;
                        AddTileToCell(cell, "Water", true);
                    }
                }

            }
        }
    }

    bool IsGrass(int x, int y)
    {
        bool[,] isGrass = new bool[Grid.Width, Grid.Lenght];
        if (Grid.TryGetCellByCoordinates(x, y, out Cell cell))
        {
            isGrass[x, y] = cell.GridObject.Template.Name == "Grass";
            return isGrass[x,y];
        }
        return false;
    }

    bool IsCorner(int x, int j)
    {
        if (x == 0 && j == 0) return true;
        if (x == 64 && j == 64) return true;
        if (x == 0 && j == 64) return true;
        if (x == 64 && j == 0) return true;
        return false;
    }

    bool IsBecomingGrass(int x, int y, int NbOfNearbyGrassNeeded)
    {
        int countgrass = 0;
        for (int i = -1; i >= 1; i++)
        {
            for (int j = -1; j >= 1; j++)
            {
                if (x+i == x && y+j == y)
                {

                }else if (IsGrass(x+i, y+j))
                {
                    countgrass++;
                }
            }
        }
        if (countgrass >= NbOfNearbyGrassNeeded)
        {
            
            return true;
        }
        Debug.Log(countgrass);
        return false;
    }
}
