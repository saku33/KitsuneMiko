using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour {
    [System.NonSerialized]
    public float maxMagicPoint = 100.0f;
    [System.NonSerialized]
    public float magicPoint;
    [System.NonSerialized]
    public float recoveryAmount = 1.0f;

    Skill[] skillSlots = new Skill[3];
    bool isActive = false;
    float totalCost;

    void Awake () {
        magicPoint = maxMagicPoint;
    }

    void Start () {
        // 空Skillをセット
    }

    void FixedUpdate () {
        if (isActive) {
            magicPoint -= totalCost;
            if (magicPoint <= 0.0f) {
                Deactivate();
                magicPoint = 0.0f;
            }
        } else {
            if (magicPoint < maxMagicPoint) {
                magicPoint += recoveryAmount;
                if (magicPoint > maxMagicPoint) {
                    magicPoint = maxMagicPoint;
                }
            }
        }
    }

    void _ReplaceSkill (int num, System.Type type) {
        Destroy(skillSlots[num]);
        skillSlots[num] = gameObject.AddComponent(type) as Skill;
        skillSlots[num].enabled = isActive;
    }

    void UpdateTotalCost () {
        totalCost = skillSlots.Sum(skill => skill.cost);
    }

    public void ReplaceSkill (int num, System.Type type) {
        _ReplaceSkill(num, type);
        UpdateTotalCost();
    }

    public void ReplaceSkills (System.Type[] types) {
        for (int i = 0; i < 3; i++) {
            _ReplaceSkill(i, types[i]);
        }
        UpdateTotalCost();
    }

    public void Activate () {
        isActive = true;
        foreach (Skill skill in skillSlots) {
            skill.enabled = true;
        }
    }

    public void Deactivate () {
        isActive = false;
        foreach (Skill skill in skillSlots) {
            skill.enabled = false;
        }
    }
}
