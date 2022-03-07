using System;
using Laba1.DrawingArea.Controller;
using Laba1.Table.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.App.Service
{
    public class AppService : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _canvas;
        [SerializeField]
        private GameObject _startDialog;
        [SerializeField]
        private GameObject _mainDialog;
        [SerializeField]
        private InputField _inputField;
        
        private void Start()
        {
            _inputField.onEndEdit.AddListener(StartApp);
        }
        
        private void StartApp(string arg)
        {
            int countVertex = 0;
            
            if (_inputField.text != string.Empty)
            {
                countVertex = int.Parse(_inputField.text);
            }
            if (countVertex >= 11 || countVertex <= 1)
            {
                return;
            }
            
            _inputField.text = string.Empty;
            _startDialog.SetActive(false);
            CreateMainDialog(countVertex);
        }

        private void CreateMainDialog(int countVertex)
        {
            GameObject mainDialog = Instantiate(_mainDialog, _canvas.transform);
            TableController tableController = mainDialog.GetComponent<TableController>();
            DrawingAreaController drawingAreaController = mainDialog.GetComponent<DrawingAreaController>();
            
            tableController.Init(countVertex);
            drawingAreaController.Init(countVertex);
        }
    }
}