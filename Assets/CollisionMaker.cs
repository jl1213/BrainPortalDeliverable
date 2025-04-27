using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;

public class CollisionMaker : MonoBehaviour
{
    public GameObject sprite;
    public Material solidLineMaterial;
    public Image screenOverlay;
    public bool showOutlines = true;

    private List<GameObject> outlineObjects = new List<GameObject>();

    void Start()
    {
        if (sprite == null)
        {
            Debug.LogError("❌ Sprite GameObject not assigned in CollisionMaker!");
            return;
        }

        if (screenOverlay != null)
        {
            screenOverlay.color = new Color(0, 0, 0, 0);
        }
        else
        {
            Debug.LogError("❌ No screen overlay assigned!");
        }

        string coordData = SceneSwitcher.correctCoords;
        Debug.Log($"🔍 Raw Coord Data: {coordData}");

        if (!string.IsNullOrEmpty(coordData))
        {
            ClearPreviousOutlines();
            CreateOutlinesFromCoordinates(coordData);
        }
        else
        {
            Debug.LogError("❌ No coordinate data found!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawRay(worldPoint, Vector3.forward * 10, Color.red, 2f);

            bool hitDetected = false;

            foreach (GameObject outline in outlineObjects)
            {
                PolygonCollider2D collider = outline.GetComponent<PolygonCollider2D>();
                if (collider != null && collider.OverlapPoint(worldPoint))
                {
                    Debug.Log($"✅ Tap detected inside collider! Position: {worldPoint}");

                    ScoreCounter.AddScore(50);
                    ScoreCounter.SetScoreFrozen(true);
                    StartCoroutine(ShowOutlineThenReturn());

                    hitDetected = true;
                    break;
                }
                else
                {
                    Debug.LogError("❌ PolygonCollider2D not found on an outline object!");
                }
            }

            if (!hitDetected)
            {
                Vector2 localPoint = sprite.transform.InverseTransformPoint(worldPoint);
                Bounds localBounds = sprite.GetComponent<SpriteRenderer>().sprite.bounds;

                if (localBounds.Contains(localPoint))
                {
                    Debug.Log($"❌ Tap was inside sprite bounds but not in any collider. Local pos: {localPoint}");
                    ScoreCounter.AddScore(-5);
                }
                else
                {
                    Debug.Log($"🚫 Tap was completely outside the sprite bounds. No penalty.");
                }
            }
        }
    }

    void ClearPreviousOutlines()
    {
        foreach (GameObject obj in outlineObjects)
        {
            Destroy(obj);
        }
        outlineObjects.Clear();
    }

    void CreateOutlinesFromCoordinates(string coordData)
    {
        List<List<Vector2>> allOutlines = ParseCoordinates(coordData);

        if (allOutlines.Count == 0)
        {
            Debug.LogError($"❌ Failed to parse coordinates! Raw Data: {coordData}");
            return;
        }

        for (int i = 0; i < allOutlines.Count; i++)
        {
            GameObject outlineObject = DrawSolidOutline(allOutlines[i]);
            if (outlineObject != null)
            {
                AddPolygonCollider(outlineObject, allOutlines[i]);
                CreateFilledMesh(outlineObject, allOutlines[i]);
                outlineObjects.Add(outlineObject);
            }
        }

        Debug.Log($"✅ Successfully created {allOutlines.Count} outlines on {sprite.name}");
    }

    GameObject DrawSolidOutline(List<Vector2> localPoints)
    {
        GameObject lineObject = new GameObject("Outline");
        lineObject.transform.SetParent(sprite.transform, false);
        lineObject.transform.localPosition = Vector3.zero;

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = localPoints.Count + 1;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;

        if (solidLineMaterial != null)
        {
            lineRenderer.material = solidLineMaterial;
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.alignment = LineAlignment.TransformZ;

        for (int i = 0; i < localPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, localPoints[i]);
        }
        lineRenderer.SetPosition(localPoints.Count, localPoints[0]);

        SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            lineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            lineRenderer.sortingOrder = spriteRenderer.sortingOrder + 50;
        }
        else
        {
            lineRenderer.sortingLayerName = "OutlineTop";
            lineRenderer.sortingOrder = 200;
        }

        lineRenderer.enabled = showOutlines;

        return lineObject;
    }

    void CreateFilledMesh(GameObject parent, List<Vector2> localPoints)
    {
        Vector3[] vertices = new Vector3[localPoints.Count];
        for (int i = 0; i < localPoints.Count; i++)
        {
            vertices[i] = localPoints[i];
        }

        Triangulator triangulator = new Triangulator(localPoints);
        int[] triangles = triangulator.Triangulate();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter mf = parent.AddComponent<MeshFilter>();
        MeshRenderer mr = parent.AddComponent<MeshRenderer>();

        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.color = new Color(1, 1, 1, 0.25f); // semi-transparent white fill

        mf.mesh = mesh;
    }

    void AddPolygonCollider(GameObject outlineObject, List<Vector2> localPoints)
    {
        PolygonCollider2D collider = outlineObject.AddComponent<PolygonCollider2D>();
        collider.points = localPoints.ToArray();
        collider.isTrigger = true;
    }

    IEnumerator ShowOutlineThenReturn()
    {
        foreach (GameObject outline in outlineObjects)
        {
            LineRenderer line = outline.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.enabled = true;
            }
        }

        yield return new WaitForSeconds(2f);
        SceneSwitcher.Instance.LoadMainScene();
    }

    List<List<Vector2>> ParseCoordinates(string coordData)
    {
        List<List<Vector2>> outlines = new List<List<Vector2>>();
        coordData = coordData.Replace("\n", "").Replace("\r", "").Trim();

        string[] groups = coordData.Split(new string[] { "1.", "2." }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string group in groups)
        {
            List<Vector2> outline = new List<Vector2>();
            string pattern = @"\(?([-+]?[0-9]*\.?[0-9]+),\s*([-+]?[0-9]*\.?[0-9]+)\)?";
            MatchCollection matches = Regex.Matches(group, pattern);

            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    float x = float.Parse(match.Groups[1].Value);
                    float y = float.Parse(match.Groups[2].Value);
                    outline.Add(new Vector2(x, y));
                }
            }

            if (outline.Count > 0)
            {
                outlines.Add(outline);
            }
        }

        return outlines;
    }
}
