using System.Collections.Generic;
using UnityEngine;

namespace Laba1.SaveAndLoad.Model
{
    public class SaveModel
    {
        public int CountVertex { get;}
        public Dictionary<string, string> Graph { get; }
        public Dictionary<string, Vector2> Positions { get; }
        
        public SaveModel(int countVertex, Dictionary<string,string> graph, Dictionary<string, Vector2> positions)
        {
            CountVertex = countVertex;
            Positions = positions;
            Graph = graph;
        }
    }
}