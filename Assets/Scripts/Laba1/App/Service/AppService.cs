using Laba1.Buttons;
using Laba1.DijkstrasAlgorithm.Service;
using Laba1.DrawingArea.Controller;
using Laba1.Maths;
using Laba1.SaveAndLoad.Service;
using Laba1.Table.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] 
        private Button _exitButton;

        private GameObject _mainDialogGameObject;
        public TableController TableController { get; private set; }
        public DrawingAreaController DrawingAreaController { get; private set; }
        public MathematicalCalculations MathematicalCalculations { get; private set; }
        public ButtonsController ButtonsController { get; private set; }
        public SaveLoadService SaveLoadService { get; private set; }
        public FindPathService FindPathService { get; private set; }
        public int CountVertex { get; private set; }
        
        private void Start()
        {
            _inputField.onEndEdit.AddListener(StartApp);
            _exitButton.onClick.AddListener(OnExitButton);
        }

        private void OnDestroy() 
        {
            _inputField.onEndEdit.RemoveListener(StartApp);
            _exitButton.onClick.RemoveListener(OnExitButton);
        }

        public void Restart()
        {
            Destroy(_mainDialogGameObject);
            _startDialog.SetActive(true);
        }

        private void OnExitButton() 
        {
            SceneManager.LoadScene("MainScene");
        }
        
        private void StartApp(string arg)
        {
            if (_inputField.text != string.Empty) {
                CountVertex = int.Parse(_inputField.text);
            }
            if (CountVertex >= 11 || CountVertex <= 1) {
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
            FindPathService = new FindPathService();
            
            _mainDialogGameObject = Instantiate(_mainDialog, _canvas.transform);
            TableController = _mainDialogGameObject.GetComponent<TableController>();
            DrawingAreaController = _mainDialogGameObject.GetComponent<DrawingAreaController>();
            ButtonsController = _mainDialogGameObject.GetComponent<ButtonsController>();

            TableController.Init(this);
            DrawingAreaController.Init(this);
            ButtonsController.Init(this);
            FindPathService.Init(this);
        }
    }
}