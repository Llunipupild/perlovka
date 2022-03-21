using Laba1.App.Service;
using Laba1.Maths;
using Laba1.SaveAndLoad.Model;
using Laba1.SaveAndLoad.Service;
using Laba1.Table.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.SaveAndLoad.Controller
{
    public class SaveLoadButtonsController : MonoBehaviour
    {
        private const string KEY = "perlLb1";
        
        [SerializeField] 
        private Button _saveButton;
        [SerializeField] 
        private Button _loadButton;
        [SerializeField] 
        private Button _findPathButton;

        private TableController _tableController;
        private MathematicalCalculations _mathematicalCalculations;
        private SaveLoadService _saveLoadService;
        private int _countVertex;
        
        public void Init(AppService appService)
        {
            _countVertex = appService.CountVertex;
            _tableController = appService.TableController;
            _saveLoadService = appService.SaveLoadService;
            _mathematicalCalculations = appService.MathematicalCalculations;

            _saveButton.onClick.AddListener(OnSaveButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _findPathButton.onClick.AddListener(OnFindPathButton);
        }

        private void OnDestroy()
        {
            _saveButton.onClick.RemoveListener(OnSaveButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _findPathButton.onClick.RemoveListener(OnFindPathButton);
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
            
        }
    }
}