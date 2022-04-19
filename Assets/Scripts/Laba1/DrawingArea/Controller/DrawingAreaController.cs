using System.Collections.Generic;
using System.Linq;
using Laba1.App.Service;
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

        private AppService _appService;
        private MathematicalCalculations _mathematicalCalculations;
        private TableController _tableController;
        
        private GameObject _changeWeightDialogObject;
        private InputField _changeWeightInputField;
        private Arc _selectedArc;
        
        private List<Vertex> _vertexes = new List<Vertex>();
        private List<Arc> _arcs = new List<Arc>();
        
        private Vector2 _startPositionNewArc;
        private Vector2 _endPositionNewArc;
        
        private bool _isCreateArc;
        private bool _isBlock;
        private int _countVertex;

        private Dictionary<string, bool> _occupiedVertexesNames = new Dictionary<string, bool>
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

        public void Init(AppService appService)
        {
            _appService = appService;
            _countVertex = appService.CountVertex;
            _mathematicalCalculations = appService.MathematicalCalculations;
            _tableController = appService.TableController;
            
            _changeWeightDialogObject = Instantiate(_changeWeightDialog, transform);
            _changeWeightInputField = _changeWeightDialogObject.GetComponentInChildren<InputField>();
            _changeWeightInputField.onEndEdit.AddListener(ChangeWeightArc);
            _changeWeightDialogObject.SetActive(false);
        }
        
        private void Update()
        {
            if (_isBlock)
            {
                return;
            }
            
            //создание вершины
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
                
                CreateVertex(mousePosition);
                return;
            }
            
            //создание дуги
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
                CreateArc(_startPositionNewArc, _endPositionNewArc);
                return;
            }
            
            //удаление вершины
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

            //удаление дуги
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Arc arc = GetArcByMouseClick(Input.mousePosition);
                if (arc != null)
                {
                    DeleteArc(arc);
                }
                return;
            }

            //изменение веса
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                _selectedArc = GetArcByMouseClick(Input.mousePosition);
                _tableController.SetTableStatus(true);
                _changeWeightDialogObject.SetActive(true);
                _isBlock = true;
            }

            //перетаскивание вершины
            if (Input.GetKey(KeyCode.Mouse0)) {
                
            }
        }
        
        public void SetVerticesColors(List<Vertex> vertices, Color color)
        {
            foreach (Vertex vertex in vertices)
            {
                if (vertex == null) {
                    continue;
                }
                
                foreach (Vertex vertexAdjacentVertex in vertex.AdjacentVertices)
                {
                    if (vertices.Contains(vertexAdjacentVertex)) {
                        Arc arc = _arcs.FirstOrDefault(a => a.name == $"arc{vertex.Name}{vertexAdjacentVertex.Name}");
                        if (arc == null) {
                            arc = _arcs.FirstOrDefault(a => a.name == $"arc{vertexAdjacentVertex.Name}{vertex.Name}");
                        }

                        arc!.gameObject.GetComponent<Image>().color = color;
                    }
                }
            }
            
        }
        
        public void CreateArc(string vertexName1, string vertexName2)
        {
            Vertex vertex1 = _vertexes.FirstOrDefault(v => v.Name == vertexName1);
            Vertex vertex2 = _vertexes.FirstOrDefault(v => v.Name == vertexName2);
            
            if (vertex1 != null && vertex2 != null)
            {
                CreateArc(vertex1.GetPosition(), vertex2.GetPosition());
            }
        }
        
        public void CreateArc(Vector2 startPosition, Vector2 endPosition)
        {
            if (IsMaxArcCount())
            {
                return;
            }
            List<Vertex> arcVertexes = FindArcVertexes(startPosition, endPosition);
            if (arcVertexes[0] == null || arcVertexes[1] == null)
            {
                return;
            }
            if (HasSuchArc(arcVertexes))
            {
                return;
            }
            
            GameObject arc = CreateObject(_arc, _arcsContainer);
            Arc arcComponent = arc.GetComponent<Arc>();
            SetArcParameters(arc, arcVertexes);
            _arcs.Add(arcComponent);
            _tableController.UpdateTable(arcComponent);
            arcVertexes[0].AddAdjacentVertex(arcVertexes[1]);
            arcVertexes[1].AddAdjacentVertex(arcVertexes[0]);
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
        
        public void DeleteArc(Arc arc, bool UpdateTable = true)
        {
            if (arc == null)
            {
                return;
            }
            
            arc.FirstVertex.RemoveArc(arc);
            arc.SecondVertex.RemoveArc(arc);

            if (UpdateTable)
            {
                _tableController.UpdateTable(arc, "");
            }
            
            _arcs.Remove(arc);
            Destroy(arc.gameObject);
        }
        
        public void CreateVertex(Vector2 position, string vertexName = null)
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
            SetVertexParameters(vertex, position, vertexName);
            _vertexes.Add(vertex.GetComponent<Vertex>());
        }

        //если будет не лень, то переименовать
        public bool CanMoved(Vector2 newPosition)
        {
            foreach (Vertex vertex in _vertexes)
            {
                if (_mathematicalCalculations.CheckDistanceBetweenEachOther(newPosition, vertex.GetPosition(), MIN_DISTANCE_VERTEX))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ExistVertexByName(string vertexName)
        {
            return _vertexes.Exists(v => v.Name == vertexName);
        }
        
        public Vertex GetVertexByName(string vertexName)
        {
            return _vertexes.FirstOrDefault(v => v.Name == vertexName);
        }

        public List<Vertex> GetVertexes()
        {
            return _vertexes;
        }
        
        public void SetArcsColor(Color color)
        {
            foreach (Arc arc in _arcs)
            {
                arc.gameObject.GetComponent<Image>().color = color;
            }
        }
        
        private void ChangeWeightArc(string text)
        {
            LockDrawingAreaAndTable();
            if (text == string.Empty)
            {
                return;
            }
            if (_selectedArc == null)
            {
                UnlockDrawingAreaAndTable();
                return;
            }
            
            _tableController.UpdateTable(_selectedArc, text);
            UnlockDrawingAreaAndTable();
        }
        
        private void SetArcVertices(List<Vertex> arcVertexes, Arc arc)
        {
            arcVertexes[0].AddArc(arc);
            arcVertexes[1].AddArc(arc);
        }
        
        private void SetArcParameters(GameObject arc, List<Vertex> vertices)
        {
            List<Vector2> positions = new List<Vector2>
            {
                new Vector2(vertices[0].X, vertices[0].Y), 
                new Vector2(vertices[1].X, vertices[1].Y)
            };
            
            RectTransform rectTransform = arc.GetComponent<RectTransform>();
            Vector2 newPosition = _mathematicalCalculations.CalculateNewObjectPosition(positions);
            double newAngle = _mathematicalCalculations.CalculateNewObjectAngle(positions);
            int newSize = _mathematicalCalculations.CalculateNewObjectSize(positions);
            
            arc.transform.position = new Vector2(newPosition.x, newPosition.y);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize);
            
            Vector3 eulerAngles = arc.transform.eulerAngles;
            arc.transform.localEulerAngles = new Vector3(
                eulerAngles.x,
                eulerAngles.y, 
                eulerAngles.z + (float)newAngle
            );
            
            arc.name = $"arc{vertices[0].Name}{vertices[1].Name}";
            Arc arcComponent = arc.GetComponent<Arc>();
            arcComponent.Init(vertices[0], vertices[1]);
            SetArcVertices(vertices, arcComponent);
        }

        private void SetVertexParameters(GameObject vertex, Vector3 position, string vertName = null)
        {
            string vertexName = vertName ?? GetFreeName();
            _occupiedVertexesNames[vertexName] = true;
            vertex.transform.position = position;
            vertex.name = vertexName;
            vertex.GetComponentInChildren<TextMeshProUGUI>().text = vertexName;
            vertex.GetComponent<Vertex>().Init(vertexName, position, _appService);
        }
        
        private void DeleteVertex(Vertex vertex)
        {
            int arcsCount = vertex.Arcs.Count;
            for (int i = 0; i < arcsCount; i++)
            {
                DeleteArc(vertex.Arcs.First());
            }
            
            _occupiedVertexesNames[vertex.Name] = false;
            _vertexes.Remove(vertex);
            Destroy(vertex.gameObject);
        }

        public void LockDrawingAreaAndTable()
        {
            _isBlock = true;
            _tableController.SetTableStatus(true);
        }
        
        //todo здесь наверное чото сломал
        public void UnlockDrawingAreaAndTable()
        {
            _isBlock = false;
            _changeWeightDialogObject.SetActive(false);
            _tableController.SetTableStatus(false);
        }
        
        private string GetFreeName()
        {
            return _occupiedVertexesNames.First(n => n.Value == false).Key;
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

        private bool IsMaxArcCount()
        {
            return _arcs.Count >= _countVertex * 2;
        }
        
        private GameObject CreateObject(GameObject obj, Transform parent)
        {
            return Instantiate(obj, parent);
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
        
        private List<Vertex> FindArcVertexes(Vector2 position1, Vector2 position2)
        {
            List<Vertex> result = new List<Vertex>
            {
                GetNearestVertex(position1), 
                GetNearestVertex(position2)
            };

            return result;
        }
    }
}