using System;
using System.Collections.Generic;
using System.Linq;
using Laba1.App.Service;
using Laba1.Arcs.Model;
using Laba1.Vertexs.Model;
using UnityEngine;

namespace Laba1.DrawingArea.Controller
{
    public class DrawingAreaController : MonoBehaviour
    {
        private const float MIN_DISTANCE_VERTEX = 10;
        private const float TOLERANCE = 10f;
        
        [SerializeField] 
        private GameObject _vertex;
        [SerializeField] 
        private GameObject _arc;

        private List<Vertex> _vertices = new List<Vertex>();
        private List<Arc> _arcs = new List<Arc>();
        private Canvas _canvas;
        
        public AppService _appService;
        
        public int _countVertex;

        private void Start()
        {
            _canvas = _appService.Canvas.GetComponent<Canvas>();
        }

        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.DrawRay(new Ray(position, Vector3.forward));
        // }
        //
        // private void OnDrawGizmos()
        // {
        //     
        // }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 position = Input.mousePosition;
                if (_vertices.Any(vertex => Math.Abs(vertex.X - position.x) < TOLERANCE && Math.Abs(vertex.Y - position.y) < TOLERANCE))
                {
                    AddArc(position);
                }
                else
                {
                    AddVertex(position);
                }
                
                return;
            }
            
            //удаление ещё подумать
            if (Input.GetMouseButtonUp(1))
            {
                Vertex vertex = _vertices.FirstOrDefault(v => Math.Abs(v.X - Input.mousePosition.x) < TOLERANCE 
                                                               && Math.Abs(v.Y - Input.mousePosition.y) < TOLERANCE);
                if (vertex != null)
                {
                    DeleteVertex(vertex);
                }
            }
        }

        private void AddArc(Vector2 position)
        {
           //  Gizmos.color = Color.blue;
           // // Gizmos.DrawLine(position);
           //  Gizmos.DrawRay(new Ray(position, Vector3.forward));
           
           
           
           // public LineRenderer line = FindObjectOfType<LineRenderer>();
           // Vector3 vec1 = new Vector3(0,0,0);
           // Vector3 vec2 = new Vector3(1,1,1);//координаты точек
           // line.setPosition(0, vec1)//0-начальная точка линии
           // line.setPosition(1, vec2)//1-конечная точка линии
           
        }
        
        private void AddVertex(Vector2 position)
        {
            if (_vertices.Count == _countVertex)
            {
                return;
            }

            if (!CheckPositionCorrections(position))
            {
                return;
            }
            
            //MIN_DISTANCE_VERTEX учитывать 
            
            GameObject vertex = Instantiate(_vertex, _canvas.transform);
            Vertex vertexComponent = vertex.GetComponent<Vertex>();
            
            string vertexName = $"x{_vertices.Count + 1}";
            vertex.transform.position = position;
            vertexComponent.Init(vertexName,position);
            vertex.name = vertexName;
            
            _vertices.Add(vertexComponent);
        }
        
        private void DeleteVertex(Vertex vertex)
        {
            _vertices.Remove(vertex);
            Destroy(vertex.gameObject);
        }
        
        private bool CheckPositionCorrections(Vector2 position)
        {
            return position.x > Screen.width * 0.45f;
        }
    }
}