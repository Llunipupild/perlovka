using UnityEngine;

namespace Laba1.Table.TableInputField
{
    public class TableCell : MonoBehaviour
    {
        public string Key { get; private set; }
        public string PreviousValue { get; set; }
        public string CurrentValue { get; set; }

        public void Init(string key, string value)
        {
            Key = key;
            CurrentValue = value;
            PreviousValue = CurrentValue;
        }
    }
}