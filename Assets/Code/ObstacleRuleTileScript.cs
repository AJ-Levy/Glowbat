using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ObstacleRuleTile", menuName = "Tiles/ObstacleRuleTile")]
// <summary>
/// A custom rule tile for stalagmites/stalctites so that they can be scaled vertically.
/// </summary>
public class ObstacleRuleTile : RuleTile
{
    
    [SerializeField] float scale = 1.75f;

    // <summary>
    /// This method scales the obstacles tiles by a random amount in a specified range.
    /// </summary>
    /// <param name="postion">The position of a tile in a tilemap./</param>
    /// <param name="tilemap">The Tilemap the tile belongs to.</param>
    /// <param name="tileData">A reference to the tile data for the tile at the given position.</param>
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        
        base.GetTileData(position, tilemap, ref tileData);
        
        float randomScaleY = Random.Range(1f, scale);
        
        Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(1f, randomScaleY, 1f));
        
        Matrix4x4 translationMatrix = Matrix4x4.Translate(new Vector3(0f, (randomScaleY - 1f) / 2f, 0f));
        
        tileData.transform = translationMatrix * scaleMatrix;
    }
}