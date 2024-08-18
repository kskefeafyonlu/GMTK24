using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class InfiniteTilemap : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] tiles; // Array to hold different tile types
    public Transform player;
    public int renderRadius = 35;

    private Vector3Int _previousPlayerPosition;
    private HashSet<Vector3Int> _generatedTiles = new HashSet<Vector3Int>();

    void Start()
    {
        _previousPlayerPosition = GetPlayerTilePosition();
        GenerateTilesAroundPlayer();
    }

    void Update()
    {
        Vector3Int currentPlayerPosition = GetPlayerTilePosition();
        if (currentPlayerPosition != _previousPlayerPosition)
        {
            _previousPlayerPosition = currentPlayerPosition;
            GenerateTilesAroundPlayer();
        }
    }

    Vector3Int GetPlayerTilePosition()
    {
        Vector3 playerPosition = player.position;
        return tilemap.WorldToCell(playerPosition);
    }

    void GenerateTilesAroundPlayer()
    {
        Vector3Int playerTilePosition = GetPlayerTilePosition();

        for (int x = -renderRadius; x <= renderRadius; x++)
        {
            for (int y = -renderRadius; y <= renderRadius; y++)
            {
                Vector3Int tilePosition = new Vector3Int(playerTilePosition.x + x, playerTilePosition.y + y, 0);
                if (!_generatedTiles.Contains(tilePosition))
                {
                    Tile selectedTile = tiles[Random.Range(0, tiles.Length)]; // Randomly select a tile
                    tilemap.SetTile(tilePosition, selectedTile);
                    _generatedTiles.Add(tilePosition);
                }
            }
        }

        // Optionally, remove tiles that are too far from the player
        List<Vector3Int> tilesToRemove = new List<Vector3Int>();
        foreach (var tile in _generatedTiles)
        {
            if (Vector3Int.Distance(tile, playerTilePosition) > renderRadius)
            {
                tilesToRemove.Add(tile);
            }
        }

        foreach (var tile in tilesToRemove)
        {
            tilemap.SetTile(tile, null);
            _generatedTiles.Remove(tile);
        }
    }
}