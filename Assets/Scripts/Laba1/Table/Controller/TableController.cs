using System.Collections.Generic;
using Laba1.Arcs.Model;
using Laba1.DrawingArea.Controller;
using Laba1.Table.TableInputField;
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
                    GameObject inputField = Instantiate(_inputField, temp.transform);
                    string inputFieldKey = $"x{i}_x{j}";
                    inputField.AddComponent<TableCell>().Init(inputFieldKey, "");
                    RectTransform rect = inputField.GetComponent<RectTransform>();
                    inputFieldX += BASE_DISTANCE;
                        
                    InputField inputFieldComponent = inputField.GetComponentInChildren<InputField>();
                    inputFieldComponent.onValueChanged.AddListener(OnChangeTable);
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
            List<InputField> inputFields = new List<InputField>();
            foreach (KeyValuePair<string,InputField> keyValuePair in InputFields)
            {
                
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
                    break;
                }

                secondPart += key[i];
            }

            return result+secondPart;
        }
        
        private void AddText(GameObject obj, int order)
        {
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"x{order}";
            obj.name = text.text;
        }
        
        private Vector2 SetNewPosition(RectTransform rectTransform, float value, bool Y = false)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            return Y ? new Vector2(anchoredPosition.x, value) : new Vector2(value, anchoredPosition.y);
        }
    }
}