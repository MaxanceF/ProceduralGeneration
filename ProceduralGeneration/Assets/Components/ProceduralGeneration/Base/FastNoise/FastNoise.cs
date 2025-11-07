using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using VTools.Grid;


[CreateAssetMenu(menuName = "Procedural Generation Method/FastNoise")]
public class FastNoise : ProceduralGenerationMethod
{
    [SerializeField] private List<List<int>> StepList;
    private int Seed;

    [Header("Noise Parameters")]
    [SerializeField] private FastNoiseLite.NoiseType _noiseType;
    [SerializeField, Tooltip("Frequency"), Range(0,1)] private float _frequency = 0.01f;
    [SerializeField, Tooltip("Amplifier"), Range(0.5f,1.5f)] private float _amplitude =0.5f;


    [Header("Fractal Parameters")]
    [SerializeField] private FastNoiseLite.FractalType _fractalType;
    [SerializeField, Tooltip("Octave"), Range(1, 5)] private int _octave = 3;
    [SerializeField, Tooltip("Control increase frequency of octaves. How much details will it be added."), Range(1f, 3f)] private float _lacunarity = 2.0f;
    [SerializeField, Tooltip("Control increase frequency of octaves. How much details will it be added."), Range(0.5f, 1f)] private float _persistance = 0.5f;


    [Header("Heights")]
    [SerializeField, Range(-1,1)] private float _waterheight = -0.6f;
    [SerializeField, Range(-1,1)] private float _sandheight = -0.3f;
    [SerializeField, Range(-1,1)] private float _grassheight = 0.8f;
    [SerializeField, Range(-1,1)] private float _rockheight = 1f;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        Seed = RandomService.Seed;
        StepList = new List<List<int>>();
        FastNoiseLite Noise = CreateNoise(Seed);
        SetUpNoise(Noise);
        DrawNoise(Noise);

    }

    FastNoiseLite CreateNoise(int seed)
    {
        FastNoiseLite noise = new FastNoiseLite(seed);
        return noise;
    }

    void SetUpNoise(FastNoiseLite noise)
    {
        noise.SetNoiseType(_noiseType);
        noise.SetFrequency(_frequency);
        noise.SetFractalType(_fractalType);
        noise.SetFractalGain(_persistance);
        noise.SetFractalOctaves(_octave);
        noise.SetFractalLacunarity(_lacunarity);
        noise.SetDomainWarpAmp(_amplitude);
    }
    
    void DrawNoise(FastNoiseLite noise)
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if (!Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                    continue;
                if (noise.GetNoise(i, j) < _waterheight)
                {
                    AddTileToCell(cell, "Water", true);
                }
                else if (noise.GetNoise(i, j) >= _waterheight && noise.GetNoise(i, j) < _sandheight)
                {
                    AddTileToCell(cell, "Sand", true);
                }
                else if (noise.GetNoise(i, j) >= _sandheight && noise.GetNoise(i, j) < _grassheight)
                {
                    AddTileToCell(cell, "Grass", true);
                }             
                else if (noise.GetNoise(i, j) <= _rockheight && noise.GetNoise(i, j) >= _grassheight)
                {
                    AddTileToCell(cell, "Rock", true);
                }    
            }
        }
    }
}
