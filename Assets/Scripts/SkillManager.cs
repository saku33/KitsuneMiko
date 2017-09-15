using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {
	public float maxMagicPoint = 100.0f;
	public float mugicPoint = 100.0f;
	public float recoveryAmount = 1.0f;

	Skill[] skillSlots = new Skill[3];
	bool isActive = false;

	void Start () {
		// 空Skillをセット
	}

	void FixedUpdate () {
		if (isActive) {
			foreach (Skill skill in skillSlots) {
				mugicPoint -= skill.cost;
			}
			if (mugicPoint <= 0) {
				Deactivate();
				mugicPoint = 0;
			}
		} else {
			if (mugicPoint < maxMagicPoint) {
				mugicPoint += recoveryAmount;
				if (mugicPoint > maxMagicPoint) {
					mugicPoint = maxMagicPoint;
				}
			}
		}
	}

	public void ReplaceSkill (int num, string name) {
		System.Type skillType = System.Type.GetType(name);
		Destroy(skillSlots[num]);
		skillSlots[num] = gameObject.AddComponent(skillType) as Skill;
		skillSlots[num].enabled = isActive;
	}

	public void ReplaceSkills (string[] names) {
		for (int i = 0; i < 3; i++) {
			ReplaceSkill(i, names[i]);
		}
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
