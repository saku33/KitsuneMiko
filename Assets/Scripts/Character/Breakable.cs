using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {
    protected static readonly Dictionary<string, HashSet<string>> DAMAGE_TAGS
        = new Dictionary<string, HashSet<string>> {
            {"Player", new HashSet<string> {"Enemy", "Neutral"}},
            {"Enemy", new HashSet<string> {"Player", "Neutral"}},
            {"Neutral", new HashSet<string> {"Player", "Enemy", "Neutral"}}
        };

    public float maxHitPoint;
    public float defencePoint = 1.0f;

    [System.NonSerialized]
    public float hitPoint;

    protected virtual void Awake () {
        hitPoint = maxHitPoint;
    }

    protected virtual void FixedUpdate () {
        if (hitPoint <= 0.0f) {
            GetComponent<Death>().enabled = true;
        }
    }

    protected virtual void OnTriggerEnter2D (Collider2D col) {
        if (DAMAGE_TAGS[tag].Contains(col.tag)) {
            Damage damage = col.GetComponent<Damage>();
            if (damage != null) {
                hitPoint -= defencePoint * damage.attackPoint;
            }
        }
    }
}
