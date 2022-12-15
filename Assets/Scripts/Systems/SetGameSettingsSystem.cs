using Unity.Entities;

public class SetGameSettingsSystem : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public float asteroidVelocity = 10f;
    public float playerForce = 50f;
    public float bulletVelocity = 500f;
    public float bulletsPerSecond = 10;

    public int numAsteroids = 200;
    public int levelWidth = 2048;
    public int levelHeight = 2048;
    public int levelDepth = 2048;

    public float mouseSpeed = 2;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = default(GameSettingsComponent);

        settings.asteroidVelocity = asteroidVelocity;
        settings.playerForce = playerForce;
        settings.bulletVelocity = bulletVelocity;
        settings.bulletsPerSecond = bulletsPerSecond;

        settings.numAsteroids = numAsteroids;
        settings.levelWidth = levelWidth;
        settings.levelHeight = levelHeight;
        settings.levelDepth = levelDepth;

        settings.mouseSpeed = mouseSpeed;

        dstManager.AddComponentData(entity, settings);
    }
}