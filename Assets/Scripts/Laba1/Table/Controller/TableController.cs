using System.Collections.Generic;
using System.Linq;
using Laba1.Arcs.Model;
using Laba1.DrawingArea.Controller;
using Laba1.Table.TableInputField;
using Laba1.Vertexes.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Laba1.Table.Controller
{
    public class TableController : MonoBehaviour
    {
        private const int START_CELL_POSITION = -10;
        private const int START_INPUT_FIELD_POSITION = -390;
        private const int START_UP_X_POSITION = -350;
        private const int START_LEFT_X_POSITION = 520;
        private const int BASE_DISTANCE = 70;

        [SerializeField]
        private GameObject _table;
        [SerializeField] 
        private GameObject _column;
        [SerializeField] 
        private GameObject _title;
        [SerializeField]
        private GameObject _upTitle;
        [SerializeField]
        private GameObject _leftTitle;
        [SerializeField]
        private GameObject _cell;
        [SerializeField]
        private GameObject _inputField;

        private int _countVertex;
        private DrawingAreaController _drawingAreaController;
        public Dictionary<string,InputField> InputFields { get; private set;}
        
        public void Init(int countVertex, DrawingAreaController drawingAreaController)
        {
            _countVertex = countVertex;
            _drawingAreaController = drawingAreaController;
            InputFields = new Dictionary<string, InputField>();
            
            CreateEntity(START_UP_X_POSITION,BASE_DISTANCE,_upTitle,_title.transform);
            CreateEntity(START_LEFT_X_POSITION,BASE_DISTANCE + 20,_leftTitle,_column.transform, false, false);
            CreateEntity(START_CELL_POSITION,BASE_DISTANCE +20,_cell,_table.transform,false,false, true);
        }
        
        public void UpdateTable(Arc arc)
        {
            int arcWeight = Random.Range(4, 31);
            UpdateTable(arc, arcWeight.ToString());
        }

        public void UpdateTable(Arc arc, string arcWeight)
        {
            string inputFieldsKey1 = $"{arc.FirstVertex.Name}_{arc.SecondVertex.Name}";
            string inputFieldsKey2 = $"{arc.SecondVertex.Name}_{arc.FirstVertex.Name}";
            
            TableCell tableCell1 = InputFields[inputFieldsKey1].GetComponentInParent<TableCell>();
            TableCell tableCell2 = InputFields[inputFieldsKey2].GetComponentInParent<TableCell>();

            tableCell1.PreviousValue = tableCell1.CurrentValue;
            tableCell1.CurrentValue = arcWeight;
            tableCell2.PreviousValue = tableCell2.CurrentValue;
            tableCell2.CurrentValue = arcWeight;
            
            InputFields[inputFieldsKey1].text = arcWeight;
            InputFields[inputFieldsKey2].text = arcWeight;
        }

        public void SetTableStatus(bool status)
        {
            foreach (InputField inputFieldsValue in InputFields.Values)
            {
                inputFieldsValue.readOnly = status;
            }
        }
        
        private void CreateEntity(float startPosition, float distance, GameObject entity, Transform parent, bool plus = true, bool X = true, bool isCell = false)
        {
            float value = startPosition;
            for (int i = 1; i < _countVertex+1; i++)
            {
                GameObject temp = Instantiate(entity, parent);
                RectTransform rectTransform = temp.GetComponent<RectTransform>();
                value = plus ? value + distance : value - distance;
                rectTransform.anchoredPosition = X ? SetNewPosition(rectTransform, value) : 
                                                     SetNewPosition(rectTransform, value, true);

                if (!isCell)
                {
                    AddText(temp,i);
                    continue;
                }
                
                float inputFieldX = START_INPUT_FIELD_POSITION;
                for (int j = 1; j < _countVertex+1; j++)
                {
                    string inputFieldKey = $"x{i}_x{j}";
                    GameObject inputField = Instantiate(_inputField, temp.transform);
                    inputField.AddComponent<TableCell>().Init(inputFieldKey, "");
                    RectTransform rect = inputField.GetComponent<RectTransform>();
                    inputFieldX += BASE_DISTANCE;
                        
                    InputField inputFieldComponent = inputField.GetComponentInChildren<InputField>();
                    inputFieldComponent.onEndEdit.AddListener(OnChangeTable);
                    inputFieldComponent.placeholder.GetComponent<Text>().text = $"x{j}";
                    rect.anchoredPosition = SetNewPosition(rect, inputFieldX);
                    
                    InputFields.Add(inputFieldKey,inputFieldComponent);
                    
                    if (i == j)
                    {
                        inputFieldComponent.readOnly = true;
                    }
                }
            }
        }

        private void OnChangeTable(string text)
        {
            foreach (KeyValuePair<string,InputField> keyValuePair in InputFields)
            {
                TableCell tableCell = keyValuePair.Value.GetComponentInParent<TableCell>();
                string secondKey = ReverseKey(tableCell.Key);

                if (keyValuePair.Value.text == tableCell.CurrentValue)
                {
                    continue;
                }

                if (keyValuePair.Value.text == 0.ToString())
                {
                    keyValuePair.Value.text = string.Empty;
                    string secKey = ReverseKey(keyValuePair.Key);
                    InputFields[secKey].text = string.Empty;
                    Vertex firstVertex = _drawingAreaController._vertexes.First(v => v.Name == GetHalfString(keyValuePair.Key));
                    Vertex secondVertex = _drawingAreaController._vertexes.First(v => v.Name == GetHalfString(secKey));
                    _drawingAreaController.DeleteArc(firstVertex, secondVertex);
                }

                if (keyValuePair.Value.text != InputFields[secondKey].text)
                {
                    if (keyValuePair.Value.text == tableCell.PreviousValue)
                    {
                        Vector2 vertex1 = GetRandomPosition();
                        Vector2 vertex2 = GetRandomPosition();
                        string key1 = GetHalfString(keyValuePair.Key);
                        string key2 = GetHalfString(secondKey);
                        
                        if (_drawingAreaController._vertexes.Exists(v => v.Name == key1) ||
                            _drawingAreaController._vertexes.Exists(v => v.Name == key2))
                        {
                            _drawingAreaController.AddArc(vertex1, vertex2,key1,key2);
                        }
                        else
                        {
                            _drawingAreaController.AddVertex(vertex1, key1);
                            _drawingAreaController.AddVertex(vertex2, key2);
                            _drawingAreaController.AddArc(vertex1, vertex2);
                        }
                        
                        tableCell.PreviousValue = InputFields[secondKey].text;
                        keyValuePair.Value.text = InputFields[secondKey].text;
                        return;
                    }
                    if (InputFields[secondKey].text == tableCell.PreviousValue)
                    {
                        Vector2 vertex1 = GetRandomPosition();
                        Vector2 vertex2 = GetRandomPosition();
                        string key1 = GetHalfString(secondKey);
                        string key2 = GetHalfString(keyValuePair.Key);

                        if (!_drawingAreaController._vertexes.Exists(v => v.Name == key1) &&
                            !_drawingAreaController._vertexes.Exists(v => v.Name == key2))
                        {
                            _drawingAreaController.AddVertex(vertex1, key1);
                            _drawingAreaController.AddVertex(vertex2, key2);
                            _drawingAreaController.AddArc(vertex1, vertex2);
                        }
                        else if(_drawingAreaController._vertexes.Exists(v => v.Name == key1) && 
                                !_drawingAreaController._vertexes.Exists(v => v.Name == key2))
                        {
                            _drawingAreaController.AddVertex(vertex2, key2);
                            _drawingAreaController.AddArc(vertex1, vertex2, key1, key2);
                        }
                        else if(!_drawingAreaController._vertexes.Exists(v => v.Name == key1) &&
                                _drawingAreaController._vertexes.Exists(v => v.Name == key2))
                        {
                            _drawingAreaController.AddVertex(vertex1, key1);
                            _drawingAreaController.AddArc(vertex1, vertex2, key1, key2);
                        }
                        else
                        {
                            _drawingAreaController.AddArc(vertex1, vertex2, key1, key2);
                        }

                        keyValuePair.Value.text = text;
                        tableCell.PreviousValue = text;
                        InputFields[secondKey].text = text;
                    }
                }
            }
        }
        
        private void AddText(GameObject obj, int order)
        {
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"x{order}";
            obj.name = text.text;
        }
        
        private string ReverseKey(string key)
        {
            string secondPart = string.Empty;
            string result = string.Empty;
            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] == '_')
                {
                    for (int j = i+1; j < key.Length; j++)
                    {
                        result += key[j];
                    }

                    result += '_';
                    break;
                }

                secondPart += key[i];
            }

            return result+secondPart;
        }

        private string GetHalfString(string key)
        {
            string result = string.Empty;
            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] == '_')
                {
                    return result;
                }

                result += key[i];
            }

            return result;
        }

        private Vector2 GetRandomPosition()
        {
            Vector2 result = new Vector2();
            float x = Random.Range(Screen.width * 0.45f, Screen.width);
            float y = Random.Range(Screen.height * 0.1f, Screen.height);
            result.x = x;
            result.y = y;

            return result;
        }
        
        private Vector2 SetNewPosition(RectTransform rectTransform, float value, bool Y = false)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            return Y ? new Vector2(anchoredPosition.x, value) : new Vector2(value, anchoredPosition.y);
        }
    }
}