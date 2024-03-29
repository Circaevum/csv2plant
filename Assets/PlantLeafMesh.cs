using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlantLeafMesh : MonoBehaviour
{
    public GameObject LeafTemplate;
    void Start()
    {
        PlotSurveyPlant("Survey01");
    }
    public void PlotSurveyPlant(string dataset)
    {
        float angle = 0;
        List<List<Segmentt>> branches = ParseCSV(dataset).Item1;
        string[] columns = ParseCSV(dataset).Item2;
        foreach (List<Segmentt> branch in branches)
        {
            DrawMesh(dataset, branch, angle, columns);
            angle += 360f / branches.Count;
        }
    }
    public static Tuple<List<List<Segmentt>>,string[]> ParseCSV(string dataset)
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, dataset + ".csv");
        string[] lines = File.ReadAllLines(filePath);
        List<List<Segmentt>> parsedBranches = new List<List<Segmentt>>();

        string[] columns = lines[0].Split(',');
        int currentBranch = 0;
        foreach (string branch in columns)
        {
            List<Segmentt> segments = new List<Segmentt>();
            int lineNumber = 0;
            foreach (string line in lines)
            {
                if (lineNumber != 0)
                {
                    var fields = line.Split(',');
                    Segmentt segment = new Segmentt();
                    segment.Value = Int32.Parse(fields[currentBranch]);
                    segments.Add(segment);
                }
                lineNumber++;
            }
            parsedBranches.Add(segments);
            currentBranch++;
        }
        return new Tuple<List<List<Segmentt>>, string[]>(parsedBranches,columns);
    }
    void DrawMesh(string datasetID, List<Segmentt> segments, float angle, string[] columns)
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



}
public class Segmentt
{
    public int Value { get; set; }
}