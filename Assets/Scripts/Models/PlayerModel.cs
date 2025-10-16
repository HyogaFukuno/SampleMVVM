using System;
using R3;
using UnityEngine;

namespace SampleMVVM.Models
{
    public class PlayerModel : IDisposable
    {
        readonly ReactiveProperty<int> health;
        
        public ReadOnlyReactiveProperty<int> Health => health;
        public int MaxHealth { get; }
        public int MinHealth { get; }

        public PlayerModel(int initialHealth, int maxHealth, int minHealth = 0)
        {
            health = new ReactiveProperty<int>(initialHealth);
            MaxHealth = maxHealth;
            MinHealth = minHealth;
        }

        public void AddHealth(int amount)
        {
            health.Value = Math.Min(MaxHealth, health.CurrentValue + amount);
        }
        
        public void RemoveHealth(int amount)
        {
            health.Value = Math.Max(MinHealth, health.CurrentValue - amount);
        }

        public void Dispose()
        {
            Health.Dispose();
        }
    }
}