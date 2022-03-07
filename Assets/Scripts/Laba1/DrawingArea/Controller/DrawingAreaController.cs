using System;
using System.Collections.Generic;
using System.Linq;
using Laba1.Arcs.Model;
using Laba1.Maths;
using Laba1.Vertexes.Model;
using UnityEngine;

namespace Laba1.DrawingArea.Controller
{
    public class DrawingAreaController : MonoBehaviour
    {
        private const float MIN_DISTANCE_VERTEX = 15f;
        private const float MIN_DISTANCE_CLICK = 10f;
        private const float SIZE_ARC_COEFFICIENT = 1.2f;
        private const float SIZE_ARC_FAULT = 75;
        
        [SerializeField] 
        private GameObject _vertex;
        [SerializeField] 
        private GameObject _arc;
        [SerializeField]
        private Transform _arcsContainer;
        [SerializeField]
        private Transform _vertexContainer;

        private MathematicalCalculations _mathematicalCalculations;
        private List<Vertex> _vertexes = new List<Vertex>();
        private List<Arc> _arcs = new List<Arc>();
        
        private Vector2 _startPositionNewArc;
        private Vector2 _endPositionNewArc;
        
        private bool _isCreateArc;
        private int _countVertex;
        
        public void Init(int countVertex)
        {
            _countVertex = countVertex;
            _mathematicalCalculations = new MathematicalCalculations();
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 position = Input.mousePosition;
                if (!_vertexes.Any(vertex => Math.Abs(vertex.X - position.x) < MIN_DISTANCE_CLICK && 
                                             Math.Abs(vertex.Y - position.y) < MIN_DISTANCE_CLICK))
                {
                    AddVertex(position);
                }
                
                return;
            }
            
            if (Input.GetKey(KeyCode.Space) && !_isCreateArc)
            {
                _isCreateArc = true;
                _startPositionNewArc = Input.mousePosition;
                return;
            }

            if (!Input.GetKey(KeyCode.Space) && _isCreateArc)
            {
                _isCreateArc = false;
                _endPositionNewArc = Input.mousePosition;
                AddArc(_startPositionNewArc, _endPositionNewArc);
                return;
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                Vertex vertex = _vertexes.FirstOrDefault(v => Math.Abs(v.X - Input.mousePosition.x) < MIN_DISTANCE_CLICK && 
                                                                   Math.Abs(v.Y - Input.mousePosition.y) < MIN_DISTANCE_CLICK);
                if (vertex != null)
                {
                    DeleteVertex(vertex);
                }
            }
        }

        private void AddArc(Vector2 startPosition, Vector2 endPosition)
        {
            if (_arcs.Count >= _countVertex * 2)
            {
                return;
            }
            
            GameObject arc = CreateObject(_arc, _arcsContainer);
            Arc arcComponent = arc.GetComponent<Arc>();
            List<Vector2> positions = new List<Vector2>
            {
                startPosition, 
                endPosition
            };

            SetArcParameters(arc, arcComponent, positions);
            _arcs.Add(arcComponent);
        }
        
        private void AddVertex(Vector2 position)
        {
            if (_vertexes.Count == _countVertex)
            {
                return;
            }
            if (!CheckPositionCorrections(position))
            {
                return;
            }
            if (!CheckMinVertexesDistance(position))
            {
                return;
            }

            GameObject vertex = CreateObject(_vertex, _vertexContainer);
            Vertex vertexComponent = vertex.GetComponent<Vertex>();

            SetVertexParameters(vertex, vertexComponent, position);
            _vertexes.Add(vertexComponent);
        }

        private void SetArcParameters(GameObject arc, Arc arcComponent, List<Vector2> positions)
        {
            RectTransform rectTransform = arc.GetComponent<RectTransform>();
            
            Vector2 startPosition = positions[0];
            Vector2 endPosition = positions[1];
            
            float diffX = endPosition.x - startPosition.x;
            float diffY = endPosition.y - startPosition.y;
            float centerX = diffX / 2;
            float centerY = diffY / 2; 
            double length = Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
            double angle = Math.Atan2(diffX, diffY) * -180 / Math.PI;
            
            arc.transform.position = new Vector2(startPosition.x + centerX, startPosition.y + centerY);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (int)((float)length * SIZE_ARC_COEFFICIENT) - SIZE_ARC_FAULT);
            
            Vector3 eulerAngles = arc.transform.eulerAngles;
            arc.transform.localEulerAngles = new Vector3(
                eulerAngles.x,
                eulerAngles.y, 
                eulerAngles.z + (float)angle
            );
            
            arc.name = $"arc{_arcs.Count + 1}";
            //arcComponent.Init();
        }

        private void SetVertexParameters(GameObject vertex, Vertex vertexComponent, Vector3 position)
        {
            string vertexName = $"x{_vertexes.Count + 1}";
            vertex.transform.position = position;
            vertex.name = vertexName;
            
            vertexComponent.Init(vertexName,position);
        }
        
        private void DeleteVertex(Vertex vertex)
        {
            _vertexes.Remove(vertex);
            Destroy(vertex.gameObject);
        }
        
        private bool CheckMinVertexesDistance(Vector2 position)
        {
            foreach (Vertex vertex in _vertexes)
            {
                if (Math.Abs(vertex.X - position.x) < MIN_DISTANCE_VERTEX && 
                    Math.Abs(vertex.Y - position.y) < MIN_DISTANCE_VERTEX)
                {
                    return false;
                }
            }

            return true;
        }
        
        private bool CheckPositionCorrections(Vector2 position)
        {
            return position.x > Screen.width * 0.45f;
        }

        private GameObject CreateObject(GameObject obj, Transform parent)
        {
            return Instantiate(obj, parent);
        }
    }
}