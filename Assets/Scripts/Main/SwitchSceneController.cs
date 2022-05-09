using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main
{
    public class SwitchSceneController : MonoBehaviour
    {
        [SerializeField] 
        private Button _lb1Button;
        [SerializeField] 
        private Button _lb2Button;

        private void Start() 
        {
            _lb1Button.onClick.AddListener(OnLb1ButtonClick);
            _lb2Button.onClick.AddListener(OnLb2ButtonClick);
        }

        private void OnDestroy() 
        {
            _lb1Button.onClick.RemoveListener(OnLb1ButtonClick);
            _lb2Button.onClick.RemoveListener(OnLb2ButtonClick);
        }

        private void OnLb1ButtonClick() 
        {
            SceneManager.LoadScene("Laba1");
        }
        
        private void OnLb2ButtonClick() 
        {
            SceneManager.LoadScene("Laba2");
        }
    }
}