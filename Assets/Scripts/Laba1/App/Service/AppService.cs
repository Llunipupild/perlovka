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

        public GameObject Canvas
        {
            get => _canvas;
        }
        
        private void Start()
        {
            _inputField.onEndEdit.AddListener(StartApp);
        }
        
        private void StartApp(string arg)
        {
            int countVertex = int.Parse(_inputField.text);
            if (countVertex >= 11 || countVertex <= 1) return;
            
            _inputField.text = string.Empty;
            _startDialog.SetActive(false);
            CreateMainDialog(countVertex);
        }

        private void CreateMainDialog(int countVertex)
        {
            GameObject temp = Instantiate(_mainDialog, _canvas.transform);
            TableController tableController = temp.GetComponent<TableController>();
            DrawingAreaController drawingAreaController = temp.GetComponent<DrawingAreaController>();
            
            tableController._appService = this;
            tableController._countVertex = countVertex;
            drawingAreaController._appService = this;
            drawingAreaController._countVertex = countVertex;
        }
    }
}