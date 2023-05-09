using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider shieldHealthBar;
    [SerializeField] private GameObject enemyUI;

    private Boss boss;

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

    public void SetBossForUI(Boss boss)
    {
        this.boss = boss;
        enemyUI.SetActive(true);
        EnableEvents();
        //DisableEvents();
    }

    public void EnableEvents()
    {
        boss.OnHealthUpdate += UpdateHealthBar;
        boss.OnShieldHealthUpdate += UpdateShieldHealthBar;
        boss.OnHealthActivated += OnHealthBarActivated;
        boss.OnShieldActivated += OnShieldBarActivated;
    }

    public void DisableEvents()
    {
        boss.OnHealthUpdate -= UpdateHealthBar;
        boss.OnShieldHealthUpdate -= UpdateShieldHealthBar;
        boss.OnHealthActivated -= OnHealthBarActivated;
        boss.OnShieldActivated -= OnShieldBarActivated;
    }
}
