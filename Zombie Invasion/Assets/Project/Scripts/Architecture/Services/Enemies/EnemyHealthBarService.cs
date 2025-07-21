using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarService : IEnemyHealthBarService
{
    public void UpdateHealthBar(Image healthBarFill, int currentHealth, int maxHealth)
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }
    
    public void ManageHealthBar(bool isOn, Canvas healthBarCanvas)
    {
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(isOn);
    }
}