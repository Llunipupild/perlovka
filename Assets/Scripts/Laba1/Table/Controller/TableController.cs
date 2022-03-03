using System.Collections.Generic;
using Laba1.App.Service;
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

        [SerializeField]
        private GameObject _table;
        [SerializeField] 
        private GameObject _column;
        [SerializeField] 
        private GameObject _title;
        [SerializeField]
        private GameObject _upX;
        [SerializeField]
        private GameObject _leftX;
        [SerializeField]
        private GameObject _cell;
        [SerializeField]
        private GameObject _inputField;

//todo проверить минус
        public Dictionary<string,InputField> InputFields { get; private set;}
        
        public AppService _appService;
        public int _countVertex;

        private void Start()
        {
            InputFields = new Dictionary<string, InputField>();
            CreateEntity(START_UP_X_POSITION,BASE_DISTANCE,_upX,_title.transform);
            CreateEntity(START_LEFT_X_POSITION,BASE_DISTANCE + 20,_leftX,_column.transform, false, false);
            CreateEntity(START_CELL_POSITION,BASE_DISTANCE +20,_cell,_table.transform,false,false, true);
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
                    RectTransform rect = inputField.GetComponent<RectTransform>();
                    inputFieldX += BASE_DISTANCE;
                        
                    InputField inputFieldComponent = inputField.GetComponentInChildren<InputField>();
                    inputFieldComponent.placeholder.GetComponent<Text>().text = $"x{j}";
                    rect.anchoredPosition = SetNewPosition(rect, inputFieldX);
                    
                    InputFields.Add($"x{i}_{j}",inputFieldComponent);
                    
                    if (i == j)
                    {
                        inputFieldComponent.readOnly = true;
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
        private Vector2 SetNewPosition(RectTransform rectTransform, float value, bool Y = false)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            return Y ? new Vector2(anchoredPosition.x, value) : new Vector2(value, anchoredPosition.y);
        }
    }
}