using Unity.Entities;
using Unity.Mathematics;


public struct OuterSpaceProperties : IComponentData
{
    public float2 FieldDimensions;
    public int NumberPlanetsToSpawn;
    public Entity PlanetPrefab;
}
