using Laba1.App.Service;
using Laba1.DijkstrasAlgorithm.Service;
using Laba1.SaveAndLoad.Model;
using Laba1.SaveAndLoad.Service;
using Laba1.Table.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.Buttons
{
    public class ButtonsController : MonoBehaviour
    {
        private const string KEY = "perlLb1";
        private const string CONTAINER1 = "FindVertexContainer1";
        private const string CONTAINER2 = "FindVertexContainer2";
        
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
        private FindPathService _findPathService;
        private SaveLoadService _saveLoadService;
        private GameObject _findPathDialogObject;
        private int _countVertex;

        private InputField _inputFieldVertex1;
        private InputField _inputFieldVertex2;
        
        public void Init(AppService appService)
        {
            _countVertex = appService.CountVertex;
            _tableController = appService.TableController;
            _saveLoadService = appService.SaveLoadService;
            _findPathService = appService.FindPathService;
            _appService = appService;

            _saveButton.onClick.AddListener(OnSaveButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _findPathButton.onClick.AddListener(OnFindPathButton);
            _exitButton.onClick.AddListener(OnExitButtonClick);
            
            _findPathDialogObject = Instantiate(_findPathDialog, transform);
            _inputFieldVertex1 = GameObject.Find(CONTAINER1).GetComponent<InputField>();
            _inputFieldVertex2 = GameObject.Find(CONTAINER2).GetComponent<InputField>();
            _inputFieldVertex1.onEndEdit.AddListener(OnChangeInputFields);
            _inputFieldVertex2.onEndEdit.AddListener(OnChangeInputFields);
            _findPathDialogObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _saveButton.onClick.RemoveListener(OnSaveButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _findPathButton.onClick.RemoveListener(OnFindPathButton);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void OnSaveButtonClick()
        {
            if (_saveLoadService.HasKey(KEY))
            {
                _saveLoadService.Delete(KEY);
            }

            SaveModel saveModel = new SaveModel(_countVertex, _tableController.GetGraph());
            _saveLoadService.Set(saveModel, KEY);
        }

        private void OnLoadButtonClick()
        {
            SaveModel saveModel = _saveLoadService.Get(KEY);
            _tableController.DeleteTable();
            _tableController.CreateTable(saveModel.CountVertex, saveModel.Graph);
        }

        private void OnFindPathButton()
        {
            _findPathDialogObject.SetActive(true);
        }

        private void OnExitButtonClick()
        {
            _appService.Restart();
        }

        private void OnChangeInputFields(string text)
        {
            if (_inputFieldVertex1.text == string.Empty || _inputFieldVertex2.text == string.Empty)
            {
                return;
            }
            
            _findPathService.FindPath(_inputFieldVertex1.text, _inputFieldVertex2.text);
        }
    }
}