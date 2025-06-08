using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnTiles : MonoBehaviour
{
    [Header("MIDI Settings")]
    public MidiFilePlayer midiPlayer;
    public float spawnDelay = 0.3f;
    public float fallSpeed = 5f;

    [Header("Tile Settings")]
    [SerializeField] private GameObject tilePrefab;
    public int numberOfLanes = 4;
    private float tileWidth;

    private Queue<MPTKEvent> noteOnQueue;
    private float gameStartTime;
    private bool isPlaying = false;
    private Transform[] spawnPoints;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputHandler inputHandler;

    private void Awake()
    {
        #if UNITY_EDITOR
        CreateTileTag();
        #endif
        noteOnQueue = new Queue<MPTKEvent>();
    }

    #if UNITY_EDITOR
    private void CreateTileTag()
    {
        var tags = UnityEditorInternal.InternalEditorUtility.tags;
        
        bool tileTagExists = false;
        foreach (string tag in tags)
        {
            if (tag == "Tile")
            {
                tileTagExists = true;
                break;
            }
        }

        if (!tileTagExists)
        {
            UnityEditorInternal.InternalEditorUtility.AddTag("Tile");
        }
    }
    #endif

    private void Start()
    {
        ValidateComponents();
        SetupSpawnPoints();
        LoadMidiEvents();
        
        if (midiPlayer != null)
        {
            midiPlayer.MPTK_Stop();
            midiPlayer.MPTK_PlayOnStart = false;
        }
    }

    private void LoadMidiEvents()
    {
        if (midiPlayer == null) return;

        if (midiPlayer.MPTK_MidiLoaded == null)
        {
            midiPlayer.MPTK_Load();
        }

        // Đọc tất cả các sự kiện MIDI
        var allEvents = midiPlayer.MPTK_ReadMidiEvents();
        if (allEvents == null || allEvents.Count == 0)
        {
            Debug.LogError("No MIDI events available after loading!");
            return;
        }

        // Lọc và thêm các note-on events vào queue
        int noteOnCount = 0;
        foreach (var evt in allEvents)
        {
            if (evt.Command == MPTKCommand.NoteOn)
            {
                noteOnQueue.Enqueue(evt);
                noteOnCount++;
            }
        }
    }

    private void ValidateComponents()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (inputHandler == null)
        {
            inputHandler = FindObjectOfType<InputHandler>();
            if (inputHandler == null)
            {
                Debug.LogError("No InputHandler found in the scene!");
                enabled = false;
                return;
            }
        }

        if (midiPlayer == null)
        {
            midiPlayer = FindObjectOfType<MidiFilePlayer>();
            if (midiPlayer == null)
            {
                Debug.LogError("No MidiFilePlayer found in the scene!");
                enabled = false;
                return;
            }
        }

        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is not assigned in the inspector!");
            enabled = false;
            return;
        }

        SpriteRenderer tileSprite = tilePrefab.GetComponent<SpriteRenderer>();
        if (tileSprite != null)
        {
            tileWidth = tileSprite.bounds.size.x;
        }
        else
        {
            // If no SpriteRenderer, try to get size from collider
            Collider2D tileCollider = tilePrefab.GetComponent<Collider2D>();
            if (tileCollider != null)
            {
                tileWidth = tileCollider.bounds.size.x;
            }
            else
            {
                Debug.LogError("Tile prefab must have either a SpriteRenderer or Collider2D to determine its size!");
                enabled = false;
                return;
            }
        }
    }

    private void SetupSpawnPoints()
    {
        if (numberOfLanes <= 0)
        {
            enabled = false;
            return;
        }

        spawnPoints = new Transform[numberOfLanes];
        
        Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        Vector3 topCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1, mainCamera.nearClipPlane));

        // Calculate the total width of the screen
        float screenWidth = topRight.x - topLeft.x;
        
        // Calculate the spacing between lanes to fit the screen width
        // Subtract tile width from total width to account for tile edges
        float spacing = (screenWidth - tileWidth) / (numberOfLanes - 1);

        // Create spawn points
        for (int i = 0; i < numberOfLanes; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
            // Position from left edge, accounting for tile width
            float xPos = topLeft.x + (spacing * i) + (tileWidth / 2f);
            spawnPoint.transform.position = new Vector3(xPos, topCenter.y, 0f);
            spawnPoint.transform.parent = transform;
            spawnPoints[i] = spawnPoint.transform;
        }

    }

    public void StartGame()
    {
        if (!enabled || midiPlayer == null) return;
        
        // Reset game state
        gameStartTime = Time.time;
        isPlaying = true;

        midiPlayer.MPTK_Play();
        Debug.Log("MIDI playback started");

        StartCoroutine(SpawnTilesCoroutine());
    }

    public void StopGame()
    {
        isPlaying = false;
        if (midiPlayer != null)
        {
            midiPlayer.MPTK_Stop();
        }
    }

    private IEnumerator SpawnTilesCoroutine()
    {
        while (isPlaying && noteOnQueue.Count > 0)
        {
            MPTKEvent currentEvent = noteOnQueue.Dequeue();
            
            // Calculate spawn position based on note value
            int noteValue = currentEvent.Value;
            int laneIndex = noteValue % spawnPoints.Length;
            
            if (spawnPoints[laneIndex] != null)
            {
                // Spawn tile from pool
                GameObject tile = ObjectPooler.Instance.SpawnFromPool("Tile", spawnPoints[laneIndex].position, Quaternion.identity);
                if (tile != null)
                {
                    Debug.Log("Tile instantiated successfully");
                    tile.tag = "Tile"; // Add tag for hit detection
                    
                    TileController tileController = tile.GetComponent<TileController>();
                    if (tileController != null)
                    {
                        tileController.OnObjectSpawn();
                    }
                    
                    TileMovement tileMovement = tile.GetComponent<TileMovement>();
                    if (tileMovement == null)
                    {
                        tileMovement = tile.AddComponent<TileMovement>();
                    }
                    
                    if (tileMovement != null)
                    {
                        tileMovement.Initialize(fallSpeed, spawnPoints[laneIndex].position, Vector3.down);
                    }
                    else
                    {
                        Debug.LogError("Failed to add TileMovement component");
                    }
                }
                else
                {
                    Debug.LogError("Failed to spawn tile from pool");
                }
            }
            else
            {
                Debug.LogError($"Spawn point at index {laneIndex} is null!");
            }

            // Đợi 0.5 giây trước khi spawn tile tiếp theo
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Make spawn points accessible to InputHandler
    public Transform[] GetSpawnPoints()
    {
        return spawnPoints;
    }

    // Optional: Visualize spawn points in the editor
    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
                }
            }
        }
    }
}

// Helper class for tile movement
public class TileMovement : MonoBehaviour
{
    private float speed;
    private Vector3 startPosition;
    private Vector3 direction;

    public void Initialize(float moveSpeed, Vector3 startPos, Vector3 moveDirection)
    {
        speed = moveSpeed;
        startPosition = startPos;
        direction = moveDirection;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
