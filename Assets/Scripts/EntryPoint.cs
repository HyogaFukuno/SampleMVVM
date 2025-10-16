using Cysharp.Threading.Tasks;
using SampleMVVM.Models;
using SampleMVVM.ViewModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace SampleMVVM
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] int initialHealth = 12;
        [SerializeField] int maxHealth = 120;
        [SerializeField] UIDocument document;
        [SerializeField] VisualTreeAsset treeAsset;
        PlayerModel model;
        PlayerViewModel viewModel;
        
        async void Start()
        {
            model = new PlayerModel(initialHealth, maxHealth);
            viewModel = new PlayerViewModel(model, document, treeAsset);

            await UniTask.WaitForSeconds(1.0f);
            model.AddHealth(4 * 5);
            await UniTask.WaitForSeconds(6.0f);
            model.RemoveHealth(120);
        }

        void OnDestroy()
        {
            viewModel.Dispose();
            model.Dispose();
        }
    }
}