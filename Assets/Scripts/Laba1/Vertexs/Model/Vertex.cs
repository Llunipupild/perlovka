using System.Collections.Generic;
using Laba1.Arcs.Model;
using UnityEngine;

namespace Laba1.Vertexs.Model
{
    public class Vertex : MonoBehaviour
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public List<Vertex> AdjacentVertices => _adjacentVertices;
        public List<Arc> Arcs => _arcs;

        private List<Arc> _arcs = new List<Arc>();
        private List<Vertex> _adjacentVertices = new List<Vertex>();
        
        public void Init(string vertexName, Vector2 position)
        {
            Name = vertexName;
            X = position.x;
            Y = position.y;
        }

        public void AddAdjacentVertex(Vertex vertex)
        {
            _adjacentVertices.Add(vertex);
        }
        
        public void RemoveAdjacentVertex(Vertex vertex)
        {
            _adjacentVertices.Remove(vertex);
        }
        
        public void AddArc(Arc arc)
        {
            _arcs.Add(arc);
        }
        
        public void Removearc(Arc arc)
        {
            _arcs.Remove(arc);
        }
    }
}