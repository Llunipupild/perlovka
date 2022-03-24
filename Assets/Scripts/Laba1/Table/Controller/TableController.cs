﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Laba1.App.Service;
using Laba1.Arcs.Model;
using Laba1.DrawingArea.Controller;
using Laba1.Maths;
using Laba1.Table.TableInputField;
using Laba1.Vertexes.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.Table.Controller
{
    public class TableController : MonoBehaviour
    {
        private const int START_CELL_POSITION = -10;
        private const int START_INPUT_FIELD_POSITION = -390;
        private const int START_UP_X_POSITION = -350;
        private const int START_LEFT_X_POSITION = 520;
        private const int BASE_DISTANCE = 70;
        private const string ZERO = "0";

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
        
        private List<GameObject> _partsTable = new List<GameObject>();
        private DrawingAreaController _drawingAreaController;
        private MathematicalCalculations _mathematicalCalculations;
        private int _countVertex;
        public Dictionary<string,InputField> InputFields { get; private set;}
        
        public void Init(AppService appService)
        {
            _mathematicalCalculations = appService.MathematicalCalculations;
            _drawingAreaController = appService.DrawingAreaController;
            InputFields = new Dictionary<string, InputField>();
            CreateTable(appService.CountVertex);
        }
        
        public void CreateTable(int countVertex, [CanBeNull] Dictionary<string, string> dictionary = null)
        {
            _countVertex = countVertex;
            CreateEntity(START_UP_X_POSITION, BASE_DISTANCE, _upTitle, _title.transform);
            CreateEntity(START_LEFT_X_POSITION,BASE_DISTANCE + 20, _leftTitle,_column.transform, false, false);
            CreateEntity(START_CELL_POSITION,BASE_DISTANCE +20, _cell,_table.transform,false,false, true);
            
            if (dictionary != null)
            {
                SetTableValue(dictionary);
            }
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
        
        public void DeleteTable()
        {
            int countPartsTable = _partsTable.Count;
            InputFields.Clear();
            
            for (int i = 0; i < countPartsTable; i++)
            {
                GameObject temp = _partsTable.First();
                _partsTable.Remove(temp);
                Destroy(temp);
            }
        }

        public int GetWeightByKey(string key)
        {
            return int.Parse(InputFields.FirstOrDefault(k => k.Key == key).Value.text);
        }

        public string CombineVertexNames(string vertex1, string vertex2)
        {
            return vertex1 + '_' + vertex2;
        }
        
        public Dictionary<string, string> GetGraph()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (KeyValuePair<string,InputField> keyValuePair in InputFields)
            {
                dictionary[keyValuePair.Key] = keyValuePair.Value.text;
            }

            return dictionary;
        }
        
        private void CreateEntity(float startPosition, float distance, GameObject entity, Transform parent, bool plus = true, bool X = true, bool isCell = false)
        {
            float value = startPosition;
            for (int i = 1; i < _countVertex+1; i++)
            {
                GameObject temp = Instantiate(entity, parent);
                _partsTable.Add(temp);
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
                    _partsTable.Add(inputField);
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
                
                if (keyValuePair.Value.text == InputFields[secondKey].text)
                {
                    continue;
                }
                
                if (keyValuePair.Value.text == ZERO)
                {
                    keyValuePair.Value.text = string.Empty;
                    InputFields[secondKey].text = string.Empty;
                    Vertex firstVertex = _drawingAreaController.GetVertexByName(GetHalfString(keyValuePair.Key));
                    Vertex secondVertex = _drawingAreaController.GetVertexByName(GetHalfString(secondKey));
                    _drawingAreaController.DeleteArc(firstVertex, secondVertex);
                    return;
                }
                
                if (keyValuePair.Value.text == tableCell.PreviousValue)
                {
                    CreateGraph(keyValuePair.Key, secondKey);
                    tableCell.PreviousValue = InputFields[secondKey].text;
                    keyValuePair.Value.text = InputFields[secondKey].text;
                    return;
                }

                if (InputFields[secondKey].text == tableCell.PreviousValue)
                {
                    CreateGraph(keyValuePair.Key, secondKey);
                    keyValuePair.Value.text = text;
                    InputFields[secondKey].text = text;
                    return;
                }
                
                keyValuePair.Value.text = text;
                InputFields[secondKey].text = text;
            }
        }
        
        private void AddText(GameObject obj, int order)
        {
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"x{order}";
            obj.name = text.text;
        }
        
        private void SetTableValue(Dictionary<string, string> dictionary)
        {
            foreach (KeyValuePair<string,string> inputField in dictionary)
            {
                InputFields[inputField.Key].text = inputField.Value;
                InputFields[inputField.Key].onEndEdit.Invoke(InputFields[inputField.Key].text);
            }
        }

        private void CreateGraph(string firstKey, string secondKey)
        {
            Vector2 vertex1Position = _mathematicalCalculations.GetRandomPosition();
            Vector2 vertex2Position = _mathematicalCalculations.GetRandomPosition();
            string key1 = GetHalfString(firstKey);
            string key2 = GetHalfString(secondKey);
            bool existVertex1 = _drawingAreaController.ExistVertexByName(key1);
            bool existVertex2 = _drawingAreaController.ExistVertexByName(key2);
            
            switch (existVertex1)
            {
                case false when !existVertex2:
                    _drawingAreaController.CreateVertex(vertex1Position, key1);
                    _drawingAreaController.CreateVertex(vertex2Position, key2);
                    _drawingAreaController.CreateArc(vertex1Position, vertex2Position);
                    break;
                case true when !existVertex2:
                    _drawingAreaController.CreateVertex(vertex2Position, key2);
                    _drawingAreaController.CreateArc(key1, key2);
                    break;
                case false when existVertex2:
                    _drawingAreaController.CreateVertex(vertex1Position, key1);
                    _drawingAreaController.CreateArc(key1, key2);
                    break;
                default:
                    _drawingAreaController.CreateArc(key1, key2);
                    break;
            }
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

            return result + secondPart;
        }

        private string GetHalfString(string key)
        {
            string result = string.Empty;
            foreach (var symbol in key)
            {
                if (symbol == '_')
                {
                    return result;
                }
                
                result += symbol;
            }

            return result;
        }
        
        private Vector2 SetNewPosition(RectTransform rectTransform, float value, bool y = false)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            return y ? new Vector2(anchoredPosition.x, value) : new Vector2(value, anchoredPosition.y);
        }
    }
}