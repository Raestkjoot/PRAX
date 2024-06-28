using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectilesUISystem : MonoBehaviour
{
    [SerializeField] private Sprite _projectileSprite;
    [SerializeField] private Color _color;
    [SerializeField] private float _padding;
    [SerializeField] private float _width;
    [SerializeField] private float _height;

    private Image[] _projectiles;
    private int _projectileAmount;

    public void InitializeProjectiles(int amount)
    {
        _projectileAmount = amount;
        _projectiles = new Image[amount];

        for (int i = 0; i < amount; i++)
        {
            _projectiles[i] = CreateProjectile(i);
        }
    }

    public void ConsumeProjectile()
    {
        if (_projectileAmount == 0)
        {
            Debug.LogWarning("Tried to consume UI projectile when none left");
            return;
        }

        _projectileAmount--;
        _projectiles[_projectileAmount].enabled = false;
    }

    private Image CreateProjectile(int offset)
    {
        GameObject projectileGO = new GameObject("Projectile", typeof(Image));

        projectileGO.transform.SetParent(transform);
        projectileGO.transform.localPosition = Vector3.zero;

        RectTransform rectTransform = projectileGO.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 1.0f);
        rectTransform.sizeDelta = new Vector2(_width, _height);
        rectTransform.localScale = Vector3.one;
        float yOffset = offset * (_height + _padding);
        Vector3 position = rectTransform.position;
        position.y -= yOffset;
        rectTransform.position = position;

        Image projectileImage = projectileGO.GetComponent<Image>();
        projectileImage.sprite = _projectileSprite;
        projectileImage.color = _color;

        return projectileImage;
    }
}