using System.Collections.Generic;
using UnityEngine;

// Efeitos temporários das poções (Força, Agilidade, Resistência), com tempo restante.
// Tomar a mesma poção de novo renova o tempo em vez de acumular.
public class PlayerEffects : MonoBehaviour
{
    public class ActiveEffect
    {
        public ItemData item;
        public float remaining;
    }

    private readonly List<ActiveEffect> active = new List<ActiveEffect>();
    private LevelSystem levelSystem;

    public IReadOnlyList<ActiveEffect> Active => active;
    public int BonusDamage { get; private set; }
    public float SpeedBonus { get; private set; }
    public float DamageReduction { get; private set; }

    private void Awake() => levelSystem = GetComponent<LevelSystem>();

    public void Add(ItemData item)
    {
        ActiveEffect existing = active.Find(e => e.item.type == item.type);
        if (existing != null) existing.remaining = item.duration;
        else active.Add(new ActiveEffect { item = item, remaining = item.duration });
        Recalculate();
    }

    private void Update()
    {
        bool expired = false;
        for (int i = active.Count - 1; i >= 0; i--)
        {
            active[i].remaining -= Time.deltaTime;
            if (active[i].remaining <= 0f)
            {
                active.RemoveAt(i);
                expired = true;
            }
        }
        if (expired) Recalculate();
    }

    private void Recalculate()
    {
        BonusDamage = 0;
        SpeedBonus = 0f;
        DamageReduction = 0f;
        foreach (ActiveEffect e in active)
        {
            switch (e.item.type)
            {
                case ItemData.ItemType.Strength: BonusDamage += Mathf.RoundToInt(e.item.amount); break;
                case ItemData.ItemType.Agility: SpeedBonus += e.item.amount; break;
                case ItemData.ItemType.Resistance: DamageReduction += e.item.amount; break;
            }
        }
        levelSystem.RefreshStats();
    }
}
