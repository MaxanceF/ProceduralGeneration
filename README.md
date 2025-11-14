Procedural Generation

Dans ce projet, on peut utiliser 4 méthodes différentes de générétion procédurale avec chacune des utilités différentes.
Toutes les méthodes sont des SO (scriptable object) et utilisent une seed pour gérer leur aléatoire.
Exemple de création d'un SO qui fonctionne avec le ProceduralGridGenerator.cs (pour l'utiliser il faut juste glisser un SO créé dans le script ProceduralGridGenerator qui est sur un gameobject dans la scene)

```csharp
 [CreateAssetMenu(menuName = "Procedural Generation Method/Simple Room Placement")]
    public class SimpleRoomPlacement : ProceduralGenerationMethod
    {
        [Header("Room Parameters")]
        [SerializeField] private int _maxRooms = 10;
        
        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            // Declare variables here
            // ........

            for (int i = 0; i < _maxSteps; i++)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();
                
                // Your algorithm here
                // .......

                // Waiting between steps to see the result.
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken : cancellationToken);
            }
            
            // Final ground building.
            BuildGround();
        }
}
```

===
## 1. Simple Room Placement
* La version simplifié du BSP, elle est utile pour la création de carte avec des salles
* On peut changer la taille des salles en x et en y

<center>
    <img src="Images/SimpleRoomMap.png">
</center>

## 2.BSP
* Une version plus complexe du simple room placement. Le BSP utilise des nodes qui se split pour créer des zones qui ensuite sont utiliser pour créer des salles et les lié
* On peut changer la taille des salles en x et en y, des leafs, le ratio de partage horizontale
<center>
    <img src="Images/BSPMap.png">
</center>

## 3.Cellular Automata
* Le cellular automata créer un bruit aléatoire (des 0 ou des 1 dans une liste dans cette situation)
* Puis il applique un script un certain nombre de fois avec des conditions (ici aujouter de la terre si 4 cases ou plus dans les 8 cases autour sont de la terre)
* On peut changer le nombre de fois qu'on répète les conditions
<center>
    <img src="Images/CellularAutomataMap.png">
</center>

## 4.FastNoiseLite
* Créé un bruit à l'aide de FastNoise (https://github.com/Auburn/FastNoiseLite)
* Possibilité de créer un bruit de perlin, simplex, etc
* Possibilité d'ajouter des fractales
<center>
    <img src="Images/FastNoiseMap.png">
</center>
