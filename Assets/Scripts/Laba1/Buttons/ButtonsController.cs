using System.Collections.Generic;
using System.Linq;
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
        private const string DEIKSTRA = "Deikstra";
        private const string FLOID = "Floid";
        private const string COMPARE = "CompareAlgorithmsButton";
        private const string OUTPUT = "Output";
        private const string OUTPUT_CONTAINER = "OutputContainer";
        private const string CLOSE = "CloseButton";
        
        [SerializeField] 
        private Button _saveButton;
        [SerializeField] 
        private Button _loadButton;
        [SerializeField] 
        private Button _findPathButton;
        [SerializeField] 
        private Button _exitButton;
        [SerializeField] 
        private Button _printTableButton;
        [SerializeField] 
        private Button _infoButton;
        [SerializeField] 
        private Button _infoButtonObject;
        [SerializeField] 
        private Button _exitButtonTable;
        [SerializeField] 
        private GameObject _findPathDialog;
        [SerializeField] 
        private GameObject _table;
        [SerializeField] 
        private GameObject _text;
        [SerializeField] 
        private bool _isLb2;

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
        private Button _compareButton;
        private Button _deikstraButton;
        private Button _floidButton;
        
        public void Init(AppService appService)
        {
            _countVertex = appService.CountVertex;
            _tableController = appService.TableController;
            _saveLoadService = appService.SaveLoadService;
            _findPathService = appService.FindPathService;
            _drawingAreaController = appService.DrawingAreaController;
            _appService = appService;
            
            _findPathDialogObject = Instantiate(_findPathDialog, transform);
            
            _inputFieldVertex1 = GameObject.Find(CONTAINER1).GetComponent<InputField>();
            _inputFieldVertex2 = GameObject.Find(CONTAINER2).GetComponent<InputField>();
            _outputText = GameObject.Find(OUTPUT).GetComponent<TextMeshProUGUI>();
            _outputContainer = GameObject.Find(OUTPUT_CONTAINER);
            _closeButton = GameObject.Find(CLOSE).GetComponent<Button>();
            
            if (_isLb2) {
                _compareButton = GameObject.Find(COMPARE).GetComponent<Button>();
                _deikstraButton = GameObject.Find(DEIKSTRA).GetComponent<Button>();
                _floidButton = GameObject.Find(FLOID).GetComponent<Button>();
                
                _compareButton.onClick.AddListener(OnCompareButtonClick);
                _deikstraButton.onClick.AddListener(OnDeikstraClick);
                _floidButton.onClick.AddListener(OnFloidCLick);
                _printTableButton.onClick.AddListener(OnPrintTableButtonClick);
                _exitButtonTable.onClick.AddListener(OnTableClick);
                _table.SetActive(false);
            }
            
            _inputFieldVertex1.onEndEdit.AddListener(OnChangeInputFields);
            _inputFieldVertex2.onEndEdit.AddListener(OnChangeInputFields);
            
            _infoButton.onClick.AddListener(OnInfoButtonClick);
            _infoButtonObject.onClick.AddListener(OnInfoObjectClick);
            
            _saveButton.onClick.AddListener(OnSaveButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _findPathButton.onClick.AddListener(OnFindPathButton);
            _exitButton.onClick.AddListener(OnExitButtonClick);
            _closeButton.onClick.AddListener(OnCloseButton);
            
            _infoButtonObject.gameObject.SetActive(false);
            _findPathDialogObject.SetActive(false);
            _outputContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            _inputFieldVertex1.onEndEdit.RemoveListener(OnChangeInputFields);
            _inputFieldVertex2.onEndEdit.RemoveListener(OnChangeInputFields);
            
            _infoButton.onClick.RemoveListener(OnInfoButtonClick);
            _infoButtonObject.onClick.RemoveListener(OnInfoObjectClick);
            _saveButton.onClick.RemoveListener(OnSaveButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _findPathButton.onClick.RemoveListener(OnFindPathButton);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
            _closeButton.onClick.RemoveListener(OnCloseButton);

            if (!_isLb2) {
                return;
            }
            
            _compareButton.onClick.RemoveListener(OnCompareButtonClick);
            _deikstraButton.onClick.RemoveListener(OnDeikstraClick);
            _floidButton.onClick.RemoveListener(OnFloidCLick);
            _exitButtonTable.onClick.RemoveListener(OnTableClick);
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

        private void OnInfoButtonClick() 
        {
            _infoButtonObject.gameObject.SetActive(true);
            _drawingAreaController.LockDrawingAreaAndTable();
        }

        private void OnInfoObjectClick() 
        {
            _infoButtonObject.gameObject.SetActive(false);
            _drawingAreaController.UnlockDrawingAreaAndTable();
        }

        private void OnTableClick() 
        {
            _drawingAreaController.UnlockDrawingAreaAndTable();
            _table.gameObject.SetActive(false);
            
            //todo люди придмула функции и есть serializefeild, но мне надоела эта лаба
            GameObject startContainer = GameObject.Find("StartContainer");
            GameObject finishContainer = GameObject.Find("FinishContainer");
            GameObject pathContainer = GameObject.Find("PathContainer");
            GameObject pathLengthContainer = GameObject.Find("PathLengthContainer");

            if (startContainer == null) {
                return;
            }
            if (finishContainer == null) {
                return;
            }
            if (pathContainer == null) {
                return;
            }
            if (pathLengthContainer == null) {
                return;
            }

            List<GameObject> childs = new List<GameObject>();
            for (int i = 0; i < startContainer.transform.childCount; i++) {
                childs.Add(startContainer.transform.GetChild(i).gameObject);
            }

            int childCount = childs.Count;
            for (int i = 0; i < childCount; i++) {
                Destroy(childs.First());
            }
            
            childs = new List<GameObject>();
            for (int i = 0; i < finishContainer.transform.childCount; i++) {
                childs.Add(finishContainer.transform.GetChild(i).gameObject);
            }

            childCount = childs.Count;
            for (int i = 0; i < childCount; i++) {
                Destroy(childs.First());
            }
            
            childs = new List<GameObject>();
            for (int i = 0; i < pathContainer.transform.childCount; i++) {
                childs.Add(pathContainer.transform.GetChild(i).gameObject);
            }

            childCount = childs.Count;
            for (int i = 0; i < childCount; i++) {
                Destroy(childs.First());
            }
            
            childs = new List<GameObject>();
            for (int i = 0; i < pathLengthContainer.transform.childCount; i++) {
                childs.Add(pathLengthContainer.transform.GetChild(i).gameObject);
            }

            childCount = childs.Count;
            for (int i = 0; i < childCount; i++) {
                Destroy(childs.First());
            }
        }

        private void OnSaveButtonClick()
        {
            if (_drawingAreaController.Blocked) {
                return;
            }
            
            if (_saveLoadService.HasKey(KEY)) {
                _saveLoadService.Delete(KEY);
            }

            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
            foreach (Vertex vertex in _drawingAreaController.GetVertexes()) {
                positions[vertex.Name] = vertex.GetPosition();
            }

            Dictionary<string,string> graph = _tableController.GetGraph();
            SaveModel saveModel = new SaveModel(_countVertex, graph, positions);
            _saveLoadService.Set(saveModel, KEY);
        }

        private void OnLoadButtonClick()
        {
            if (_drawingAreaController.Blocked) {
                return;
            }
            
            SaveModel saveModel = _saveLoadService.Get(KEY);
            _tableController.DeleteTable();
            _tableController.CreateTable(saveModel.CountVertex, saveModel.Graph, saveModel.Positions);
        }

        private void OnFindPathButton()
        {
            if (_drawingAreaController.Blocked) {
                return;
            }
            
            _findPathDialogObject.SetActive(true);
            _drawingAreaController.LockDrawingAreaAndTable();
        }

        private void OnExitButtonClick()
        {
            _appService.Restart();
        }

        private void OnCompareButtonClick() {
            _findPathDialogObject.SetActive(false);
            _inputFieldVertex1.text = string.Empty;
            _inputFieldVertex2.text = string.Empty;
            
            _findPathService.CompareAlghoritms();
        }

        private void OnPrintTableButtonClick() {
            _drawingAreaController.LockDrawingAreaAndTable();
            List<FindPathService.Table> result = _findPathService.FindAll();
            _table.SetActive(true);
            
            if (result == null) {
                _table.SetActive(false);
                _drawingAreaController.UnlockDrawingAreaAndTable();
                return;
            }
            
            GameObject startContainer = GameObject.Find("StartContainer");
            GameObject finishContainer = GameObject.Find("FinishContainer");
            GameObject pathContainer = GameObject.Find("PathContainer");
            GameObject pathLengthContainer = GameObject.Find("PathLengthContainer");

            foreach (FindPathService.Table table in result) {
                GameObject start = Instantiate(_text, startContainer.transform);
                start.GetComponent<TextMeshProUGUI>().text = table.Source;
                
                GameObject finish = Instantiate(_text, finishContainer.transform);
                finish.GetComponent<TextMeshProUGUI>().text = table.Finish;
                
                GameObject path = Instantiate(_text, pathContainer.transform);
                path.GetComponent<TextMeshProUGUI>().text = table.Path;
                
                GameObject lengthPath = Instantiate(_text, pathLengthContainer.transform);
                lengthPath.GetComponent<TextMeshProUGUI>().text = table.LengthPath.ToString();
            }
        }

        private void OnDeikstraClick() 
        {
            if (_inputFieldVertex1.text == string.Empty || _inputFieldVertex2.text == string.Empty) {
                return;
            }
            
            _findPathService.FindPathDeikstra(_inputFieldVertex1.text, _inputFieldVertex2.text);
            _findPathDialogObject.SetActive(false);
            _inputFieldVertex1.text = string.Empty;
            _inputFieldVertex2.text = string.Empty;
        }

        private void OnFloidCLick() {
            if (_inputFieldVertex1.text == string.Empty || _inputFieldVertex2.text == string.Empty) {
                return;
            }
            
            _findPathService.FindPathFloid(_inputFieldVertex1.text, _inputFieldVertex2.text);
            _findPathDialogObject.SetActive(false);
            _inputFieldVertex1.text = string.Empty;
            _inputFieldVertex2.text = string.Empty;
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
            if (_isLb2) {
                return;
            }
            if (_inputFieldVertex1.text == string.Empty || _inputFieldVertex2.text == string.Empty) {
                return;
            }
            
            _findPathService.FindPathDeikstra(_inputFieldVertex1.text, _inputFieldVertex2.text);
            _findPathDialogObject.SetActive(false);
            _inputFieldVertex1.text = string.Empty;
            _inputFieldVertex2.text = string.Empty;
        }
    }
}