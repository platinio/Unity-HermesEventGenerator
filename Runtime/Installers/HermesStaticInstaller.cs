using ArcaneOnyx.EditorExtension;
using Zenject;

namespace ArcaneOnyx.GameEventGenerator
{
    [AutoAssetGeneration("Installers/Resources/Static", "HermesStaticInstaller")]
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

