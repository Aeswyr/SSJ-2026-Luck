using TMPro;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    [Header("Simple Particles")]
    [SerializeField] private GameObject template;
	[SerializeField] private AnimationClip[] anims;
    [Header("Floating Text")]
    [SerializeField] private GameObject toastPrefab;

	public void CreateVFX(VFXType type, Vector3 pos, bool flip, Transform parent = null, bool renderBehind = false, float duration = -1)
	{
		if (type == VFXType.NONE)
			return;

		GameObject gameObject;
		if (parent != null)
		{
			gameObject = Instantiate(template, parent);
			gameObject.transform.localPosition = pos;
		}
		else
		{
			gameObject = Instantiate(template, pos, Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
		}
		Animator component = gameObject.GetComponent<Animator>();
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(component.runtimeAnimatorController);
		animatorOverrideController["particle"] = anims[(int)type];
		component.runtimeAnimatorController = animatorOverrideController;
		gameObject.GetComponent<DestroyAfterDelay>().Init(duration == -1 ? anims[(int)type].length - 0.1f : duration);
		SpriteRenderer component2 = gameObject.GetComponent<SpriteRenderer>();
		component2.flipX = flip;
		if (renderBehind)
		{
			component2.sortingLayerName = "VFX_Back";
		}
	}
    public void CreateToast(string text, Vector3 pos)
    {
        CreateToast(text, pos, Color.white);
    }

    public void CreateToast(string text, Vector3 pos, Color color, float size = 24)
    {
        var toast = Instantiate(toastPrefab, pos, Quaternion.identity);
        var tmp = toast.GetComponent<TextMeshPro>();
        tmp.text = text;
        tmp.color = color;
		tmp.fontSize = size;
    }
}

public enum VFXType
{
    DUST_SMALL, DUST_LARGE, DUST_JUMP, HITSPARK_LARGE, HITSPARK_SMALL,
	CORPSE_ANGLER, CORPSES_HOLLOW, CORPSE_SALAMANDER, CORPSE_WING, 
	BUFFSPARK_GENERIC, NONE
}
