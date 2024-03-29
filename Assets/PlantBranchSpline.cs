using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.PolySpatial;
using TMPro;

public class PlantSegmentSpline : MonoBehaviour
{
    public GameObject SegmentTemplate;
    void Start()
    {
        PlotSurveyPlant("Survey01");
    }
    public void PlotSurveyPlant(string dataset)
    {
        float angle = 0;
        List<List<Segment>> branches = ParseCSV(dataset);
        foreach(List<Segment> branch in branches)
        {
            DrawStem(dataset, branch, angle);
            angle += 360f / branches.Count;
        }
    }
    public static List<List<Segment>> ParseCSV(string dataset)
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
                if(lineNumber!=0)
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
        return parsedBranches;
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


        GameObject Z1_Branch = Instantiate(SegmentTemplate);
        Z1_Branch.name = datasetID;
        Z1_Branch.transform.parent = transform;
        Z1_Branch.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Z1_Branch.transform.Rotate(new Vector3(0, angle, 0));
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

        /*
        Z1_Branch.GetComponentInChildren<TextMeshPro>().text = datasetID;
        Z1_Branch.GetComponentInChildren<TextMeshPro>().gameObject.transform.position = new Vector3(segments[j].Value,j,0);
        Z1_Branch.GetComponentInChildren<TextMeshPro>().gameObject.transform.Rotate(new Vector3(0, angle, 0));
        Z1_Branch.GetComponentInChildren<TextMeshPro>().color = Color.green;
        Z1_Branch.GetComponentInChildren<TextMeshPro>().gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        */
        return Z1_Branch;
    }
    
}
public class Segment
{
    public int Value { get; set; }
}