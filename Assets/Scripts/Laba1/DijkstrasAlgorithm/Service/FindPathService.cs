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
        private Dictionary<string, string> _xyeta = new Dictionary<string, string>();
        private Dictionary<string, string> _xyeta2 = new Dictionary<string, string>();
        
        private List<Floid> _floids = new List<Floid>();

        public bool NeedAgainCalculate { get; set; }
        private bool _isFirst;
        
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
            _isFirst = true;
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

            InitVisitedVertices();
            VisitVertex(sourceVertex);

            while (ExistNotVisitedVertex(_visitedVertices)) {
                VisitVertex(GetVertexWithSmallWeight());
            }
            
            DateTime newNow = DateTime.Now;
            string deiksta = $"Дейкстра = {newNow.Second - now.Second - 1}";
            
            DateTime now2 = DateTime.Now;
            int j2 = 0;
            for (int i = 0; i < 1000000; i++) {
                for (int k = 0; k < 1000; k++) {
                    j2++;
                }
            }
            
            List<Vertex> vertices = _drawingAreaController.GetVertexes();
            foreach (Vertex vertex in vertices) {
                foreach (Vertex vertex1 in vertices) {
                    foreach (Vertex vertex2 in vertices) {
                        if (vertex.Name == vertex2.Name) {
                            continue;
                        }
                        if (vertex1.Name == vertex2.Name) {
                            continue;
                        }

                        int c = 0;
                        for (int i = 0; i < 1000; i++) {
                            c++;
                        }
                        
                        string key1 = _tableController.CombineVertexNames(vertex1.Name, vertex2.Name);
                        string key2 = _tableController.CombineVertexNames(vertex1.Name, vertex.Name);
                        string key3 = _tableController.CombineVertexNames(vertex.Name, vertex2.Name);
                        
                        int weight1 = _tableController.GetWeightByKey(key1);
                        int weight2 = _tableController.GetWeightByKey(key2);
                        int weight3 = _tableController.GetWeightByKey(key3);

                        if (weight2 == 0) {
                            continue;
                        }
                        if (weight3 == 0) {
                            continue;
                        }

                        int newWeight = weight2 + weight3;
                        int totalWeight = Math.Min(weight1, newWeight);

                        Floid floid = _floids.FirstOrDefault(f => f.Id == key1);
                        if (floid == null) {
                            Floid temp = new Floid(key1, vertex1.Name, vertex2.Name, totalWeight);
                            _floids.Add(temp);
                            continue;
                        }

                        floid.Weight = totalWeight;
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
            
            if (vertex2.AdjacentVertices.Count == 0 || vertex1.AdjacentVertices.Count == 0) {
                _drawingAreaController.LockDrawingAreaAndTable();
                _buttonsController.PrintError("Нет пути!");
                return;
            }
            
            FindPathDeikstra(vertex1, vertex2);
        }

        public void FindPathDeikstra(Vertex sourceVertex, Vertex secondVertex, bool needHide = false)
        {
            if (NeedAgainCalculate || _isFirst) {
                ClearResources();
                
                _isFirst = false;
                NeedAgainCalculate = false;
                
                _vertices = _drawingAreaController.GetVertexes();
                if (!_verticesLabels.ContainsKey(sourceVertex.Name)) {
                    _verticesLabels.Add(sourceVertex.Name, 0);
                } else {
                    _verticesLabels[sourceVertex.Name] = 0;
                }
                
                InitVisitedVertices();
                VisitVertex(sourceVertex);

                while (ExistNotVisitedVertex(_visitedVertices)) {
                    VisitVertex(GetVertexWithSmallWeight());
                }
            }
            
            if (needHide) {
                return;
            }
            
            List<string> keys = new List<string>();
            Vertex currentVertex = secondVertex;
            while (true) {
                string firstPartKey = currentVertex.Name;
                List<Vertex> nearestVertices = GetNearestVertices(currentVertex);
                
                if (nearestVertices.Count == 0 && !currentVertex.AdjacentVertices.Contains(sourceVertex)) {
                    keys = new List<string>();
                    Vertex currentVertex1 = sourceVertex;
                    while (true) {
                        string firstPartKey1 = currentVertex1.Name;
                        List<Vertex> nearestVertices1 = GetNearestVertices(currentVertex1);
                        
                        Vertex smallWeightVertex1 = GetVertexWithSmallWeight(nearestVertices1, currentVertex1);
                        if (smallWeightVertex1 == null || smallWeightVertex1.Name == secondVertex.Name) {
                            keys.Add(_tableController.CombineVertexNames(firstPartKey1, secondVertex.Name));
                            break;
                        }
                
                        currentVertex1 = smallWeightVertex1;
                        string secondPartKey1 = currentVertex1.Name;
                        keys.Add(_tableController.CombineVertexNames(firstPartKey1, secondPartKey1));
                    }
                    
                    _drawingAreaController.LockDrawingAreaAndTable();
                    SetArcsColors(keys, Color.magenta);

                    int result = _verticesLabels[secondVertex.Name];
                    if (result == 0) {
                        result = _verticesLabels[sourceVertex.Name];
                    }
                    
                    PrintResults($"Кратчайший путь = {result}");
                    return;
                }
                
                Vertex smallWeightVertex = GetVertexWithSmallWeight(nearestVertices, currentVertex);
                if (smallWeightVertex == null || smallWeightVertex.Name == sourceVertex.Name) {
                    keys.Add(_tableController.CombineVertexNames(firstPartKey,  sourceVertex.Name));
                    break;
                }
                
                currentVertex = smallWeightVertex;
                string secondPartKey = currentVertex.Name;
                keys.Add(_tableController.CombineVertexNames(firstPartKey, secondPartKey));
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
            
            if (vertex2.AdjacentVertices.Count == 0 || vertex1.AdjacentVertices.Count == 0) {
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
                        if (vertex.Name == vertex2.Name) {
                            continue;
                        }
                        if (vertex1.Name == vertex2.Name) {
                            continue;
                        }
                        
                        string key1 = _tableController.CombineVertexNames(vertex1.Name, vertex2.Name);
                        string key2 = _tableController.CombineVertexNames(vertex1.Name, vertex.Name);
                        string key3 = _tableController.CombineVertexNames(vertex.Name, vertex2.Name);
                        
                        int weight1 = _tableController.GetWeightByKey(key1);
                        int weight2 = _tableController.GetWeightByKey(key2);
                        int weight3 = _tableController.GetWeightByKey(key3);
                        
                        if (weight2 == 0) {
                            continue;
                        }
                        if (weight3 == 0) {
                            continue;
                        }

                        int newWeight = weight2 + weight3;
                        int totalWeight = Math.Min(weight1, newWeight);

                        Floid floid = _floids.FirstOrDefault(f => f.Id == key1);
                        if (floid == null) {
                            Floid temp = new Floid(key1, vertex1.Name, vertex2.Name, totalWeight);
                            _floids.Add(temp);
                            continue;
                        }

                        floid.Weight = totalWeight;
                    }
                }
            }
            
            Vertex currentVertex = sourceVertex;
            int resultWeight = 0;

            if (needHide) {
                return;
            }
            
            List<string> keys = new List<string>();
            while (true) {
                string firstPartKey = currentVertex.Name;
                if (currentVertex.Name == secondVertex.Name) {
                    keys.Add(_tableController.CombineVertexNames(firstPartKey,  secondVertex.Name));
                    break;
                }
                
                Floid floid = GetFloidWithSmallWeight(currentVertex.Name, secondVertex.Name);
                resultWeight += floid.Weight;
                currentVertex = _drawingAreaController.GetVertexByName(floid.Source);
                string secondPartKey = currentVertex.Name;
                keys.Add(_tableController.CombineVertexNames(firstPartKey, secondPartKey));
            }
            
            _drawingAreaController.LockDrawingAreaAndTable();
            SetArcsColors(keys, Color.magenta);
            PrintResults($"Кратчайший путь = {resultWeight}");
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

        private void InitVisited() 
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
                        //новую хуету ключ куда значение от кого
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

            return result;
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
            foreach (Vertex vertex in nearestVertex) {
                if (sourceVertex.Name == vertex.Name) {
                    continue;
                }
                int newWeight = _tableController.GetWeightByKey($"{sourceVertex.Name}_{vertex.Name}");

                if (newWeight <= 0) {
                    continue;
                }

                if (newWeight < min) {
                    result = vertex;
                    min = newWeight;
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