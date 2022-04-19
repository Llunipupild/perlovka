using System.Collections.Generic;
using JetBrains.Annotations;
using Laba1.App.Service;
using Laba1.Buttons;
using Laba1.DrawingArea.Controller;
using Laba1.Table.Controller;
using Laba1.Vertexes.Model;
using UnityEngine;

namespace Laba1.DijkstrasAlgorithm.Service
{
    public class FindPathService
    {
        private DrawingAreaController _drawingAreaController;
        private ButtonsController _buttonsController;
        private TableController _tableController;

        private List<Vertex> _vertices;
        private Dictionary<string, int> _verticesLabels;
        private Dictionary<string, bool> _visitedVertices;
        private Dictionary<string, string> _intermediateVertices;

        public void Init(AppService appService)
        {
            _drawingAreaController = appService.DrawingAreaController;
            _buttonsController = appService.ButtonsController;
            _tableController = appService.TableController;

            _verticesLabels = new Dictionary<string, int>();
            _visitedVertices = new Dictionary<string, bool>();
            _intermediateVertices = new Dictionary<string, string>();
        }

        public void FindPath(string vertex1Name, string vertex2Name)
        {
            Vertex vertex1 = _drawingAreaController.GetVertexByName(vertex1Name);
            Vertex vertex2 = _drawingAreaController.GetVertexByName(vertex2Name);

            if (vertex1 == null || vertex2 == null) {
                _buttonsController.PrintError();
                return;
            }

            if (vertex2.AdjacentVertices.Count == 0) {
                _drawingAreaController.LockDrawingAreaAndTable();
                _buttonsController.PrintError("Нет пути!");
                return;
            }

            FindPath(vertex1, vertex2);
        }

        public void FindPath(Vertex sourceVertex, Vertex secondVertex)
        {
            ClearResources();
            _vertices = _drawingAreaController.GetVertexes();
            _verticesLabels.Add(sourceVertex.Name, 0);

            InitVisitedVertices();
            VisitVertex(sourceVertex);

            while (ExistNotVisitedVertex(_visitedVertices))
            {
                VisitVertex(GetVertexWithSmallWeight());
            }
            
            List<Vertex> drawVertices = new List<Vertex>();
            Vertex currentVertex = secondVertex;
            while (true)
            {
                drawVertices.Add(currentVertex);
                List<Vertex> nearestVertices = GetNearestVertices(currentVertex);
                Vertex smallWeightVertex = GetVertexWithSmallWeight(nearestVertices, currentVertex);
                if (smallWeightVertex == null) {
                    drawVertices.Add(sourceVertex);
                    break;
                }
                if (smallWeightVertex.Name == sourceVertex.Name) {
                    drawVertices.Add(sourceVertex);
                    break;
                }
                currentVertex = smallWeightVertex;
            }

            _drawingAreaController.LockDrawingAreaAndTable();
            SetArcsColors(drawVertices, Color.magenta);
            PrintResults($"Кратчайший путь = {_verticesLabels[secondVertex.Name]}");
        }

        private void PrintResults(string message = null)
        {
            _drawingAreaController.LockDrawingAreaAndTable();
            _buttonsController.ShowOutputContainer(message);
        }

        private void SetArcsColors(List<Vertex> vertices, Color color)
        {
            _drawingAreaController.SetVerticesColors(vertices, color);
        }

        private void InitVisitedVertices()
        {
            foreach (Vertex vertex in _vertices)
            {
                _intermediateVertices.Add(vertex.Name, "x1");
                _visitedVertices.Add(vertex.Name, false);
                if (_verticesLabels.ContainsKey(vertex.Name)) {
                    continue;
                }

                _verticesLabels.Add(vertex.Name, int.MaxValue);
            }
        }
        
        private void VisitVertex(Vertex sourceVertex)
        {
            foreach (Vertex vertex in sourceVertex.AdjacentVertices)
            {
                string key = _tableController.CombineVertexNames(sourceVertex.Name, vertex.Name);
                int newWeight = _verticesLabels[sourceVertex.Name] + _tableController.GetWeightByKey(key);

                if (newWeight < _verticesLabels[vertex.Name]) {
                    _intermediateVertices[sourceVertex.Name] = vertex.Name;
                    _verticesLabels[vertex.Name] = newWeight;
                }
            }

            _visitedVertices[sourceVertex.Name] = true;
        }

        private void ClearResources()
        {
            _verticesLabels = new Dictionary<string, int>();
            _visitedVertices = new Dictionary<string, bool>();
            _intermediateVertices = new Dictionary<string, string>();
        }
        
        private bool ExistNotVisitedVertex(Dictionary<string, bool> visitedVertices)
        {
            foreach (KeyValuePair<string, bool> visitedVertex in visitedVertices)
            {
                if (visitedVertex.Value == false) {
                    return true;
                }
            }

            return false;
        }

        private Vertex GetVertexWithSmallWeight()
        {
            int min = int.MaxValue;
            string vertexName = string.Empty;

            foreach (KeyValuePair<string, bool> kvp in _visitedVertices)
            {
                if (kvp.Value) {
                    continue;
                }

                if (_verticesLabels[kvp.Key] >= min) {
                    continue;
                }

                min = _verticesLabels[kvp.Key];
                vertexName = kvp.Key;
            }

            return _drawingAreaController.GetVertexByName(vertexName);
        }

        [CanBeNull]
        private Vertex GetVertexWithSmallWeight(List<Vertex> nearestVertex, Vertex sourceVertex)
        {
            int min = int.MaxValue;
            Vertex result = null;
            foreach (Vertex vertex in nearestVertex)
            {
                if (sourceVertex.Name == vertex.Name) {
                    continue;
                }
                int nextValue = _tableController.GetWeightByKey($"{sourceVertex.Name}_{vertex.Name}");

                if (nextValue <= 0) {
                    continue;
                }

                if (nextValue < min) {
                    result = vertex;
                    min = nextValue;
                }
            }

            return result;
        }

        private List<Vertex> GetNearestVertices(Vertex sourceVertex)
        {
            List<Vertex> result = new List<Vertex>();
            foreach (KeyValuePair<string, string> keyValuePair in _intermediateVertices)
            {
                if (keyValuePair.Value == sourceVertex.Name) {
                    result.Add(_drawingAreaController.GetVertexByName(keyValuePair.Key));
                }
            }

            return result;
        }
    }
}