using System;
using System.Threading.Tasks;
using UnityEngine;

public class CrashCarActivator : BaseController
{
   [SerializeField] private GameObject playerCar;
   [SerializeField] private GameObject crashCar;
   [SerializeField] private GameObject crashEffect;

   protected override Task Initialize()
   {
      try
      {
         Subscribe();
      }
      catch (Exception e)
      {
         Debug.LogException(e);
      }
      
      return Task.CompletedTask;
   }

   private void Subscribe()
   {
      EventBus.Subscribe<GameOverEvent>(OnGameOver);
      EventBus.Subscribe<RestarGameEvent>(OnRestartGame);
   }

   private void Unsubscribe()
   {
      EventBus?.Unsubscribe<GameOverEvent>(OnGameOver);
      EventBus?.Unsubscribe<RestarGameEvent>(OnRestartGame);
   }

   private void OnGameOver(GameOverEvent gameOverEvent)
   {
      if(!gameOverEvent.IsShowAd) return;
      
      Crash();
   }

   private void OnRestartGame(RestarGameEvent restartGameEvent)
   {
      Hide();
   }
   
   private void Crash()
   {
      playerCar.SetActive(false);
      crashCar.SetActive(true);
      
      crashEffect.SetActive(true);
   }

   private void Hide()
   {
      playerCar.SetActive(true);
      
      crashCar.SetActive(false);
      crashEffect.SetActive(false);
   }

   private void OnDestroy()
   {
      Unsubscribe();
   }
}
