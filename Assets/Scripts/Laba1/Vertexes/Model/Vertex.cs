using System.Collections.Generic;
using Laba1.Arcs.Model;
using UnityEngine;

namespace Laba1.Vertexes.Model
{
    public class Vertex : MonoBehaviour
    {
        private readonly List<Arc> _arcs = new List<Arc>();
        private readonly List<Vertex> _adjacentVertices = new List<Vertex>();
        
        public string Name { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public List<Vertex> AdjacentVertices => _adjacentVertices;
        public List<Arc> Arcs => _arcs;
        
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
        
        public void RemoveArc(Arc arc)
        {
            _arcs.Remove(arc);
        }
    }
}