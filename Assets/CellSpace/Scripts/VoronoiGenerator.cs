// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using VoroGen;
using Bibcam.Encoder;
using Bibcam.Common;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

namespace CellSpace
{
    public class VoronoiGenerator : MonoBehaviour 
    {
        // Game objects used to visualize the results
        #region Public accessors
        public GameObject[] Cells { get => _cells; }
        public GameObject[] Points { get => _points; }
        public GameObject[] Wireframes { get => _wireframes; }
        public GameObject[] Edges { get => _edges; }
        public GameObject[] Sites { get => _sites; set => _sites = value; }
        
        #endregion

        #region Editable attributes
        [SerializeField] bool _cellDisplayed = false;
        [SerializeField] bool _pointDisplayed = false;
        [SerializeField] bool _wireframeDisplayed = false;
        [SerializeField] bool _edgeDisplayed = false;
        [SerializeField] bool _wireframeLineDisplayed = false;

        [SerializeField] Gradient _cellColorGradient;

        [SerializeField] Color _wireframeColor = Color.white;
        [SerializeField] Color _edgeColor = Color.white;

        [SerializeField] Color _cellNonPlayerColor = Color.white;
        [SerializeField] Color _wireframeNonPlayerColor = Color.white;
        [SerializeField] Color _edgeNonPlayerColor = Color.white;

        [SerializeField] Material _cellMaterial;
        [SerializeField] Material _pointMaterial;
        [SerializeField] Material _wireframeMaterial;
        [SerializeField] Material _edgeMaterial;

        [SerializeField] Material _wireframeLineMaterial;

        [SerializeField] Material _cellNonPlayerMaterial;
        [SerializeField] Material _wireframeNonPlayerMaterial;

        [SerializeField] Bounds _bounds;

        [SerializeField, Range(0.0f, 0.1f)] float _offset;

        [SerializeField, Range(0.001f, 0.2f)] float _wireframeTubeWidth;
        [SerializeField, Range(0.001f, 0.2f)] float _wireframeNonPlayerTubeWidth;

        [SerializeField, Range(0.001f, 0.2f)] float _edgeTubeWidth;
        [SerializeField, Range(0.001f, 0.2f)] float _edgeNonPlayerTubeWidth;

        #endregion

        #region Private objects
        GameObject[] _cells;
        GameObject[] _points;
        GameObject[] _wireframes;
        GameObject[] _edges;

        Material[] _cellMaterials;
        Material[] _wireframeMaterials;
        Material[] _pointMaterials;
        Material[] _edgeMaterials;

        GameObject[] _sites;
        Color[] _colors;
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            
        }


        // Update is called once per frame
        void Update()
        {
            //Find all objects with PlayerPoseSynchronizer in the scenes
            CreateVoronoi();
        }

