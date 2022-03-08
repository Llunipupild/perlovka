using UnityEngine;

namespace Laba1.Table.TableInputField
{
    public class TableCell : MonoBehaviour
    {
        private string _value;
        public string Key { get; private set; }
        public string PreviousValue { get; private set; }
        
        public string CurrentValue
        {
            get => _value;
            set
            {
                PreviousValue = _value;
                _value = value;
            }
        }

        public void Init(string key, string value)
        {
            Key = key;
            _value = value;
        }
    }
}