using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.PolySpatial;
using TMPro;

public class PlantSegmentSpline : MonoBehaviour
{
    public GameObject StemTemplate;
    public GameObject LeafTemplate;
    public GameObject PedalTemplate;
    void Start()
    {
        PlotSurveyPlant("Survey01");
    }
    public void PlotSurveyPlant(string dataset)
    {
        float angle = 0;
        int i = 0;
        Tuple<List<List<Segment>>, string[]> branches = ParseCSV(dataset);
        foreach (List<Segment> branch in branches.Item1)
        {
            DrawStem(branches.Item2[i], branch, angle);
            DrawLeaf(branches.Item2[i], branch, angle);
            DrawPedal(branches.Item2[i], branch, angle);
            angle += 360f / branches.Item1.Count;
            i++;
        }
    }
    public static Tuple<List<List<Segment>>, string[]> ParseCSV(string dataset)
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, dataset + ".csv");
        string[] lines = File.ReadAllLines(filePath);
        List<List<Segment>> parsedBranches = new List<List<Segment>>();

        string[] columns = lines[0].Split(',');
        int currentBranch = 0;
        foreach (string branch in columns)
        {
            List<Segment> segments = new List<Segment>();
            int lineNumber = 0;
            foreach (string line in lines)
            {
                if (lineNumber != 0)
                {
                    var fields = line.Split(',');
                    Segment segment = new Segment();
                    segment.Value = Int32.Parse(fields[currentBranch]);
                    segments.Add(segment);
                }
                lineNumber++;
            }
            parsedBranches.Add(segments);
            currentBranch++;
        }
        return new Tuple<List<List<Segment>>, string[]>(parsedBranches, columns);
    }
    public GameObject DrawStem(string datasetID,List<Segment> segments, float angle)
    {
        int branchLength = segments.Count;
        print("DaysInSegment:" + branchLength);
        List<Vector3> branchSegments = new List<Vector3>();
        int j = 0;
        for (int i = 0; i < branchLength; i++)
        {
            branchSegments.Add(new Vector3(segments[i].Value,i,0));
            j++;
        }
        //branchSegments.Add(new Vector3(0, 0, 0));


        GameObject Z1_Branch = Instantiate(StemTemplate);
        Z1_Branch.name = "Stem_" + datasetID;
        Z1_Branch.transform.parent = transform;
        Z1_Branch.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        SplineContainer splineContainer = Z1_Branch.GetComponent<SplineContainer>();
        foreach (Vector3 segment in branchSegments)
            splineContainer.Spline.Add(new BezierKnot(segment));
        splineContainer.Spline.Closed = false;
        print("Spline length: " + splineContainer.Spline.GetLength());


        MeshFilter meshFilter = Z1_Branch.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        MeshRenderer branchMesh = Z1_Branch.AddComponent<MeshRenderer>();
        branchMesh.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        branchMesh.material.SetColor("_BaseColor", Color.green);

        SplineExtrude line = Z1_Branch.AddComponent<SplineExtrude>();
        line.Container = splineContainer;
        line.SegmentsPerUnit = branchLength;
        line.Sides = 3;
        line.Rebuild();
        line.RebuildFrequency = 1;
        float radius = 0.1f;
        line.Radius = radius;

        Z1_Branch.GetComponentInChildren<TextMeshPro>().text = datasetID;
        Z1_Branch.GetComponentInChildren<TextMeshPro>().gameObject.transform.position = new Vector3(1.2f,(j-1)/10f,0);
        Z1_Branch.GetComponentInChildren<TextMeshPro>().gameObject.transform.Rotate(new Vector3(90, -90, 0));
        Z1_Branch.GetComponentInChildren<TextMeshPro>().color = Color.green;
        Z1_Branch.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        Z1_Branch.transform.Rotate(new Vector3(0, angle, 0));
        return Z1_Branch;
    }
    void DrawLeaf(string datasetID, List<Segment> segments, float angle)
    {
        // Assuming your GameObject template is properly set up
        GameObject leaf = Instantiate(LeafTemplate);
        leaf.name = "Leaf_" + datasetID;
        leaf.transform.Rotate(new Vector3(0, angle, 0)); // Rotate based on the angle provided
        leaf.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        int pointsCount = segments.Count;

        // Initialize lists for vertices and triangles
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate vertices
        for (int i = 0; i < pointsCount; i++)
        {
            Vector3 segmentVertex = new Vector3(segments[i].Value, i, 0);
            Vector3 yAxisVertex = new Vector3(0, i, 0); // Vertex on the y-axis at the same y-level

            vertices.Add(segmentVertex); // Add the segment vertex
            vertices.Add(yAxisVertex); // Add the corresponding y-axis vertex
        }

        for (int i = 0; i < pointsCount - 1; i++)
        {
            int currentSegment = i * 2;
            int nextSegment = (i + 1) * 2;

            // Front-facing triangle
            triangles.Add(currentSegment);
            triangles.Add(nextSegment);
            triangles.Add(currentSegment + 1);

            triangles.Add(currentSegment + 1);
            triangles.Add(nextSegment);
            triangles.Add(nextSegment + 1);

            // Back-facing triangle (inverted order)
            triangles.Add(currentSegment + 1);
            triangles.Add(nextSegment);
            triangles.Add(currentSegment);

            triangles.Add(nextSegment + 1);
            triangles.Add(nextSegment);
            triangles.Add(currentSegment + 1);
        }

        // Create mesh and assign data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); // To ensure correct lighting

        // Assign the mesh to the MeshFilter component
        MeshFilter meshFilter = leaf.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Set up the MeshRenderer component with the appropriate material
        MeshRenderer meshRenderer = leaf.GetComponent<MeshRenderer>();
    }
    void DrawPedal(string datasetID, List<Segment> segments, float angle)
    {
        GameObject pedal = Instantiate(PedalTemplate);
        pedal.transform.parent = GameObject.Find("Stem_" + datasetID).transform;
        pedal.name = "Pedal_" + datasetID;
        //pedal.transform.Rotate(new Vector3(0, angle, 0));
        //pedal.transform.localScale = new Vector3(3f, 3f, 3f);

        // Your existing code for generating the main mesh...

        // Now let's add the individual triangles for the last row
        int lastRowIndex = segments.Count - 1;
        float thickness = 0.2f; // Thickness of the base of the triangle

        // The segment value determines the height of the triangle
        float valueHeight = segments[lastRowIndex].Value;

        // Create vertices for the triangle, flipped upside down
        Vector3[] triangleVertices = new Vector3[3];
        triangleVertices[0] = new Vector3(1 + thickness / 2, 0, 0); // Peak vertex, now at the bottom
        triangleVertices[1] = new Vector3(1 + thickness, lastRowIndex/20f, 0); // Right base vertex
        triangleVertices[2] = new Vector3(0, lastRowIndex/20f, 0); // Left base vertex

        // Create mesh and assign data
        Mesh triangleMesh = new Mesh();
        triangleMesh.vertices = triangleVertices;

        // Triangles visible from the front
        triangleMesh.triangles = new int[]
        {
        0, 1, 2, // Front face
        2, 1, 0  // Back face (reverse winding)
        };

        pedal.GetComponent<MeshFilter>().mesh = triangleMesh;

        // Recalculate normals for lighting
        triangleMesh.RecalculateNormals();

        // Position the triangle appropriately based on the segment's position
        pedal.transform.position = new Vector3(0, 4, 0);
        pedal.transform.Rotate(new Vector3(30, angle, 0));
    }


}
public class Segment
{
    public int Value { get; set; }
}