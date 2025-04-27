using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShowCoord : MonoBehaviour
{
    public TextMeshProUGUI coordText;         // Assign UI text for coordinate list
    public GameObject markerPrefab;           // Assign your white dot prefab

    private List<Vector2> storedCoords = new List<Vector2>();
    private List<GameObject> spawnedMarkers = new List<GameObject>(); // Track all markers

    void Start()
    {
        SpawnMarkerAt(Vector3.zero); // should place a dot at (0, 0) for test
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 localPos = transform.InverseTransformPoint(mouseWorldPos);
        Vector2 roundedPos = new Vector2(localPos.x, localPos.y);

        // On click: add coord + spawn marker
        if (IsMouseOverObject() && Input.GetMouseButtonDown(0))
        {
            storedCoords.Add(roundedPos);
            SpawnMarkerAt(localPos);
            UpdateCoordDisplay();
        }

        // On X press: clear coords and destroy markers
        if (Input.GetKeyDown(KeyCode.X))
        {
            storedCoords.Clear();
            ClearMarkers();
            UpdateCoordDisplay();
        }
    }

    void SpawnMarkerAt(Vector3 localPosition)
    {
        if (markerPrefab != null)
        {
            Vector3 worldPos = transform.TransformPoint(localPosition);
            worldPos.z = 0f; // Ensure visibility

            GameObject marker = Instantiate(markerPrefab, worldPos, Quaternion.identity);
            marker.transform.localScale = Vector3.one * 0.05f;

            SpriteRenderer sr = marker.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Default";
                sr.sortingOrder = 1000;
                sr.color = Color.white;
            }

            spawnedMarkers.Add(marker);

            Debug.Log($"✅ Marker created at {worldPos}");
        }
        else
        {
            Debug.LogError("❌ No markerPrefab assigned!");
        }
    }

    void ClearMarkers()
    {
        foreach (GameObject marker in spawnedMarkers)
        {
            if (marker != null)
                Destroy(marker);
        }
        spawnedMarkers.Clear();
    }

    public void CopyToClipboard()
    {
        string coordString = "";
        foreach (Vector2 coord in storedCoords)
        {
            coordString += $"({coord.x:F2}, {coord.y:F2}) ";
        }

        GUIUtility.systemCopyBuffer = coordString.Trim();
        Debug.Log("[COORDS] Copied to clipboard!");
    }

    void UpdateCoordDisplay()
    {
        if (coordText == null) return;

        string formatted = "";
        foreach (Vector2 coord in storedCoords)
        {
            formatted += $"({coord.x:F2}, {coord.y:F2}) ";
        }

        coordText.text = formatted.Trim();
    }

    bool IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        RaycastHit hit3D;

        if (hit2D.collider != null && hit2D.collider.gameObject == gameObject)
            return true;

        if (Physics.Raycast(ray, out hit3D) && hit3D.collider.gameObject == gameObject)
            return true;

        return false;
    }
}
