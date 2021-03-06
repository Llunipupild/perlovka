using System.Collections.Generic;
using System.Linq;
using Laba1.App.Service;
using Laba1.Arcs.Model;
using Laba1.DrawingArea.Controller;
using Laba1.Table.Controller;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Laba1.Vertexes.Model
{
    public class Vertex : MonoBehaviour, IDragHandler ,IBeginDragHandler, IEndDragHandler
    {
        private readonly List<Arc> _arcs = new List<Arc>();
        private readonly List<Vertex> _adjacentVertices = new List<Vertex>();

        private DrawingAreaController _drawingAreaController;
        private TableController _tableController;

        private Dictionary<string, int> _arcsWeigh = new Dictionary<string, int>();
        
        public string Name { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public List<Arc> Arcs => _arcs;
        public List<Vertex> AdjacentVertices => _adjacentVertices;

        public void Init(string vertexName, Vector2 position, AppService app)
        {
            Name = vertexName;
            X = position.x;
            Y = position.y;

            _tableController = app.TableController;
            _drawingAreaController = app.DrawingAreaController;
        }

        public void AddArc(Arc arc)
        {
            _arcs.Add(arc);
        }

        public void RemoveArc(Arc arc)
        {
            _arcs.Remove(arc);
        }

        public void AddAdjacentVertex(Vertex vertex)
        {
            if (_adjacentVertices.Contains(vertex)) {
                return;
            }
            
            _adjacentVertices.Add(vertex);
        }
        
        public void SetNewPosition(Vector2 position)
        {
            X = position.x;
            Y = position.y;
            gameObject.transform.position = position;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(X, Y);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_drawingAreaController.Blocked) {
                return;
            }
            
            foreach (Arc arc in _arcs) {
                string key = _tableController.CombineVertexNames(arc.FirstVertex.Name, Name);
                _arcsWeigh.Add(arc.FirstVertex.Name == Name ? arc.SecondVertex.Name : arc.FirstVertex.Name, _tableController.GetWeightByKey(key));
            }
            
            int countArcs = _arcs.Count;
            for (int i = 0; i < countArcs; i++) {
                _drawingAreaController.DeleteArc(_arcs.First(), false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == null) {
                return;
            }
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Vertex>() == null) {
                return;
            }

            Vertex vertex = eventData.pointerCurrentRaycast.gameObject.GetComponent<Vertex>();
            if (!_drawingAreaController.CanMoved(eventData.pointerCurrentRaycast.gameObject.transform.position, vertex.Name)) {
                return;
            }
            
            eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
            X += eventData.delta.x;
            Y += eventData.delta.y;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (KeyValuePair<string, int> arc in _arcsWeigh) {
                _drawingAreaController.CreateArc(GetPosition(), _drawingAreaController.GetVertexByName(arc.Key).GetPosition(), false);
            }
            
            _arcsWeigh.Clear();
            _drawingAreaController.UnlockDrawingAreaAndTable();
        }
    }
}