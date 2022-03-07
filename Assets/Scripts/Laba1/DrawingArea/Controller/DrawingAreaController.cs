using System.Collections.Generic;
using Laba1.Arcs.Model;
using Laba1.Maths;
using Laba1.Vertexes.Model;
using UnityEngine;

namespace Laba1.DrawingArea.Controller
{
    public class DrawingAreaController : MonoBehaviour
    {
        private const float MIN_DISTANCE_VERTEX = 150f;
        private const float MIN_DISTANCE_CLICK = 15f;
        private const float SIZE_ARC_COEFFICIENT = 1.3f;
        private const float SIZE_ARC_FAULT = 80;
        
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
        
        public void Init(int countVertex, MathematicalCalculations mathematicalCalculations)
        {
            _countVertex = countVertex;
            _mathematicalCalculations = mathematicalCalculations;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                foreach (Vertex vertex in _vertexes)
                {
                    Vector2 vertexPosition = new Vector2(vertex.X, vertex.Y);
                    if (_mathematicalCalculations.CheckDistanceBetweenEachOther(vertexPosition, mousePosition, MIN_DISTANCE_VERTEX))
                    {
                        return;
                    }
                }
                
                AddVertex(mousePosition);
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
                Vector2 mousePosition = Input.mousePosition;
                foreach (Vertex vertex in _vertexes)
                {
                    Vector2 vertexPosition = new Vector2(vertex.X, vertex.Y);
                    if (!_mathematicalCalculations.CheckDistanceBetweenEachOther(vertexPosition, mousePosition,
                        MIN_DISTANCE_CLICK))
                    {
                        continue;
                    }
                    
                    DeleteVertex(vertex);
                    return;
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
            if (!_mathematicalCalculations.CheckPositionCorrections(position))
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
            Vector2 newPosition = _mathematicalCalculations.CalculateNewObjectPosition(positions);
            double newAngle = _mathematicalCalculations.CalculateNewObjectAngle(positions);
            int newSize = _mathematicalCalculations.CalculateNewObjectSize(positions, SIZE_ARC_COEFFICIENT, SIZE_ARC_FAULT);
            
            arc.transform.position = new Vector2(newPosition.x, newPosition.y);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize);
            
            Vector3 eulerAngles = arc.transform.eulerAngles;
            arc.transform.localEulerAngles = new Vector3(
                eulerAngles.x,
                eulerAngles.y, 
                eulerAngles.z + (float)newAngle
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
        
        private GameObject CreateObject(GameObject obj, Transform parent)
        {
            return Instantiate(obj, parent);
        }
    }
}