using UnityEngine;
using UnityEngine.UI;

public interface IEnemyHealthBarService
{
    void ManageHealthBar(bool isOn, Canvas healthBarCanvas);
    void UpdateHealthBar(Image healthBarFill, int currentHealth, int maxHealth);
}