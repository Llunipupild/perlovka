using Laba1.DrawingArea.Controller;
using Laba1.SaveController.Model;
using Laba1.Table.Controller;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Laba1.SaveController.Controller
{
    public class SaveLoadController : MonoBehaviour
    {
        private const string KEY = "perlLb1";
        
        [SerializeField] 
        private Button _saveButton;
        [SerializeField] 
        private Button _loadButton;

        private TableController _tableController;
        private DrawingAreaController _drawingAreaController;
        private int _countVertex;
        
        private SaveModel _cache;

        public void Init(TableController tableController, DrawingAreaController drawingAreaController, int countVertex)
        {
            _tableController = tableController;
            _drawingAreaController = drawingAreaController;
            _countVertex = countVertex;
            
            _saveButton.onClick.AddListener(OnSaveButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
        }
        
        private void OnSaveButtonClick()
        {
            if (PlayerPrefs.HasKey(KEY))
            {
                Delete();
            }

            SaveModel saveModel = new SaveModel(_tableController._countVertex, _tableController.GetDictionary());
            Set(saveModel);
        }

        private void OnLoadButtonClick()
        {
            //todo кнопку загрузить на главный экран и тода наверно лучше буит не будет бага (баг не воспроизводится)
            SaveModel saveModel = Get();
            _tableController.DeleteTable();
            _tableController.CreateTable(saveModel.CountVertex, saveModel.Dictionary);
        }
        
        private void Set(SaveModel model)
        {
            _cache = model;
            PlayerPrefs.SetString(KEY, JsonConvert.SerializeObject(model, new JsonSerializerSettings()));
            PlayerPrefs.Save();
        }
        
        private SaveModel Get()
        {
            if (_cache != null)
            {
                return _cache;
            }

            string result = PlayerPrefs.GetString(KEY);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            _cache = JsonConvert.DeserializeObject<SaveModel>(result, new JsonSerializerSettings());
            return _cache;
        }
        
        private void Delete()
        {
            _cache = null;
            PlayerPrefs.DeleteKey(KEY);
        }
    }
}