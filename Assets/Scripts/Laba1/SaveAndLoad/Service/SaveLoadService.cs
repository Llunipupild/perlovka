using Laba1.SaveAndLoad.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Laba1.SaveAndLoad.Service
{
    public class SaveLoadService
    {
        private SaveModel _cache;
        
        public SaveModel Get(string key)
        {
            if (_cache != null) {
                return _cache;
            }

            string result = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(result)) {
                return null;
            }

            _cache = JsonConvert.DeserializeObject<SaveModel>(result, new JsonSerializerSettings());
            return _cache;
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        
        public void Set(SaveModel model, string key)
        {
            _cache = model;
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(model, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            PlayerPrefs.Save();
        }
        
        public void Delete(string key)
        {
            _cache = null;
            PlayerPrefs.DeleteKey(key);
        }
    }
}