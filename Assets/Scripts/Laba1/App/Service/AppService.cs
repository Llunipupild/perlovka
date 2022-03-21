using Laba1.DrawingArea.Controller;
using Laba1.Maths;
using Laba1.SaveAndLoad.Controller;
using Laba1.SaveAndLoad.Service;
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

        public TableController TableController { get; private set; }
        public DrawingAreaController DrawingAreaController { get; private set; }
        public MathematicalCalculations MathematicalCalculations { get; private set; }
        public SaveLoadButtonsController SaveLoadButtonsController { get; private set; }
        public SaveLoadService SaveLoadService { get; private set; }
        public int CountVertex { get; private set; }
        
        private void Start()
        {
            _inputField.onEndEdit.AddListener(StartApp);
        }
        
        private void StartApp(string arg)
        {
            if (_inputField.text != string.Empty)
            {
                CountVertex = int.Parse(_inputField.text);
            }
            if (CountVertex >= 11 || CountVertex <= 1)
            {
                return;
            }
            
            _inputField.text = string.Empty;
            _startDialog.SetActive(false);
            CreateAllControllers();
        }

        private void CreateAllControllers()
        {
            MathematicalCalculations = new MathematicalCalculations();
            SaveLoadService = new SaveLoadService();
            
            GameObject mainDialog = Instantiate(_mainDialog, _canvas.transform);
            TableController = mainDialog.GetComponent<TableController>();
            DrawingAreaController = mainDialog.GetComponent<DrawingAreaController>();
            SaveLoadButtonsController = mainDialog.GetComponent<SaveLoadButtonsController>();

            TableController.Init(this);
            DrawingAreaController.Init(this);
            SaveLoadButtonsController.Init(this);
        }
    }
}