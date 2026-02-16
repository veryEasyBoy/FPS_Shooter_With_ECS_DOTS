using Assets.Scripts.InputActions;
using Zenject;

namespace Assets.Scripts.Installers
{
	public class GlobalInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PlayerInputActions>().AsSingle();
			Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle();
		}
	}
}

