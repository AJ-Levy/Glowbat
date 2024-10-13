using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ObstacleRuleTile", menuName = "Tiles/ObstacleRuleTile")]
public class ObstacleRuleTile : RuleTile
{
    [SerializeField] float scale = 1.75f;
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        
        float randomScaleY = Random.Range(1f, scale);
        
        Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(1f, randomScaleY, 1f));
        
        Matrix4x4 translationMatrix = Matrix4x4.Translate(new Vector3(0f, (randomScaleY - 1f) / 2f, 0f));
        
        tileData.transform = translationMatrix * scaleMatrix;
    }
}