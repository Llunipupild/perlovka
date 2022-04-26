using System.Collections.Generic;
using Laba1.App.Service;
using Laba1.DijkstrasAlgorithm.Service;
using Laba1.DrawingArea.Controller;
using Laba1.SaveAndLoad.Model;
using Laba1.SaveAndLoad.Service;
using Laba1.Table.Controller;
using Laba1.Vertexes.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.Buttons
{
    public class ButtonsController : MonoBehaviour
    {
        private const string KEY = "perlLb1";
        private const string CONTAINER1 = "FindVertexContainer1";
        private const string CONTAINER2 = "FindVertexContainer2";
        private const string OUTPUT = "Output";
        
        [SerializeField] 
        private Button _saveButton;
        [SerializeField] 
        private Button _loadButton;
        [SerializeField] 
        private Button _findPathButton;
        [SerializeField] 
        private Button _exitButton;
        [SerializeField] 
        private GameObject _findPathDialog;

        private AppService _appService;
        private TableController _tableController;
        private DrawingAreaController _drawingAreaController;
        private FindPathService _findPathService;
        private SaveLoadService _saveLoadService;
        private GameObject _findPathDialogObject;
        private int _countVertex;

        private InputField _inputFieldVertex1;
        private InputField _inputFieldVertex2;
        private TextMeshProUGUI _outputText;
        private GameObject _outputContainer;
        private Button _closeButton;
        
        public void Init(AppService appService)
        {
            _countVertex = appService.CountVertex;
            _tableController = appService.TableController;
            _saveLoadService = appService.SaveLoadService;
            _findPathService = appService.FindPathService;
            _drawingAreaController = appService.DrawingAreaController;
            _appService = appService;

            _saveButton.onClick.AddListener(OnSaveButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _findPathButton.onClick.AddListener(OnFindPathButton);
            _exitButton.onClick.AddListener(OnExitButtonClick);
            
            _findPathDialogObject = Instantiate(_findPathDialog, transform);
            _inputFieldVertex1 = GameObject.Find(CONTAINER1).GetComponent<InputField>();
            _inputFieldVertex2 = GameObject.Find(CONTAINER2).GetComponent<InputField>();
            _outputText = GameObject.Find(OUTPUT).GetComponent<TextMeshProUGUI>();
            _outputContainer = GameObject.Find("OutputContainer");
            _closeButton = GameObject.Find("CloseButton").GetComponent<Button>();
            _inputFieldVertex1.onEndEdit.AddListener(OnChangeInputFields);
            _inputFieldVertex2.onEndEdit.AddListener(OnChangeInputFields);
            _closeButton.onClick.AddListener(OnCloseButton);
            _findPathDialogObject.SetActive(false);
            _outputContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            _saveButton.onClick.RemoveListener(OnSaveButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _findPathButton.onClick.RemoveListener(OnFindPathButton);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
            _closeButton.onClick.RemoveListener(OnCloseButton);
        }

        public void PrintError(string message = "Некорректные данные")
        {
            ShowOutputContainer(message);
            _findPathDialogObject.SetActive(false);
        }

        public void ShowOutputContainer(string message = null)
        {
            _findPathButton.gameObject.SetActive(false);
            _outputContainer.SetActive(true);
            if (message == null) {
                return;
            }

            _outputText.text = message;
        }

        private void OnSaveButtonClick()
        {
            if (_saveLoadService.HasKey(KEY))
            {
                _saveLoadService.Delete(KEY);
            }

            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
            foreach (Vertex vertex in _drawingAreaController.GetVertexes())
            {
                positions[vertex.Name] = vertex.GetPosition();
            }
            
            SaveModel saveModel = new SaveModel(_countVertex, _tableController.GetGraph(), positions);
            _saveLoadService.Set(saveModel, KEY);
        }

        private void OnLoadButtonClick()
        {
            SaveModel saveModel = _saveLoadService.Get(KEY);
            _tableController.DeleteTable();
            _tableController.CreateTable(saveModel.CountVertex, saveModel.Graph, saveModel.Positions);
        }

        private void OnFindPathButton()
        {
            _findPathDialogObject.SetActive(true);
            _drawingAreaController.LockDrawingAreaAndTable();
        }

        private void OnExitButtonClick()
        {
            _appService.Restart();
        }

        private void OnCloseButton()
        {
            _drawingAreaController.SetArcsColor(Color.red);
            _outputContainer.SetActive(false);
            _drawingAreaController.UnlockDrawingAreaAndTable();
            _findPathButton.gameObject.SetActive(true);
        }

        private void OnChangeInputFields(string text)
        {
            if (_inputFieldVertex1.text == string.Empty || _inputFieldVertex2.text == string.Empty) {
                return;
            }
            
            _findPathService.FindPath(_inputFieldVertex1.text, _inputFieldVertex2.text);
            _findPathDialogObject.SetActive(false);
            _inputFieldVertex1.text = string.Empty;
            _inputFieldVertex2.text = string.Empty;
        }
    }
}