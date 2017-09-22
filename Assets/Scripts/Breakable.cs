using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LAYER {
    ALLY_BREAKABLE = 8,
    ALLY_DAMAGE,
    ENEMY,
    NEUTRAL,
    INVINCIBLE
}

public class Breakable : MonoBehaviour {
    protected static readonly Dictionary<LAYER, HashSet<LAYER>> DAMAGE_LAYERS
        = new Dictionary<LAYER, HashSet<LAYER>> {
            {LAYER.ALLY_BREAKABLE, new HashSet<LAYER> {LAYER.ENEMY, LAYER.NEUTRAL}},
            {LAYER.ENEMY, new HashSet<LAYER> {LAYER.ALLY_DAMAGE, LAYER.NEUTRAL}},
            {LAYER.NEUTRAL, new HashSet<LAYER> {LAYER.ALLY_DAMAGE, LAYER.ENEMY, LAYER.NEUTRAL}},
            {LAYER.INVINCIBLE, new HashSet<LAYER> {}}
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
        if (DAMAGE_LAYERS[(LAYER)gameObject.layer].Contains((LAYER)col.gameObject.layer)) {
            Damage damage = col.gameObject.GetComponent<Damage>();
            if (damage != null) {
                hitPoint -= defencePoint * damage.attackPoint;
            }
        }
    }
}
