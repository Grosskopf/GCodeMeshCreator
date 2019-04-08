using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class meshcreator : MonoBehaviour
{
    public GameObject cube;
    public Transform skin;
    public Transform wallouter;
    public Transform wallinner;
    public Transform fill;
    public Transform skirt;
    void Start()
    {

        string path = "Assets/gcode/gcode.gcode";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        List<string> meshnames=new List<string>();
        int currentmesh=-1;
        List<List<Vector3>> newVertices = new List<List<Vector3>>();
        List<List<Vector2>> newUV = new List<List<Vector2>>();
        List<List<int>> newTriangles = new List<List<int>>();
        List<Vector3> tmpmove = new List<Vector3>();
        Vector3 currpos = new Vector3(0, 0, 0);
        int linesread = 0;
        int layernum = 0;
        while (!reader.EndOfStream)
        {
            linesread += 1;
            string line = reader.ReadLine();
            if (line.StartsWith(";TYPE:"))
            {
                //here i change the type of 3d printed part i print next, this only works in cura-sliced stuff, slic3r doesnt have those comments
                //print("setting type");
                if (!meshnames.Contains(line.Substring(6) + layernum))
                {
                    meshnames.Add(line.Substring(6) + layernum);
                    currentmesh = meshnames.Count - 1;
                    newVertices.Add(new List<Vector3>());
                    newUV.Add(new List<Vector2>());
                    newTriangles.Add(new List<int>());
                    print("adding: " + line + " as: " + line.Substring(6) + " Line " + layernum + " with number " + currentmesh);
                }
                else
                {
                    currentmesh = meshnames.FindIndex((line.Substring(6) + layernum).StartsWith);
                    //print("changed mesh to: " + currentmesh + " because of " + line);
                }
                //print("currentmesh" + currentmesh);
            }
            else if (line.StartsWith(";LAYER:")) {
                layernum = int.Parse(line.Substring(7));
            }
            else if (line.StartsWith("G1") && currentmesh != -1)
            {
                //here i add a point to the list of visited points of the current part
                //print("Adding object");
                string[] parts = line.Split(' ');
                if (line.Contains("E"))
                {
                    tmpmove.Add(currpos);
                    foreach (string part in parts)
                    {
                        if (part.StartsWith("X"))
                        {
                            currpos.x = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                        }
                        else if (part.StartsWith("Y"))
                        {
                            currpos.z = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                        }
                        else if (part.StartsWith("Z"))
                        {
                            currpos.y = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                        }
                    }
                }
                //tmpmove.Add(currpos);
                //print(currpos);
            }
            else if (line.StartsWith("G0"))
            {
                if (tmpmove.Count > 0)
                {
                    tmpmove.Add(currpos);
                    //here i generate the mesh from the tmpmove list, wich is a list of points the extruder goes to
                    int vstart = newVertices[currentmesh].Count;
                    Vector3 dv = tmpmove[1] - tmpmove[0];
                    Vector3 dvt = dv; dvt.x = dv.z; dvt.z = -dv.x;
                    newVertices[currentmesh].Add(tmpmove[0] - dv.normalized * 0.5f + dvt.normalized * 0.5f);
                    newVertices[currentmesh].Add(tmpmove[0] - dv.normalized * 0.5f - dvt.normalized * 0.5f);
                    newVertices[currentmesh].Add(tmpmove[0] - dv.normalized * 0.5f - dvt.normalized * 0.5f - new Vector3(0, -0.5f, 0));
                    newVertices[currentmesh].Add(tmpmove[0] - dv.normalized * 0.5f + dvt.normalized * 0.5f - new Vector3(0, -0.5f, 0));
                    newVertices[currentmesh].Add(tmpmove[0] + dvt.normalized * 0.6f);
                    newVertices[currentmesh].Add(tmpmove[0] - dvt.normalized * 0.6f);
                    newVertices[currentmesh].Add(tmpmove[0] - dvt.normalized * 0.6f - new Vector3(0, -0.5f, 0));
                    newVertices[currentmesh].Add(tmpmove[0] + dvt.normalized * 0.6f - new Vector3(0, -0.5f, 0));
                    newUV[currentmesh].Add(new Vector2(0.0f, 0.0f));
                    newUV[currentmesh].Add(new Vector2(0.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 0.0f));
                    newUV[currentmesh].Add(new Vector2(0.0f, 0.0f));
                    newUV[currentmesh].Add(new Vector2(0.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 0.0f));

                    newTriangles[currentmesh].Add(vstart + 0);
                    newTriangles[currentmesh].Add(vstart + 1);
                    newTriangles[currentmesh].Add(vstart + 2); //back (those need to be in clockwise orientation for culling to work right)
                    newTriangles[currentmesh].Add(vstart + 0);
                    newTriangles[currentmesh].Add(vstart + 2);
                    newTriangles[currentmesh].Add(vstart + 3);

                    newTriangles[currentmesh].Add(vstart + 0);
                    newTriangles[currentmesh].Add(vstart + 1);
                    newTriangles[currentmesh].Add(vstart + 5); //top
                    newTriangles[currentmesh].Add(vstart + 0);
                    newTriangles[currentmesh].Add(vstart + 5);
                    newTriangles[currentmesh].Add(vstart + 4);

                    newTriangles[currentmesh].Add(vstart + 1);
                    newTriangles[currentmesh].Add(vstart + 2);
                    newTriangles[currentmesh].Add(vstart + 5);//left
                    newTriangles[currentmesh].Add(vstart + 2);
                    newTriangles[currentmesh].Add(vstart + 6);
                    newTriangles[currentmesh].Add(vstart + 5);

                    newTriangles[currentmesh].Add(vstart + 0);
                    newTriangles[currentmesh].Add(vstart + 4);
                    newTriangles[currentmesh].Add(vstart + 3);//right
                    newTriangles[currentmesh].Add(vstart + 3);
                    newTriangles[currentmesh].Add(vstart + 4);
                    newTriangles[currentmesh].Add(vstart + 7);

                    newTriangles[currentmesh].Add(vstart + 4);
                    newTriangles[currentmesh].Add(vstart + 3);
                    newTriangles[currentmesh].Add(vstart + 7);//bottom
                    newTriangles[currentmesh].Add(vstart + 4);
                    newTriangles[currentmesh].Add(vstart + 7);
                    newTriangles[currentmesh].Add(vstart + 6);
                    for (int i = 1; i < tmpmove.Count - 1; i++)
                    {
                        //print(tmpmove[i+1]);
                        Vector3 dv1 = tmpmove[i] - tmpmove[i - 1];
                        Vector3 dvt1 = dv1; dvt1.x = dv1.z; dvt1.z = -dv1.x;
                        Vector3 dv2 = tmpmove[i + 1] - tmpmove[i];
                        Vector3 dvt2 = dv2; dvt2.x = dv2.z; dvt2.z = -dv2.x;
                        dvt = (dvt1 + dvt2).normalized * 0.6f;
                        newVertices[currentmesh].Add(tmpmove[i] + dvt);
                        newVertices[currentmesh].Add(tmpmove[i] - dvt);
                        newVertices[currentmesh].Add(tmpmove[i] - dvt - new Vector3(0, -0.5f, 0));
                        newVertices[currentmesh].Add(tmpmove[i] + dvt - new Vector3(0, -0.5f, 0));
                        newUV[currentmesh].Add(new Vector2(0.0f, 0.0f));
                        newUV[currentmesh].Add(new Vector2(0.0f, 1.0f));
                        newUV[currentmesh].Add(new Vector2(1.0f, 1.0f));
                        newUV[currentmesh].Add(new Vector2(1.0f, 0.0f));

                        newTriangles[currentmesh].Add(vstart + 4 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 5 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 9 + 4 * i); //top
                        newTriangles[currentmesh].Add(vstart + 4 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 9 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 8 + 4 * i);

                        newTriangles[currentmesh].Add(vstart + 5 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 6 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 10 + 4 * i);//left
                        newTriangles[currentmesh].Add(vstart + 5 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 10 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 9 + 4 * i);

                        newTriangles[currentmesh].Add(vstart + 4 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 8 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 7 + 4 * i);//right
                        newTriangles[currentmesh].Add(vstart + 7 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 8 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 11 + 4 * i);

                        newTriangles[currentmesh].Add(vstart + 6 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 7 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 11 + 4 * i);//bottom
                        newTriangles[currentmesh].Add(vstart + 6 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 11 + 4 * i);
                        newTriangles[currentmesh].Add(vstart + 10 + 4 * i);
                    }
                    dv = tmpmove[tmpmove.Count - 1] - tmpmove[tmpmove.Count - 2];
                    dvt = dv; dvt.x = dv.z; dvt.z = -dv.x;
                    dvt = dvt.normalized * 0.5f;
                    dv = dv.normalized * 0.5f;
                    int maxi = tmpmove.Count - 2;

                    newVertices[currentmesh].Add(tmpmove[maxi] + dv + dvt);
                    newVertices[currentmesh].Add(tmpmove[maxi] + dv - dvt);
                    newVertices[currentmesh].Add(tmpmove[maxi] + dv - dvt - new Vector3(0, -0.5f, 0));
                    newVertices[currentmesh].Add(tmpmove[maxi] + dv + dvt - new Vector3(0, -0.5f, 0));
                    newVertices[currentmesh].Add(tmpmove[0] + dv.normalized * 0.5f + dvt.normalized * 0.5f);
                    newVertices[currentmesh].Add(tmpmove[0] + dv.normalized * 0.5f - dvt.normalized * 0.5f);
                    newVertices[currentmesh].Add(tmpmove[0] + dv.normalized * 0.5f - dvt.normalized * 0.5f - new Vector3(0, -0.5f, 0));
                    newVertices[currentmesh].Add(tmpmove[0] + dv.normalized * 0.5f + dvt.normalized * 0.5f - new Vector3(0, -0.5f, 0));
                    newUV[currentmesh].Add(new Vector2(0.0f, 0.0f));
                    newUV[currentmesh].Add(new Vector2(0.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 0.0f));
                    newUV[currentmesh].Add(new Vector2(0.0f, 0.0f));
                    newUV[currentmesh].Add(new Vector2(0.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 1.0f));
                    newUV[currentmesh].Add(new Vector2(1.0f, 0.0f));

                    newTriangles[currentmesh].Add(vstart + 4 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 5 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi); //top
                    newTriangles[currentmesh].Add(vstart + 4 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi);

                    newTriangles[currentmesh].Add(vstart + 5 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 6 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi);//left
                    newTriangles[currentmesh].Add(vstart + 5 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi);

                    newTriangles[currentmesh].Add(vstart + 4 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 7 + 4 * maxi);//right
                    newTriangles[currentmesh].Add(vstart + 7 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi);

                    newTriangles[currentmesh].Add(vstart + 6 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 7 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi);//bottom
                    newTriangles[currentmesh].Add(vstart + 6 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi);

                    newTriangles[currentmesh].Add(vstart + 4 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 5 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi + 1); //top
                    newTriangles[currentmesh].Add(vstart + 4 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi + 1);

                    newTriangles[currentmesh].Add(vstart + 5 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 6 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi + 1);//left
                    newTriangles[currentmesh].Add(vstart + 5 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi + 1);

                    newTriangles[currentmesh].Add(vstart + 4 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 7 + 4 * maxi + 1);//right
                    newTriangles[currentmesh].Add(vstart + 7 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi + 1);

                    newTriangles[currentmesh].Add(vstart + 6 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 7 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi + 1);//bottom
                    newTriangles[currentmesh].Add(vstart + 6 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi + 1);

                    newTriangles[currentmesh].Add(vstart + 8 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi + 1);//front
                    newTriangles[currentmesh].Add(vstart + 11 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 9 + 4 * maxi + 1);
                    newTriangles[currentmesh].Add(vstart + 10 + 4 * maxi + 1);

                    tmpmove.Clear();
                }
                string[] parts = line.Split(' ');
                foreach (string part in parts)
                {
                    if (part.StartsWith("X"))
                    {
                        currpos.x = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                    }
                    else if (part.StartsWith("Y"))
                    {
                        currpos.z = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                    }
                    else if (part.StartsWith("Z"))
                    {
                        currpos.y = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                    }
                }
            }
            // Do Something with the input. 
        }
        reader.Close();
        //print("read lines: " + linesread);
        for (int i=0; i < meshnames.Count; i++)
        {
            Mesh mesh = new Mesh();
            GameObject part = Instantiate(cube);
            part.transform.position = this.transform.position;
            part.transform.localScale = this.transform.localScale;
            part.name = meshnames[i];
            part.GetComponent<MeshFilter>().mesh = mesh;
            if (meshnames[i].StartsWith("SKIN")){
                part.transform.SetParent(skin);
            }
            else if (meshnames[i].StartsWith("WALL-OUTER"))
            {
                part.transform.SetParent(wallouter);
            }
            else if (meshnames[i].StartsWith("WALL-INNER"))
            {
                part.transform.SetParent(wallinner);
            }
            else if (meshnames[i].StartsWith("FILL"))
            {
                part.transform.SetParent(fill);
            }
            else if (meshnames[i].StartsWith("SKIRT"))
            {
                part.transform.SetParent(skirt);
            }
            switch (meshnames[i])
            {
                case "SKIN":
                    part.transform.SetParent(skin);
                    break;
                case "WALL-OUTER":
                    part.transform.SetParent(wallouter);
                    break;
                case "WALL-INNER":
                    part.transform.SetParent(wallinner);
                    break;
                case "FILL":
                    part.transform.SetParent(fill);
                    break;
                case "SKIRT":
                    part.transform.SetParent(skirt);
                    break;
            }
            print("trianglecount: ");
            print(newTriangles[i].Count);
            print("lasttrianglepoint: ");
            print(newTriangles[i][newTriangles[i].Count - 1]);
            print("vertexcount: ");
            print(newVertices[i].Count);
            printbounding(newVertices[i].ToArray());
            mesh.vertices = newVertices[i].ToArray();
            mesh.uv = newUV[i].ToArray();
            mesh.triangles = newTriangles[i].ToArray();
        }
    }
    void printbounding(Vector3[] arr)
    {
        float minx = float.MaxValue;
        float miny = float.MaxValue;
        float minz = float.MaxValue;
        float maxx = float.MinValue;
        float maxy = float.MinValue;
        float maxz = float.MinValue;
        foreach (Vector3 vec in arr)
        {
            if (vec.x < minx)
            {
                minx = vec.x;
            }
            if (vec.y < miny)
            {
                miny = vec.y;
            }
            if (vec.z < minz)
            {
                minz = vec.z;
            }
            if (vec.x > maxx)
            {
                maxx = vec.x;
            }
            if (vec.y > maxy)
            {
                maxy = vec.y;
            }
            if (vec.z > maxz)
            {
                maxz = vec.z;
            }
        }
        print("min :" + minx + "/" + miny + "/" + minz);
        print("max :" + maxx + "/" + maxy + "/" + maxz);
    }

}
