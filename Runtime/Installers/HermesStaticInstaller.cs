using ArcaneOnyx.UnityExtensions;
using Zenject;

namespace ArcaneOnyx.GameEventGenerator
{
    [AutoAssetGeneration("Installers/Static", "HermesStaticInstaller")]
    [StaticInstaller(StaticInstallerExecutionOrder.NonImportant)]
    public class HermesStaticInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<ISceneGameEvents>().To<SceneGameEvents>()
                .FromInstance(FindAnyObjectByType<SceneGameEvents>());
        }
    }
}

