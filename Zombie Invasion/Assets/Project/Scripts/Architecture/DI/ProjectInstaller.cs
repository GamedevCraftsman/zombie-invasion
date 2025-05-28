using Zenject;

public class ProjectInstaller : MonoInstaller
{
   public override void InstallBindings()
   {
      Container.Bind<IEventBus>().To<EventBus>().AsSingle();
      Container.Bind<IInputService>().To<InputService>().AsSingle();
   }
}
