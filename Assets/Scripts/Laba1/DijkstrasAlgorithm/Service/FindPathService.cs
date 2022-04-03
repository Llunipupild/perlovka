using System.Collections.Generic;
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
        private Dictionary<string, string> _vertPath;
        
        public void Init(AppService appService)
        {
            _drawingAreaController = appService.DrawingAreaController;
            _buttonsController = appService.ButtonsController;
            _tableController = appService.TableController;
        }
        
        public void FindPath(string vertex1Name, string vertex2Name)
        {
            Vertex vertex1 = _drawingAreaController.GetVertexByName(vertex1Name);
            Vertex vertex2 = _drawingAreaController.GetVertexByName(vertex2Name);

            if (vertex1 == null || vertex2 == null)
            {
                _buttonsController.PrintError();
                return;
            }

            FindPath(vertex1, vertex2);
        }

        public void FindPath(Vertex sourceVertex, Vertex secondVertex)
        {
            _vertices = _drawingAreaController.GetVertexes();
            _verticesLabels = new Dictionary<string, int>();
            _visitedVertices = new Dictionary<string, bool>();
            _vertPath = new Dictionary<string, string>();
            
            _verticesLabels.Add(sourceVertex.Name, 0);
            InitVisitedVertices();
            VisitVertex(sourceVertex);

            while (ExistNotVisitedVertex(_visitedVertices))
            {
                VisitVertex(GetVertexWithSmallWeight());
            }

            foreach (var VARIABLE in _vertPath)
            {
                Debug.Log($"{VARIABLE.Key} + {VARIABLE.Value}");
            }
        }

        private void PrintResults()
        {
            _buttonsController.ShowOutputContainer();
            //вывод пути и изменение палок графа
        }

        private void InitVisitedVertices()
        {
            foreach (Vertex vertex in _vertices)
            {
                _vertPath.Add(vertex.Name, "xxxx");
                _visitedVertices.Add(vertex.Name, false);
                if (_verticesLabels.ContainsKey(vertex.Name))
                {
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
                
                if (newWeight < _verticesLabels[vertex.Name])
                {
                    _vertPath[sourceVertex.Name] = vertex.Name;
                    _verticesLabels[vertex.Name] = newWeight;
                }
            }

            _visitedVertices[sourceVertex.Name] = true;
        }

        private bool ExistNotVisitedVertex(Dictionary<string, bool> visitedVertices)
        {
            foreach (KeyValuePair<string,bool> visitedVertex in visitedVertices)
            {
                if (visitedVertex.Value == false)
                {
                    return true;
                }
            }

            return false;
        }

        private Vertex GetVertexWithSmallWeight()
        {
            int min = int.MaxValue;
            string vertexName = string.Empty;
            
            foreach (KeyValuePair<string,bool> kvp in _visitedVertices)
            {
                if (kvp.Value)
                {
                    continue;
                }
                if (_verticesLabels[kvp.Key] >= min)
                {
                    continue;
                }
                
                min = _verticesLabels[kvp.Key];
                vertexName = kvp.Key;
            }

            return _drawingAreaController.GetVertexByName(vertexName);
        }
    }
}