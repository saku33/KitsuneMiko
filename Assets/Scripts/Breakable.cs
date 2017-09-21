using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {
    protected static readonly Dictionary<string, List<string>> DAMAGE_TAGS
        = new Dictionary<string, List<string>> {
            {"AllyBreakable", new List<string> {"Enemy", "Neutral"}},
            {"Enemy", new List<string> {"AllyDamage", "Neutral"}},
            {"Neutral", new List<string> {"Ally", "Enemy", "Neutral"}},
            {"Invincible", new List<string> {}}
        };

    public float maxHitPoint;
    public float defencePoint = 1.0f;

    [System.NonSerialized]
    public float hitPoint;

    protected virtual void Awake () {
        hitPoint = maxHitPoint;
    }

    protected virtual void FixedUpdate () {
        if (hitPoint <= 0) {
            GetComponent<Death>().enabled = true;
        }
    }

    protected virtual void OnTriggerEnter2D (Collider2D col) {
        if (DAMAGE_TAGS[tag].Contains(col.gameObject.tag)) {
            Damage damage = col.gameObject.GetComponent<Damage>();
            if (damage != null) {
                hitPoint -= defencePoint * damage.attackPoint;
            }
        }
    }
}
