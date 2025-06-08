using UnityEngine;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpawnTiles spawnTiles;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (spawnTiles == null)
            spawnTiles = FindObjectOfType<SpawnTiles>();
    }

    private void Update()
    {
        // Handle both touch and mouse input
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    HandleInput(touch.position);
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }
    }

    private void HandleInput(Vector2 inputPosition)
    {
        // Convert input position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, mainCamera.nearClipPlane));
        
        // Find the closest lane
        int laneIndex = GetLaneIndex(worldPosition.x);
        
        // Check for hits in that lane
        CheckHitsInLane(laneIndex);
    }

    private int GetLaneIndex(float xPosition)
    {
        // Get the spawn points from SpawnTiles
        Transform[] spawnPoints = spawnTiles.GetSpawnPoints();
        if (spawnPoints == null || spawnPoints.Length == 0)
            return -1;

        // Find the closest lane
        float minDistance = float.MaxValue;
        int closestLane = 0;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float distance = Mathf.Abs(spawnPoints[i].position.x - xPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestLane = i;
            }
        }

        return closestLane;
    }

    private void CheckHitsInLane(int laneIndex)
    {
        if (laneIndex < 0) return;

        // Find all tiles in the specified lane
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in tiles)
        {
            // Check if tile is in the correct lane
            if (Mathf.Abs(tile.transform.position.x - spawnTiles.GetSpawnPoints()[laneIndex].position.x) < 0.1f)
            {
                TileController tileController = tile.GetComponent<TileController>();
                if (tileController != null)
                {
                    tileController.Hit();
                    break; // Chỉ hit một tile trong lane
                }
            }
        }
    }
}