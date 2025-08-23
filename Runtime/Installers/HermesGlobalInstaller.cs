using ArcaneOnyx.EditorExtension;
using Zenject;

namespace ArcaneOnyx.GameEventGenerator
{
    [AutoAssetGeneration("Installers/Global", "HermesGlobalInstaller")]
    public class HermesGlobalInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISceneGameEvents>().To<SceneGameEvents>()
                .FromInstance(FindAnyObjectByType<SceneGameEvents>());
        }
    }
}

