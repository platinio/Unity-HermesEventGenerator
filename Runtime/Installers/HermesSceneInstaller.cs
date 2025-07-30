using Zenject;

namespace ArcaneOnyx.GameEventGenerator
{
    public class HermesSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISceneGameEvents>().To<SceneGameEvents>().FromComponentsInHierarchy().AsSingle();
        }
    }
}

