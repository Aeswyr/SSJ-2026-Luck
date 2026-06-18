using UnityEngine;
using System.Collections.Generic;

public class UnitVFXController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject afterImagePrefab;
	private float afterImageTimer;

	private float afterImageDelay;

	private float nextImage;
	private List<AfterimageData> activeImages = new();

	private void FixedUpdate()
	{
		for (int i = 0; i < activeImages.Count; i++)
		{
			var image = activeImages[i];
			if (Time.time > image.end)
			{
				activeImages.RemoveAt(i);
				i--;
			}
			else if (Time.time > image.next)
			{
				CreateAfterimage(image);
				image.next = Time.time + image.delay;

				activeImages[i] = image;
			}
		}
	}

    public void StartAfterImageChain(float duration, float imageDelay, float imageDuration = 0.5f, bool overrideMaterial = true, Color color = default, string tag = null)
	{
		if (color == default)
		{
			color = Color.white;
		}

		AfterimageData data = new()
		{
			end = Time.time + duration,
			next = Time.time + imageDelay,
			delay = imageDelay,
			duration = imageDuration,
			materialOverride = overrideMaterial,
			color = color,
			tag = tag
		};

		activeImages.Add(data);
	}

	public void CreateAfterimage(float duration, float imageDelay, float imageDuration = 0.5f, bool overrideMaterial = true, Color color = default)
	{
		if (color == default)
		{
			color = Color.white;
		}

		AfterimageData data = new()
		{
			end = Time.time + duration,
			next = Time.time + imageDelay,
			delay = imageDelay,
			duration = imageDuration,
			materialOverride = overrideMaterial,
			color = color,
			tag = null
		};

		CreateAfterimage(data);
	}

	private void CreateAfterimage(AfterimageData data)
	{
		GameObject gameObject = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
		SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
		SpriteRenderer spriteRenderer = sprite;
		component.sprite = spriteRenderer.sprite;
		if (data.materialOverride)
			component.material = spriteRenderer.material;
		else
			component.color = data.color;
		component.flipX = spriteRenderer.flipX;
		gameObject.GetComponent<DestroyAfterDelay>().Init(data.duration);
		gameObject.GetComponent<AlphaDecay>().SetDuration(data.duration);
	}

	public void EndChain(string tag)
	{
        for (int i = 0; i < activeImages.Count; i++)
        {
            if (activeImages[i].tag == tag)
            {
                activeImages.RemoveAt(i);
                i--;
            }
        }
	}

    public struct AfterimageData {
		public float end;
		public float next;
		public float duration;
		public float delay;
		public bool materialOverride;
		public Color color;
		public string tag;
	}

}
