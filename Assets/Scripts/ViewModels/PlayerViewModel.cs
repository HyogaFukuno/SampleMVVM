using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using SampleMVVM.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace SampleMVVM.ViewModels
{
    public class PlayerViewModel : IDisposable
    {
        const int HealthPerHeart = 4;

        readonly VisualElement heartContainer;
        readonly VisualTreeAsset treeAsset;
        readonly IDisposable disposable;
        readonly List<VisualElement> heartViews = new();
        int currentMaxHeart;
        
        public PlayerViewModel(PlayerModel model, UIDocument document, VisualTreeAsset treeAsset)
        {
            heartContainer = document.rootVisualElement.Q("HeartContainer");
            this.treeAsset = treeAsset;
            
            var d1 = model.Health.Pairwise()
                .Where(x => x.Current > x.Previous)
                .SubscribeAwait(async (pair, ct) => await OnHealAsync(pair.Previous, pair.Current, ct));
            
            var d2 = model.Health.Pairwise()
                .Where(x => x.Current < x.Previous)
                .SubscribeAwait(async (pair, ct) => await OnDamageAsync(pair.Previous, pair.Current, ct));
            
            disposable = Disposable.Combine(d1, d2);

            Initialize(model);
        }

        void Initialize(PlayerModel model)
        {
            CreateHeartView(model.Health.CurrentValue / HealthPerHeart);
            heartViews.ForEach(x =>
            {
                x.AddToClassList("health_1");
                x.AddToClassList("health_2");
                x.AddToClassList("health_3");
                x.AddToClassList("health_4");
            });
        }

        void CreateHeartView(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                var view = treeAsset.CloneTree();

                heartViews.Add(view);
                heartContainer.Add(view);       
            }
        }

        async UniTask OnHealAsync(int previousValue, int newValue, CancellationToken ct)
        {
            for (var i = previousValue; i < newValue; i++)
            {
                var index = i / HealthPerHeart;
                if (heartViews.Count <= index)
                {
                    CreateHeartView();
                }

                heartViews[index].AddToClassList($"health_{i % HealthPerHeart + 1}");
                await UniTask.WaitForSeconds(0.04f, cancellationToken: ct);
            }
        }

        async UniTask OnDamageAsync(int previousValue, int newValue, CancellationToken ct)
        {
            for (var i = previousValue - 1; i >= newValue; i--)
            {
                var index = Math.Max(0, i / HealthPerHeart);
                heartViews[index].RemoveFromClassList($"health_{i % HealthPerHeart + 1}");
                await UniTask.WaitForSeconds(0.04f, cancellationToken: ct);
            }
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}