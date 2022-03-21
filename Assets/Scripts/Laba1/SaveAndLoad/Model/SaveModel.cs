using System.Collections.Generic;

namespace Laba1.SaveAndLoad.Model
{
    public class SaveModel
    {
        public int CountVertex { get;}
        public Dictionary<string, string> Graph { get; }
        
        public SaveModel(int countVertex, Dictionary<string,string> graph)
        {
            CountVertex = countVertex;
            Graph = graph;
        }
    }
}