using System.Collections.Generic;
using System.Linq;
using Laba1.Arcs.Model;
using Laba1.Maths;
using Laba1.Table.Controller;
using Laba1.Vertexes.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.DrawingArea.Controller
{
    public class DrawingAreaController : MonoBehaviour
    {
        private const float MIN_DISTANCE_VERTEX = 250f;
        private const float MIN_DISTANCE_ARC = 80f;
        private const float MIN_DISTANCE_CLICK = 50f;
        
        [SerializeField] 
        private GameObject _changeWeightDialog;
        [SerializeField] 
        private GameObject _vertex;
        [SerializeField] 
        private GameObject _arc;

        [SerializeField]
        private Transform _arcsContainer;
        [SerializeField]
        private Transform _vertexContainer;
        
        private MathematicalCalculations _mathematicalCalculations;
        private TableController _tableController;
        
        private GameObject _changeWeightDialogObject;
        private Arc _selectedArc;
        private InputField _inputField;
        
        public List<Vertex> _vertexes = new List<Vertex>();
        public List<Arc> _arcs = new List<Arc>();
        
        private Vector2 _startPositionNewArc;
        private Vector2 _endPositionNewArc;
        
        private bool _isCreateArc;
        private bool _isChangeWeightDialog;
        private int _countVertex;

        private Dictionary<string, bool> _vertexesName = new Dictionary<string, bool>()
        {
            {"x1", false},
            {"x2", false},
            {"x3", false},
            {"x4", false},
            {"x5", false},
            {"x6", false},
            {"x7", false},
            {"x8", false},
            {"x9", false},
            {"x10", false}
        };

        public void Init(int countVertex, MathematicalCalculations mathematicalCalculations, TableController tableController)
        {
            _countVertex = countVertex;
            _mathematicalCalculations = mathematicalCalculations;
            _tableController = tableController;
            
            _changeWeightDialogObject = Instantiate(_changeWeightDialog, transform);
            _inputField = _changeWeightDialogObject.GetComponentInChildren<InputField>();
            _inputField.onEndEdit.AddListener(ChangeWeightArc);
            _changeWeightDialogObject.SetActive(false);
        }
        
        private void Update()
        {
            if (_isChangeWeightDialog)
            {
                return;
            }
            
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
                    if (!_mathematicalCalculations.CheckDistanceBetweenEachOther(vertexPosition, mousePosition, MIN_DISTANCE_CLICK))
                    {
                        continue;
                    }
                    
                    DeleteVertex(vertex);
                    return;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector2 mousePosition = Input.mousePosition;
                Arc arc = GetArcByMouseClick(mousePosition);
                if (arc != null)
                {
                    DeleteArc(arc);
                }
                return;
            }

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Vector2 mousePosition = Input.mousePosition;
                Arc arc = GetArcByMouseClick(mousePosition);
                
                _selectedArc = arc;
                _tableController.SetTableStatus(true);
                _isChangeWeightDialog = true;
                _changeWeightDialogObject.SetActive(true);
            }
        }
        
        private void ChangeWeightArc(string arg0)
        {
            if (_inputField.text == string.Empty)
            {
                return;
            }
            if (_selectedArc == null)
            {
                UnlockDrawingAreaAndTable();
                return;
            }
            
            _tableController.UpdateTable(_selectedArc, _inputField.text);
            UnlockDrawingAreaAndTable();
        }
        
        public void AddArc(Vector2 startPosition, Vector2 endPosition, string vertexName1 = null, string vertexName2 = null)
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
            List<Vertex> arcVertexes = FindArcVertexes(positions);
            
            if (arcVertexes[0] == null || arcVertexes[1] == null || HasSuchArc(arcVertexes))
            {
                if (vertexName1 == null && vertexName2 == null)
                {
                    Destroy(arc);
                    return;
                }
                
                arcVertexes.Clear();
                Vertex vertex1 = _vertexes.First(v => v.Name == vertexName1);
                arcVertexes.Add(vertex1);
                Vertex vertex2 = _vertexes.First(v => v.Name == vertexName2);
                arcVertexes.Add(vertex2);
                
                positions.Clear();
                positions.Add(new Vector2(vertex1.X,vertex1.Y));
                positions.Add(new Vector2(vertex2.X, vertex2.Y));
            }
            
            SetArcVertices(arcVertexes, arcComponent);
            SetArcParameters(arc, arcComponent, positions, arcVertexes);
            _arcs.Add(arcComponent);
            _tableController.UpdateTable(arcComponent);
        }

        private void SetArcVertices(List<Vertex> arcVertexes, Arc arc)
        {
            arcVertexes[0].AddArc(arc);
            arcVertexes[1].AddArc(arc);
            arcVertexes[0].AddAdjacentVertex(arcVertexes[1]);
            arcVertexes[1].AddAdjacentVertex(arcVertexes[0]);
        }
        
        private void SetArcParameters(GameObject arc, Arc arcComponent, List<Vector2> positions, List<Vertex> vertices)
        {
            RectTransform rectTransform = arc.GetComponent<RectTransform>();
            Vector2 newPosition = _mathematicalCalculations.CalculateNewObjectPosition(positions);
            double newAngle = _mathematicalCalculations.CalculateNewObjectAngle(positions);
            
            List<Vector2> positionFromSize = new List<Vector2>();
            positionFromSize.Add(new Vector2(vertices[0].X, vertices[0].Y));
            positionFromSize.Add(new Vector2(vertices[1].X, vertices[1].Y));
            
            int newSize = _mathematicalCalculations.CalculateNewObjectSize(positionFromSize);
            
            arc.transform.position = new Vector2(newPosition.x, newPosition.y);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize);
            
            Vector3 eulerAngles = arc.transform.eulerAngles;
            arc.transform.localEulerAngles = new Vector3(
                eulerAngles.x,
                eulerAngles.y, 
                eulerAngles.z + (float)newAngle
            );
            
            arc.name = $"arc{vertices[0].Name}{vertices[1].Name}";
            arcComponent.Init(vertices[0], vertices[1]);
        }
        
        public void DeleteArc(Vertex firstVertex, Vertex secondVertex)
        {
            Arc arc = _arcs.FirstOrDefault(a => a.FirstVertex == firstVertex && a.SecondVertex == secondVertex);
            if (arc == null)
            {
                arc = _arcs.FirstOrDefault(a => a.FirstVertex == secondVertex && a.SecondVertex == firstVertex);
            }
            DeleteArc(arc);
        }
        
        public void DeleteArc(Arc arc)
        {
            arc.FirstVertex.RemoveArc(arc);
            arc.SecondVertex.RemoveArc(arc);
            
            _tableController.UpdateTable(arc, "");
            _arcs.Remove(arc);
            Destroy(arc.gameObject);
        }
        
        public void AddVertex(Vector2 position, string vertexName = null)
        {
            if (_vertexes.Count == _countVertex)
            {
                return;
            }
            if (!_mathematicalCalculations.CheckPositionCorrections(position))
            {
                return;
            }
            if (GetArcByMouseClick(position) != null)
            {
                return;
            }
            if (_vertexes.Exists(v => v.Name == vertexName))
            {
                return;
            }

            GameObject vertex = CreateObject(_vertex, _vertexContainer);
            Vertex vertexComponent = vertex.GetComponent<Vertex>();

            SetVertexParameters(vertex, vertexComponent, position, vertexName);
            _vertexes.Add(vertexComponent);
        }

        private void SetVertexParameters(GameObject vertex, Vertex vertexComponent, Vector3 position, string vertName = null)
        {
            string vertexName = vertName ?? GetFreeName();
            _vertexesName[vertexName] = true;
            vertex.transform.position = position;
            vertex.name = vertexName;
            vertex.GetComponentInChildren<TextMeshProUGUI>().text = vertexName;
            vertexComponent.Init(vertexName,position);
        }
        
        private void DeleteVertex(Vertex vertex)
        {
            int arcsCount = vertex.Arcs.Count;
            
            for (int i = 0; i < arcsCount; i++)
            {
                DeleteArc(vertex.Arcs.First());
            }
            
            _vertexesName[vertex.Name] = false;
            _vertexes.Remove(vertex);
            
            Destroy(vertex.gameObject);
        }
        
        private void UnlockDrawingAreaAndTable()
        {
            _changeWeightDialogObject.SetActive(false);
            _isChangeWeightDialog = false;
            _tableController.SetTableStatus(false);
        }
        
        private string GetFreeName()
        {
            return _vertexesName.First(n => n.Value == false).Key;
        }

        private bool HasSuchArc(List<Vertex> vertices)
        {
            foreach (Arc arc in _arcs)
            {
                if(arc.FirstVertex == vertices[0] && arc.SecondVertex == vertices[1])
                {
                    return true;
                }
                if (arc.FirstVertex == vertices[1] && arc.SecondVertex == vertices[0])
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private Vertex GetNearestVertex(Vector2 position)
        {
            Vertex result = null;
            foreach (Vertex vertex in _vertexes)
            {
                Vector2 vertexPosition = new Vector2(vertex.X, vertex.Y);
                if (!_mathematicalCalculations.CheckDistanceBetweenEachOther(vertexPosition, position,
                    MIN_DISTANCE_CLICK))
                {
                    continue;
                }

                result = vertex;
                break;
            }

            return result;
        }
        
        private List<Vertex> FindArcVertexes(List<Vector2> positions)
        {
            List<Vertex> result = new List<Vertex>();
            result.Add(GetNearestVertex(positions[0]));
            result.Add(GetNearestVertex(positions[1]));

            return result;
        }

        private Arc GetArcByMouseClick(Vector2 mousePosition)
        {
            Arc result = null;
            foreach (Arc arc in _arcs)
            {
                if (!_mathematicalCalculations.CheckDistanceBetweenEachOther(arc.transform.position,mousePosition,MIN_DISTANCE_ARC))
                {
                    continue;
                }

                result = arc;
                break;
            }

            return result;
        }
        
        private GameObject CreateObject(GameObject obj, Transform parent)
        {
            return Instantiate(obj, parent);
        }
    }
}