        private void CreateVoronoi()
        {
            if (_sites == null)
            {
                return;
            }
            int n = _sites.Length;

            if (_sites.Any(p => p == null))
            {
                return;
            }

            if (_colors == null || _colors.Length != n)
            {
                _colors = new Color[n];
                _wireframeMaterials = new Material[n];
                _cellMaterials = new Material[n];
                _edgeMaterials = new Material[n];

                for (int i = 0; i < n; i++)
                {
                    _colors[i] = _cellColorGradient.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
                    _wireframeMaterials[i] = new Material(_wireframeMaterial);
                    _cellMaterials[i] = new Material(_cellMaterial);
                    _edgeMaterials[i] = new Material(_edgeMaterial);

                    var cellRole = _sites[i].GetComponent<CellRole>();

                    _cellMaterials[i].SetColor("_Color", cellRole.isPlayer ? _colors[i] : _cellNonPlayerColor);
                    _wireframeMaterials[i].SetColor("_Color", cellRole.isPlayer ? _colors[i] : _wireframeNonPlayerColor);
                    _edgeMaterials[i].SetColor("_Color", cellRole.isPlayer ? _colors[i] : _edgeNonPlayerColor);
                }
            }

            var weightedPoints = _sites.Select(p => new WeightedPoint { x = p.transform.localPosition.x,
                    y = p.transform.localPosition.y,
                    z = p.transform.localPosition.z,
                    w = p.transform.localScale.x} ).ToArray();

            var meshes = VoronoiGeneratorAPI.GenerateVoronoi(weightedPoints, _bounds, _offset);

            int totalEdges = 0;

            for (int i = 0; i < meshes.Length; i++)
            {
                var (cellVertices, cellTriangles, cellLines, cellEdges) = meshes[i];

                if (_cellDisplayed)
                {
                    string cellName = $"Cell {i}";
                    GameObject cell = transform.Find(cellName)?.gameObject;
                    if (cell == null) {
                        cell = new GameObject(cellName);
                        cell.transform.parent = transform;
                        cell.AddComponent<MeshFilter>().mesh = new Mesh();
                        cell.AddComponent<MeshRenderer>().material = _cellMaterials[i];
                        cell.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }

                    var mesh = cell.GetComponent<MeshFilter>().mesh;
                    if (cellVertices.Length > 0) {
                        mesh.SetVertices(cellVertices);
                        mesh.SetIndices(cellTriangles, MeshTopology.Triangles, 0);
                        mesh.RecalculateNormals();
                        mesh.RecalculateBounds();
                    } else {
                        mesh.Clear();
                    }
                }

                if (_wireframeDisplayed)
                {
                    string wireframeName = $"Wireframe {i}";
                    GameObject wireframe = transform.Find(wireframeName)?.gameObject;
                    if (wireframe == null) {
                        wireframe = new GameObject(wireframeName);
                        wireframe.transform.parent = transform;
                        wireframe.AddComponent<MeshFilter>().mesh = new Mesh();
                        wireframe.AddComponent<MeshRenderer>().material = _wireframeMaterials[i];
                        wireframe.AddComponent<TubeRenderer>();
                    } 

                    var tube = wireframe.GetComponent<TubeRenderer>();
                    tube.SetPositions(cellVertices);
                    tube.SetIndices(cellLines);
                    tube.tubularSegments = 1;
                    tube.radialSegments = 8;
                    var cellRole = _sites[i].GetComponent<CellRole>();
                    tube.radius = cellRole.isPlayer ? _wireframeTubeWidth : _wireframeNonPlayerTubeWidth;
                }

                if (_wireframeLineDisplayed)
                {
                    string wireframeName = $"Wireframe Line {i}";
                    GameObject wireframe = transform.Find(wireframeName)?.gameObject;
                    if (wireframe == null) {
                        wireframe = new GameObject(wireframeName);
                        wireframe.transform.parent = transform;
                        wireframe.AddComponent<MeshFilter>().mesh = new Mesh();
                        wireframe.AddComponent<MeshRenderer>().material = _wireframeMaterials[i];
                    } 

                    var mesh = wireframe.GetComponent<MeshFilter>().mesh;
                    if (cellVertices.Length > 0) {
                        //mesh.SetVertices(cellVertices);
                        //mesh.SetIndices(cellLines, MeshTopology.Lines, 0);
                        //mesh.RecalculateBounds();

                        var middlePoints = cellLines.Where((x, index) => index % 2 == 0)
                                   .Zip(cellLines.Where((x, index) => index % 2 == 1),
                                   (a, b) => Vector3.Lerp(cellVertices[a], cellVertices[b], 0.5f)).ToArray();

                        var indices = cellLines.Where((x, index) => index % 2 == 0)
                           .Zip(cellLines.Where((x, index) => index % 2 == 1),
                           (a, b) => (a, b)).SelectMany((x, j) => new int[] { x.a, cellVertices.Length + j, cellVertices.Length + j, x.b }).ToArray();
                        var vertices = cellVertices.Concat(middlePoints).ToArray();
                        var uvs = Enumerable.Repeat(Vector2.zero, cellVertices.Length).Concat(Enumerable.Repeat(Vector2.one, cellLines.Length / 2)).ToArray();
                        
                        mesh.Clear();
                        mesh.SetVertices(vertices);
                        mesh.SetIndices(indices, MeshTopology.Lines, 0);
                        mesh.SetUVs(0, uvs);
                        mesh.RecalculateBounds();
                    }
                    else {
                        mesh.Clear();
                    }
                }

                if (_edgeDisplayed)
                {
                    for (int j = 0; j < cellEdges.Length; j++)
                    {
                        var cellRole = _sites[i].GetComponent<CellRole>();
                        var cellRole2 = _sites[cellEdges[j]].GetComponent<CellRole>();

                        if (i < cellEdges[j] && cellRole.isPlayer && cellRole2.isPlayer)
                        {
                            string edgeName = $"Edge {totalEdges++}";
                            GameObject edge = transform.Find(edgeName)?.gameObject;
                            if (edge == null)
                            {
                                edge = new GameObject(edgeName);
                                edge.transform.parent = transform;
                                edge.AddComponent<MeshFilter>().mesh = new Mesh();
                                edge.AddComponent<MeshRenderer>().material = _edgeMaterials[i];
                                //edge.AddComponent<LineRenderer>();
                                edge.AddComponent<TubeRenderer>();
                            }

                            var mesh = edge.GetComponent<MeshFilter>().mesh;


                            //var lineRender = edge.GetComponent<LineRenderer>();
                            //lineRender.SetPositions(new Vector3[] { _sites[i].transform.localPosition, _sites[cellEdges[j]].transform.localPosition });
                            //lineRender.SetWidth(0.01f, 0.01f);
                            var tube = edge.GetComponent<TubeRenderer>();
                            tube.SetPositions(new Vector3[] { _sites[i].transform.localPosition, _sites[cellEdges[j]].transform.localPosition });
                            tube.SetIndices(new int[] { 0, 1 });
                            tube.tubularSegments = 1;
                            tube.radialSegments = 8;
                            tube.radius = cellRole.isPlayer && cellRole2.isPlayer ? _edgeTubeWidth : _edgeNonPlayerTubeWidth;
//                            Debug.Log($"{edgeName}: {i} {cellEdges[j]}");
                            //else
                            //{
                            //    var lineRender = edge.GetComponent<LineRenderer>();
                            //    lineRender.SetPositions(new Vector3[] { });
                            //}

                        }
                    }
                }
            }
            while (true)
            {
                string edgeName = $"Edge {totalEdges++}";
                GameObject edge = transform.Find(edgeName)?.gameObject;
                if (edge == null)
                {
                    break;
                }
                // var lineRender = edge.GetComponent<LineRenderer>();
                // lineRender.SetPositions(new Vector3[] { });
                //edge.GetComponent<MeshFilter>().mesh = null;
                //var mesh = edge.GetComponent<MeshFilter>().mesh;
                //Debug.Log($"Clear {edgeName}");
                //mesh.Clear();
                var tube = edge.GetComponent<TubeRenderer>();

                tube.SetPositions(new Vector3[0]);
                tube.SetIndices(new int[0]);
            }
        }
       
    }
}