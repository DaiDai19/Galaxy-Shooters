using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider shieldHealthBar;

    private Boss boss;

    private void Awake()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
    }

    private void OnEnable()
    {
        boss.OnHealthUpdate += UpdateHealthBar;
        boss.OnShieldHealthUpdate += UpdateShieldHealthBar;
        boss.OnHealthActivated += OnHealthBarActivated;
        boss.OnShieldActivated += OnShieldBarActivated;
    }

    private void OnDisable()
    {
        boss.OnHealthUpdate -= UpdateHealthBar;
        boss.OnShieldHealthUpdate -= UpdateShieldHealthBar;
        boss.OnHealthActivated -= OnHealthBarActivated;
        boss.OnShieldActivated -= OnShieldBarActivated;
    }

    private void UpdateHealthBar(int curHealth, int maxHealth)
    {
        healthBar.value = (float)curHealth / (float)maxHealth;
    }

    private void UpdateShieldHealthBar(int curHealth, int maxHealth)
    {
        shieldHealthBar.value = (float)curHealth / (float) maxHealth;
    }
    private void OnHealthBarActivated(bool activate)
    {
        healthBar.gameObject.SetActive(activate);
    }

    private void OnShieldBarActivated(bool activate)
    {
        shieldHealthBar.gameObject.SetActive(activate);
    }
}
