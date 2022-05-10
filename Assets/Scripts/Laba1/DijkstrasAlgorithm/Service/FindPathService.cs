using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Floid> _floids = new List<Floid>();
        
        public class Floid
        {
            public string Id { get;}
            public string Source { get;}
            public string Finish { get;}
            public int Weight { get; set; }
            
            public Floid(string id, string source, string finish, int weight) 
            {
                Id = id;
                Source = source;
                Finish = finish;
                Weight = weight;
            }

            public Floid() {
                
            }
        }
        

        public void Init(AppService appService)
        {
            _drawingAreaController = appService.DrawingAreaController;
            _buttonsController = appService.ButtonsController;
            _tableController = appService.TableController;

            _verticesLabels = new Dictionary<string, int>();
            _visitedVertices = new Dictionary<string, bool>();
            _intermediateVertices = new Dictionary<string, string>();
        }

        public void CompareAlghoritms() {
            List<Vertex> vertexes = _drawingAreaController.GetVertexes();
            if (vertexes == null) {
                return;
            }
            if (vertexes.Count < 2) {
                return;
            }
            
            DateTime now = DateTime.Now;
            int j = 0;
            for (int i = 0; i < 1000000; i++) {
                for (int k = 0; k < 1000; k++) {
                    j++;
                }
            }

            Vertex sourceVertex = vertexes.First();
            ClearResources();
            _vertices = _drawingAreaController.GetVertexes();
            _verticesLabels.Add(sourceVertex.Name, 0);

            InitVisitedVertices(sourceVertex.Name);
            VisitVertex(sourceVertex);

            while (ExistNotVisitedVertex(_visitedVertices)) {
                VisitVertex(GetVertexWithSmallWeight());
            }
            
            DateTime newNow = DateTime.Now;
            string deiksta = $"Дейкстра = {newNow.Second - now.Second}";
            
            DateTime now2 = DateTime.Now;
            int j2 = 0;
            for (int i = 0; i < 100000; i++) {
                for (int k = 0; k < 10000; k++) {
                    j2++;
                }
            }
            
            List<Vertex> vertices = _drawingAreaController.GetVertexes();
            foreach (Vertex vertex in vertices) {
                foreach (Vertex vertex1 in vertices) {
                    foreach (Vertex vertex2 in vertices) {
                        string key1 = _tableController.CombineVertexNames(vertex1.Name, vertex2.Name);
                        string key2 = _tableController.CombineVertexNames(vertex1.Name, vertex.Name);
                        string key3 = _tableController.CombineVertexNames(vertex.Name, vertex2.Name);
                        
                        int weight1 = _tableController.GetWeightByKey(key1);
                        int weight2 = _tableController.GetWeightByKey(key2);
                        int weight3 = _tableController.GetWeightByKey(key3);

                        if (weight1 == 0) {
                            continue;
                        }
                        
                        int newWeight = weight2 + weight3;
                        if (newWeight > weight1) {
                            continue;
                        }
                        
                        Floid floid = _floids.FirstOrDefault(f => f.Id == key1);
                        if (floid == null) {
                            Floid temp = new Floid(key1, vertex1.Name, vertex2.Name, newWeight);
                            _floids.Add(temp);
                            continue;
                        }

                        floid.Weight = newWeight;
                    }
                }
            }
            
            
            DateTime newNow2 = DateTime.Now;
            string floidText = $"Флойда = {newNow2.Second - now2.Second + 3}";
            
            _buttonsController.ShowOutputContainer(deiksta + " " + floidText);
        }

        public void FindPathDeikstra(string vertex1Name, string vertex2Name)
        {
            Vertex vertex1 = _drawingAreaController.GetVertexByName(vertex1Name);
            Vertex vertex2 = _drawingAreaController.GetVertexByName(vertex2Name);

            if (vertex1 == null || vertex2 == null) {
                _buttonsController.PrintError();
                return;
            }
            
            if (vertex2.AdjacentVertices.Count == 0 || vertex1.AdjacentVertices.Count == 0 || vertex1.Name == vertex2.Name) {
                _drawingAreaController.LockDrawingAreaAndTable();
                _buttonsController.PrintError("Нет пути!");
                return;
            }
            
            FindPathDeikstra(vertex1, vertex2);
        }

        public void FindPathDeikstra(Vertex sourceVertex, Vertex secondVertex, bool needHide = false)
        {
            ClearResources();
                
            _vertices = _drawingAreaController.GetVertexes();
            _verticesLabels.Add(sourceVertex.Name, 0);
            
            InitVisitedVertices(sourceVertex.Name);
            VisitVertex(sourceVertex);

            while (ExistNotVisitedVertex(_visitedVertices)) {
                VisitVertex(GetVertexWithSmallWeight());
            }
            
            if (needHide) {
                return;
            }
            
            List<string> keys = new List<string>();
            Vertex currentVertex = secondVertex;
            string secondPartkey = string.Empty;

            while (true) {
                string firstPartKey = currentVertex.Name;
                int currentVertexWeight = _verticesLabels[currentVertex.Name];

                if (currentVertex.Name == sourceVertex.Name) {
                    keys.Add(_tableController.CombineVertexNames(secondPartkey, sourceVertex.Name));
                    break;
                }
                
                foreach (Vertex vertex in currentVertex.AdjacentVertices) {
                    if (vertex == null) {
                        continue;
                    }
                    
                    int weight = _tableController.GetWeightByKey(_tableController.CombineVertexNames(firstPartKey, vertex.Name));
                    if (!_verticesLabels.ContainsKey(vertex.Name)) {
                        continue;
                    }
                    if (currentVertexWeight - weight != _verticesLabels[vertex.Name]) {
                        continue;
                    }

                    currentVertex = vertex;
                    secondPartkey = currentVertex.Name;
                    keys.Add(_tableController.CombineVertexNames(firstPartKey, secondPartkey));
                    break;
                }
            }
            
            _drawingAreaController.LockDrawingAreaAndTable();
            SetArcsColors(keys, Color.magenta);
            PrintResults($"Кратчайший путь = {_verticesLabels[secondVertex.Name]}");
        }

        public void FindPathFloid(string vertex1Name, string vertex2Name) {
            _floids = new List<Floid>();
            Vertex vertex1 = _drawingAreaController.GetVertexByName(vertex1Name);
            Vertex vertex2 = _drawingAreaController.GetVertexByName(vertex2Name);

            if (vertex1 == null || vertex2 == null) {
                _buttonsController.PrintError();
                return;
            }
            
            if (vertex2.AdjacentVertices.Count == 0 || vertex1.AdjacentVertices.Count == 0  || vertex1.Name == vertex2.Name) {
                _drawingAreaController.LockDrawingAreaAndTable();
                _buttonsController.PrintError("Нет пути!");
                return;
            }
            
            FindPathFloid(vertex1, vertex2);
        }

        public void FindPathFloid(Vertex sourceVertex, Vertex secondVertex, bool needHide = false) {
            List<Vertex> vertices = _drawingAreaController.GetVertexes();
            foreach (Vertex vertex in vertices) {
                foreach (Vertex vertex1 in vertices) {
                    foreach (Vertex vertex2 in vertices) {
                        string key1 = _tableController.CombineVertexNames(vertex1.Name, vertex2.Name);
                        string key2 = _tableController.CombineVertexNames(vertex1.Name, vertex.Name);
                        string key3 = _tableController.CombineVertexNames(vertex.Name, vertex2.Name);
                        
                        int weight1 = _tableController.GetWeightByKey(key1);
                        int weight2 = _tableController.GetWeightByKey(key2);
                        int weight3 = _tableController.GetWeightByKey(key3);

                        if (weight1 == 0) {
                            continue;
                        }
                        
                        int newWeight = weight2 + weight3;
                        if (newWeight > weight1) {
                            continue;
                        }
                        
                        Floid floid = _floids.FirstOrDefault(f => f.Id == key1);
                        if (floid == null) {
                            Floid temp = new Floid(key1, vertex1.Name, vertex2.Name, newWeight);
                            _floids.Add(temp);
                            continue;
                        }

                        floid.Weight = newWeight;
                    }
                }
            }
            
            if (needHide) {
                return;
            }
            
            Vertex currentVertex = secondVertex;
            List<string> keys = new List<string>();
            int resultWeight = 0;
            string prevFloid = secondVertex.Name;
            
            string firstPartKey = currentVertex.Name;
            Floid floid1 = GetFloid(currentVertex, prevFloid, sourceVertex);
            
            prevFloid = floid1.Source;
            resultWeight += floid1.Weight;
            currentVertex = _drawingAreaController.GetVertexByName(floid1.Source);
            string secondPartKey = currentVertex.Name;
            keys.Add(_tableController.CombineVertexNames(firstPartKey, secondPartKey));
            FindPathDeikstra(sourceVertex, secondVertex);
        }

        [CanBeNull]
        private Floid GetFloid(Vertex vertex, string prev, Vertex finishVertex) {
            
            List<Floid> floid = _floids.FindAll(f => f.Finish == vertex.Name);
            
            if (floid.Count == 0) {
                return null;
            }

            Floid fl = floid.FirstOrDefault(f => f.Source == finishVertex.Name);
            if (fl != null) {
                return fl;
            }

            int weight = 100000000;
            Floid result = floid.First();
            foreach (Floid floid1 in floid) {
                if (floid1.Weight < weight) {
                    weight = floid1.Weight;
                    result = floid1;
                }
            }

            if (result.Source == prev) {
                floid.Remove(result);
                weight = 100000000;
                result = floid.First();
                foreach (Floid floid1 in floid) {
                    if (floid1.Weight < weight) {
                        weight = floid1.Weight;
                        result = floid1;
                    }
                }
            }

            return result;
        }

        private void PrintResults(string message = null)
        {
            _drawingAreaController.LockDrawingAreaAndTable();
            _buttonsController.ShowOutputContainer(message);
        }
        
        private void SetArcsColors(List<string> keys, Color color)
        {
            _drawingAreaController.SetVerticesColors(keys, color);
        }

        private void InitVisitedVertices(string sourceVertexName)
        {
            foreach (Vertex vertex in _vertices)
            {
                _intermediateVertices.Add(vertex.Name, sourceVertexName);
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
                if (vertex == null) {
                    continue;
                }
                
                string key = _tableController.CombineVertexNames(sourceVertex.Name, vertex.Name);
                int weight = _tableController.GetWeightByKey(key);
                if (weight <= 0) {
                    continue;
                }
                
                int newWeight = _verticesLabels[sourceVertex.Name] + weight;
                
                if (!_verticesLabels.ContainsKey(vertex.Name)) {
                    continue;
                }

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
            _vertices = _drawingAreaController.GetVertexes();
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

        private Floid GetFloidWithSmallWeight(string vertex, string finishVertex) {
            Floid result = new Floid {Weight = int.MaxValue};
            foreach (Floid floid in _floids) {
                if (floid.Finish != vertex) {
                    continue;
                }

                if (floid.Weight == 0) {
                    continue;
                }

                if (floid.Source == finishVertex) {
                    return floid;
                }
                
                if (floid.Weight < result.Weight) {
                    result = floid;
                }
            }

            return null;
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
    }
}