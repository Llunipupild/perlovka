using Laba1.Vertexes.Model;
using UnityEngine;

namespace Laba1.Arcs.Model
{
    public class Arc : MonoBehaviour
    {
        public Vertex FirstVertex { get; private set; }
        public Vertex SecondVertex { get; private set;}
        
        public void Init(Vertex firstVertex, Vertex secondVertex)
        {
            FirstVertex = firstVertex;
            SecondVertex = secondVertex;
        }
    }
}